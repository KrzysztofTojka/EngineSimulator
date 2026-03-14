using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

[ToolboxItem(true)]
[ToolboxBitmap(typeof(ProgressBar))]
public class VerticalFillBarSimple : Control {
    private float minValue = 0f;
    private float maxValue = 100f;
    private float value = 50f;

    [Category("VerticalFill")]
    public float MinValue {
        get => minValue;
        set { minValue = value; Invalidate(); }
    }

    [Category("VerticalFill")]
    public float MaxValue {
        get => maxValue;
        set { maxValue = value; Invalidate(); }
    }

    [Category("VerticalFill")]
    public float Value {
        get => value;
        set {
            float newVal = Math.Max(minValue, Math.Min(maxValue, value));
            if (this.value != newVal) {
                this.value = newVal;
                Invalidate();
            }
        }
    }

    [Category("Appearance")]
    public Color FillColor { get; set; } = Color.Orange;

    [Category("Appearance")]
    public Color BackgroundColor { get; set; } = Color.FromArgb(40, 40, 40);

    [Category("Appearance")]
    public Color RimColor { get; set; } = Color.Orange;

    public VerticalFillBarSimple() {
        DoubleBuffered = true;
        Width = 20;
        Height = 100;
    }

    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);

        if (Width <= 1 || Height <= 1) return; // zabezpieczenie przed zerowym rozmiarem

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int radius = 6;
        Rectangle barRect = new Rectangle(0, 0, Width - 1, Height - 1);

        float percent = (value - minValue) / (maxValue - minValue);
        int fillHeight = (int)(Height * percent);

        Rectangle fillRect = new Rectangle(
            0,
            Height - fillHeight,
            Width - 1,
            fillHeight
        );

        DrawBarBackground(g, barRect, radius);
        DrawFill(g, fillRect, radius);
    }

    private void DrawBarBackground(Graphics g, Rectangle rect, int radius) {
        using (GraphicsPath path = RoundedRect(rect, radius))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(120, BackgroundColor),
            Color.FromArgb(40, BackgroundColor),
            LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
        }

        using (Pen pen = new Pen(Color.FromArgb(100, RimColor))) {
            g.DrawPath(pen, RoundedRect(rect, radius));
        }
    }

    private void DrawFill(Graphics g, Rectangle rect, int radius) {
        if (rect.Height <= 0 || rect.Width <= 0) return;

        using (GraphicsPath path = RoundedRect(rect, radius))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(200, FillColor),
            Color.FromArgb(120, FillColor),
            LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
        }
    }

    private GraphicsPath RoundedRect(Rectangle rect, int radius) {
        GraphicsPath path = new GraphicsPath();
        int r = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

        path.AddArc(rect.X, rect.Y, r, r, 180, 90);
        path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
        path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
        path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
        path.CloseFigure();
        return path;
    }
}