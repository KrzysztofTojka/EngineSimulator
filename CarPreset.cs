using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class CarPreset {

        public string Name { get; set; }

        public string EnginePresetName { get; set; }
        public string GearboxPresetName { get; set; }

        public override string ToString() {
            return Name;
        }

    }
}
