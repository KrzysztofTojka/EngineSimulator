using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class TorqueConverter {

        public const double STALL_TORQUE_RATIO = 2.0;
        public const double COUPLING_POINT = 0.9;
        public const double K_FACTOR = 100.0;

        Engine engine;
        AutomaticGearbox gearbox;

        private double pumpRpm;
        private double turbineRpm;

        public TorqueConverter(Engine engine, AutomaticGearbox gearbox) {
            this.engine = engine;
            this.gearbox = gearbox;

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

            double ratioScaled = speedRatio > 1.0 ? Math.Min(1.0, 1.0 / speedRatio) : Math.Min(1.0, Math.Abs(speedRatio));

            double torqueMultiplier = 1.0;
            if (speedRatio >= 0 && speedRatio < COUPLING_POINT) {
                torqueMultiplier = STALL_TORQUE_RATIO - (speedRatio / COUPLING_POINT) * (STALL_TORQUE_RATIO - 1.0);
            }
            
            double kFactor = 150.0 - (130.0 * Math.Pow(Math.Min(1.0, ratioScaled), 0.5));

            double slipRpm = pumpRpm - turbineRpm;
            double transferTorque = Math.Pow(slipRpm / kFactor, 2) * Math.Sign(slipRpm);

            bool isLocked = false; // todo

            double outputTorque = transferTorque * torqueMultiplier;

            engine.SetLoadTorque(transferTorque);
            gearbox.SetInputTorque(outputTorque);

            //Console.WriteLine($"RPM: {engine.GetRPM(),4:F0} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TQ_OUT: {outputTorque,6:F1} Nm | SPD_RATIO: {speedRatio:F2} | MULT: {torqueMultiplier:F2} | RAT: {gearbox.GetTotalRatio():F2}");
        }
    }
}
