using CurveComponents;
using DraftHorse.Helper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DraftHorse.Component.Detail
{
    public class DetailEdit : Base.DH_ButtonComponent
    {

        /// <summary>
        /// Initializes a new instance of the ReplaceDetails class.
        /// </summary>
        public DetailEdit()
          : base("Edit Details", "DetailEdit",
              "Modify detail views in a layout",
              "DraftHorse", "Detail")
        {
            ButtonName = "Modify";
            Hidden = true;
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            var bToggle = new Params.Param_BooleanToggle();
            pManager.AddParameter(bToggle, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item);
            var guidParam = new Param_Guid();
            pManager.AddParameter(guidParam, "GUID", "G", "GUID for Detail Object", GH_ParamAccess.item);
            pManager.AddTextParameter("Display", "D[]", "Display Mode \nAttach Value List for list of Display Modes", GH_ParamAccess.item);
            
            pManager.AddBoxParameter("Target", "T", "Target for Detail\nPoint is acceptable input for Parallel Views\nOverrides View", GH_ParamAccess.item);
            //pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit\nOverrides Target to set scale\nOverrides View", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Projection", "P[]", "View Projection \nAttach Value List for list of projections\nOverrides View", GH_ParamAccess.item);
            
            var viewParam = new Params.Param_View();
            viewParam.Hidden = true;
            pManager.AddParameter(viewParam, "View", "V", "Define using Make2D view components\nGood for Perspective and Named Views", GH_ParamAccess.item);
            /*
             */

            Params.Input[0].Optional = true;
            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
            Params.Input[4].Optional = true;
            Params.Input[5].Optional = true;
            Params.Input[6].Optional = true;

            Param_Integer obj = (Param_Integer)pManager[5];
            obj.AddNamedValue("None", 0);
            obj.AddNamedValue("Top", 1);
            obj.AddNamedValue("Bottom", 2);
            obj.AddNamedValue("Left", 3);
            obj.AddNamedValue("Right", 4);
            obj.AddNamedValue("Front", 5);
            obj.AddNamedValue("Back", 6);
            obj.AddNamedValue("Perspective", 7);
            obj.AddNamedValue("Two-Point Perspective", 8);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "R", "Success or Failure for each detail", GH_ParamAccess.item);
            var guidParam = new Param_Guid();
            pManager.AddParameter(guidParam, "GUID", "G", "GUID for Detail Object", GH_ParamAccess.item);
            pManager.AddTextParameter("Display", "D", "Display Mode", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit", GH_ParamAccess.item);
            pManager.AddTextParameter("Projection", "P", "Viewport Projection", GH_ParamAccess.item);

            var viewParam = new Params.Param_View();
            viewParam.Hidden = true;
            pManager.AddParameter(viewParam, "View", "V", "Input View from Make2D Components", GH_ParamAccess.item);
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

            //used for qualified override
            bool targetDefined = false;
            /*
            bool scaleDefined = false;
            */

            Guid detailGUID = Guid.Empty;
            DA.GetData("GUID", ref detailGUID);
            Rhino.DocObjects.DetailViewObject detail = Rhino.RhinoDoc.ActiveDoc.Objects.FindId(detailGUID) as Rhino.DocObjects.DetailViewObject; ;
            if (detail == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Detail is not valid");
                return;
            }

            /*
            Point3d target = new Point3d();
            if (!DA.GetData("Target", ref target)) target = detail.Viewport.CameraTarget;
            //else targetDefined = true;
            */
            Box targetBox = Box.Unset;
            if (!DA.GetData("Target", ref targetBox)) targetBox = new Box(new BoundingBox(detail.Viewport.CameraTarget, detail.Viewport.CameraTarget));
            else targetDefined = true;
            BoundingBox targetBBox = targetBox.BoundingBox;
            
            double scale = 1.0;
            if (!DA.GetData("Scale", ref scale)) scale = detail.DetailGeometry.PageToModelRatio;
            //else scaleDefined = true;

           
            int? pNum = null;
            if (!DA.GetData("Projection", ref pNum)) pNum = 0;
            
            //Check that Projection is valid
            if (!Enum.IsDefined(typeof(DefinedViewportProjection), pNum))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, pNum + " is not a valid Projection number. Projection will not be modified");
            DefinedViewportProjection projection = (DefinedViewportProjection)pNum;
            
            string dName = string.Empty;
            if (!DA.GetData("Display", ref dName)) dName = detail.Viewport.DisplayMode.EnglishName;

            //convert LocalName to EnglishName
            if (ValList.GetDisplaySettingsList(true).Contains(dName))
                dName = ValList.GetDisplaySettingsList(false)[ValList.GetDisplaySettingsList(true).IndexOf(dName)];
            
            if (!ValList.GetDisplaySettingsList(false).Contains(dName))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, dName + " is not a valid Display Mode name");
            
            DisplayModeDescription displayMode = DisplayModeDescription.GetDisplayModes().First(mode => mode.DisplayAttributes.EnglishName == dName);
            
            Make2DViewInfoGoo view = new Make2DViewInfoGoo();
            if (!DA.GetData("View", ref view)) view = new Make2DViewInfoGoo(new Rhino.DocObjects.ViewportInfo(detail.Viewport));
            else
            {
                if (!targetDefined) targetBBox = new BoundingBox(view.Value.TargetPoint, view.Value.TargetPoint);
                //if (!scaleDefined) scale = view.Value.; //Don't know what to set here
            }
            /*
            */

            if (Execute || run)
            {

                //Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, target, scale, projection, displayMode, view.Value);
                //view = new Make2DViewInfoGoo(new Rhino.DocObjects.ViewportInfo(detail.Viewport));
                //Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, target, scale, projection, displayMode);
                //Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, targetBox.Center, scale, projection, displayMode);
                Rhino.Commands.Result detailResult = Layout.ReviseDetail(detail, targetBBox, scale, projection, displayMode, view.Value);
                              
                DA.SetData("Result", detailResult);
            }
            DA.SetData("GUID", detailGUID);
            DA.SetData("Display", detail.Viewport.DisplayMode.EnglishName);
            DA.SetData("Target", detail.Viewport.CameraTarget); 
            //double scaleOut = detail.Viewport.IsParallelProjection ? detail.DetailGeometry.PageToModelRatio : double.NaN; // alternative for Scale on non-parallel views
            DA.SetData("Scale", detail.DetailGeometry.PageToModelRatio);
            DA.SetData("Projection", detail.Viewport.Name); 
            DA.SetData("View", new Make2DViewInfoGoo(new Rhino.DocObjects.ViewportInfo(detail.Viewport)));
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
            get { return new Guid("fbc48c30-816b-11ee-836d-9198bca92de7"); }
        }

        #region Add Value Lists
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            
            Menu_AppendItem(menu, "Add List of Display Modes", Menu_DisplayClick);
            Menu_AppendItem(menu, "Add List of Projections", Menu_ViewClick);
        }

        private void Menu_ViewClick(object sender, EventArgs e)
        {
            
            string[] pNames = Enum.GetNames(typeof(DefinedViewportProjection));
            List<string> projNames = pNames.Select(v => v.ToString()).ToList();
            List<int> pVals = ((DefinedViewportProjection[])Enum.GetValues(typeof(DefinedViewportProjection))).Select(c => (int)c).ToList();
            List<string> projVals = pVals.ConvertAll(v => v.ToString());


            if (!ValList.AddOrUpdateValueList(this, 5, "Views", "Pick Projection: ", projNames, projVals))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

            ExpireSolution(true);
        }

        private void Menu_DisplayClick(object sender, EventArgs e)
        {
            List<string> dNames = ValList.GetDisplaySettingsList(false);
            List<string> dLocalNames = ValList.GetDisplaySettingsList(true);

            if (!ValList.AddOrUpdateValueList(this, 2, "Display Mode", "Pick DisplayMode: ", dLocalNames, dNames))
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

            Params.Input[2].ObjectChanged += InputParamChanged;
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
            if (sender.NickName == Params.Input[5].NickName)
            {
                // optional feedback
                // Rhino.RhinoApp.WriteLine("This is the right input");

                //List<string> standardViewNames = ValList.GetStandardViewList();
                string[] pNames = Enum.GetNames(typeof(DefinedViewportProjection));
                List<string> projNames = pNames.Select(v => v.ToString()).ToList();
                List<int> pVals = ((DefinedViewportProjection[])Enum.GetValues(typeof(DefinedViewportProjection))).Select(c => (int)c).ToList();
                List<string> projVals = pVals.ConvertAll(v => v.ToString());

                //try to modify input as a valuelist
                try
                {
                    ValList.UpdateValueList(this, 5, "Views", "Pick Projection: ", projNames, projVals);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
             
            if (sender.NickName == Params.Input[2].NickName)
            { 
                List<string> displayNames = ValList.GetDisplaySettingsList(true);
                List<string> displayVals = ValList.GetDisplaySettingsList(false);

                //try to modify input as a valuelist
                try
                {
                    ValList.UpdateValueList(this, 2, "Display", "Pick Display: ", displayNames, displayVals);
                    ExpireSolution(true);
                }
                //if it's not a value list, ignore
                catch (Exception) { };
            }
        }


        #endregion AutoValueList
    }
}