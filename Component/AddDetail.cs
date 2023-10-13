using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace DraftHorse.Component
{
    public class AddDetail : GH_Component
    {
        //Goal: Change from instantiator to separate Detail settings so that a detail can either be modified or baked

        /// <summary>
        /// Initializes a new instance of the AddDetail class.
        /// </summary>
        public AddDetail()
          : base("Add Detail", "AddDetail",
              "Add a new detail to an existing layout",
              "DraftHorse", "Layouts")
        {
        }


        //This hides the component from view!  is it callable?  Don't know.
        public override GH_Exposure Exposure => GH_Exposure.hidden;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item);
            Params.Input[0].Optional = true;

            pManager.AddIntegerParameter("Index", "Li", "Indexed Layout", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of Detail to replace", GH_ParamAccess.list);
            pManager.AddRectangleParameter("Viewing Rectangles", "R", "Target View for Detail", GH_ParamAccess.list);//attributes: Name, Layer, Space OR existing detail object (goal).
            //or point as target for detail

            //Layout name[] (Space)
            //namedView[]
            //target (pt)
            //scale (double)
            //Display Mode[]

            //need to define detail location on page.  use rectangle?

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //Done - success
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> indexList = new List<int>();
            indexList.Sort();
            indexList.Reverse();

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