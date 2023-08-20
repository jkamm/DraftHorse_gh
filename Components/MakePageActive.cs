using Grasshopper.Kernel;
using System;

namespace DraftHorse.Components
{
    public class MakePageActive : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MakePageActive()
          : base("Make Page Active", "Active Page",
              "Make a page active (primarily for Baking)",
              "DraftHorse", "Layouts")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
             *  if (Component.Params.Input[Component.Params.IndexOfInputParam("LayoutIndex")].DataType.ToString() != "void")
    {
      bool run = Run;

      //int LayoutIndex = TemplateIndex;

      #region EscapeBehavior
      //Esc behavior code snippet from
      // http://james-ramsden.com/you-should-be-implementing-esc-behaviour-in-your-grasshopper-development/
      if (GH_Document.IsEscapeKeyDown())
      {
        GrasshopperDocument.RequestAbortSolution();
      }
      #endregion EscapeBehavior

      if (run)
      {
        RhinoPageView activePage = GetPage(LayoutIndex, Component);
        RhinoDocument.Views.ActiveView = activePage;
        Result = true;
      }
    }
             */
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BB2E2A26-F6E8-45A3-A31A-F20AF37A727B"); }
        }
    }
}