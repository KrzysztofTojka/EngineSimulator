using System;

namespace EngineSimulator {
    public class ECU {

        public double idleThrottle;

        Engine engine;

        double throttle;

        public ECU(Engine engine) {
            this.engine = engine;
            this.throttle = 0.0;
            this.idleThrottle = 0.0175;
        }

        public double GetThrottleMap(double throttle) {
            return MathHelper.PowerFunction(throttle, 0.0, 1.0, 1.5);
        }

        public void SetThrottle(double throttle) {
            this.throttle = throttle;
        }

        public bool ShouldCutFuel() {
            bool redline = engine.GetRPM() > engine.GetMaxRPM();
            bool decel = engine.GetRPM() > 1300 && throttle == 0.0;
            return redline /*|| decel*/;
        }

        public double GetThrottle() {
            return idleThrottle + (1.0 - idleThrottle) * GetThrottleMap(throttle);
        }

        public double GetAFR(double rpm, double throttle) {
            double afrMax = 15.2;
            double afrMin = 12.8;

            return afrMax - (afrMax - afrMin) * Math.Sin(throttle * (Math.PI / 2)) * MathHelper.Random(0.97, 1.03);
        }

    }
}
