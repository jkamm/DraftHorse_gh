using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Windows.Forms;
using static DraftHorse.Helper.Layout;


namespace DraftHorse.Component
{
    public class LayoutFromBBox : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public LayoutFromBBox()
          : base("Layout By Bounds", "BBox Layout",
              "Generate a Layout with a single detail using a bounding rectangle",
              "DraftHorse", "Layout")
        {
            ButtonName = "Generate";
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddTextParameter("Name", "N", "PageName for new layout", GH_ParamAccess.item)].Optional = true;
            pManager.AddRectangleParameter("BBox", "B", "Bounding Box target for single detail per Layout", GH_ParamAccess.item);
            //goal: Add view as option for setting projection, default of Plan View.  Rectangle may need to change to Box
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Layout Index", "Li", "Index of new Layout(s)", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Name", "N", "Name of new Layout(s)", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.UnitSystem pageUnits = unitsAreInches ? Rhino.UnitSystem.Inches : Rhino.UnitSystem.Millimeters;

            //goal: add pageUnits to input options (default == inches)

            bool run = false;
            DA.GetData("Run", ref run);

            string pageName = string.Empty;
            DA.GetData("Name", ref pageName);

            string detailName = "mainDetail";

            Rectangle3d detailRec = new Rectangle3d();
            if (!DA.GetData("BBox", ref detailRec)) return;


            double width = detailRec.X.Length;
            double height = detailRec.Y.Length;

            double scale = 1;
            //goal: add scale to input?  then someone could generate reduced size version of a layout, even a standard size, with added scale. 
            //This would have to modify the ratio of modelToPage as well.  

            string layoutName = string.Empty;
            int layoutIndex = new int();

            if (run || Execute)
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
                Rhino.Display.RhinoPageView[] layout = new Rhino.Display.RhinoPageView[1];

                Rhino.RhinoDoc.ActiveDoc.AdjustPageUnitSystem(pageUnits, true);
                double modelToPage = Rhino.RhinoMath.UnitScale(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem, pageUnits);
                width *= modelToPage;
                height *= modelToPage;
                scale = modelToPage;
                

                Rhino.Commands.Result result = AddLayout(pageName, detailName, width, height, detailRec, scale, out layout[0]);
                if (result == Rhino.Commands.Result.Success)
                {
                    layoutName = layout[0].PageName;
                    layoutIndex = layout[0].PageNumber;
                }

                DA.SetData("Layout Index", layoutIndex);
                DA.SetData("Layout Name", layoutName);
            }

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Layout_bitmap;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6410ed9f-d25e-425e-88be-0633cd00456d"); }
        }

        #region Units Message
        //implements a menu item that is preserved on the component, along with undo/redo capability
        private bool unitsAreInches = true;

        public bool Units
        {
            get { return unitsAreInches; }
            set
            {
                unitsAreInches = value;
                if ((unitsAreInches))
                {
                    Message = "inch";
                    //Rhino.RhinoDoc.ActiveDoc.AdjustPageUnitSystem(Rhino.UnitSystem.Inches, true);
                }
                else
                {
                    Message = "millimeter";
                    //Rhino.RhinoDoc.ActiveDoc.AdjustPageUnitSystem(Rhino.UnitSystem.Millimeters, true);
                }
            }
        }
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendSeparator(menu);
            ToolStripMenuItem item = Menu_AppendItem(menu, "layout units", Menu_UnitsClicked, true);
            item.ToolTipText = "Switch layout page units between inch & millimeter.";
        }
        private void Menu_UnitsClicked(object sender, EventArgs e)
        {
            //if createFolders is true, turn to false and if false, turn to true.
            RecordUndoEvent("Units");
            Units = !Units;
            ExpireSolution(true);
        }
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // First add our own field.
            writer.SetBoolean("Units", Units);
            // Then call the base class implementation.
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // First read our own field.
            Units = reader.GetBoolean("Units");
            // Then call the base class implementation.
            return base.Read(reader);
        }

        protected override void BeforeSolveInstance()
        {
            // sets the message to display immediately according to default
            base.BeforeSolveInstance();
            /*
            if (Rhino.RhinoDoc.ActiveDoc.PageUnitSystem == Rhino.UnitSystem.None)
            {
                Units = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem == Rhino.UnitSystem.Inches ? true :
                    Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem == Rhino.UnitSystem.Feet ? true :
                    false;
            }
            else Units = Rhino.RhinoDoc.ActiveDoc.PageUnitSystem == Rhino.UnitSystem.Inches ? true : false;
             */
            Units = unitsAreInches;
        }
        #endregion 
    }
}