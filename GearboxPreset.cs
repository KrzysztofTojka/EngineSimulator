using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class GearboxPreset {
        public string Name { get; set; }

        public Func<Gearbox> Create { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}
