using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

[ToolboxItem(true)]
[ToolboxBitmap(typeof(TrackBar))]
public class DialSlider : Control {

    private float minValue = 0f;
    private float maxValue = 100f;
    private float value = 50f;

    private int labelWidth = 80;
    private int sliderSize = 8;

    private bool dragging = false;
    private bool valueChanged = false;

    private Font valueFont = new Font("Segoe UI", 9, FontStyle.Bold);
    private Font nameFont = new Font("Segoe UI", 9);

    [Category("Dial")]
    public Orientation Orientation { get; set; } = Orientation.Horizontal;

    [Category("Dial")]
    [Description("Szerokoœæ/Gruboœæ samego paska slidera")]
    public int SliderSize {
        get => sliderSize;
        set { sliderSize = Math.Max(1, value); Invalidate(); }
    }

    [Category("Dial")]
    public string SliderName { get; set; } = "Parameter";

    [Category("Dial")]
    [DefaultValue(0f)]
    public float MinValue {
        get => minValue;
        set { minValue = value; Invalidate(); }
    }

    [Category("Dial")]
    public float MaxValue {
        get => maxValue;
        set { maxValue = value; Invalidate(); }
    }

    [Category("Dial")]
    [Description("Increment/decrement step for the slider")]
    public float Step { get; set; } = 1f;

    [Category("Dial")]
    [TypeConverter(typeof(SingleConverter))]
    public float Value {
        get => value;
        set {
            float newVal = Math.Max(minValue, Math.Min(maxValue, value));

            if (Step > 0f) {
                newVal = minValue + (float)Math.Round((newVal - minValue) / Step) * Step;
                newVal = Math.Max(minValue, Math.Min(maxValue, newVal));
            }

            if (this.value != newVal) {
                this.value = newVal;
                Invalidate();
                valueChanged = true;
            }
        }
    }

    [Category("Dial")]
    public int DecimalPlaces { get; set; } = 2;

    [Category("Appearance")]
    public Font ValueFont {
        get => valueFont;
        set { valueFont = value; Invalidate(); }
    }

    [Category("Appearance")]
    public Font NameFont {
        get => nameFont;
        set { nameFont = value; Invalidate(); }
    }

    [Category("Layout")]
    public int LabelWidth {
        get => labelWidth;
        set { labelWidth = Math.Max(0, value); Invalidate(); }
    }

    public bool ValueChanged() {
        if (valueChanged) {
            valueChanged = false;
            return true;
        }
        return false;
    }

    public Color RimColor { get; set; } = Color.Orange;
    public Color DialColor { get; set; } = Color.FromArgb(40, 40, 40);
    public Color PointerColor { get; set; } = Color.Orange;

    public DialSlider() {
        DoubleBuffered = true;
        Height = 50;
        Width = 300;
        Font = new Font("Segoe UI", 9);
    }

    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int knobSize = 22;
        float percent = (value - minValue) / (maxValue - minValue);

        if (Orientation == Orientation.Horizontal) {
            int nameWidth = labelWidth;
            Rectangle track = new Rectangle(
                nameWidth,
                Height / 2 - sliderSize / 2,
                Width - nameWidth - knobSize,
                sliderSize
            );

            int fillWidth = (int)(track.Width * percent);
            Rectangle fill = new Rectangle(track.X, track.Y, fillWidth, track.Height);

            DrawSliderName(g, nameWidth);
            DrawTrack(g, track);
            DrawFill(g, fill);
            DrawKnob(g, track, percent, knobSize);
            DrawValue(g, track, percent);
        } else {
            int valueHeight = 20;
            int centerX = Width / 2;

            Rectangle track = new Rectangle(
                centerX - sliderSize / 2,
                knobSize / 2,
                sliderSize,
                Height - knobSize - valueHeight
            );

            int fillHeight = (int)(track.Height * percent);
            Rectangle fill = new Rectangle(track.X, track.Bottom - fillHeight, track.Width, fillHeight);

            DrawTrack(g, track);
            DrawFill(g, fill);
            DrawKnobVertical(g, track, percent, knobSize);
            DrawValueVertical(g);
        }
    }

    private void DrawSliderName(Graphics g, int width) {
        Rectangle rect = new Rectangle(0, 0, width - 5, Height);
        StringFormat sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        using (Brush b = new SolidBrush(ForeColor)) { g.DrawString(SliderName, nameFont, b, rect, sf); }
    }

    private string GetValueText() {
        return (Math.Abs(value % 1) < 0.0001f) ? ((int)value).ToString() : value.ToString("F" + DecimalPlaces);
    }

    private void DrawValue(Graphics g, Rectangle track, float percent) {
        int knobX = track.X + (int)(track.Width * percent);
        Rectangle rect = new Rectangle(knobX - 25, track.Y - 25, 50, 20);
        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using (Brush b = new SolidBrush(RimColor)) { g.DrawString(GetValueText(), valueFont, b, rect, sf); }
    }

    private void DrawValueVertical(Graphics g) {
        Rectangle rect = new Rectangle(0, Height - 20, Width, 20);
        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using (Brush b = new SolidBrush(RimColor)) { g.DrawString(GetValueText(), valueFont, b, rect, sf); }
    }

    private void DrawTrack(Graphics g, Rectangle rect) {
        int radius = Math.Min(rect.Width, rect.Height) / 2;
        using (GraphicsPath path = RoundedRect(rect, radius))
        using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(120, DialColor), Color.FromArgb(40, DialColor), LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
            using (Pen pen = new Pen(Color.FromArgb(100, RimColor))) { g.DrawPath(pen, path); }
        }
    }

    private void DrawFill(Graphics g, Rectangle rect) {
        if (rect.Width <= 0 || rect.Height <= 0) return;
        int radius = Math.Min(rect.Width, rect.Height) / 2;
        using (GraphicsPath path = RoundedRect(rect, radius))
        using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(200, RimColor), Color.FromArgb(120, RimColor), LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
        }
    }

    private void DrawKnob(Graphics g, Rectangle track, float percent, int size) {
        int x = track.X + (int)(track.Width * percent);
        int y = track.Y + track.Height / 2;
        DrawKnobShape(g, x, y, size);
    }

    private void DrawKnobVertical(Graphics g, Rectangle track, float percent, int size) {
        int x = track.X + track.Width / 2;
        int y = track.Bottom - (int)(track.Height * percent);
        DrawKnobShape(g, x, y, size);
    }

    private void DrawKnobShape(Graphics g, int x, int y, int size) {
        Rectangle knob = new Rectangle(x - size / 2, y - size / 2, size, size);
        using (LinearGradientBrush brush = new LinearGradientBrush(knob, PointerColor, Color.FromArgb(255, DialColor), LinearGradientMode.Vertical)) {
            g.FillEllipse(brush, knob);
        }
        using (Pen pen = new Pen(Color.FromArgb(180, RimColor), 2)) { g.DrawEllipse(pen, knob); }
        DrawGloss(g, knob);
    }

    private void DrawGloss(Graphics g, Rectangle rect) {
        Rectangle gloss = new Rectangle(rect.X + rect.Width / 6, rect.Y + rect.Height / 8, rect.Width * 2 / 3, rect.Height / 3);
        using (LinearGradientBrush brush = new LinearGradientBrush(gloss, Color.FromArgb(120, Color.White), Color.Transparent, LinearGradientMode.Vertical)) {
            g.FillEllipse(brush, gloss);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e) { dragging = true; UpdateValueFromPos(e.X, e.Y); }
    protected override void OnMouseMove(MouseEventArgs e) { if (dragging) UpdateValueFromPos(e.X, e.Y); }
    protected override void OnMouseUp(MouseEventArgs e) { dragging = false; }

    private void UpdateValueFromPos(int mouseX, int mouseY) {
        int knobSize = 22;
        float percent = 0;
        if (Orientation == Orientation.Horizontal) {
            int trackWidth = Width - labelWidth - knobSize;
            percent = (float)(mouseX - labelWidth) / trackWidth;
        } else {
            int trackHeight = Height - knobSize - 20;
            percent = 1.0f - ((float)(mouseY - knobSize / 2) / trackHeight);
        }
        percent = Math.Max(0, Math.Min(1, percent));
        Value = minValue + ((maxValue - minValue) * percent);
    }

    private GraphicsPath RoundedRect(Rectangle rect, int radius) {
        GraphicsPath path = new GraphicsPath();
        if (radius <= 0) { path.AddRectangle(rect); return path; }
        int d = radius * 2;
        if (rect.Width < d) d = rect.Width;
        if (rect.Height < d) d = rect.Height;

        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}