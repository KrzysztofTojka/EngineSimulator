using System;

namespace EngineSimulator {
    public class ECU {

        public double idleThrottle;

        private Engine engine;

        private double throttle;
        private double throttleOverride;

        public ECU(Engine engine) {
            this.engine = engine;
            this.throttle = 0.0;
            this.idleThrottle = 0.012; // 4.0 - 0.006, 2.0 - 0.012
            this.throttleOverride = -1;
        }

        public double GetThrottleMap(double throttle) {
            if (engine is DieselEngine) {
                return MathHelper.PowerFunction(throttle, 0.0, 1.0, 1.4);
            }
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
            if (throttleOverride != -1) {
                return throttleOverride;
            }

            double throttleMapped = GetThrottleMap(throttle);
            double throttleFinal;

            if (!(engine is DieselEngine) && engine.HasTurbo()) {
                double maxMafNA = engine.GetMaxMAF(engine.GetMaxRPM(), 1.0, false);
                double maxMafTurbo = engine.GetMaxMAF(engine.GetMaxRPM(), 1.0, true);

                double requestedAirflow = maxMafTurbo * throttleMapped; // TODO use Math.Sin

                throttleFinal = Math.Min(1.0, requestedAirflow / maxMafNA);
            } else {
                throttleFinal = throttleMapped;
            }

            return GetIdleThrottle() + (1.0 - GetIdleThrottle()) * throttleFinal;
        }

        public double GetIdleThrottle() {
            double multiplier = 1.0;
            if (Program.GetGearbox() is AutomaticGearbox && !(Program.GetGearbox() is DualClutchGearbox) && Program.GetGearbox().GetCurrentGear() != 0) {
                multiplier = 1.1 + ((1.0 - Program.GetBrakePedalPosition()) * 0.25);
            }
            return idleThrottle * multiplier;
        }

        public void SetIdleThrottle(double idleThrottle) {
            this.idleThrottle = idleThrottle;
        }

        public double GetThrottlePedal() {
            return throttle;
        }

        public void SetThrottleOverride(double throttle) {
            this.throttleOverride = throttle;
        }

        public double GetAFR(double rpm, double map, double throttle, bool random = true) {
            if (engine is GasolineEngine) {
                return GetAFRGasoline(rpm, map, random);
            } else if (engine is DieselEngine) {
                return GetAFRDiesel(rpm, throttle, random);
            } else {
                throw new NotImplementedException();
            }
        }

        public double GetAFRGasoline(double rpm, double map, bool random = true) {
            double mapKpa = map * Units.kPa;

            if (mapKpa <= 35) {
                return 14.7;
            }

            double loadFactor = Math.Min(1.1, Math.Pow((mapKpa - 35.0) / (engine.GetPressureAtm() * Units.kPa - 35.0), 5));

            double rpmFactor = 0.8 + 0.2 * (rpm / engine.GetMaxRPM());

            return 14.7 - (14.7 - 12.5) * loadFactor * rpmFactor;
        }

        public double GetAFRDiesel(double rpm, double throttle, bool random = true) {
            double maxFuelFlow = (1 / engine.AFR_STOICH) * engine.GetMaxAirflowRpm();
            double fuelFlowRequested = throttle * maxFuelFlow;
            double afrRequested = 1 / (fuelFlowRequested / rpm);

            
            return MathHelper.Clamp(afrRequested, engine.AFR_STOICH, 120.0);
        }
    }
}
