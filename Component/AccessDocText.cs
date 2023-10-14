using Grasshopper.Kernel;
using System;

namespace DraftHorse.Component
{
    public class AccessDocText : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AccessDocText class.
        /// </summary>
        public AccessDocText()
          : base("Access Document Text", "AccessDocText",
              "Retrieve a value from the Document Text by key",
              "Drafthorse", "Doc Text")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Key", "K", "Key(s) to return Value for.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Value", "V", "Values corresponding to Key", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string key = string.Empty;
            DA.GetData("Key", ref key);

            var docText = Rhino.RhinoDoc.ActiveDoc.Strings;

            string value = docText.GetValue(key);
            //Add message if key is not found in Document Text
            if (value == null) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Key not found in Document Text");
            else DA.SetData("Value", value);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AccessDocText;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("27a90b59-6890-48e2-b4d2-8ed919ffb5e3"); }
        }
    }
}