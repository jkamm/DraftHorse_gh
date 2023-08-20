using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using static DraftHorse.Helper.XParams;

namespace DraftHorse.Component
{
    public class LayoutBakeAtts : Base.CondensedParamComponent
    {
        /// <summary>
        /// Initializes a new instance of the LayoutBakeAtts class.
        /// </summary>
        public LayoutBakeAtts()
          : base("Layout Bake Attributes", "LOBakeAtts",
              "Attributes for Layout Bake Component",
              "DraftHorse", "Layouts")
        {
            MinInputs = 4;  //Drive this off the list of inputs in the RegisterInputs command?
            MinOutputs = 1;

            XParamInputs = new List<object[]>  //this could be a method of the interface for the condensedParamsComponent
            {
                GenerateNumberParam("Number", "N", "generated number parameter", GH_ParamAccess.item),
                GenerateNumberParam("Number", "N2", "generated Next number parameter", GH_ParamAccess.item),
                GenerateIntegerParam("Integer", "I", "generated integer", GH_ParamAccess.item)
            };
            }

        //while in testing, set component to Hidden    
        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //keys
            pManager.AddTextParameter("Keys", "K", "User Keys", GH_ParamAccess.list);
            //values
            pManager.AddTextParameter("Vals", "V", "User Values", GH_ParamAccess.list);
            //name
            pManager.AddTextParameter("Name", "N", "Object Name", GH_ParamAccess.item);
            //layer

            pManager.AddTextParameter("Layer", "L[]", "Layer Name", GH_ParamAccess.item);
            //space
            pManager.AddTextParameter("Layout Index", "Li[]", "Layout index", GH_ParamAccess.item);
            //other? Color, Material, group, visibility, etc...
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
            get { return new Guid("79c173b7-aa8b-45df-9b12-f3ca1605d3a8"); }
        }
    }
}