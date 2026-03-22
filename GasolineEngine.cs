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
            double staticTorque = 17.0 * ((displacement * Units.L) / 2.0);

            double referenceTorque = 5.5;
            double referenceRpm = 1000;

            double frictionTorque = referenceTorque * (rpm / referenceRpm);

            double deltaPressure = Math.Max(0, pressureAtm - map);
            double pumpingTorque = Math.Max(1.0, Math.Pow(rpm / 4000, 0.3)) * (deltaPressure * displacement) / (4.0 * Math.PI);

            return staticTorque + frictionTorque + pumpingTorque;
        }

        public override double GetThermalEfficiency() {
            return BASE_THERMAL_EFFICIENCY * (0.92 + 0.08 * (2.0 / (displacement * Units.L)));
        }

    }
}