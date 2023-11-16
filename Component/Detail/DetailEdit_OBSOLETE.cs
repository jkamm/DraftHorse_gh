using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static DraftHorse.Helper.Layout;

namespace DraftHorse.Component
{
    public class DetailEdit_OBSOLETE: Base.DH_ButtonComponent
    {


        /// <summary>
        /// Initializes a new instance of the ReplaceDetails class.
        /// </summary>
        public DetailEdit_OBSOLETE()
          : base("Edit Details", "DetailEdit",
              "Modify detail views in a layout",
              "DraftHorse", "Detail")
        {
            ButtonName = "Modify";
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggle = new Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggle, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            var guidParam = new Grasshopper.Kernel.Parameters.Param_Guid();
            pManager.AddParameter(guidParam, "Detail GUID", "D", "GUID for Detail Object", GH_ParamAccess.item);
            Params.Input[pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddIntegerParameter("Projection", "P[]", "View Projection \nAttach Value List for list of projections", GH_ParamAccess.item)].Optional = true;
            //Goal: Add Value List Generation for Named Views
            Params.Input[pManager.AddTextParameter("DisplayMode", "D[]", "Display Mode \nAttach Value List for list of Display Modes", GH_ParamAccess.item)].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "R", "Success or Failure for each detail", GH_ParamAccess.item);
            var guidParam = new Grasshopper.Kernel.Parameters.Param_Guid();
            pManager.AddParameter(guidParam, "Detail GUID", "D", "GUID for Detail Object", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit", GH_ParamAccess.item);
            pManager.AddTextParameter("ViewName", "V", "ViewPort Viewname", GH_ParamAccess.item);  
            pManager.AddTextParameter("DisplayMode", "D", "Display Mode", GH_ParamAccess.item);
            /*
             */
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            #region EscapeBehavior
            //Esc behavior code snippet from 
            // http://james-ramsden.com/you-should-be-implementing-esc-behaviour-in-your-grasshopper-development/
            if (GH_Document.IsEscapeKeyDown())
            {
                GH_Document GHDocument = OnPingDocument();
                GHDocument.RequestAbortSolution();
            }
            #endregion EscapeBehavior

            bool run = false;
            DA.GetData("Run", ref run);

            Guid detailGUID = Guid.Empty;
            DA.GetData("Detail GUID", ref detailGUID);
            if (detailGUID == null) { return; }
            Rhino.DocObjects.DetailViewObject detail = Rhino.RhinoDoc.ActiveDoc.Objects.FindId(detailGUID) as Rhino.DocObjects.DetailViewObject; ;


            Point3d target = new Point3d();
            if (!DA.GetData("Target", ref target)) target = detail.Viewport.CameraTarget;
            //DA.GetData("Target", ref target);

            double scale = 1.0;
            if (!DA.GetData("Scale", ref scale)) scale = detail.DetailGeometry.PageToModelRatio;
            //DA.GetData("Scale", ref scale);

            int pNum = 0;
            //if (!DA.GetData("Projection", ref pNum)) pNum = 0;
            DA.GetData("Projection", ref pNum);

            if (!Enum.IsDefined(typeof(Rhino.Display.DefinedViewportProjection), pNum))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, pNum + " is not a valid Projection number. Projection will not be modified");
            }

            Rhino.Display.DefinedViewportProjection projection = (Rhino.Display.DefinedViewportProjection)pNum;

            string dName = String.Empty;
            if (!DA.GetData("DisplayMode", ref dName)) dName = detail.Viewport.DisplayMode.EnglishName;
            DisplayModeDescription displayMode = detail.Viewport.DisplayMode;

            //convert LocalName to EnglishName
            if (ValList.GetDisplaySettingsList(true).Contains(dName))
                dName = ValList.GetDisplaySettingsList(false)[ValList.GetDisplaySettingsList(true).IndexOf(dName)];

            if (!ValList.GetDisplaySettingsList(false).Contains(dName))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, dName + " is not a valid Display Mode name");
            else
            {
                //Change to type DisplayModeDescription
                displayMode = DisplayModeDescription.GetDisplayModes().First(mode => mode.DisplayAttributes.EnglishName == dName);
            }

            if (Execute || run)
            {
                //Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, target, scale);
                //Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, target, scale, projection);
                Rhino.Commands.Result detailResult = ReviseDetail(detail, target, scale, projection, displayMode);
                DA.SetData("Result", detailResult);
            }
            DA.SetData("Detail GUID", detailGUID);
            DA.SetData("Target", detail.Viewport.CameraTarget);
            DA.SetData("Scale", detail.DetailGeometry.PageToModelRatio);
            DA.SetData("ViewName", detail.Viewport.Name);
            DA.SetData("DisplayMode", detail.Viewport.DisplayMode.EnglishName);
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LayoutDetail_bitmap;
        
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("95ff5c32-54c5-44c3-8f0e-ed0db7f1b702"); }
        }

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Add List of Views", Menu_ViewClick);
            Menu_AppendItem(menu, "Add List of DisplayModes", Menu_DisplayClick);
        }

        private void Menu_ViewClick(object sender, EventArgs e)
        {
            //List<string> pageViewNames = ValList.GetStandardViewList();
            string[] pNames = Enum.GetNames(typeof(Rhino.Display.DefinedViewportProjection));
            List<string> projNames = pNames.Select(v => v.ToString()).ToList();
            List<int> pVals = ((Rhino.Display.DefinedViewportProjection[])Enum.GetValues(typeof(Rhino.Display.DefinedViewportProjection))).Select(c => (int)c).ToList();
            List<string> projVals = pVals.ConvertAll<string>(v => v.ToString());


            if (!ValList.AddOrUpdateValueList(this, 4, "Views", "Pick Projection: ", projNames, projVals))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

            ExpireSolution(true);
        }

        private void Menu_DisplayClick(object sender, EventArgs e)
        {
            List<string> dNames = ValList.GetDisplaySettingsList(false);
            List<string> dLocalNames = ValList.GetDisplaySettingsList(true);

            if (!ValList.AddOrUpdateValueList(this, 5, "DisplayMode", "Pick DisplayMode: ", dLocalNames, dNames))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

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

            Params.Input[4].ObjectChanged += InputParamChanged;
            Params.Input[5].ObjectChanged += InputParamChanged;

            _handled = true;
        }

        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();
            SetupEventHandlers();
        }


        public void InputParamChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (sender.NickName == Params.Input[4].NickName)
            {
                // optional feedback
                // Rhino.RhinoApp.WriteLine("This is the right input");

                //List<string> standardViewNames = ValList.GetStandardViewList();
                string[] pNames = Enum.GetNames(typeof(Rhino.Display.DefinedViewportProjection));
                List<string> projNames = pNames.Select(v => v.ToString()).ToList();
                List<int> pVals = ((Rhino.Display.DefinedViewportProjection[])Enum.GetValues(typeof(Rhino.Display.DefinedViewportProjection))).Select(c => (int)c).ToList();
                List<string> projVals = pVals.ConvertAll<string>(v => v.ToString());

                //try to modify input as a valuelist
                try
                {
                    ValList.UpdateValueList(this, 4, "Views", "Pick Projection: ", projNames, projVals);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
            else if (sender.NickName == Params.Input[5].NickName)
            {
                // optional feedback
                // Rhino.RhinoApp.WriteLine("This is the right input");

                //List<string> standardViewNames = ValList.GetStandardViewList();

                List<string> displayNames = ValList.GetDisplaySettingsList(true);
                List<string> displayVals = ValList.GetDisplaySettingsList(false);

                //try to modify input as a valuelist
                try
                {
                    ValList.UpdateValueList(this, 5, "Display", "Pick Display: ", displayNames, displayVals);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList
    }
}