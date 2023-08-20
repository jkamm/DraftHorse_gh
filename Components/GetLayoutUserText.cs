using System;
using Grasshopper.Kernel;
using DraftHorse.Helper;

namespace DraftHorse.Component
{
    public class GetLayoutUserText : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetLayoutUserText class.
        /// </summary>
        public GetLayoutUserText()
          : base("Get Layout UserText", "GetLOUserText",
              "Retrieve a Layout's User Text",
              "Drafthorse", "Layouts")
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "Li", "Indices for Layouts. /nGet using 'Get Layout Index'", GH_ParamAccess.item);
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

            Rhino.Display.RhinoPageView pageView = Layout.GetPage(index);

            if(pageView != null)
            {
                var userText = pageView.MainViewport.GetUserStrings();
                                
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

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GetLayoutKV;
        
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0b6db220-1d27-4b7a-b70a-63de00bc54b1"); }
        }
    }
}