using Grasshopper.Kernel;
using System;


namespace DraftHorse.Params
{
    class Param_BooleanToggle : Grasshopper.Kernel.Parameters.Param_Boolean
    {
        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Add Toggle", Menu_DoClick);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            var bToggle = new Grasshopper.Kernel.Special.GH_BooleanToggle();
            bToggle.Name = this.Name;
            bToggle.NickName = this.Name;

            bToggle.CreateAttributes();

            bToggle.Attributes.Pivot = new System.Drawing.PointF((float)
                   this.Attributes.InputGrip.X - 150,
                   (float)this.Attributes.Bounds.Y);


            GH_Document GrasshopperDocument = this.OnPingDocument();
            if (GrasshopperDocument == null)
                throw new ArgumentNullException("Grasshopper document is null");

            GrasshopperDocument.AddObject(bToggle, false);

            this.AddSource(bToggle);
            bToggle.ExpireSolution(true);


        }
    }
}
