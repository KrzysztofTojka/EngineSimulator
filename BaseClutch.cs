using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public abstract class BaseClutch {

        protected Engine engine;
        protected Gearbox gearbox;

        public BaseClutch(Gearbox gearbox) {
            this.gearbox = gearbox;
        }

        public void SetEngine(Engine engine) {
            this.engine = engine;
        }

    }
}
