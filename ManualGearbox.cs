using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class ManualGearbox : Gearbox {

        private Clutch clutch;

        public ManualGearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) : base(engine, gears, gearRatios, finalGearRatio) {
            this.type = Type.Manual;
            this.clutch = new Clutch(engine, this, 700);
        }

        public override void Update(double dt) {
            base.Update(dt);
            clutch.SetEngagement(Program.GetClutchPedalPosition());
            clutch.Update(dt);
        }

    }
}
