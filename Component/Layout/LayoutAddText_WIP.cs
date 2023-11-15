using Grasshopper.Kernel;
using System;

namespace DraftHorse.Component
{
    public class LayoutAddText_WIP : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddLayoutText class.
        /// </summary>
        public LayoutAddText_WIP()
          : base("Add Layout Text", "LayoutNewText",
              "Add a new text object to a layout",
              "DraftHorse", "Layouts")
        {
        }

        //Hide component until it's ready for primetime.
        public override GH_Exposure Exposure => GH_Exposure.hidden;

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
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LayoutNewText_bitmap;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d53e4c17-6628-4db6-9d60-6589458bb513"); }
        }
    }
}