using System;

namespace EngineSimulator {
    public class ECU {

        public double idleThrottle;

        Engine engine;

        double throttle;

        public ECU(Engine engine) {
            this.engine = engine;
            this.throttle = 0.0;
            this.idleThrottle = 0.012; // 4.0 - 0.006, 2.0 - 0.012
        }

        public double GetThrottleMap(double throttle) {
            return MathHelper.PowerFunction(throttle, 0.0, 1.0, 1.75);
        }

        public void SetThrottle(double throttle) {
            this.throttle = throttle;
        }

        public bool ShouldCutFuel() {
            // for now
            //if (Program.GetGearbox() is AutomaticGearbox auto) {
            //    if (auto.GetShiftPhase() != AutomaticGearbox.ShiftPhase.IDLE) {
            //        return true;
            //    }
            //}

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

        public void SetIdleThrottle(double idleThrottle) {
            this.idleThrottle = idleThrottle;
        }

        public double GetThrottlePedal() {
            return throttle;
        }

        public double GetAFR(double rpm, double map, bool random = true) {
            double mapKpa = map * Units.kPa;

            if (mapKpa <= 35) {
                return 14.7;
            }

            double loadFactor = Math.Pow((mapKpa - 35.0) / (engine.GetPressureAtm() * Units.kPa - 35.0), 5);

            double rpmFactor = 0.8 + 0.2 * (rpm / engine.GetMaxRPM());

            return 14.7 - (14.7 - 12.5) * loadFactor * rpmFactor;
        }

    }
}
