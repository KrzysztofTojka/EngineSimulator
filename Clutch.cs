using EngineSimulator;
using System;
using System.Windows.Forms;

namespace EngineSimulator {
    public class Clutch : BaseClutch {

        public const double DEAD_ZONE = 0.25;

        private double engagement;
        private double outputTorque;
        private double outputRpm;

        private double maxTorque = 400;
        private double damping = 15.0;

        public Clutch(Engine engine, Gearbox gearbox, double maxTorque = 400) : base(engine, gearbox) {
            this.maxTorque = maxTorque;
            this.engagement = 0.0;
            this.outputTorque = 0.0;
            this.outputRpm = 0.0;
        }

        public void Update(double dt) {
            double engineOmega = engine.GetRPM() * 2 * Math.PI / 60;
            double gearboxOmega = gearbox.GetInputRPM() * 2 * Math.PI / 60;

            double slip = engineOmega - gearboxOmega;
            double torqueLimit = maxTorque * engagement;

            bool isLocked = engagement > 0.9 && Math.Abs(slip) < 35.0 && gearbox.GetCurrentGear() > 0;

            double torqueTransfer;
            if (isLocked) {
                double resistanceTorque = (gearbox.GetTotalResistance() * gearbox.GetWheelRadius()) / gearbox.GetTotalRatio();
                double netTorque = engine.GetBrakeTorque() - resistanceTorque;
                double netAccel = netTorque / (engine.GetInertia() + gearbox.GetCarInertia());
                torqueTransfer = engine.GetBrakeTorque() - (engine.GetInertia() * netAccel);
                double avgRpm = (engine.GetRPM() + gearbox.GetInputRPM()) / 2.0;
                engine.SetRPM(avgRpm);
                gearbox.SetInputRPM(avgRpm);
                //outputRpm = avgRpm;
            } else {
                torqueTransfer = slip * damping * engagement;
                torqueTransfer = MathHelper.Clamp(torqueTransfer, -torqueLimit, torqueLimit);
            }

            engine.AddLoadTorque(torqueTransfer);
            //gearbox.SetInputTorque(torqueTransfer);
            outputTorque = torqueTransfer;

            //Console.WriteLine($"RPM: {engine.GetRPM(),4:F0} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TRSF: {torqueTransfer,6:F1} Nm | SLIP: {slip,6:F2}");
        }

        public void SetPosition(double position) {
            position = Math.Max(0.0, (position - DEAD_ZONE) / (1.0 - DEAD_ZONE));
            engagement = Math.Pow(position, 2.5);
            engagement = MathHelper.Clamp(engagement, 0.0, 1.0);
        }

        public void SetEngagement(double engagement) {
            this.engagement = MathHelper.Clamp(engagement, 0.0, 1.0);
        }

        public double GetEngangement() {
            return engagement;
        }

        public double GetOutputTorque() {
            return outputTorque;
        }

    }
}