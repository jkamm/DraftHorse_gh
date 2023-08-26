using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace DraftHorse.Components.Base
{ 
    //Borrowed liberally from Metahopper with permission from Andrew Heumann, 2023
public class DH_UpdateButtonComponent_Attributes : GH_ComponentAttributes
{
    private bool mouseOver;

    private bool mouseDown;

    private RectangleF buttonArea;

    private RectangleF textArea;

    public DH_UpdateButtonComponent_Attributes(DH_UpdateButtonComponent owner)
        : base(owner)
    {
        mouseOver = false;
        mouseDown = false;
    }

    protected override void Layout()
    {
        Bounds = RectangleF.Empty;
        base.Layout();
        buttonArea = new RectangleF(Bounds.Left, Bounds.Bottom, Bounds.Width, 22f);
        textArea = buttonArea;
        Bounds = RectangleF.Union(Bounds, buttonArea);
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);
        if (channel == GH_CanvasChannel.Objects)
        {
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Black, Selected, Owner.Locked, hidden: true);
            GH_Capsule gH_Capsule = GH_Capsule.CreateTextCapsule(buttonArea, textArea, GH_Palette.Black, "Update", GH_FontServer.Standard, 1, 9);
            gH_Capsule.RenderEngine.RenderBackground(graphics, canvas.Viewport.Zoom, impliedStyle);
            if (!mouseDown)
            {
                gH_Capsule.RenderEngine.RenderHighlight(graphics);
            }
            gH_Capsule.RenderEngine.RenderOutlines(graphics, canvas.Viewport.Zoom, impliedStyle);
            if (mouseOver)
            {
                gH_Capsule.RenderEngine.RenderBackground_Alternative(graphics, Color.FromArgb(50, Color.Blue), drawAlphaGrid: false);
            }
            gH_Capsule.RenderEngine.RenderText(graphics, Color.White);
            gH_Capsule.Dispose();
        }
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f && buttonArea.Contains(e.CanvasLocation))
        {
            mouseDown = true;
            Owner.RecordUndoEvent("Update Selection");
            DH_UpdateButtonComponent dH_UpdateButtonComponent = Owner as DH_UpdateButtonComponent;
            Owner.ExpireSolution(recompute: true);
            return GH_ObjectResponse.Capture;
        }
        return base.RespondToMouseDown(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (!buttonArea.Contains(e.CanvasLocation))
        {
            mouseOver = false;
        }
        if (mouseDown)
        {
            mouseDown = false;
            sender.Invalidate();
            return GH_ObjectResponse.Release;
        }
        return base.RespondToMouseUp(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        Point point = GH_Convert.ToPoint(e.CanvasLocation);
        if (e.Button != 0)
        {
            return base.RespondToMouseMove(sender, e);
        }
        if (buttonArea.Contains(point))
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
}
}
