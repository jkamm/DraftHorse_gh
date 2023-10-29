using DraftHorse.Helper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using static DraftHorse.Helper.ValList;

namespace DraftHorse.Component
{
    public class LayoutCopy : Base.DH_ButtonComponent
    {
        #region GH_Component
        /// <summary>
        /// Initializes a new instance of the CopyLayout class.
        /// </summary>
        public LayoutCopy()
          : base("Copy Layout", "CopyLayout",
              "Instantiate a Layout from a template in the document",
              "DraftHorse", "Layout-Add")
        {
            ButtonName = "Copy";
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item);
            Params.Input[0].Optional = true;
            pManager.AddIntegerParameter("Template Index", "Li[]", "Index of Template Layout \nAdd ValueList to get list of layouts", GH_ParamAccess.item);
            pManager.AddTextParameter("NewName", "N", "Name of Copy", GH_ParamAccess.item);
            Params.Input[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Layout Index", "Li", "Index of new Layout(s)", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Name", "N", "Name of new Layout(s)", GH_ParamAccess.item);
            //goal: Add Result param?
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //define input variables
            bool run = false;
            DA.GetData("Run", ref run);
            //if (!DA.GetData("Run", ref run)) return;

            int templateIndex = new int();
            if (!DA.GetData("Template Index", ref templateIndex)) return;

            //define local variables
            int newIndex = new int();
            string newName = "";

            #region EscapeBehavior
            //Esc behavior code snippet from 
            // http://james-ramsden.com/you-should-be-implementing-esc-behaviour-in-your-grasshopper-development/
            if (GH_Document.IsEscapeKeyDown())
            {
                GH_Document GHDocument = OnPingDocument();
                GHDocument.RequestAbortSolution();
            }
            #endregion EscapeBehavior

            if (run || Execute)
            {
                Rhino.Display.RhinoPageView template = Layout.GetPage(templateIndex);
                Rhino.Display.RhinoPageView dup = template.Duplicate(true);

                //if no newName is defined, then set newName to template + PageNumber
                if (!DA.GetData("NewName", ref newName)) newName = template.PageName + "." + dup.PageNumber.ToString();

                //rename duplicate
                dup.PageName = newName;
                newIndex = dup.PageNumber;
                Layout.RefreshView(dup);

                DA.SetData("Layout Index", newIndex);
                DA.SetData("Layout Name", newName);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.CopyLayout_bitmap;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f1d0ba78-7f55-4bed-a60d-eb46b5770c9e"); }
        }
        #endregion GH_Component 

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add/Update ValueList", Menu_DoClick);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            var pageDictionary = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews().ToDictionary(v => v.PageName, v => v.PageNumber);
            List<string> pageViewNames = pageDictionary.Keys.ToList();
            List<string> layoutIndices = new List<string>();
            for (int i = 0; i < pageViewNames.Count; i++)
                layoutIndices.Add(pageDictionary[pageViewNames[i]].ToString());

            if (!AddOrUpdateValueList(this, 1, "Layouts", "Pick Template: ", pageViewNames, layoutIndices))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 1 + "] failed to update");

            ExpireSolution(true);
        }
        #endregion Add Value Lists

        #region AutoValueList

        //Update a value list if added to a given input(based on Elefront and FabTools)
        //on event for a source added to a given input

        private bool _handled = false;

        private void SetupEventHandlers()
        {
            if (_handled)
                return;

            Params.Input[1].ObjectChanged += InputParamChanged;

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

            if (sender.NickName == Params.Input[1].NickName)
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
                    UpdateValueList(this, 1, "Layouts", "Pick Template: ", pageViewNames, layoutIndices);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList
    }
}