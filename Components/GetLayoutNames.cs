using Grasshopper.Kernel;
using Rhino.Display;
using System;
using System.Collections.Generic;

namespace DraftHorse.Component
{
    public class GetLayoutNames : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetLayoutNames class.
        /// </summary>
        public GetLayoutNames()
          : base("Get Layout Names", "GetLONames",
              "Get LayoutNames by Index or get all (default)",
              "DraftHorse", "Layouts")
        {
        }



        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Update", Menu_DoClick);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            this.ExpireSolution(true);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// 
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Params.Input[pManager.AddIntegerParameter("Index", "Li", "Indices for Layouts to return names", GH_ParamAccess.list)].Optional = true;
            //Params.Input[pManager.AddBooleanParameter("Update", "U", "Use button, not Toggle.  Update from the Layout Table", GH_ParamAccess.item)].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Names", "N", "Names for layouts at indices", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //bool update = false;
            //DA.GetData("Update", ref update);

            //if (update) this.ExpireSolution(true);

            List<string> names = new List<string>();
            List<int> index = new List<int>();

            DA.GetDataList("Index", index);

            RhinoPageView[] page_views = Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews();


            //Reorder Pages based on Page Number
            //why reorder?
            RhinoPageView[] namesByPageNumber = new RhinoPageView[page_views.Length];
            for (int i = 0; i < page_views.Length; i++)
            {
                namesByPageNumber[page_views[i].PageNumber] = page_views[i];
            }

            // if (no input on index, then return all)
            int inputCount = Params.Input[0].SourceCount;

            //AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, inputCount + " inputs");

            if (inputCount == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default with no input: get all Layout Names");
                List<string> allNames = new List<string>();

                for (int i = 0; i < page_views.Length; i++)
                {
                    allNames.Add(page_views[i].PageName);
                }
                names = allNames;
            }
            else
            {
                List<string> pageNames = new List<string>();
                for (int i = 0; i < index.Count; i++)
                {
                    pageNames.Add(page_views[index[i]].PageName);
                }
                names = pageNames;
            }

            DA.SetDataList("Names", names);
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
                return Properties.Resources.Layout_NameTag;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ef05781d-49c7-47d9-ae7e-e0048005f887"); }
        }

        //public override void CreateAttributes()
        //{
        //    base.m_attributes = new MyAttributes_Custom(this);
        //}
    }

    /*
    /// <summary>
    /// borrowed from https://github.com/dantaeyoung/GrasshopperExchange/blob/master/Hairworm/HairwormComponent.cs
    /// </summary>
    public class MyAttributes_Custom : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        GH_Component thisowner = null;
        public MyAttributes_Custom(GH_Component owner) : base(owner) { thisowner = owner; }
        public Grasshopper.GUI.Canvas.GH_Capsule button = null;

        protected override void Layout()
        {
            base.Layout();

            // Draw a button 
            System.Drawing.Rectangle rec0 = GH_Convert.ToRectangle(Bounds);
            rec0.Height += 22;

            System.Drawing.Rectangle rec1 = rec0;
            rec1.Y = rec1.Bottom - 22;
            rec1.Height = 22;
            rec1.Inflate(-2, -2);

            Bounds = rec0;
            ButtonBounds = rec1;
        }
        private System.Drawing.Rectangle ButtonBounds { get; set; }

        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                button = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, "Update", 2, 0);
                button.Render(graphics, Selected, Owner.Locked, false);
                button.Dispose();
            }
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec = ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
}
        */
}
