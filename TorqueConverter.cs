using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class TorqueConverter : BaseClutch {

        public const double STALL_TORQUE_RATIO = 2.0;
        public const double COUPLING_POINT = 0.9;
        public const double K_FACTOR_MIN = 5.0;
        public const double K_FACTOR_MAX = 110.0;

        private double pumpRpm;
        private double turbineRpm;

        public TorqueConverter(Engine engine, Gearbox gearbox) : base(engine, gearbox) {
            this.pumpRpm = 0.0;
            this.turbineRpm = 0.0;
        }

        public void Update(double dt) {
            pumpRpm = engine.GetRPM();
            turbineRpm = gearbox.GetInputRPM();

            if (pumpRpm <= 0.1) {
                gearbox.SetInputTorque(0.0);
                return;
            }
            
            double speedRatio = turbineRpm / pumpRpm;

            bool isLocked = false;
            double transferTorque;
            double outputTorque;

            if (isLocked && gearbox.GetCurrentGear() != 0) {
                double resistanceTorque = (gearbox.GetTotalResistance() * gearbox.GetWheelRadius()) / gearbox.GetTotalRatio();

                double totalInertia = engine.GetInertia() + gearbox.GetCarInertia();
                double netTorque = engine.GetBrakeTorque() - resistanceTorque;
                double netAccel = netTorque / totalInertia;

                transferTorque = engine.GetBrakeTorque() - (engine.GetInertia() * netAccel);
                outputTorque = transferTorque;

                double avgRpm = (pumpRpm + turbineRpm) / 2.0;
                engine.SetRPM(avgRpm);
                gearbox.SetInputRPM(avgRpm);
                Console.WriteLine($"{(DateTimeOffset.Now.ToUnixTimeMilliseconds() - Program.startTime) / 1000.0:F2}s | RPM: {engine.GetRPM(),4:F0} | SPD: {gearbox.GetCarSpeed() * (Units.km / Units.h):F2} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TQ_OUT: {outputTorque,6:F1} Nm | SPD_RATIO: {speedRatio:F2} | LOCKED");
            } else {
                double torqueMultiplier = 1.0;
                if (speedRatio >= 0 && speedRatio < COUPLING_POINT) {
                    torqueMultiplier = STALL_TORQUE_RATIO - (speedRatio / COUPLING_POINT) * (STALL_TORQUE_RATIO - 1.0);
                }

                double ratioScaled = speedRatio > 1.0 ? Math.Min(1.0, 1.0 / speedRatio) : Math.Min(1.0, Math.Abs(speedRatio));
                double kFactor = MathHelper.Lerp(K_FACTOR_MAX, K_FACTOR_MIN, Math.Pow(Math.Min(1.0, ratioScaled), 1.2));
                //double kFactor = MathHelper.Lerp(K_FACTOR_MAX, K_FACTOR_MIN, MathHelper.SigmoidFunction(ratioScaled, 3.0, 0.5));

                double slipRpm = pumpRpm - turbineRpm;
                transferTorque = Math.Pow(slipRpm / kFactor, 2) * Math.Sign(slipRpm);
                outputTorque = transferTorque * torqueMultiplier;

                Console.WriteLine($"{(DateTimeOffset.Now.ToUnixTimeMilliseconds() - Program.startTime) / 1000.0:F2}s | RPM: {engine.GetRPM(),4:F0} | SPD: {gearbox.GetCarSpeed() * (Units.km / Units.h):F2} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TQ_OUT: {outputTorque,6:F1} Nm | SPD_RATIO: {speedRatio:F2} | K: {kFactor:F2} | MULT: {torqueMultiplier:F2} | SLIP: {slipRpm:F2}");
            }

            engine.AddLoadTorque(transferTorque);
            gearbox.SetInputTorque(outputTorque);
        }
    }
}
