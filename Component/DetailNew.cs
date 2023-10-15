using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Commands;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static DraftHorse.Helper.Layout;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DraftHorse.Component
{
    public class DetailNew : Base.DH_ButtonComponent
    {
        //Goal: Change from instantiator to separate Detail settings so that a detail can either be modified or baked

        /// <summary>
        /// Initializes a new instance of the AddDetail class.
        /// </summary>
        public DetailNew()
          : base("New Detail", "NewDetail",
              "Add a new detail to an existing layout",
              "DraftHorse", "Layout-Add")
        {
            ButtonName = "Create";
        }


        //This hides the component from view!  is it callable?  Don't know.
        public override GH_Exposure Exposure => GH_Exposure.primary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggle = new Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggle, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            
            pManager.AddIntegerParameter("Index", "Li[]", "Layout index for new detail\nAdd ValueList to get list of layouts", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Detail Bounds", "B", "Detail Boundary Rectangle on Layout Page", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item, new Rhino.Geometry.Point3d(0,0,0));
            pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Projection", "P[]", "View Projection \nAttach Value List for list of projections", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("DisplayMode", "D[]", "Display Mode \nAttach Value List for list of Display Modes", GH_ParamAccess.item, "Wireframe");
            
            //Add Name?
            //Add Layer or other attributes?

            //attributes: Name, Layer, Space OR existing detail object (goal).
            //or point as target for detail
            
            //need to define detail location on page.  use rectangle?

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "R", "Success or Failure for each detail", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "Li", "Index of Layout detail was created on", GH_ParamAccess.item);
            var guidParam = new Grasshopper.Kernel.Parameters.Param_Guid();
            pManager.AddParameter(guidParam, "Detail GUID", "D", "GUID for Detail Object", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "T", "Camera Target for Detail", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Page Units per Model Unit", GH_ParamAccess.item);
            pManager.AddTextParameter("ViewName", "V", "ViewPort Viewname", GH_ParamAccess.item);
            pManager.AddTextParameter("DisplayMode", "D", "Display Mode", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            DA.GetData("Run", ref run);

            int index = new int();
            DA.GetData("Index", ref index);

            Rhino.Display.RhinoPageView pageView = GetPage(index);

            Rectangle3d dBounds = new Rectangle3d();
            DA.GetData("Detail Bounds", ref dBounds);

            Point3d target = new Point3d();
            DA.GetData("Target", ref target);

            double scale = 1.0;
            DA.GetData("Scale", ref scale);

            int pNum = 0;
            DA.GetData("Projection", ref pNum);

            if (!Enum.IsDefined(typeof(Rhino.Display.DefinedViewportProjection), pNum))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, pNum + " is not a valid Projection number. Projection will not be modified");
            }

            Rhino.Display.DefinedViewportProjection projection = (Rhino.Display.DefinedViewportProjection)pNum;

            string dName = String.Empty;
            DA.GetData("DisplayMode", ref dName);
            DisplayModeDescription displayMode = null;

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

            RhinoDoc doc = RhinoDoc.ActiveDoc;

            Result result = Rhino.Commands.Result.Failure;
            
            if (Execute || run)
            {
                if (pageView != null)
                {
                    //goal: Check that detail fits on page
                    Rectangle3d pageBounds = new Rectangle3d(Plane.WorldXY, pageView.PageWidth, pageView.PageHeight);
                    bool upperLeft = pageBounds.Contains(dBounds.Corner(3)) == PointContainment.Inside;
                    bool lowerRight = pageBounds.Contains(dBounds.Corner(1)) == PointContainment.Inside;
                    if (!upperLeft || !lowerRight)
                          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "New detail is (atleast partially) outside of layout boundaries");
                    
                    var detail = pageView.AddDetailView("ModelView", new Point2d(dBounds.Corner(3)), new Point2d(dBounds.Corner(1)), projection);
                    if (detail != null)
                    {
                        pageView.SetActiveDetail(detail.Id);
                        if (!projection.Equals(DefinedViewportProjection.None)) detail.Viewport.SetProjection(projection, projection.ToString(), true);
                        //doc.NamedViews.Restore(nViewIndex, detail.Viewport);
                        detail.Viewport.SetCameraTarget(target, true);
                        detail.Viewport.DisplayMode = displayMode;
                        detail.CommitViewportChanges();

                        detail.DetailGeometry.IsProjectionLocked = false;
                        detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, scale, doc.PageUnitSystem);
                        detail.CommitChanges();

                        result = Rhino.Commands.Result.Success;
                    }
                    pageView.SetPageAsActive();
                    doc.Views.ActiveView = pageView;
                    doc.Views.Redraw();
                    
                    DA.SetData("Result", result);
                    DA.SetData("Index", pageView.PageNumber);
                    DA.SetData("Detail GUID", detail.Id);
                    DA.SetData("Target", detail.Viewport.CameraTarget);
                    DA.SetData("Scale", detail.DetailGeometry.PageToModelRatio);
                    DA.SetData("ViewName", detail.Viewport.Name);
                    DA.SetData("DisplayMode", detail.Viewport.DisplayMode.EnglishName);

                }
            }

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


            if (!ValList.AddOrUpdateValueList(this, 5, "Views", "Pick Projection: ", projNames, projVals))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

            ExpireSolution(true);
        }

        private void Menu_DisplayClick(object sender, EventArgs e)
        {
            List<string> dNames = ValList.GetDisplaySettingsList(false);
            List<string> dLocalNames = ValList.GetDisplaySettingsList(true);

            if (!ValList.AddOrUpdateValueList(this, 6, "DisplayMode", "Pick DisplayMode: ", dLocalNames, dNames))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ValueList at input [" + 0 + "] failed to update");

            ExpireSolution(true);
        }
        #endregion Add Value Lists

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.LayoutNewDetail_bitmap;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("92c1c7ab-9640-452a-bafe-a7bd4bcbf323"); }
        }
    }
}