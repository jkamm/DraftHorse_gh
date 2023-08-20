using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DraftHorse.Component
{
    public class GetDocText : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetDocText class.
        /// </summary>
        public GetDocText()
          : base("Get Document Text", "GetDocText",
              "Return Document Text Table as Keys and Values",
              "Drafthorse", "Layouts")
        {
        }
     
        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //Params.Input[pManager.AddBooleanParameter("Update", "U", "Update Document Text", GH_ParamAccess.item)].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Keys", "K", "Document Text Keys", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "V", "Document Text Values", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.DocObjects.Tables.StringTable docText = Rhino.RhinoDoc.ActiveDoc.Strings;


            string[] allKeysPost = new string[docText.Count];
            string[] allVals = new string[docText.Count];

            for (int i = 0; i < docText.Count; i++)
            {
                //Print("Key {0}: {1}", i, docText.GetKey(i));
                allKeysPost[i] = docText.GetKey(i);
                allVals[i] = docText.GetValue(i);
            }
            DA.SetDataList("Keys", allKeysPost.ToList());
            DA.SetDataList("Values", allVals.ToList());

        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Update", Menu_DoClick, true);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            base.ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GetDocText;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2a69f3a0-f208-4538-bab3-008458739b78"); }
        }
    }
}