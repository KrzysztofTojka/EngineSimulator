using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class DialSlider : Control
{
    private int minValue = 0;
    private int maxValue = 100;
    private int value = 50;

    private bool dragging = false;

    public int MinValue
    {
        get => minValue;
        set { minValue = value; Invalidate(); }
    }

    public int MaxValue
    {
        get => maxValue;
        set { maxValue = value; Invalidate(); }
    }

    public int Value
    {
        get => value;
        set
        {
            this.value = Math.Max(minValue, Math.Min(maxValue, value));
            Invalidate();
        }
    }

    public Color RimColor { get; set; } = Color.Orange;
    public Color DialColor { get; set; } = Color.FromArgb(40, 40, 40);
    public Color PointerColor { get; set; } = Color.Orange;

    public DialSlider()
    {
        DoubleBuffered = true;
        Height = 40;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int trackHeight = 8;
        int knobSize = 22;

        Rectangle track = new Rectangle(
            knobSize / 2,
            Height / 2 - trackHeight / 2,
            Width - knobSize,
            trackHeight
        );

        float percent = (float)(value - minValue) / (maxValue - minValue);
        int fillWidth = (int)(track.Width * percent);

        Rectangle fill = new Rectangle(track.X, track.Y, fillWidth, track.Height);

        DrawTrack(g, track);
        DrawFill(g, fill);
        DrawKnob(g, track, percent, knobSize);
    }

    private void DrawTrack(Graphics g, Rectangle rect)
    {
        using (GraphicsPath path = RoundedRect(rect, 6))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(120, DialColor),
            Color.FromArgb(40, DialColor),
            LinearGradientMode.Vertical))
        {
            g.FillPath(brush, path);
        }

        using (Pen pen = new Pen(Color.FromArgb(100, RimColor), 1))
        {
            g.DrawRectangle(pen, rect);
        }
    }

    private void DrawFill(Graphics g, Rectangle rect)
    {
        if (rect.Width <= 0) return;

        using (GraphicsPath path = RoundedRect(rect, 6))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(200, RimColor),
            Color.FromArgb(120, RimColor),
            LinearGradientMode.Vertical))
        {
            g.FillPath(brush, path);
        }
    }

    private void DrawKnob(Graphics g, Rectangle track, float percent, int size)
    {
        int x = track.X + (int)(track.Width * percent);
        int y = track.Y + track.Height / 2;

        Rectangle knob = new Rectangle(
            x - size / 2,
            y - size / 2,
            size,
            size
        );

        using (LinearGradientBrush brush = new LinearGradientBrush(
            knob,
            PointerColor,
            Color.FromArgb(80, DialColor),
            LinearGradientMode.Vertical))
        {
            g.FillEllipse(brush, knob);
        }

        using (Pen pen = new Pen(Color.FromArgb(180, RimColor), 2))
        {
            g.DrawEllipse(pen, knob);
        }

        DrawGloss(g, knob);
    }

    private void DrawGloss(Graphics g, Rectangle rect)
    {
        Rectangle gloss = new Rectangle(
            rect.X + rect.Width / 6,
            rect.Y + rect.Height / 8,
            rect.Width * 2 / 3,
            rect.Height / 3
        );

        using (LinearGradientBrush brush = new LinearGradientBrush(
            gloss,
            Color.FromArgb(120, Color.White),
            Color.Transparent,
            LinearGradientMode.Vertical))
        {
            g.FillEllipse(brush, gloss);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        dragging = true;
        UpdateValue(e.X);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (dragging)
            UpdateValue(e.X);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        dragging = false;
    }

    private void UpdateValue(int mouseX)
    {
        int knobSize = 22;
        int trackStart = knobSize / 2;
        int trackWidth = Width - knobSize;

        float percent = (float)(mouseX - trackStart) / trackWidth;
        percent = Math.Max(0, Math.Min(1, percent));

        Value = minValue + (int)((maxValue - minValue) * percent);
    }

    private GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();

        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

        path.CloseFigure();
        return path;
    }
}