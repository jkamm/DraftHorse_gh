using DraftHorse.Helper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using static DraftHorse.Helper.Layout;
using static DraftHorse.Helper.ValList;

namespace DraftHorse.Component
{
    public class LayoutDeconstruct : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructLayout class.
        /// </summary>
        public LayoutDeconstruct()
          : base("Deconstruct Layout", "LODecLayout",
              "Deconstruct a Layout into its Composition and Attributes",
              "DraftHorse", "Layout")
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //get a layout by layout index (or several by layoutName)
            pManager.AddIntegerParameter("LayoutIndex", "Li[]", "Index of a Layout (from LayoutIndex Component) /nAttach Value List for Layout List", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //return properties of the layout: Name, pageNumber, page attributes
            pManager.AddTextParameter("Name", "Na", "PageName", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "No", "PageNumber", GH_ParamAccess.item);
            var guidParam = new Grasshopper.Kernel.Parameters.Param_Guid();

            pManager.AddNumberParameter("Width", "W", "PageWidth", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "PageHeight", GH_ParamAccess.item);
            pManager.AddTextParameter("Paper", "P", "Returns the name of the layout's media, or paper (e.g. Letter, Legal, A1, etc.)," +
                "\nused to determine the page width and page height.", GH_ParamAccess.item);
            //var guidParam2 = new Grasshopper.Kernel.Parameters.Param_Guid();
            //pManager.AddParameter(guidParam2, "Guids", "G", "Guids of Geometry on the layout", GH_ParamAccess.list);
            //goal: need something to collect Text Objects. Guids cast through the Extended Geometry Param, but not otherwise.
            pManager.AddGeometryParameter("Geo", "G", "Generic Geometry on the Layout", GH_ParamAccess.list);
            pManager.AddParameter(guidParam, "Detail GUID", "D", "GUIDs of Details on the layout", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int templateIndex = new int();
            if (!DA.GetData("LayoutIndex", ref templateIndex)) return;

            Rhino.Display.RhinoPageView target = Layout.GetPage(templateIndex);

            string name = target.PageName;
            int pageNumber = target.PageNumber;
            double pageWidth = target.PageWidth;
            double pageHeight = target.PageHeight;
            string paperName = target.PaperName;
            List<Guid> detailGUIDs = target.GetDetailViews().Select(v => v.Id).ToList();

            //search object table for object with viewportID matching this Layout.  
            //List<Guid> guids = GetPageGeoGuids(target);
            List<Grasshopper.Kernel.Types.IGH_GeometricGoo> gooList = GetPageGeoGoos(target);


            DA.SetData("Name", name);
            DA.SetData("Number", pageNumber);
            DA.SetData("Width", pageWidth);
            DA.SetData("Height", pageHeight);
            DA.SetData("Paper", paperName);
            DA.SetDataList("Detail GUID", detailGUIDs);
            //DA.SetDataList("Guids", guids);
            DA.SetDataList("Geo", gooList);
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Dec_Layout;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cf90e7c1-5c2b-43e1-a8ad-a97118a74f78"); }
        }


    }
}