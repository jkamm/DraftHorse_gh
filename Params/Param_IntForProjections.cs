using System;

namespace DraftHorse.Params
{
    class Param_IntForProjections : Grasshopper.Kernel.Parameters.Param_Integer
    {
        /// <summary>
        /// Goal: a custom Param component that allows the user to select and store one or more Projections by name
        /// goal: add list of projections in toolstripmenu that store value to the param
        /// </summary>

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Projection"); //, Menu_DoClick);
            Menu_AppendSeparator(menu);
            //Menu_AppendItem(menu, "Parallel", Menu_SetParallel);
            //Menu_AppendItem(menu, "Perspective", Menu_SetPerspective);
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
            get { return new Guid("2686b601-b661-41a5-8c45-8c8141867e32"); }
        }
    }
}