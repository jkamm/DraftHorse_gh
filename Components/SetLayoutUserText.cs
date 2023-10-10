using DraftHorse.Helper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DraftHorse.Component
{
    public class SetLayoutUserText : Base.DH_ButtonComponent
    {
        /// <summary>
        /// Initializes a new instance of the SetLayoutUserText class.
        /// </summary>
        public SetLayoutUserText()
          : base("SetLayoutUserText", "SetLOUText",
              "Add Key/Value Pair to Layout's User Text",
              "Drafthorse", "Layouts")
        {
            ButtonName = "Write";
        }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            Params.Input[pManager.AddParameter(bToggleParam, "Run", "R", "Do not use button to activate - toggle only", GH_ParamAccess.item)].Optional = true;
            pManager.AddIntegerParameter("Index", "Li", "Indices for Layouts. \nGet using 'Get Layout Index'", GH_ParamAccess.item);
            pManager.AddTextParameter("Keys", "K", "Keys for User Texts (unique)", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "V", "Values for User Texts (must match Key count)", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Keys", "K", "Keys written to Layout", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "V", "Values written to Layout", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int index = new int();
            DA.GetData("Index", ref index);

            List<string> iKeys = new List<string>();
            DA.GetDataList("Keys", iKeys);

            List<string> iVals = new List<string>();
            DA.GetDataList("Values", iVals);

            bool run = false;
            if (!DA.GetData("Run", ref run)) run = false;


            Rhino.Display.RhinoPageView pageView = Layout.GetPage(index);

            if (run || Execute)
            {
                if (pageView != null)
                {
                    if (iVals.Count != iKeys.Count) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Key/Value pairs do not match. Check the data structure.");

                    var userText = pageView.MainViewport.GetUserStrings();

                    IEnumerable<string> keysFound = from key in iKeys
                                                    where userText.Get(key) != null
                                                    select key;
                    if (keysFound.Count() != 0) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Duplicate Keys Found, all duplicates will be replaced");

                    for (int i = 0; i < iKeys.Count; i++)
                    {
                        pageView.MainViewport.SetUserString(iKeys[i], iVals[i]);
                    }

                    //Get UserText from MainViewPort of Layout                              
                    string[] userVals = new string[userText.Count];
                    if (userText.Count != 0)
                    {
                        for (int i = 0; i < userText.Count; i++)
                        {
                            userVals[i] = userText.Get(i);
                        }
                    }

                    DA.SetDataList("Keys", userText.AllKeys);
                    DA.SetDataList("Values", userVals);
                }
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AddLayoutKV;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ecb359cb-5d8d-46a6-b227-15f7b2053399"); }
        }
    }
}