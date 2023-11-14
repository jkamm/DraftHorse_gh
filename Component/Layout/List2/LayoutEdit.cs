using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using System.Linq;
using DraftHorse.Helper;
using static DraftHorse.Helper.Layout;
using static DraftHorse.Helper.ValList;
using Rhino.Geometry;

namespace DraftHorse.Component
{
    public class LayoutEdit : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the LayoutEdit class.
        /// </summary>
        public LayoutEdit()
          : base("Edit Layout", "LayoutEdit",
              "Modify Layout Attributes",
              "Drafthorse", "Layout")
        {
            ButtonName = "Modify";
        }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            //get a layout by layout index (or several by layoutName)
            pManager.AddIntegerParameter("Index", "Li[]", "Index of a Layout (from LayoutIndex Component)\nAttach Value List for Layout List", GH_ParamAccess.item);
            Params.Input[pManager.AddTextParameter("Name", "N", "PageName", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddNumberParameter("Width", "W", "PageWidth", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddNumberParameter("Height", "H", "PageHeight", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddTextParameter("Keys", "K", "Keys for User Texts (unique)", GH_ParamAccess.list)].Optional = true;
            Params.Input[pManager.AddTextParameter("Values", "V", "Values for User Texts (must match Key count)", GH_ParamAccess.list)].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //return properties of the layout: Name, pageNumber, page attributes
            pManager.AddIntegerParameter("Index", "Li", "PageNumber", GH_ParamAccess.item); 
            pManager.AddTextParameter("Name", "N", "PageName", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "PageWidth", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "PageHeight", GH_ParamAccess.item);
            pManager.AddTextParameter("Keys", "K", "Keys written to Layout", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "V", "Values written to Layout", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int index = new int();
            if (!DA.GetData("Index", ref index)) return;

            Rhino.Display.RhinoPageView target = Layout.GetPage(index);

            if (target != null)
            {
                string newPageName = string.Empty;
                if (!DA.GetData("Name", ref newPageName)) newPageName = target.PageName;

                double newPageWidth = target.PageWidth;
                if (!DA.GetData("Width", ref newPageWidth)) newPageWidth = target.PageWidth;

                double newPageHeight = target.PageHeight;
                if (!DA.GetData("Height", ref newPageHeight)) newPageHeight = target.PageHeight;

                List<string> iKeys = new List<string>();
                DA.GetDataList("Keys", iKeys);

                List<string> iVals = new List<string>();
                DA.GetDataList("Values", iVals);

                bool run = false;
                if (!DA.GetData("Run", ref run)) run = false;
                                
                if (iVals.Count != iKeys.Count) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Key/Value pairs do not match. Check the data structure.");
                
                if (run || Execute)
                {

                    var uText = target.MainViewport.GetUserStrings();

                    IEnumerable<string> keysFound = from key in iKeys
                                                    where uText.Get(key) != null
                                                    select key;
                    if (keysFound.Count() != 0) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Duplicate Keys Found, all duplicates will be replaced");

                    for (int i = 0; i < iKeys.Count; i++)
                    {
                        target.MainViewport.SetUserString(iKeys[i], iVals[i]);
                    }
                                        
                    target.PageName = newPageName;
                    target.PageWidth = newPageWidth;
                    target.PageHeight = newPageHeight;
                }

                int pageNumber = target.PageNumber;
                string name = target.PageName;
                double pageWidth = target.PageWidth;
                double pageHeight = target.PageHeight;

                //Get UserText from MainViewPort of Layout
                var userText = target.MainViewport.GetUserStrings();
                string[] userVals = new string[userText.Count];
                if (userText.Count != 0)
                {
                    for (int i = 0; i < userText.Count; i++)
                    {
                        userVals[i] = userText.Get(i);
                    }
                }

                DA.SetData("Index", pageNumber);
                DA.SetData("Name", name);
                DA.SetData("Width", pageWidth);
                DA.SetData("Height", pageHeight);
                DA.SetDataList("Keys", userText.AllKeys);
                DA.SetDataList("Values", userVals);

            }
        }

        #region AutoValueList

        //Update a value list if added to a given input(based on Elefront and FabTools)
        //on event for a source added to a given input

        private bool _handled = false;

        private void SetupEventHandlers()
        {
            if (_handled)
                return;

            Params.Input[0].ObjectChanged += InputParamChanged;

            _handled = true;
        }

        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();
            SetupEventHandlers();
        }


        public void InputParamChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {

            //optional feedback
            //if (e.Type == GH_ObjectEventType.Sources)
            //    Rhino.RhinoApp.WriteLine("Sources changed on input '{0}'.", sender.NickName);

            if (sender.NickName == Params.Input[0].NickName)
            {
                // optional feedback
                // Rhino.RhinoApp.WriteLine("This is the right input");

                var pageDictionary = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews().ToDictionary(v => v.PageName, v => v.PageNumber);
                List<string> pageViewNames = pageDictionary.Keys.ToList();
                List<string> layoutIndices = new List<string>();
                for (int i = 0; i < pageViewNames.Count; i++)
                    layoutIndices.Add(pageDictionary[pageViewNames[i]].ToString());

                //try to modify input as a valuelist
                try
                {
                    UpdateValueList(this, 0, "Layouts", "Pick Layout(s): ", pageViewNames, layoutIndices);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add/Update ValueList", Menu_DoClick);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            var pageViews = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews();
            Layout.SortPagesByPageNumber(pageViews);
            List<string> pageNums = pageViews.Select(p => p.PageNumber.ToString()).ToList();
            List<string> pageNames = pageViews.Select(p => p.PageName.ToString()).ToList();

            if (!AddOrUpdateValueList(this, 0, "Layouts", "Layouts To Deconstruct: ", pageNames, pageNums))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 1 + "] failed to update");
            ExpireSolution(true);
        }
        #endregion Add Value Lists

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.EditLayout_bitmap;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("30E64E87-8FD5-45A5-8F28-07E46821FC81"); }
        }
    }
}