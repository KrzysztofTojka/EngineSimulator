using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineSimulator {
    public class GasolineEngine : Engine {

        public override double AFR_STOICH => 14.7;
        public override double FUEL_DENSITY => 748.9; // kg/m3
        public override double LHV => 43_000_000; // J/kg
        public override double BASE_THERMAL_EFFICIENCY => 0.455; // 0.45

        public GasolineEngine(double displacementL = 2.0, double maxRpm = 6000, double inertia = 0.12) : base(displacementL, maxRpm, inertia) {
            ecu.SetIdleThrottle(0.0165); // 2.0 - 0.0165
            this.maxVe = 0.95;
            this.optimalIntakeRpm = 4700;
            this.veRangeScale = 2.0;
            this.maxAirflowRpm = 5800;
        }

        public GasolineEngine(GasolineEngine other) : base(other) {
        }

        public override Engine Clone() {
            return new GasolineEngine(this);
        }

        public double GetBrakingTorque_Old(double rpm, double throttle) {
            double referenceTorque = 30.0;
            double referenceRpm = 1000;
            double maxRpm = 6000;
            double dropRate = 0.75;

            double deltaPressure = Math.Max(0, pressureAtm - map);
            double pumpingTorque = 1.2 * (deltaPressure * displacement) / (4.0 * Math.PI);

            double frictionTorque = referenceTorque * (1 + ((rpm - referenceRpm) / (maxRpm / dropRate - referenceRpm)));

            return pumpingTorque + frictionTorque;
        }

        public override double GetBrakingTorque(double rpm, double throttle) {
            double staticTorque = 22.0 * ((displacement * Units.L) / 2.0); // 17.0

            double referenceTorque = 4.5; // 5.5
            double referenceRpm = 1000;

            double frictionTorque = referenceTorque * (rpm / referenceRpm);

            double deltaPressure = Math.Max(0, pressureAtm - map);
            double pumpingTorque = Math.Max(1.0, Math.Pow(rpm / 4000, 0.3)) * (deltaPressure * displacement) / (4.0 * Math.PI);

            return staticTorque + frictionTorque + pumpingTorque;
        }

        public override double GetThermalEfficiency() {
            return BASE_THERMAL_EFFICIENCY * (0.92 + 0.08 * (2.0 / (displacement * Units.L)));
        }

        public override double GetFuelPower(double fuelRate, double rpm, double afr, bool random = true) {
            if (rpm < 200) return 0;
            return fuelRate * (Math.Min(afr, AFR_STOICH) / AFR_STOICH) * LHV * MathHelper.Random(0.97, 1.03, random) * (Math.Sin(Math.PI * Math.Min(1.0, rpm / 500) - Math.PI / 2) + 1) / 2; // W
        }

        public override double GetVolumetricEfficiency(double rpm) {
            double lowRpmFactor = 1.0;
            if (rpm < 3000) {
                lowRpmFactor = Math.Pow(rpm / 3000, 0.2) * (0.8 + 0.2 * MathHelper.Clamp((rpm) / 1300, 0.0, 1.0));
            }

            double baseRpmFactor = Math.Exp(-Math.Pow((rpm - optimalIntakeRpm) / (optimalIntakeRpm * veRangeScale), 2));
            double rpmFactor = lowRpmFactor * baseRpmFactor;

            double mapFactor = 0.5 + 0.5 * Math.Pow(map / pressureAtm, 0.7);

            return MathHelper.Clamp(maxVe * rpmFactor * mapFactor, 0.2, 1.0);
        }
    }
}