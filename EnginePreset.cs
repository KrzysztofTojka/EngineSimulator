using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class EnginePreset {
        public string Name { get; set; }

        public Func<Engine> Create { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}
