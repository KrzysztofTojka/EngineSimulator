using AquaControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        private bool initialized = false;
        private long lastTickTime;

        public Window() {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.engine = Program.GetEngine();
            this.dyno = Program.GetDyno();
            this.lastTickTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            InitializeComponent();
            DrawDyno();
            DrawShifting();

            InitControls();
            
            initialized = true;
        }

        private void InitControls() {
            idleThrottleSlider.Value = (float) engine.GetECU().GetIdleThrottle();
            displacementSlider.Value = (float) (engine.GetDisplacement() * Units.L);
            inertiaSlider.Value = (float) engine.GetInertia();
            rpmLimitSlider.Value = (float) engine.GetMaxRPM();
            maxVeSlider.Value = (float) engine.GetMaxVe();
            maxVeRpmSlider.Value = (float) engine.GetMaxVeRpm();
            maxVeRpmSlider.MaxValue = rpmLimitSlider.Value;
            veScaleSlider.Value = (float) engine.GetVeRangeScale();
            maxAirflowRpmSlider.Value = (float) engine.GetMaxAirflowRpm();
            maxAirflowRpmSlider.MaxValue = rpmLimitSlider.Value;

            if (engine is DieselEngine) {
                afrGauge.MinValue = 14.0f;
                afrGauge.MaxValue = 100.0f;
            }

            if (engine.HasTurbo()) {
                mapGauge.MaxValue = (float)(100.0 * (1.0 + engine.GetTurbocharger().GetMaxBoost()));
                mapGauge.NoOfDivisions = (int)(mapGauge.MaxValue / (mapGauge.MaxValue > 150.0 ? 20 : 10));
                mapGauge.Threshold2Start = (float)(engine.GetPressureAtm() * Units.kPa);
                mapGauge.Threshold2Stop = mapGauge.MaxValue;
            }
        }


        private void DrawDyno() {
            torqueBar.MaxValue = (float) dyno.GetMaxTorque();
            powerBar.MaxValue = (float) dyno.GetMaxPower();

            dynoChart.Series.Clear();
            dynoChart.ChartAreas[0].AxisY.Minimum = 0.0;
            dynoChart.ChartAreas[0].Position.X = 2;
            dynoChart.ChartAreas[0].Position.Y = 4;
            dynoChart.ChartAreas[0].Position.Width = 96;
            dynoChart.ChartAreas[0].Position.Height = 90;

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

        private void DrawShifting() {
            Gearbox.ShiftData shiftData = Program.GetGearbox().GetShiftData();
            var area = shiftingChart.ChartAreas[0];

            area.AxisY.Minimum = 0.0;
            area.AxisY.Maximum = 1.0;

            area.AxisX.Interval = 20.0;
            area.AxisX.Minimum = 0.0;
            area.AxisX.Maximum = (int)((shiftData.speedValues[shiftData.throttleValues.Last().Value.Count - 1] + 10.0) / 10) * 10.0;

            shiftingChart.Series.Clear();

            foreach (int gear in shiftData.throttleValues.Keys) {
                Series line = new Series($"Gear {gear} -> {gear + 1}");
                line.ChartType = SeriesChartType.Line;
                line.BorderWidth = 3;

                List<double> throttleValues = shiftData.throttleValues[gear];
                for (int i = 0; i < throttleValues.Count; i++) {
                    line.Points.AddXY(shiftData.speedValues[i], throttleValues[i]);
                }

                shiftingChart.Series.Add(line);
            }
        }

        private void UpdatePowerBars() {
            torqueBar.Value = (int) MathHelper.Clamp(engine.GetTorque(), 0.0, torqueBar.MaxValue);
            powerBar.Value = (int)MathHelper.Clamp(engine.GetPower() * Units.HP, 0.0, powerBar.MaxValue);
        }

        private void UpdateDyno() {
            dyno.DoMaxTorqueRun(dynoThrottleSlider.Value);
            DrawDyno();
        }

        private void dyno_Paint(object sender, PaintEventArgs e) {
           
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (!initialized) return;

            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double dt = (now - lastTickTime) / 1000.0;

            throttleBar.Value = (float) Program.GetThrottlePedalPosition();
            brakeBar.Value = (float) Program.GetBrakePedalPosition();
            clutchBar.Value = (float) Program.GetClutchPedalPosition();

            SetGaugeValue(rpmGauge, engine.GetRPM() / 100.0, dt);
            rpmGauge.DigitalValue = (float)engine.GetRPM();

            SetGaugeValue(speedGauge, Program.GetGearbox().GetCarSpeed() * (Units.km / Units.h), dt);
            SetGaugeValue(throttleGauge, engine.GetThrottle() * 100.0, dt);
            SetGaugeValue(loadGauge, engine.GetLoad() * 100.0, dt);
            SetGaugeValue(mapGauge, engine.GetMAP() * Units.kPa, dt);
            SetGaugeValue(mafGauge, engine.GetMAF() * 1000.0, dt);
            SetGaugeValue(fuelRateGauge, Units.kgs_to_Lh(engine.GetFuelRate(), Program.GetEngine().FUEL_DENSITY), dt);
            SetGaugeValue(afrGauge, engine.GetAFR(), dt);

            gearLabel.Text = Program.GetGearbox().GetGearLabel();

            if (idleThrottleSlider.ValueChanged()) {
                engine.GetECU().idleThrottle = idleThrottleSlider.Value;
            }
            if (displacementSlider.ValueChanged()) {
                engine.SetDisplacement(displacementSlider.Value);
                UpdateDyno();
            }
            if (inertiaSlider.ValueChanged()) {
                engine.SetInertia(inertiaSlider.Value);
            }
            if (rpmLimitSlider.ValueChanged()) {
                SetRpmLimit(rpmLimitSlider.Value);
                maxVeRpmSlider.MaxValue = rpmLimitSlider.Value;
                maxAirflowRpmSlider.MaxValue = rpmLimitSlider.Value;
                UpdateDyno();
            }

            if (maxVeSlider.ValueChanged()) {
                engine.SetMaxVe(maxVeSlider.Value);
                UpdateDyno();
            }

            if (maxVeRpmSlider.ValueChanged()) {
                engine.SetMaxVeRpm(maxVeRpmSlider.Value);
                UpdateDyno();
            }

            if (veScaleSlider.ValueChanged()) {
                engine.SetVeRangeScale(veScaleSlider.Value);
                UpdateDyno();
            }

            if (maxAirflowRpmSlider.ValueChanged()) {
                engine.SetMaxAirflowRpm(maxAirflowRpmSlider.Value);
                UpdateDyno();
            }

            if (dynoThrottleSlider.ValueChanged()) {
                UpdateDyno();
            }

            UpdatePowerBars();

            lastTickTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }


        /// <param name="smoothFactor">time (s) to reach approx. 2/3 of targetValue</param>
        /// <param name="maxSpeed">Max % of scale per second</param>
        private void SetGaugeValue(AquaGauge gauge, double targetValue, double dt, float smoothFactor = 0.05f, float maxSpeed = 3.0f) {
            double smoothing = Math.Exp(-(1.0f / smoothFactor));
            float newValue = (float)(targetValue + (gauge.Value - targetValue) * Math.Pow(smoothing, dt));

            float step = (float)newValue - gauge.Value;

            float range = gauge.MaxValue - gauge.MinValue;

            float maxStepValue = range * maxSpeed * (float) dt;

            if (Math.Abs(step) > maxStepValue) {
                step = Math.Sign(step) * maxStepValue;
            }

            gauge.Value = gauge.Value + step;
            gauge.DigitalValue = (float)targetValue;
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

        private void torqueGraphButton_Click(object sender, EventArgs e) {
            dyno.DoFullRun(printInfo: false);
        }
    }
}
