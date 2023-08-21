using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System.Drawing;

namespace DraftHorse.Component.Base
{
    public class LO_ButtonComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        private bool mouseOver;
        private bool mouseDown;

        private RectangleF buttonArea;
        private RectangleF textArea;

        //GH_Component thisowner = null;

        public LO_ButtonComponentAttributes(GH_Component owner) : base(owner) 
        { 
            //thisowner = owner;
            mouseOver = false;
            mouseDown = false;
        }

        public Grasshopper.GUI.Canvas.GH_Capsule button = null;

        protected override void Layout()
        {

            // Draw a button 
            //Bounds = RectangleF.Empty;
            base.Layout();
            //buttonArea = new RectangleF(Bounds.Left,Bounds.Bottom,Bounds.Width, 20f);
            //textArea = buttonArea;
            //Bounds = RectangleF.Union(Bounds, buttonArea);

            Rectangle rec0 = GH_Convert.ToRectangle(Bounds);
            rec0.Height += 22;

            Rectangle rec1 = rec0;
            rec1.Y = rec1.Bottom - 22;
            rec1.Height = 22;
            rec1.Inflate(-2, -2);

            Bounds = rec0;
            ButtonBounds = rec1;
        }
        private Rectangle ButtonBounds { get; set; }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                button = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, (base.Owner as LO_ButtonComponent).ButtonName, 2, 0);
                button.Render(graphics, Selected, Owner.Locked, false);
                button.Dispose();
            }
        }



        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    //Set Execute to true
                    (base.Owner as LO_ButtonComponent).Execute = true;

                    //Run component
                    Owner.ExpireSolution(true);

                    //Set Execute to false
                    (base.Owner as LO_ButtonComponent).Execute = false;
                    return GH_ObjectResponse.Handled;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
    }
}

