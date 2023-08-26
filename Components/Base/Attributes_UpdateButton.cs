using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace DraftHorse.Component.Base
{
    public class Attributes_UpdateButton : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        GH_Component thisowner = null;
        public Attributes_UpdateButton(GH_Component owner) : base(owner) { thisowner = owner; }
        public GH_Capsule button = null;

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
}

