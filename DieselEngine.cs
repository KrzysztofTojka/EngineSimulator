using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class DieselEngine : Engine {

        public override double AFR_STOICH => 15.0;
        public override double FUEL_DENSITY => 846.0; // kg/m3
        public override double LHV => 42_600_000; // J/kg
        public override double BASE_THERMAL_EFFICIENCY => 0.35;

        public DieselEngine(double displacementL, double maxRpm = 4500, double inertia = 0.2) : base(displacementL, maxRpm, inertia) {
            this.SetTurbocharger(new Turbocharger(this, 1.6, 1.537, electronicWastegate: false));
            this.GetECU().SetIdleThrottle(0.042);
            this.maxVe = 0.97;
            this.optimalIntakeRpm = 2400;
            this.veRangeScale = 2.0;
            this.maxAirflowRpm = 4200;
        }

        public DieselEngine(DieselEngine other) : base(other) {
        }

        public override double GetMAF(double throttle, double rpm, bool random = true) {
            return GetMaxMAF(rpm, throttle, true); // kg/s
        }

        public override Engine Clone() {
            return new DieselEngine(this);
        }

        public override double GetBrakingTorque(double rpm, double throttle) {
            // TODO rework
            double staticTorque = 20.0 * ((displacement * Units.L) / 2.0); // 17.0

            double referenceTorque = 12.5; // 7.5
            double referenceRpm = 1000;

            double frictionTorque = referenceTorque * (rpm / referenceRpm);

            double pumpingTorque = Math.Max(1.0, Math.Pow(rpm / 4000, 0.3)) * (100_000 * displacement) / (4.0 * Math.PI);

            return staticTorque + frictionTorque + pumpingTorque;
        }

        public override double GetThermalEfficiency() {
            // TODO rework
            return BASE_THERMAL_EFFICIENCY * (0.92 + 0.08 * (2.0 / (displacement * Units.L)));
        }

        public override double GetFuelPower(double fuelRate, double rpm, double afr, bool random = true) {
            if (rpm < 200) return 0;
            return fuelRate * LHV * MathHelper.Random(0.97, 1.03, random) * (Math.Sin(Math.PI * Math.Min(1.0, rpm / 500) - Math.PI / 2) + 1) / 2; // W
        }

        public override double GetVolumetricEfficiency(double rpm) {
            double minVeLow = 0.85;
            double minVeHigh = 0.7;

            if (rpm < GetMaxVeRpm()) {
                return MathHelper.Lerp(minVeLow, maxVe, (rpm / GetMaxVeRpm()) * (Math.Sin(Math.PI / 2)));
            } else {
                return MathHelper.Lerp(maxVe, minVeHigh, Math.Pow((rpm - GetMaxVeRpm()) / (GetMaxRPM() - GetMaxVeRpm()), 1.3));
            }
        }
    }
}
