using System;

namespace EngineSimulator {
    public class Car {

        private Engine engine;
        private Gearbox gearbox;
        private BaseClutch clutch; // TODO

        private double wheelRadius = 0.340;
        private double mass = 1520;
        private double Cd = 0.32;
        private double area = 2.2;
        private double rollingResistance = 0.015;
        private double brakesTorque = 8000.0;

        public void Update(double dt) {
            engine.Update(dt);
            //gearbox.SetInputRPM(engine.GetRPM());
            gearbox.Update(dt);
            engine.UpdateRpm(dt);
        }

        public void SetEngine(Engine engine) {
            this.engine = engine;
        }

        public Engine GetEngine() {
            return engine;
        }

        public void SetGearbox(Gearbox gearbox) {
            this.gearbox = gearbox;
            gearbox.SetEngine(this.engine);
        }

        public Gearbox GetGearbox() {
            return gearbox;
        }
    
    }
}
