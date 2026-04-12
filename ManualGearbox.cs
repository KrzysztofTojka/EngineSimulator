using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class ManualGearbox : Gearbox {

        private Clutch clutch;
        private TorqueConverter torqueConverter;

        public ManualGearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) : base(engine, gears, gearRatios, finalGearRatio) {
            this.type = Type.Manual;
            this.clutch = new Clutch(engine, this, 700);
            this.torqueConverter = new TorqueConverter(engine, this);
        }

        public override void Update(double dt) {
            clutch.SetPosition(1.0 - Program.GetClutchPedalPosition());
            clutch.Update(dt);
            SetInputTorque(clutch.GetOutputTorque());
            base.Update(dt);
            //torqueConverter.Update(dt);
        }

    }
}
