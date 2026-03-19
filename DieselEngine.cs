using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class DieselEngine : Engine {

        public override double AFR_STOICH => 14.4;
        public override double FUEL_DENSITY => 846.0; // kg/m3
        public override double LHV => 42_600_000; // J/kg
        public override double BASE_THERMAL_EFFICIENCY => 0.5;

        public DieselEngine(double displacementL, double maxRpm = 4500, double inertia = 0.2) : base(displacementL, maxRpm, inertia) {
            this.SetTurbocharger(new Turbocharger(this, 2.0));
            this.GetECU().SetIdleThrottle(0.034);
            this.maxVe = 0.97;
            this.optimalIntakeRpm = 2300;
            this.veRangeScale = 2.0;
            this.maxAirflowRpm = 4000;
        }

        public DieselEngine(DieselEngine other) : base(other) {
        }

        public override Engine Clone() {
            return new DieselEngine(this);
        }

        public override double GetBrakingPower(double rpm) {
            double referenceTorque = 50.0;
            double referenceRpm = 1000;
            double dropRate = 0.85;

            return TorqueToPower(referenceTorque * (1 + ((rpm - referenceRpm) / (maxRpm / dropRate - referenceRpm))), rpm);
        }
    }
}
