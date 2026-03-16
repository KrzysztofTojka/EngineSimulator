using System;

namespace EngineSimulator {
    public class ECU {

        public double idleThrottle;

        Engine engine;

        double throttle;

        public ECU(Engine engine) {
            this.engine = engine;
            this.throttle = 0.0;
            this.idleThrottle = 0.0118;
        }

        public double GetThrottleMap(double throttle) {
            return MathHelper.PowerFunction(throttle, 0.0, 1.0, 1.5);
        }

        public void SetThrottle(double throttle) {
            this.throttle = throttle;
        }

        public bool ShouldCutFuel() {
            // for now
            if (Program.GetGearbox() is AutomaticGearbox auto) {
                if (auto.GetShiftPhase() != AutomaticGearbox.ShiftPhase.IDLE) {
                    return true;
                }
            }

            bool redline = engine.GetRPM() > engine.GetMaxRPM();
            bool decel = engine.GetRPM() > 1300 && throttle == 0.0;
            return redline /*|| decel*/;
        }

        public double GetThrottle() {
            return idleThrottle + (1.0 - idleThrottle) * GetThrottleMap(throttle);
        }

        public double GetIdleThrottle() {
            return idleThrottle;
        }

        public double GetThrottlePedal() {
            return throttle;
        }

        public double GetAFR(double rpm, double throttle, bool random = true) {
            double afrMax = 15.2;
            double afrMin = 12.8;

            return afrMax - (afrMax - afrMin) * Math.Sin(throttle * (Math.PI / 2)) * MathHelper.Random(0.98, 1.02, random);
        }

    }
}
