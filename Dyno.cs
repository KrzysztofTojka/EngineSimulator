using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class Dyno {

        private bool running;

        private double maxTorque;
        private double maxPower;

        public List<double> rpmList = new List<double>();
        public List<double> torqueList = new List<double>();
        public List<double> powerList = new List<double>();

        public Dyno() {
            this.running = false;
            this.maxTorque = 0.0;
            this.maxPower = 0.0;
        }

        public void DoMaxTorqueRun(double throttle) {
            //if (true) return;
            running = true;
            Engine engine = Program.GetEngine().Clone();

            MathHelper.UseRandom(false);
            rpmList.Clear();
            torqueList.Clear();
            powerList.Clear();

            engine.SetIgnition(true);
            engine.GetECU().SetThrottle(throttle);

            double rpm = 0.0;

            for (int i = 0; i <= (int) engine.GetMaxRPM() / 10; i++) {
                engine.SetRPM(rpm);
                engine.Update(0);

                double torque = engine.GetBrakeTorque();
                double power = engine.GetBrakePower() * Units.HP;

                double fuelTorque = engine.GetTorque();
                double fuelPower = engine.GetPower() * Units.HP;

                if (fuelTorque > maxTorque) maxTorque = fuelTorque;
                if (fuelPower > maxPower) maxPower = fuelPower;

                if (i > 0 && i % 10 == 0) {
                    rpmList.Add(rpm);
                    torqueList.Add(torque);
                    powerList.Add(power);
                }

                //Console.WriteLine($"RPM: {rpm}, Torque: {torque}, Power: {power}");
                //engine.ShowInfo();

                rpm += 10;
            }

            MathHelper.UseRandom(true);
            running = false;
        }

        public void DoFullRun(bool printInfo = true) {
            Engine engine = Program.GetEngine().Clone();
            MathHelper.UseRandom(false);

            File.WriteAllLines("result.csv", new string[] { engine.GetCsvHeader() });

            engine.SetIgnition(true);
            engine.SetRPM(500);
            engine.GetECU().SetThrottle(engine.GetECU().GetIdleThrottle());
            engine.Update(0);

            for (int rpm = 500; rpm <= engine.GetMaxRPM(); rpm += 250) {
                engine.SetRPM(rpm);

                for (double throttle = 0.0; throttle <= 1.0; throttle += 0.07 * Math.Min(1.0, rpm / engine.GetMaxRPM())) {
                    if (throttle == 0.0) {
                        throttle = engine.GetECU().GetIdleThrottle();
                    }

                    engine.GetECU().SetThrottle(throttle);

                    engine.Update(0);

                    string line = engine.GetCsvLine();
                    File.AppendAllLines("result.csv", new string[] { line });
                }

                if (printInfo) {
                    engine.ShowInfo();
                    if (engine.HasTurbo()) {
                        engine.GetTurbocharger().ShowInfo();
                    }
                }
            }

            MathHelper.UseRandom(true);

            Process.Start("cmd", "/C python torque_graph.py");
        }

        public double GetMaxTorque() {
            return maxTorque;
        }

        public double GetMaxPower() {
            return maxPower;
        }

        public bool isRunning() {
            return running;
        }

    }
}
