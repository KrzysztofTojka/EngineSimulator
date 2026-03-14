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

    private bool dragging = false;

    private bool valueChanged = false;

    private Font valueFont = new Font("Segoe UI", 9, FontStyle.Bold);
    private Font nameFont = new Font("Segoe UI", 9);

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

            // zastosowanie step
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
        set {
            valueFont = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    public Font NameFont {
        get => nameFont;
        set {
            nameFont = value;
            Invalidate();
        }
    }

    [Category("Layout")]
    [Description("Width reserved for the slider label")]
    public int LabelWidth {
        get => labelWidth;
        set {
            labelWidth = Math.Max(0, value);
            Invalidate();
        }
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
        int trackHeight = 8;
        int nameWidth = labelWidth;

        Rectangle track = new Rectangle(
            nameWidth,
            Height / 2 - trackHeight / 2,
            Width - nameWidth - knobSize,
            trackHeight
        );

        float percent = (value - minValue) / (maxValue - minValue);
        int fillWidth = (int)(track.Width * percent);

        Rectangle fill = new Rectangle(track.X, track.Y, fillWidth, track.Height);

        DrawSliderName(g, nameWidth);
        DrawTrack(g, track);
        DrawFill(g, fill);
        DrawKnob(g, track, percent, knobSize);
        DrawValue(g, track, percent);
    }

    private void DrawSliderName(Graphics g, int width) {
        Rectangle rect = new Rectangle(0, 0, width - 5, Height);

        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Far;
        sf.LineAlignment = StringAlignment.Center;

        using (Brush b = new SolidBrush(ForeColor)) {
            g.DrawString(SliderName, nameFont, b, rect, sf);
        }
    }

    private void DrawValue(Graphics g, Rectangle track, float percent) {
        int knobX = track.X + (int)(track.Width * percent);

        Rectangle rect = new Rectangle(
            knobX - 25,
            track.Y - 25,
            50,
            20
        );

        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Center;
        sf.LineAlignment = StringAlignment.Center;

        string text;

        if (Math.Abs(value % 1) < 0.0001f)
            text = ((int)value).ToString();
        else
            text = value.ToString("F" + DecimalPlaces);

        using (Brush b = new SolidBrush(RimColor)) {
            g.DrawString(text, valueFont, b, rect, sf);
        }
    }

    private void DrawTrack(Graphics g, Rectangle rect) {
        using (GraphicsPath path = RoundedRect(rect, 6))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(120, DialColor),
            Color.FromArgb(40, DialColor),
            LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
        }

        using (Pen pen = new Pen(Color.FromArgb(100, RimColor))) {
            g.DrawPath(pen, RoundedRect(rect, 6));
        }
    }

    private void DrawFill(Graphics g, Rectangle rect) {
        if (rect.Width <= 0) return;

        using (GraphicsPath path = RoundedRect(rect, 6))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(200, RimColor),
            Color.FromArgb(120, RimColor),
            LinearGradientMode.Vertical)) {
            g.FillPath(brush, path);
        }
    }

    private void DrawKnob(Graphics g, Rectangle track, float percent, int size) {
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
            Color.FromArgb(255, DialColor),
            LinearGradientMode.Vertical)) {
            g.FillEllipse(brush, knob);
        }

        using (Pen pen = new Pen(Color.FromArgb(180, RimColor), 2)) {
            g.DrawEllipse(pen, knob);
        }

        DrawGloss(g, knob);
    }

    private void DrawGloss(Graphics g, Rectangle rect) {
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
            LinearGradientMode.Vertical)) {
            g.FillEllipse(brush, gloss);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e) {
        dragging = true;
        UpdateValue(e.X);
    }

    protected override void OnMouseMove(MouseEventArgs e) {
        if (dragging)
            UpdateValue(e.X);
    }

    protected override void OnMouseUp(MouseEventArgs e) {
        dragging = false;
    }

    private void UpdateValue(int mouseX) {
        int nameWidth = labelWidth;
        int knobSize = 22;

        int trackStart = nameWidth;
        int trackWidth = Width - nameWidth - knobSize;

        float percent = (float)(mouseX - trackStart) / trackWidth;
        percent = Math.Max(0, Math.Min(1, percent));

        Value = minValue + ((maxValue - minValue) * percent);
    }

    private GraphicsPath RoundedRect(Rectangle rect, int radius) {
        GraphicsPath path = new GraphicsPath();

        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

        path.CloseFigure();
        return path;
    }
}