using DraftHorse.Component.Base;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DraftHorse.Component.Base
{
    public class DH_ButtonComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        private bool mouseOver;
        private bool mouseDown;
        public bool Active { get; set; }

        public event EventHandler ButtonPressed;

       //GH_Component thisowner = null;

        public DH_ButtonComponentAttributes(DH_ButtonComponent owner) : base(owner)
        {
            //thisowner = owner;
            mouseOver = false;
            mouseDown = false;
            Active = false;
            ButtonPressed += owner.OnButtonActivate;
        }

        public GH_Capsule button = null;

        protected override void Layout()
        {
            base.Layout();

            RectangleF rec0 = GH_Convert.ToRectangle(Bounds);
            rec0.Height += 22;

            RectangleF rec1 = rec0;
            rec1.Y = rec1.Bottom - 22;
            rec1.Height = 22;
            rec1.Inflate(-2, -2);

            Bounds = rec0;
            ButtonBounds = rec1;
        }
        private RectangleF ButtonBounds { get; set; }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Black, Selected, Owner.Locked, hidden: true);
                GH_Capsule gH_Capsule = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, (base.Owner as DH_ButtonComponent).ButtonName, 2, 9);
                gH_Capsule.RenderEngine.RenderBackground(graphics, canvas.Viewport.Zoom, impliedStyle);
                if (!mouseDown)
                {
                    gH_Capsule.RenderEngine.RenderHighlight(graphics);
                }
                gH_Capsule.RenderEngine.RenderOutlines(graphics, canvas.Viewport.Zoom, impliedStyle);
                if (mouseOver)
                {
                    gH_Capsule.RenderEngine.RenderBackground_Alternative(graphics, Color.FromArgb(50, Color.Gray), drawAlphaGrid: false);
                }

                gH_Capsule.RenderEngine.RenderText(graphics, Color.White);
                gH_Capsule.Dispose();
            }
        }

        /*
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f && ButtonBounds.Contains(e.CanvasLocation))
            {
                mouseDown = true;
                DH_ButtonComponent dH_ButtonComponent = Owner as DH_ButtonComponent;
                (base.Owner as DH_ButtonComponent).Execute = true;
                Owner.RecordUndoEvent("Execute Action");
                Owner.ExpireSolution(recompute: true);
                return GH_ObjectResponse.Capture;
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (!ButtonBounds.Contains(e.CanvasLocation))
            {
                mouseOver = false;
            }
            if (mouseDown)
            {
                mouseDown = false;
                (base.Owner as DH_ButtonComponent).Execute = false;
                sender.Invalidate();
                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseUp(sender, e);
        }
         */

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            Point point = GH_Convert.ToPoint(e.CanvasLocation);
            if (e.Button != 0)
            {
                return base.RespondToMouseMove(sender, e);
            }
            if (ButtonBounds.Contains(point))
            {
                if (!mouseOver)
                {
                    mouseOver = true;
                    sender.Invalidate();
                }
                return GH_ObjectResponse.Capture;
            }
            if (mouseOver)
            {
                mouseOver = false;
                sender.Invalidate();
            }
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return base.RespondToMouseUp(sender, e);
            }
            if (!ButtonBounds.Contains(e.CanvasLocation))
            {
                mouseDown = false;
                return base.RespondToMouseUp(sender, e);
            }
            mouseDown = false;
            Active = true;
            ButtonPressed?.Invoke(this, EventArgs.Empty);
            ((Control)(object)sender).Refresh();
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return base.RespondToMouseDown(sender, e);
            }
            if (!ButtonBounds.Contains(e.CanvasLocation))
            {
                return base.RespondToMouseDown(sender, e);
            }
            mouseDown = true;
            ((Control)(object)sender).Refresh();
            return GH_ObjectResponse.Capture;
        }
    }
}

