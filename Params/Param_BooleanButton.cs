using Grasshopper.Kernel;
using System;


namespace DraftHorse.Params
{
    class Param_BooleanButton : Grasshopper.Kernel.Parameters.Param_Boolean
    {
        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Add Button", Menu_DoClick);
        }

        private void Menu_DoClick(object sender, EventArgs e)
        {
            var bButton = new Grasshopper.Kernel.Special.GH_ButtonObject
            {
                Name = this.Name,
                NickName = this.Name
            };

            bButton.CreateAttributes();

            bButton.Attributes.Pivot = new System.Drawing.PointF((float)
                   this.Attributes.InputGrip.X - 150,
                   (float)this.Attributes.Bounds.Y);

            GH_Document GrasshopperDocument = this.OnPingDocument();
            if (GrasshopperDocument == null)
                throw new ArgumentNullException("Grasshopper document is null");

            GrasshopperDocument.AddObject(bButton, false);

            this.AddSource(bButton);
            bButton.ExpireSolution(true);
        }
    }
}
