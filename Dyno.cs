using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class Dyno {

        private Engine engine;

        private double maxTorque;
        private double maxPower;

        public List<double> rpmList = new List<double>();
        public List<double> torqueList = new List<double>();
        public List<double> powerList = new List<double>();

        public Dyno() {
            this.maxTorque = 0.0;
            this.maxPower = 0.0;
        }

        public void Run(double throttle) {
            engine = Program.GetEngine().Clone();

            MathHelper.UseRandom(false);
            rpmList.Clear();
            torqueList.Clear();
            powerList.Clear();

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

                rpm += 10;

            }

            MathHelper.UseRandom(true);
        }

        public double GetMaxTorque() {
            return maxTorque;
        }

        public double GetMaxPower() {
            return maxPower;
        }

    }
}
