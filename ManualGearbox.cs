using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class ManualGearbox : Gearbox {

        private Clutch clutch;
        private TorqueConverter torqueConverter;

        public ManualGearbox(int gears, double[] gearRatios, double finalGearRatio) : base(gears, gearRatios, finalGearRatio) {
            this.type = Type.Manual;
            this.clutch = new Clutch(this, 700);
            //this.torqueConverter = new TorqueConverter(this);
        }

        public override void Update(double dt) {
            clutch.SetPosition(1.0 - Program.GetClutchPedalPosition());
            clutch.Update(dt);
            SetInputTorque(clutch.GetOutputTorque());
            base.Update(dt);
            //torqueConverter.Update(dt);
        }

        public override void SetEngine(Engine engine) {
            base.SetEngine(engine);
            clutch.SetEngine(engine);
        }

    }
}
