using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace DraftHorse.Component
{
    public class NewLayout : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the NewLayout class.
        /// </summary>
        public NewLayout()
          : base("Create New Layout", "NewLayout",
              "Create a new layout from scratch",
              "Drafthorse", "Layout-Make")
        {
            ButtonName = "Create";
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddTextParameter("Name", "N", "PageName for new layout", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddNumberParameter("Height", "H", "Custom Page Height", GH_ParamAccess.item, 11)].Optional = false;
            Params.Input[pManager.AddNumberParameter("Width", "W", "Custom Page Width", GH_ParamAccess.item, 17)].Optional = false; 
            Params.Input[pManager.AddIntegerParameter("Details", "D", "Details (0-4)", GH_ParamAccess.item,4)].Optional = true;
            Params.Input[pManager.AddPointParameter("Target", "T", "Single target for all details on a single layout",GH_ParamAccess.item )].Optional = true;
            Params.Input[pManager.AddNumberParameter("Scale", "S", "Scale for details", GH_ParamAccess.item,1)].Optional = true;
            pManager.AddIntegerParameter("Units", "U", "Sets Page Units\n\n0 = inches\n1 = centimeters\n2 = millimeters", GH_ParamAccess.item,0);
            //Params.Input[pManager.AddTextParameter("Paper", "P[]", "PaperName - Set to Custom to use H and W \nAttach ValueList for Papernames", GH_ParamAccess.item,"Custom")].Optional = false;
            //pManager.AddBooleanParameter("Orientation", "O","Sets Page Orientation\nFalse = Portrait\nTrue = Landscape",GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Layout Index", "Li", "Index of last created Layout(s)", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Name", "N", "Name of last created Layout(s)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //goal: If papername and H and W are undefined, then throw error that one or the other must be defined

            //goal: Add CustomValueList that generates PaperNames - is there a table, or is this universal (by language)? are they elements in an IEnumerable?

            //in dev: Develop NewLayout function that takes different numbers of details, defining them relative to the size of the layout

            //goal: handle all "optional" inputs

            bool run = false;
            DA.GetData("Run", ref run);

            
            //example code for optional handling
            //if (!DA.GetData("Template Index", ref templateIndex)) return;

            //add default pagename if none?

            //Get and define paper UnitSystem
            int units = 0;
            DA.GetData("Units", ref units);
            Rhino.UnitSystem pageUnits = units == 2 ? Rhino.UnitSystem.Millimeters : units == 1 ? Rhino.UnitSystem.Centimeters : Rhino.UnitSystem.Inches;

            double width = 0;
            if (!DA.GetData("Width", ref width)) return;

            double height = 0;
            if (!DA.GetData("Height", ref height)) return;

            Point3d target = new Point3d(0, 0, 0);
            DA.GetData("Target", ref target);

            int detailCount = 0;
            DA.GetData("Details", ref detailCount);
            detailCount = Math.Min(detailCount, 4);
            detailCount = Math.Max(detailCount, 0);

            string pageName = string.Empty;
            if(!DA.GetData("Name", ref pageName)) pageName = "New Layout (" + detailCount.ToString() + " details)"  ;
            
            double scale = 1;
            DA.GetData("Scale", ref scale);
            //do some exception handling on scale?
            scale = Math.Min(scale, 1000);
            scale = Math.Max(scale, 0.001);

            Rhino.Commands.Result result = new Rhino.Commands.Result();
            

            if (Execute || run)
            {
                Rhino.Display.RhinoPageView[] layout = new Rhino.Display.RhinoPageView[1];

                Rhino.RhinoDoc.ActiveDoc.AdjustPageUnitSystem(pageUnits, true);

                result = Layout.AddLayout(pageName, width, height, target, detailCount, scale, out layout[0]);

                if (result != Rhino.Commands.Result.Success)
                {
                    return;
                }
                layoutIndex = layout[0].PageNumber.ToString();
                layoutName = layout[0].PageName;
            }

            if (layoutIndex != string.Empty) DA.SetData("Layout Index", int.Parse(layoutIndex));
            if (layoutName != string.Empty) DA.SetData("Layout Name", layoutName);

            return;


        }
        string layoutIndex = string.Empty;
        string layoutName = string.Empty;
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.NewLayout;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2C185D1A-6FD1-4F2F-BB08-1807247DD10C"); }
        }
    }
}