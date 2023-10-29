using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DraftHorse.Component
{
    public class DocTextEdit : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the SetDocText class.
        /// </summary>
        public DocTextEdit()
          : base("Edit Document Text", "EditDocText",
              "Get and/or Set Key/Value pairs to the Document Text Table. \nStrings starting with '.' are hidden from users",
              "Drafthorse", "Doc Text")
        {
            ButtonName = "Write";
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            Params.Input[pManager.AddTextParameter("Keys", "K", "Keys for Doc Texts (unique)", GH_ParamAccess.list)].Optional = true;
            Params.Input[pManager.AddTextParameter("Values", "V", "Values for Doc Texts (must match Key count)", GH_ParamAccess.list)].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Keys", "K", "Keys written to Document", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "V", "Values written to Document", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.DocObjects.Tables.StringTable docText = Rhino.RhinoDoc.ActiveDoc.Strings;
            string[] allKeys = new string[docText.Count];

            for (int i = 0; i < docText.Count; i++) allKeys[i] = docText.GetKey(i);

            List<string> newKeys = new List<string>();
            DA.GetDataList("Keys", newKeys);

            List<string> newVals = new List<string>();
            DA.GetDataList("Values", newVals);

            bool run = false;
            if (!DA.GetData("Run", ref run)) run = false;

            //Goal: Check that there are the same number of Keys and Values
            if (newKeys.Count != newVals.Count) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Key/Value Pairs do not match. Double-check the data structure");

            //var userText = thisPage.MainViewport.GetUserStrings();

            IEnumerable<string> keysFound = from key in newKeys
                                            where Array.IndexOf(allKeys, key) != -1
                                            select key;
            if (keysFound.Count() != 0) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Duplicate Keys Found, all duplicates will be replaced");

            if (run || Execute)
            {
                for (int i = 0; i < newKeys.Count; i++)
                {
                    //if (thisPage.MainViewport.GetUserString(iKeys[j]) != null) Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Duplicate Keys Found, all duplicates will be replaced");
                    docText.SetString(newKeys[i], newVals[i]);
                }
            }
                        

            docText = Rhino.RhinoDoc.ActiveDoc.Strings;
            
            string[] allKeysPost = new string[docText.Count];
            string[] allVals = new string[docText.Count];

            for (int i = 0; i < docText.Count; i++)
            {
                allKeysPost[i] = docText.GetKey(i);
                allVals[i] = docText.GetValue(i);
            }
            DA.SetDataList("Keys", allKeysPost.ToList());
            DA.SetDataList("Values", allVals.ToList());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetDocText;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9ee69741-b39c-40dc-810f-4d3f8bce16f4"); }
        }
    }
}