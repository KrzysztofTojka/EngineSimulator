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
        public override double BASE_THERMAL_EFFICIENCY => 0.5;

        public GasolineEngine(double displacementL, double maxRpm = 6000, double inertia = 0.12) : base(displacementL, maxRpm, inertia) {
            this.maxVe = 0.97;
            this.optimalIntakeRpm = 4000;
            this.veRangeScale = 2.0;
            this.maxAirflowRpm = 5500;
        }

        public GasolineEngine(GasolineEngine other) : base(other) {
        }   

        public override Engine Clone() {
            return new GasolineEngine(this);
        }

        public override double GetBrakingPower(double rpm) {
            double referenceTorque = 40.0;
            double referenceRpm = 1000;
            double maxRpm = 6000;
            double dropRate = 0.75;

            return TorqueToPower(referenceTorque * (1 + ((rpm - referenceRpm) / (maxRpm / dropRate - referenceRpm))), rpm);
        }

    }
}
