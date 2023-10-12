using DraftHorse.Helper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using static DraftHorse.Helper.ValList;


namespace DraftHorse.Component
{
    public class GetLayoutIndex : GH_Component
    {
        #region GH_Component
        /// <summary>
        /// Initializes a new instance of the GetLayoutIndex class.
        /// </summary>
        public GetLayoutIndex()
          : base("Get Layout Index", "GetLOIndex",
              "Get index map for layout name(s). Be careful of duplicate names",
              "DraftHorse", "Layouts")
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N[]", "Layout Name \nAdd ValueList to get list of names", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Layout Index", "Li", "Index matching Layout Name", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Count", "C", "Instances of Layout Name", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";

            if (!DA.GetData("Name", ref name)) return;
            //DA.GetData("Name", ref name);

            int indexCount = 0;
            List<int> indexList = new List<int>();

            int[] pages = Layout.GetPages(name);

            //foreach (Rhino.Display.RhinoPageView page in pages)
            //{
            //    indexList.Add(page.PageNumber);
            //    indexCount++;
            //}
            indexList.AddRange(pages);
            indexCount = pages.Length;
            if (indexCount != 1) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning! Page name '" + name + "' occurs " + indexCount + " times!");

            DA.SetDataList("Layout Index", indexList); //index map
            DA.SetData("Count", indexCount); //Count of how many of this name
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LayoutIndex;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8dc334f6-2c09-4a37-98c0-0945d58438de"); }
        }
        #endregion GH_Component
        /*
        #region AddUpdateButton
        public override void CreateAttributes()
        {
            base.m_attributes = new DraftHorse.Component.Base.Attributes_UpdateButton(this);
        }
        #endregion AddUpdateButton
         */

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add/Update ValueList", Menu_DoClick, true);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            List<string> pageViewNames = GetLayoutList();

            if (!AddOrUpdateValueList(this, 0, "Layouts", "Pick Named Layout: ", pageViewNames, pageViewNames))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

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

                List<string> pageViewNames = GetLayoutList();

                //try to modify input as a valuelist
                try
                {
                    UpdateValueList(this, 0, "Layouts", "Pick Named Layout: ", pageViewNames, pageViewNames);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList
    }
}