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
            bool redline = engine.GetRPM() > engine.GetMaxRPM();
            bool decel = engine.GetRPM() > 1300 && throttle == 0.0;
            return redline /*|| decel*/;
        }

        public double GetThrottle() {
            double throttleMapped = GetThrottleMap(throttle);
            double throttleFinal;

            if (engine.HasTurbo()) {
                double maxMafNA = engine.GetMaxMAF(engine.GetMaxRPM(), false);
                double maxMafTurbo = engine.GetMaxMAF(engine.GetMaxRPM(), true);

                double requestedAirflow = maxMafTurbo * throttleMapped; // TODO use Math.Sin

                throttleFinal = Math.Min(1.0, requestedAirflow / maxMafNA);
            } else {
                throttleFinal = throttleMapped;
            }

            return idleThrottle + (1.0 - idleThrottle) * throttleFinal;
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

            double loadFactor = Math.Min(1.1, Math.Pow((mapKpa - 35.0) / (engine.GetPressureAtm() * Units.kPa - 35.0), 5));

            double rpmFactor = 0.8 + 0.2 * (rpm / engine.GetMaxRPM());

            return 14.7 - (14.7 - 12.5) * loadFactor * rpmFactor;
        }

    }
}
