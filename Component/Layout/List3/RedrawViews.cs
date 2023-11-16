using System;
using Grasshopper.Kernel;
using Rhino;
using Grasshopper.Kernel.Special;


namespace DraftHorse.Component.Layout.List3
{
    public class RedrawViews : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RedrawViews class.
        /// </summary>
        public RedrawViews()
          : base("RedrawViews", "Redraw",
              "Updates Views while in Paper Space (not Model Space)",
              "Drafthorse", "Layout")
        {
        }
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
            //This seems not to do anything... Custom Preview component has the desired effect without all the nonsense

            //if (RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ViewportType == Rhino.Display.ViewportType.PageViewMainViewport
            //    || RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ViewportType == Rhino.Display.ViewportType.DetailViewport) 
            //{
            RhinoDoc.ActiveDoc.Views.Redraw();
            //}
        }
       

        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);

            GH_Timer timer = new GH_Timer();

            document.AddObject(timer, false);

            timer.Attributes.Pivot = new System.Drawing.PointF((float)
            Attributes.DocObject.Attributes.Pivot.X - timer.Attributes.Bounds.Width-50,
            (float)(Attributes.DocObject.Attributes.Pivot.Y-12));

            timer.AddTarget(InstanceGuid);
            timer.Interval = 100;
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
            get { return new Guid("194986E9-780D-4CD8-AA67-60AF31D9C606"); }
        }
    }
}