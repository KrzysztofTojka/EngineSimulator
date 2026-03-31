using System.Windows.Forms;

namespace EngineSimulator {
    partial class Window {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.dynoChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.rpmGauge = new AquaControls.AquaGauge();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.throttleGauge = new AquaControls.AquaGauge();
            this.mapGauge = new AquaControls.AquaGauge();
            this.loadGauge = new AquaControls.AquaGauge();
            this.fuelRateGauge = new AquaControls.AquaGauge();
            this.afrGauge = new AquaControls.AquaGauge();
            this.speedGauge = new AquaControls.AquaGauge();
            this.mafGauge = new AquaControls.AquaGauge();
            this.label1 = new System.Windows.Forms.Label();
            this.gearLabel = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.dynoPage = new System.Windows.Forms.TabPage();
            this.shiftingPage = new System.Windows.Forms.TabPage();
            this.shiftingChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.torqueGraphButton = new System.Windows.Forms.Button();
            this.dynoThrottleSlider = new DialSlider();
            this.torqueBar = new VerticalFillBarSimple();
            this.powerBar = new VerticalFillBarSimple();
            this.maxAirflowRpmSlider = new DialSlider();
            this.veScaleSlider = new DialSlider();
            this.maxVeRpmSlider = new DialSlider();
            this.rpmLimitSlider = new DialSlider();
            this.inertiaSlider = new DialSlider();
            this.idleThrottleSlider = new DialSlider();
            this.displacementSlider = new DialSlider();
            this.maxVeSlider = new DialSlider();
            ((System.ComponentModel.ISupportInitialize)(this.dynoChart)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.dynoPage.SuspendLayout();
            this.shiftingPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shiftingChart)).BeginInit();
            this.SuspendLayout();
            // 
            // dynoChart
            // 
            chartArea1.Name = "ChartArea1";
            this.dynoChart.ChartAreas.Add(chartArea1);
            this.dynoChart.Location = new System.Drawing.Point(3, 3);
            this.dynoChart.Name = "dynoChart";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.dynoChart.Series.Add(series1);
            this.dynoChart.Size = new System.Drawing.Size(772, 454);
            this.dynoChart.TabIndex = 0;
            this.dynoChart.Text = "dynoChart";
            this.dynoChart.Paint += new System.Windows.Forms.PaintEventHandler(this.dyno_Paint);
            // 
            // rpmGauge
            // 
            this.rpmGauge.BackColor = System.Drawing.Color.Transparent;
            this.rpmGauge.DecimalPlaces = 0;
            this.rpmGauge.DialAlpha = 255;
            this.rpmGauge.DialBorderColor = System.Drawing.Color.Black;
            this.rpmGauge.DialColor = System.Drawing.Color.Transparent;
            this.rpmGauge.DialText = "RPM";
            this.rpmGauge.DialTextColor = System.Drawing.Color.Black;
            this.rpmGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.rpmGauge.DialTextVOffset = 0;
            this.rpmGauge.DigitalValue = 0F;
            this.rpmGauge.DigitalValueBackAlpha = 1;
            this.rpmGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.rpmGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.rpmGauge.DigitalValueDecimalPlaces = 0;
            this.rpmGauge.Glossiness = 40F;
            this.rpmGauge.Location = new System.Drawing.Point(13, 13);
            this.rpmGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rpmGauge.MaxValue = 70F;
            this.rpmGauge.MinValue = 0F;
            this.rpmGauge.Name = "rpmGauge";
            this.rpmGauge.NoOfDivisions = 7;
            this.rpmGauge.PointerColor = System.Drawing.Color.Black;
            this.rpmGauge.RimAlpha = 255;
            this.rpmGauge.RimColor = System.Drawing.Color.Silver;
            this.rpmGauge.ScaleColor = System.Drawing.Color.Black;
            this.rpmGauge.ScaleFontSizeDivider = 22;
            this.rpmGauge.Size = new System.Drawing.Size(365, 365);
            this.rpmGauge.TabIndex = 1;
            this.rpmGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.rpmGauge.Threshold1Start = 0F;
            this.rpmGauge.Threshold1Stop = 0F;
            this.rpmGauge.Threshold2Color = System.Drawing.Color.Red;
            this.rpmGauge.Threshold2Start = 55F;
            this.rpmGauge.Threshold2Stop = 70F;
            this.rpmGauge.Value = 0F;
            this.rpmGauge.ValueToDigital = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // throttleGauge
            // 
            this.throttleGauge.BackColor = System.Drawing.Color.Transparent;
            this.throttleGauge.DecimalPlaces = 0;
            this.throttleGauge.DialAlpha = 255;
            this.throttleGauge.DialBorderColor = System.Drawing.Color.Black;
            this.throttleGauge.DialColor = System.Drawing.Color.Transparent;
            this.throttleGauge.DialText = "Throttle [%]";
            this.throttleGauge.DialTextColor = System.Drawing.Color.Black;
            this.throttleGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.throttleGauge.DialTextVOffset = 0;
            this.throttleGauge.DigitalValue = 0F;
            this.throttleGauge.DigitalValueBackAlpha = 1;
            this.throttleGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.throttleGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.throttleGauge.DigitalValueDecimalPlaces = 2;
            this.throttleGauge.Glossiness = 40F;
            this.throttleGauge.Location = new System.Drawing.Point(421, 13);
            this.throttleGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.throttleGauge.MaxValue = 100F;
            this.throttleGauge.MinValue = 0F;
            this.throttleGauge.Name = "throttleGauge";
            this.throttleGauge.PointerColor = System.Drawing.Color.Black;
            this.throttleGauge.RimAlpha = 255;
            this.throttleGauge.RimColor = System.Drawing.Color.Silver;
            this.throttleGauge.ScaleColor = System.Drawing.Color.Black;
            this.throttleGauge.ScaleFontSizeDivider = 22;
            this.throttleGauge.Size = new System.Drawing.Size(250, 250);
            this.throttleGauge.TabIndex = 2;
            this.throttleGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.throttleGauge.Threshold1Start = 0F;
            this.throttleGauge.Threshold1Stop = 0F;
            this.throttleGauge.Threshold2Color = System.Drawing.Color.Red;
            this.throttleGauge.Threshold2Start = 0F;
            this.throttleGauge.Threshold2Stop = 0F;
            this.throttleGauge.Value = 0F;
            this.throttleGauge.ValueToDigital = false;
            // 
            // mapGauge
            // 
            this.mapGauge.BackColor = System.Drawing.Color.Transparent;
            this.mapGauge.DecimalPlaces = 0;
            this.mapGauge.DialAlpha = 255;
            this.mapGauge.DialBorderColor = System.Drawing.Color.Black;
            this.mapGauge.DialColor = System.Drawing.Color.Transparent;
            this.mapGauge.DialText = "MAP [kPa]";
            this.mapGauge.DialTextColor = System.Drawing.Color.Black;
            this.mapGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.mapGauge.DialTextVOffset = 0;
            this.mapGauge.DigitalValue = 0F;
            this.mapGauge.DigitalValueBackAlpha = 1;
            this.mapGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.mapGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.mapGauge.DigitalValueDecimalPlaces = 2;
            this.mapGauge.Glossiness = 40F;
            this.mapGauge.Location = new System.Drawing.Point(421, 263);
            this.mapGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mapGauge.MaxValue = 110F;
            this.mapGauge.MinValue = 0F;
            this.mapGauge.Name = "mapGauge";
            this.mapGauge.NoOfDivisions = 11;
            this.mapGauge.PointerColor = System.Drawing.Color.Black;
            this.mapGauge.RimAlpha = 255;
            this.mapGauge.RimColor = System.Drawing.Color.Silver;
            this.mapGauge.ScaleColor = System.Drawing.Color.Black;
            this.mapGauge.ScaleFontSizeDivider = 22;
            this.mapGauge.Size = new System.Drawing.Size(250, 250);
            this.mapGauge.TabIndex = 3;
            this.mapGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.mapGauge.Threshold1Start = 0F;
            this.mapGauge.Threshold1Stop = 0F;
            this.mapGauge.Threshold2Color = System.Drawing.Color.Red;
            this.mapGauge.Threshold2Start = 0F;
            this.mapGauge.Threshold2Stop = 0F;
            this.mapGauge.Value = 0F;
            this.mapGauge.ValueToDigital = false;
            // 
            // loadGauge
            // 
            this.loadGauge.BackColor = System.Drawing.Color.Transparent;
            this.loadGauge.DecimalPlaces = 0;
            this.loadGauge.DialAlpha = 255;
            this.loadGauge.DialBorderColor = System.Drawing.Color.Black;
            this.loadGauge.DialColor = System.Drawing.Color.Transparent;
            this.loadGauge.DialText = "Load [%]";
            this.loadGauge.DialTextColor = System.Drawing.Color.Black;
            this.loadGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.loadGauge.DialTextVOffset = 0;
            this.loadGauge.DigitalValue = 0F;
            this.loadGauge.DigitalValueBackAlpha = 1;
            this.loadGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.loadGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.loadGauge.DigitalValueDecimalPlaces = 2;
            this.loadGauge.Glossiness = 40F;
            this.loadGauge.Location = new System.Drawing.Point(679, 13);
            this.loadGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.loadGauge.MaxValue = 100F;
            this.loadGauge.MinValue = 0F;
            this.loadGauge.Name = "loadGauge";
            this.loadGauge.PointerColor = System.Drawing.Color.Black;
            this.loadGauge.RimAlpha = 255;
            this.loadGauge.RimColor = System.Drawing.Color.Silver;
            this.loadGauge.ScaleColor = System.Drawing.Color.Black;
            this.loadGauge.ScaleFontSizeDivider = 22;
            this.loadGauge.Size = new System.Drawing.Size(250, 250);
            this.loadGauge.TabIndex = 4;
            this.loadGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.loadGauge.Threshold1Start = 0F;
            this.loadGauge.Threshold1Stop = 0F;
            this.loadGauge.Threshold2Color = System.Drawing.Color.Red;
            this.loadGauge.Threshold2Start = 0F;
            this.loadGauge.Threshold2Stop = 0F;
            this.loadGauge.Value = 0F;
            this.loadGauge.ValueToDigital = false;
            // 
            // fuelRateGauge
            // 
            this.fuelRateGauge.BackColor = System.Drawing.Color.Transparent;
            this.fuelRateGauge.DecimalPlaces = 0;
            this.fuelRateGauge.DialAlpha = 255;
            this.fuelRateGauge.DialBorderColor = System.Drawing.Color.Black;
            this.fuelRateGauge.DialColor = System.Drawing.Color.Transparent;
            this.fuelRateGauge.DialText = "Fuel rate [L/h]";
            this.fuelRateGauge.DialTextColor = System.Drawing.Color.Black;
            this.fuelRateGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.fuelRateGauge.DialTextVOffset = 0;
            this.fuelRateGauge.DigitalValue = 0F;
            this.fuelRateGauge.DigitalValueBackAlpha = 1;
            this.fuelRateGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.fuelRateGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.fuelRateGauge.DigitalValueDecimalPlaces = 2;
            this.fuelRateGauge.Glossiness = 40F;
            this.fuelRateGauge.Location = new System.Drawing.Point(421, 511);
            this.fuelRateGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fuelRateGauge.MaxValue = 50F;
            this.fuelRateGauge.MinValue = 0F;
            this.fuelRateGauge.Name = "fuelRateGauge";
            this.fuelRateGauge.NoOfDivisions = 5;
            this.fuelRateGauge.NoOfSubDivisions = 4;
            this.fuelRateGauge.PointerColor = System.Drawing.Color.Black;
            this.fuelRateGauge.RimAlpha = 255;
            this.fuelRateGauge.RimColor = System.Drawing.Color.Silver;
            this.fuelRateGauge.ScaleColor = System.Drawing.Color.Black;
            this.fuelRateGauge.ScaleFontSizeDivider = 22;
            this.fuelRateGauge.Size = new System.Drawing.Size(250, 250);
            this.fuelRateGauge.TabIndex = 5;
            this.fuelRateGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.fuelRateGauge.Threshold1Start = 0F;
            this.fuelRateGauge.Threshold1Stop = 0F;
            this.fuelRateGauge.Threshold2Color = System.Drawing.Color.Red;
            this.fuelRateGauge.Threshold2Start = 40F;
            this.fuelRateGauge.Threshold2Stop = 40F;
            this.fuelRateGauge.Value = 0F;
            this.fuelRateGauge.ValueToDigital = false;
            // 
            // afrGauge
            // 
            this.afrGauge.BackColor = System.Drawing.Color.Transparent;
            this.afrGauge.DecimalPlaces = 0;
            this.afrGauge.DialAlpha = 255;
            this.afrGauge.DialBorderColor = System.Drawing.Color.Black;
            this.afrGauge.DialColor = System.Drawing.Color.Transparent;
            this.afrGauge.DialText = "AFR";
            this.afrGauge.DialTextColor = System.Drawing.Color.Black;
            this.afrGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.afrGauge.DialTextVOffset = 0;
            this.afrGauge.DigitalValue = 0F;
            this.afrGauge.DigitalValueBackAlpha = 1;
            this.afrGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.afrGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.afrGauge.DigitalValueDecimalPlaces = 2;
            this.afrGauge.Glossiness = 40F;
            this.afrGauge.Location = new System.Drawing.Point(679, 511);
            this.afrGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.afrGauge.MaxValue = 17F;
            this.afrGauge.MinValue = 12F;
            this.afrGauge.Name = "afrGauge";
            this.afrGauge.NoOfDivisions = 5;
            this.afrGauge.PointerColor = System.Drawing.Color.Black;
            this.afrGauge.RimAlpha = 255;
            this.afrGauge.RimColor = System.Drawing.Color.Silver;
            this.afrGauge.ScaleColor = System.Drawing.Color.Black;
            this.afrGauge.ScaleFontSizeDivider = 22;
            this.afrGauge.Size = new System.Drawing.Size(250, 250);
            this.afrGauge.TabIndex = 9;
            this.afrGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.afrGauge.Threshold1Start = 14.7F;
            this.afrGauge.Threshold1Stop = 14.7F;
            this.afrGauge.Threshold2Color = System.Drawing.Color.Red;
            this.afrGauge.Threshold2Start = 12F;
            this.afrGauge.Threshold2Stop = 12F;
            this.afrGauge.Value = 14.7F;
            this.afrGauge.ValueToDigital = false;
            // 
            // speedGauge
            // 
            this.speedGauge.BackColor = System.Drawing.Color.Transparent;
            this.speedGauge.DecimalPlaces = 0;
            this.speedGauge.DialAlpha = 255;
            this.speedGauge.DialBorderColor = System.Drawing.Color.Black;
            this.speedGauge.DialColor = System.Drawing.Color.Transparent;
            this.speedGauge.DialText = "Speed [km/h]";
            this.speedGauge.DialTextColor = System.Drawing.Color.Black;
            this.speedGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.speedGauge.DialTextVOffset = 0;
            this.speedGauge.DigitalValue = 0F;
            this.speedGauge.DigitalValueBackAlpha = 1;
            this.speedGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.speedGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.speedGauge.DigitalValueDecimalPlaces = 2;
            this.speedGauge.Glossiness = 40F;
            this.speedGauge.Location = new System.Drawing.Point(13, 396);
            this.speedGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.speedGauge.MaxValue = 240F;
            this.speedGauge.MinValue = 0F;
            this.speedGauge.Name = "speedGauge";
            this.speedGauge.NoOfDivisions = 12;
            this.speedGauge.NoOfSubDivisions = 1;
            this.speedGauge.PointerColor = System.Drawing.Color.Black;
            this.speedGauge.RimAlpha = 255;
            this.speedGauge.RimColor = System.Drawing.Color.Silver;
            this.speedGauge.ScaleColor = System.Drawing.Color.Black;
            this.speedGauge.ScaleFontSizeDivider = 22;
            this.speedGauge.Size = new System.Drawing.Size(365, 365);
            this.speedGauge.TabIndex = 10;
            this.speedGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.speedGauge.Threshold1Start = 14.7F;
            this.speedGauge.Threshold1Stop = 14.7F;
            this.speedGauge.Threshold2Color = System.Drawing.Color.Red;
            this.speedGauge.Threshold2Start = 0F;
            this.speedGauge.Threshold2Stop = 0F;
            this.speedGauge.Value = 0F;
            this.speedGauge.ValueToDigital = false;
            // 
            // mafGauge
            // 
            this.mafGauge.BackColor = System.Drawing.Color.Transparent;
            this.mafGauge.DecimalPlaces = 0;
            this.mafGauge.DialAlpha = 255;
            this.mafGauge.DialBorderColor = System.Drawing.Color.Black;
            this.mafGauge.DialColor = System.Drawing.Color.Transparent;
            this.mafGauge.DialText = "MAF [g/s]";
            this.mafGauge.DialTextColor = System.Drawing.Color.Black;
            this.mafGauge.DialTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.mafGauge.DialTextVOffset = 0;
            this.mafGauge.DigitalValue = 0F;
            this.mafGauge.DigitalValueBackAlpha = 1;
            this.mafGauge.DigitalValueBackColor = System.Drawing.Color.White;
            this.mafGauge.DigitalValueColor = System.Drawing.Color.Black;
            this.mafGauge.DigitalValueDecimalPlaces = 2;
            this.mafGauge.Glossiness = 40F;
            this.mafGauge.Location = new System.Drawing.Point(679, 263);
            this.mafGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mafGauge.MaxValue = 130F;
            this.mafGauge.MinValue = 0F;
            this.mafGauge.Name = "mafGauge";
            this.mafGauge.NoOfDivisions = 13;
            this.mafGauge.NoOfSubDivisions = 2;
            this.mafGauge.PointerColor = System.Drawing.Color.Black;
            this.mafGauge.RimAlpha = 255;
            this.mafGauge.RimColor = System.Drawing.Color.Silver;
            this.mafGauge.ScaleColor = System.Drawing.Color.Black;
            this.mafGauge.ScaleFontSizeDivider = 22;
            this.mafGauge.Size = new System.Drawing.Size(250, 250);
            this.mafGauge.TabIndex = 11;
            this.mafGauge.Threshold1Color = System.Drawing.Color.LawnGreen;
            this.mafGauge.Threshold1Start = 0F;
            this.mafGauge.Threshold1Stop = 0F;
            this.mafGauge.Threshold2Color = System.Drawing.Color.Red;
            this.mafGauge.Threshold2Start = 0F;
            this.mafGauge.Threshold2Stop = 0F;
            this.mafGauge.Value = 0F;
            this.mafGauge.ValueToDigital = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.label1.Location = new System.Drawing.Point(329, 347);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 31);
            this.label1.TabIndex = 15;
            this.label1.Text = "Gear";
            // 
            // gearLabel
            // 
            this.gearLabel.AutoSize = true;
            this.gearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.gearLabel.Location = new System.Drawing.Point(334, 378);
            this.gearLabel.Name = "gearLabel";
            this.gearLabel.Size = new System.Drawing.Size(73, 69);
            this.gearLabel.TabIndex = 16;
            this.gearLabel.Text = "N";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.dynoPage);
            this.tabControl1.Controls.Add(this.shiftingPage);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(1023, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(785, 505);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 21;
            this.tabControl1.TabStop = false;
            // 
            // dynoPage
            // 
            this.dynoPage.Controls.Add(this.torqueGraphButton);
            this.dynoPage.Controls.Add(this.dynoChart);
            this.dynoPage.Location = new System.Drawing.Point(4, 27);
            this.dynoPage.Name = "dynoPage";
            this.dynoPage.Padding = new System.Windows.Forms.Padding(3);
            this.dynoPage.Size = new System.Drawing.Size(777, 474);
            this.dynoPage.TabIndex = 1;
            this.dynoPage.Text = "Dyno";
            this.dynoPage.UseVisualStyleBackColor = true;
            // 
            // shiftingPage
            // 
            this.shiftingPage.Controls.Add(this.shiftingChart);
            this.shiftingPage.Location = new System.Drawing.Point(4, 27);
            this.shiftingPage.Name = "shiftingPage";
            this.shiftingPage.Padding = new System.Windows.Forms.Padding(3);
            this.shiftingPage.Size = new System.Drawing.Size(777, 458);
            this.shiftingPage.TabIndex = 0;
            this.shiftingPage.Text = "Shifting";
            this.shiftingPage.UseVisualStyleBackColor = true;
            // 
            // shiftingChart
            // 
            chartArea2.Name = "ChartArea1";
            this.shiftingChart.ChartAreas.Add(chartArea2);
            this.shiftingChart.Location = new System.Drawing.Point(3, 3);
            this.shiftingChart.Name = "shiftingChart";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.shiftingChart.Series.Add(series2);
            this.shiftingChart.Size = new System.Drawing.Size(772, 454);
            this.shiftingChart.TabIndex = 0;
            this.shiftingChart.Text = "shiftingChart";
            // 
            // torqueGraphButton
            // 
            this.torqueGraphButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.torqueGraphButton.Location = new System.Drawing.Point(607, 438);
            this.torqueGraphButton.Name = "torqueGraphButton";
            this.torqueGraphButton.Size = new System.Drawing.Size(164, 30);
            this.torqueGraphButton.TabIndex = 23;
            this.torqueGraphButton.Text = "Generate torque graph";
            this.torqueGraphButton.UseVisualStyleBackColor = true;
            this.torqueGraphButton.Click += new System.EventHandler(this.torqueGraphButton_Click);
            // 
            // dynoThrottleSlider
            // 
            this.dynoThrottleSlider.DecimalPlaces = 2;
            this.dynoThrottleSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.dynoThrottleSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dynoThrottleSlider.LabelWidth = 140;
            this.dynoThrottleSlider.Location = new System.Drawing.Point(1808, 39);
            this.dynoThrottleSlider.MaxValue = 1F;
            this.dynoThrottleSlider.Name = "dynoThrottleSlider";
            this.dynoThrottleSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.dynoThrottleSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.dynoThrottleSlider.PointerColor = System.Drawing.Color.DimGray;
            this.dynoThrottleSlider.RimColor = System.Drawing.Color.Black;
            this.dynoThrottleSlider.Size = new System.Drawing.Size(41, 498);
            this.dynoThrottleSlider.SliderName = "Dyno throttle";
            this.dynoThrottleSlider.SliderSize = 15;
            this.dynoThrottleSlider.Step = 0.01F;
            this.dynoThrottleSlider.TabIndex = 22;
            this.dynoThrottleSlider.Text = "dialSlider1";
            this.dynoThrottleSlider.Value = 1F;
            this.dynoThrottleSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // torqueBar
            // 
            this.torqueBar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.torqueBar.FillColor = System.Drawing.Color.Blue;
            this.torqueBar.Location = new System.Drawing.Point(948, 51);
            this.torqueBar.MaxValue = 100F;
            this.torqueBar.MinValue = 0F;
            this.torqueBar.Name = "torqueBar";
            this.torqueBar.RimColor = System.Drawing.Color.Gray;
            this.torqueBar.Size = new System.Drawing.Size(27, 474);
            this.torqueBar.TabIndex = 14;
            this.torqueBar.Text = "torqueBar";
            this.torqueBar.Value = 50F;
            // 
            // powerBar
            // 
            this.powerBar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.powerBar.FillColor = System.Drawing.Color.Red;
            this.powerBar.Location = new System.Drawing.Point(981, 51);
            this.powerBar.MaxValue = 100F;
            this.powerBar.MinValue = 0F;
            this.powerBar.Name = "powerBar";
            this.powerBar.RimColor = System.Drawing.Color.Gray;
            this.powerBar.Size = new System.Drawing.Size(27, 474);
            this.powerBar.TabIndex = 13;
            this.powerBar.Text = "powerBar";
            this.powerBar.Value = 50F;
            // 
            // maxAirflowRpmSlider
            // 
            this.maxAirflowRpmSlider.DecimalPlaces = 1;
            this.maxAirflowRpmSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.maxAirflowRpmSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.maxAirflowRpmSlider.LabelWidth = 140;
            this.maxAirflowRpmSlider.Location = new System.Drawing.Point(1422, 695);
            this.maxAirflowRpmSlider.MaxValue = 6000F;
            this.maxAirflowRpmSlider.MinValue = 3000F;
            this.maxAirflowRpmSlider.Name = "maxAirflowRpmSlider";
            this.maxAirflowRpmSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.maxAirflowRpmSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.maxAirflowRpmSlider.PointerColor = System.Drawing.Color.DimGray;
            this.maxAirflowRpmSlider.RimColor = System.Drawing.Color.Black;
            this.maxAirflowRpmSlider.Size = new System.Drawing.Size(365, 60);
            this.maxAirflowRpmSlider.SliderName = "Max airflow RPM";
            this.maxAirflowRpmSlider.SliderSize = 8;
            this.maxAirflowRpmSlider.Step = 100F;
            this.maxAirflowRpmSlider.TabIndex = 20;
            this.maxAirflowRpmSlider.Text = "maxAirflowRpmSlider";
            this.maxAirflowRpmSlider.Value = 5500F;
            this.maxAirflowRpmSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // veScaleSlider
            // 
            this.veScaleSlider.DecimalPlaces = 1;
            this.veScaleSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.veScaleSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.veScaleSlider.LabelWidth = 140;
            this.veScaleSlider.Location = new System.Drawing.Point(1422, 646);
            this.veScaleSlider.MaxValue = 3F;
            this.veScaleSlider.MinValue = 0.5F;
            this.veScaleSlider.Name = "veScaleSlider";
            this.veScaleSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.veScaleSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.veScaleSlider.PointerColor = System.Drawing.Color.DimGray;
            this.veScaleSlider.RimColor = System.Drawing.Color.Black;
            this.veScaleSlider.Size = new System.Drawing.Size(365, 60);
            this.veScaleSlider.SliderName = "VE scale";
            this.veScaleSlider.SliderSize = 8;
            this.veScaleSlider.Step = 0.1F;
            this.veScaleSlider.TabIndex = 19;
            this.veScaleSlider.Text = "veScaleSlider";
            this.veScaleSlider.Value = 2F;
            this.veScaleSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // maxVeRpmSlider
            // 
            this.maxVeRpmSlider.DecimalPlaces = 1;
            this.maxVeRpmSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.maxVeRpmSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.maxVeRpmSlider.LabelWidth = 140;
            this.maxVeRpmSlider.Location = new System.Drawing.Point(1422, 602);
            this.maxVeRpmSlider.MaxValue = 6000F;
            this.maxVeRpmSlider.MinValue = 1000F;
            this.maxVeRpmSlider.Name = "maxVeRpmSlider";
            this.maxVeRpmSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.maxVeRpmSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.maxVeRpmSlider.PointerColor = System.Drawing.Color.DimGray;
            this.maxVeRpmSlider.RimColor = System.Drawing.Color.Black;
            this.maxVeRpmSlider.Size = new System.Drawing.Size(365, 60);
            this.maxVeRpmSlider.SliderName = "Max VE RPM";
            this.maxVeRpmSlider.SliderSize = 8;
            this.maxVeRpmSlider.Step = 100F;
            this.maxVeRpmSlider.TabIndex = 18;
            this.maxVeRpmSlider.Text = "maxVeRpmSlider";
            this.maxVeRpmSlider.Value = 4000F;
            this.maxVeRpmSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // rpmLimitSlider
            // 
            this.rpmLimitSlider.DecimalPlaces = 0;
            this.rpmLimitSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rpmLimitSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rpmLimitSlider.LabelWidth = 140;
            this.rpmLimitSlider.Location = new System.Drawing.Point(1027, 695);
            this.rpmLimitSlider.MaxValue = 10000F;
            this.rpmLimitSlider.MinValue = 4000F;
            this.rpmLimitSlider.Name = "rpmLimitSlider";
            this.rpmLimitSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.rpmLimitSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.rpmLimitSlider.PointerColor = System.Drawing.Color.DimGray;
            this.rpmLimitSlider.RimColor = System.Drawing.Color.Black;
            this.rpmLimitSlider.Size = new System.Drawing.Size(365, 60);
            this.rpmLimitSlider.SliderName = "RPM limiter";
            this.rpmLimitSlider.SliderSize = 8;
            this.rpmLimitSlider.Step = 100F;
            this.rpmLimitSlider.TabIndex = 12;
            this.rpmLimitSlider.Text = "rpmLimitSlider";
            this.rpmLimitSlider.Value = 6000F;
            this.rpmLimitSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // inertiaSlider
            // 
            this.inertiaSlider.DecimalPlaces = 3;
            this.inertiaSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.inertiaSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.inertiaSlider.LabelWidth = 140;
            this.inertiaSlider.Location = new System.Drawing.Point(1027, 651);
            this.inertiaSlider.MaxValue = 0.3F;
            this.inertiaSlider.MinValue = 0.05F;
            this.inertiaSlider.Name = "inertiaSlider";
            this.inertiaSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.inertiaSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.inertiaSlider.PointerColor = System.Drawing.Color.DimGray;
            this.inertiaSlider.RimColor = System.Drawing.Color.Black;
            this.inertiaSlider.Size = new System.Drawing.Size(365, 60);
            this.inertiaSlider.SliderName = "Inertia";
            this.inertiaSlider.SliderSize = 8;
            this.inertiaSlider.Step = 0.001F;
            this.inertiaSlider.TabIndex = 8;
            this.inertiaSlider.Text = "inertiaSlider";
            this.inertiaSlider.Value = 0.12F;
            this.inertiaSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // idleThrottleSlider
            // 
            this.idleThrottleSlider.DecimalPlaces = 4;
            this.idleThrottleSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.idleThrottleSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.idleThrottleSlider.LabelWidth = 140;
            this.idleThrottleSlider.Location = new System.Drawing.Point(1027, 602);
            this.idleThrottleSlider.MaxValue = 0.08F;
            this.idleThrottleSlider.MinValue = 0.005F;
            this.idleThrottleSlider.Name = "idleThrottleSlider";
            this.idleThrottleSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.idleThrottleSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.idleThrottleSlider.PointerColor = System.Drawing.Color.DimGray;
            this.idleThrottleSlider.RimColor = System.Drawing.Color.Black;
            this.idleThrottleSlider.Size = new System.Drawing.Size(365, 60);
            this.idleThrottleSlider.SliderName = "Idle throttle";
            this.idleThrottleSlider.SliderSize = 8;
            this.idleThrottleSlider.Step = 0.001F;
            this.idleThrottleSlider.TabIndex = 7;
            this.idleThrottleSlider.Text = "idleThrottleSlider";
            this.idleThrottleSlider.Value = 0.017F;
            this.idleThrottleSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // displacementSlider
            // 
            this.displacementSlider.DecimalPlaces = 1;
            this.displacementSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.displacementSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.displacementSlider.LabelWidth = 140;
            this.displacementSlider.Location = new System.Drawing.Point(1027, 555);
            this.displacementSlider.MaxValue = 6F;
            this.displacementSlider.MinValue = 1F;
            this.displacementSlider.Name = "displacementSlider";
            this.displacementSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.displacementSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.displacementSlider.PointerColor = System.Drawing.Color.DimGray;
            this.displacementSlider.RimColor = System.Drawing.Color.Black;
            this.displacementSlider.Size = new System.Drawing.Size(365, 60);
            this.displacementSlider.SliderName = "Displacement (L)";
            this.displacementSlider.SliderSize = 8;
            this.displacementSlider.Step = 0.1F;
            this.displacementSlider.TabIndex = 6;
            this.displacementSlider.Text = "displacementSlider";
            this.displacementSlider.Value = 2F;
            this.displacementSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // maxVeSlider
            // 
            this.maxVeSlider.DecimalPlaces = 2;
            this.maxVeSlider.DialColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.maxVeSlider.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.maxVeSlider.LabelWidth = 140;
            this.maxVeSlider.Location = new System.Drawing.Point(1422, 555);
            this.maxVeSlider.MaxValue = 1.2F;
            this.maxVeSlider.MinValue = 0.8F;
            this.maxVeSlider.Name = "maxVeSlider";
            this.maxVeSlider.NameFont = new System.Drawing.Font("Segoe UI", 10F);
            this.maxVeSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.maxVeSlider.PointerColor = System.Drawing.Color.DimGray;
            this.maxVeSlider.RimColor = System.Drawing.Color.Black;
            this.maxVeSlider.Size = new System.Drawing.Size(365, 60);
            this.maxVeSlider.SliderName = "Max VE";
            this.maxVeSlider.SliderSize = 8;
            this.maxVeSlider.Step = 0.01F;
            this.maxVeSlider.TabIndex = 18;
            this.maxVeSlider.Text = "maxVeSlider";
            this.maxVeSlider.Value = 1.03F;
            this.maxVeSlider.ValueFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1858, 767);
            this.Controls.Add(this.dynoThrottleSlider);
            this.Controls.Add(this.torqueBar);
            this.Controls.Add(this.powerBar);
            this.Controls.Add(this.maxAirflowRpmSlider);
            this.Controls.Add(this.veScaleSlider);
            this.Controls.Add(this.maxVeRpmSlider);
            this.Controls.Add(this.gearLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rpmLimitSlider);
            this.Controls.Add(this.mafGauge);
            this.Controls.Add(this.afrGauge);
            this.Controls.Add(this.inertiaSlider);
            this.Controls.Add(this.idleThrottleSlider);
            this.Controls.Add(this.displacementSlider);
            this.Controls.Add(this.fuelRateGauge);
            this.Controls.Add(this.loadGauge);
            this.Controls.Add(this.throttleGauge);
            this.Controls.Add(this.speedGauge);
            this.Controls.Add(this.rpmGauge);
            this.Controls.Add(this.mapGauge);
            this.Controls.Add(this.maxVeSlider);
            this.Controls.Add(this.tabControl1);
            this.Name = "Window";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dynoChart)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.dynoPage.ResumeLayout(false);
            this.shiftingPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.shiftingChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart dynoChart;
        private AquaControls.AquaGauge rpmGauge;
        private Timer timer1;
        private AquaControls.AquaGauge throttleGauge;
        private AquaControls.AquaGauge mapGauge;
        private AquaControls.AquaGauge loadGauge;
        private AquaControls.AquaGauge fuelRateGauge;
        private DialSlider displacementSlider;
        private DialSlider idleThrottleSlider;
        private DialSlider inertiaSlider;
        private AquaControls.AquaGauge afrGauge;
        private AquaControls.AquaGauge speedGauge;
        private AquaControls.AquaGauge mafGauge;
        private DialSlider rpmLimitSlider;
        private VerticalFillBarSimple powerBar;
        private VerticalFillBarSimple torqueBar;
        private Label label1;
        private Label gearLabel;
        private DialSlider maxVeRpmSlider;
        private DialSlider veScaleSlider;
        private DialSlider maxAirflowRpmSlider;
        private DialSlider maxVeSlider;
        private TabControl tabControl1;
        private TabPage shiftingPage;
        private TabPage dynoPage;
        private System.Windows.Forms.DataVisualization.Charting.Chart shiftingChart;
        private DialSlider dynoThrottleSlider;
        private Button torqueGraphButton;
    }
}

