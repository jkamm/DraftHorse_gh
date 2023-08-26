using DraftHorse.Helper;
using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;

namespace DraftHorse.Component
{
    public class LayoutSortTabs : Base.DH_ButtonComponent
    {
        //Obsolete: No longer necessary in Rhino 7
        //goal: exception handling - what happens if there is a duplicate index? throw exception/error?

        /// <summary>
        /// Initializes a new instance of the LayoutSortTabs class.
        /// </summary>
        public LayoutSortTabs()
          : base("Layout Sort Tabs", "LOSort",
              "Sort Layout Tabs in Rhino\nDefault Sorts Layouts by Name",
              "DraftHorse", "Layouts")
        {
            ButtonName = "Sort";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //Layout Indices (as tree, run once)
            pManager.AddIntegerParameter("Layout Index", "Li", "Layout Indices in desired order", GH_ParamAccess.tree);
            //Run
            var bButtonParam = new DraftHorse.Params.Param_BooleanButton();
            pManager.AddParameter(bButtonParam, "Run", "R", "Button or toggle", GH_ParamAccess.item);
            //Revert
            var bToggleParam = new DraftHorse.Params.Param_BooleanToggle();
            pManager.AddParameter(bToggleParam, "Revert", "Re", "Revert order to Document Pageview Table\nDo not use button - toggle only", GH_ParamAccess.item);
            //ByName (set this as a message too?)

            Params.Input[0].Optional = true;
            Params.Input[1].Optional = true;
            Params.Input[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //Success/Done
            pManager.AddBooleanParameter("Done", "D", "True on operation complete", GH_ParamAccess.item);
            //Out - List of strings
            pManager.AddTextParameter("Result", "R", "resulting state of layouts", GH_ParamAccess.list);
        }



        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Setup input variables            
            bool run = false;
            DA.GetData("Run", ref run);

            bool revert = false;
            DA.GetData("Revert", ref revert);

            var index = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Integer>();
            DA.GetDataTree("Layout Index", out index);

            //If no inputs to List Index, sorts by name
            int inputCount = this.Params.Input[0].SourceCount;
            bool byName = inputCount == 0 ? true : false;

            //Setup output variables
            List<string> results = new List<string>();
            bool done = false;

            //Always flatten input list of indices
            var indexList = index.FlattenData();
            int thisInt = new int();

            HashSet<int> indexSet = new HashSet<int>();
            List<int> uniqIndexList = new List<int>();

            var x = uniqIndexList;

            //Get the layout based on the name (not the LayoutNumber)
            //how does order-of operations matter?
            //Should do lowest number first.  If the first in is the new first,
            //then the renumbering can be comprehensive, starting at 0.
            var page_views = RhinoDoc.ActiveDoc.Views.GetPageViews();
            var staticPageViews = RhinoDoc.ActiveDoc.Views.GetPageViews();

            //Check pre-sorted state
            results.Add("Presorted State: ");
            for (int i = 0; i < page_views.Length; i++)
                results.Add(string.Format("Page {0}: [{1}] = {2}", page_views[i].PageName, i, page_views[i].PageNumber));

            if (run || Execute)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    GH_Convert.ToInt32(indexList[i], out thisInt, GH_Conversion.Both);
                    if (indexSet.Add(thisInt))
                        uniqIndexList.Add(thisInt);
                }

                PageNumCompare pageNumCompare = new PageNumCompare();
                PageNameCompare pageNameCompare = new PageNameCompare();

                if (!revert)
                {
                    //exception handling to check that layouts exist. If it doesn't, throws Exception
                    try
                    {
                        for (int i = 0; i < uniqIndexList.Count; i++)
                        {
                            page_views[uniqIndexList[i]].PageNumber = i;
                        }
                    }
                    catch (Exception)
                    {
                        throw new IndexOutOfRangeException("No layouts beyond index [" + (page_views.Length - 1).ToString() + "]");
                    }


                    if (byName) Array.Sort(page_views, pageNameCompare);
                    else Array.Sort(page_views, pageNumCompare);
                }
                else page_views = staticPageViews;

                for (int i = 0; i < page_views.Length; i++)
                {
                    page_views[i].PageNumber = i;
                }

                Rhino.RhinoApp.RunScript("ViewportTabs Hide ViewportTabs Show", true);
                done = true;
            }

            //Check post-sorted state
            //goal: change to PageNumber order, preserve original order.  Create new array? Concurrently sort list of original indices?  
            if (done == true)
            {
                results.Add("Post-sorted State: ");
                for (int i = 0; i < page_views.Length; i++)
                    results.Add(string.Format("Page {0}: [{1}] = {2}", staticPageViews[i].PageName, i, staticPageViews[i].PageNumber));
            }

            DA.SetDataList("Result", results);
            DA.SetData("Done", done);

        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => DraftHorse.Properties.Resources.LayoutSort;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("64769836-5b94-4aa0-80c3-542a6c6a5b45"); }
        }
    }
}