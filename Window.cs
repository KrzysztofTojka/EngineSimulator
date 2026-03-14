using AquaControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EngineSimulator {
    public partial class Window : Form {

        private Engine engine;
        private Dyno dyno;

        public Window() {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.engine = Program.GetEngine();
            this.dyno = Program.GetDyno();
            InitializeComponent();
            DrawDyno();

            InitControls();
        }

        private void InitControls() {
            idleThrottleSlider.Value = (float) engine.GetECU().idleThrottle;
        }

        private void DrawDyno() {
            torqueBar.MaxValue = (float) dyno.GetMaxTorque();
            powerBar.MaxValue = (float) dyno.GetMaxPower();

            dynoChart.Series.Clear();

            Series powerLine = new Series("Power");
            powerLine.ChartType = SeriesChartType.Line;
            powerLine.BorderWidth = 3;
            powerLine.Color = Color.Red;

            Series torqueLine = new Series("Torque");
            torqueLine.ChartType = SeriesChartType.Line;
            torqueLine.BorderWidth = 3;
            torqueLine.Color = Color.Blue;

            dynoChart.Series.Add(powerLine);
            dynoChart.Series.Add(torqueLine);

            for (int i = 0; i < dyno.rpmList.Count; i++) {
                powerLine.Points.AddXY(dyno.rpmList[i], dyno.powerList[i]);
                torqueLine.Points.AddXY(dyno.rpmList[i], dyno.torqueList[i]);
            }
        }

        private void UpdateDyno() {
            torqueBar.Value = (int) MathHelper.Clamp(engine.GetTorque(), 0.0, torqueBar.MaxValue);
            powerBar.Value = (int) MathHelper.Clamp(engine.GetPower() * Units.HP, 0.0, powerBar.MaxValue);
        }

        private void dyno_Paint(object sender, PaintEventArgs e) {
           
        }

        private void timer1_Tick(object sender, EventArgs e) {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double dt = (now - Program.GetLastUpdateTime()) / 50.0;

            SetGaugeValue(rpmGauge, engine.GetRPM() / 100.0, dt);
            rpmGauge.DigitalValue = (float)engine.GetRPM();

            SetGaugeValue(speedGauge, Program.GetGearbox().GetCarSpeed() * (Units.km / Units.h), dt);
            SetGaugeValue(throttleGauge, engine.GetThrottle() * 100.0, dt);
            SetGaugeValue(loadGauge, engine.GetLoad() * 100.0, dt);
            SetGaugeValue(mapGauge, engine.GetMAP() * Units.KPA, dt);
            SetGaugeValue(mafGauge, engine.GetMAF() * 1000.0, dt);
            SetGaugeValue(fuelRateGauge, Units.kgs_to_Lh(engine.GetFuelRate(), Engine.FUEL_DENSITY), dt);
            SetGaugeValue(afrGauge, engine.GetAFR(), dt);

            gearLabel.Text = Program.GetGearbox().GetCurrentGear() == 0 ? "N" : Program.GetGearbox().GetCurrentGear().ToString();

            if (idleThrottleSlider.ValueChanged()) {
                engine.GetECU().idleThrottle = idleThrottleSlider.Value;
            }
            if (displacementSlider.ValueChanged()) {
                engine.SetDisplacement(displacementSlider.Value);
            }
            if (inertiaSlider.ValueChanged()) {
                engine.SetInertia(inertiaSlider.Value);
            }
            if (rpmLimitSlider.ValueChanged()) {
                SetRpmLimit(rpmLimitSlider.Value);
            }

            UpdateDyno();
        }

        private void SetGaugeValue(AquaGauge gauge, double value, double dt) {
            //gauge.Value = (float) MathHelper.Lerp(gauge.Value, value, dt);
            gauge.Value = (float) value;
            gauge.DigitalValue = (float) value;
        }

        private void SetRpmLimit(double rpmLimit) {
            float maxRpm = ((int)((rpmLimit - 500) / 1000)) * 1000 + 2000;

            rpmGauge.MaxValue = (int)(maxRpm / 100);
            rpmGauge.Threshold2Start = (int)((rpmLimit - 500) / 100);
            rpmGauge.Threshold2Stop = (float) rpmGauge.MaxValue;
            rpmGauge.NoOfDivisions = (int)(rpmGauge.MaxValue / 10);
            rpmGauge.NoOfSubDivisions = maxRpm >= 9000 ? 2 : 3;
            engine.SetMaxRPM(rpmLimit);
        }
    }
}
