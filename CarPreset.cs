using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class CarPreset {

        public string Name { get; set; }

        public List<string> Engines { get; set; } = new List<string>();
        public string Gearbox { get; set; }

        public double Weight { get; set; }
        public double WheelRadius { get; set; }

        public override string ToString() {
            return Name;
        }

    }
}
