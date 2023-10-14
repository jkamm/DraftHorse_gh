using DraftHorse.Helper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;


namespace DraftHorse.Component
{
    public class ModifyText : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the ModifyText class.
        /// </summary>
        public ModifyText()
          : base("Modify Text", "LOText",
              "Replace text of Named Text Objects on a Layout with new text",
              "DraftHorse", "Layout-Modify")
        {
            ButtonName = "Modify";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Run the Component", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Index", "Li", "Indexed Layout to change", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Names of Text Objects to replace", GH_ParamAccess.list);
            pManager.AddTextParameter("Text", "T", "New Text for Text Objects", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "R", "Success or Failure for each text", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            DA.GetData("Run", ref run);

            //DA.GetData("Layout", ref name);

            List<string> textKeys = new List<string>();
            DA.GetDataList("Name", textKeys);

            List<string> textVals = new List<string>();
            DA.GetDataList("Text", textVals);

            int index = new int();
            DA.GetData("Index", ref index);

            #region EscapeBehavior
            //Esc behavior code snippet from 
            // http://james-ramsden.com/you-should-be-implementing-esc-behaviour-in-your-grasshopper-development/
            if (GH_Document.IsEscapeKeyDown())
            {
                GH_Document GHDocument = OnPingDocument();
                GHDocument.RequestAbortSolution();
            }
            #endregion EscapeBehavior

            List<Rhino.Commands.Result> results = new List<Rhino.Commands.Result>();

            if (run || Execute)
            {
                //goal: Test validity of all input before processing?  Currently will copy a template before producing name error.

                //Check that Keys and Values have same length
                if (textKeys.Count == textVals.Count)
                {
                    Rhino.Display.RhinoPageView page = Layout.GetPage(index);
                    results = Layout.ReplaceText(textKeys, textVals, page);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Must have equal number of Keys and Values");
                }
                DA.SetDataList("Result", results);
            }
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
                return Properties.Resources.LayoutText_bitmap;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("50eb7d93-793f-4cdc-86f0-565fe24ccdde"); }
        }
    }
}