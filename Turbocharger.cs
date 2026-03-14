using System;

namespace EngineSimulator {
    public class Turbocharger {

        private Engine engine;

        private const double COMPRESSOR_DRAG_FACTOR = 30.0;
        private const double AIRFLOW_ENERGY = 200.0;

        private double maxBoost;
        private double inertia;
        private double wastegate;

        private double boost;
        private double speed;

        public Turbocharger(Engine engine, double maxBoost, double inertia = 3.0, double wastegate = 0.9) {
            this.engine = engine;
            this.maxBoost = maxBoost;
            this.inertia = inertia;
            this.wastegate = wastegate;
        }

        public Turbocharger(Engine engine, Turbocharger other) {
            this.engine = engine;
            this.maxBoost = other.maxBoost;
            this.inertia = other.inertia;
            this.wastegate = other.wastegate;
        }

        public void Update(double dt) {
            boost = CalculateBoost(engine.GetMAF(), engine.GetRPM(), dt);
        }

        public double CalculateBoost(double maf, double rpm, double dt) {
            if (dt == 0) {
                return CalculateSteadyStateBoost(maf, rpm);
            }

            double lowRpmBonus = Math.Max(1.0, 3.0 - (rpm / 1500.0));

            double exhaustEnergy = (maf * AIRFLOW_ENERGY) * (rpm / 2000.0) * lowRpmBonus;
            double compressorDrag = Math.Pow(speed, 2) * COMPRESSOR_DRAG_FACTOR;
            double acceleration = (exhaustEnergy - compressorDrag) / inertia;

            speed += acceleration * dt;
            speed = MathHelper.Clamp(speed, 0.0, 1.0);

            boost = Math.Pow(speed, 2) * maxBoost;

            return Math.Min(boost, wastegate);
        }

        public double CalculateSteadyStateBoost(double maf, double rpm) {
            double lowRpmBonus = Math.Max(1.0, 3.0 - (rpm / 1500.0));

            double exhaustEnergy = (maf * AIRFLOW_ENERGY) * (rpm / 2000.0) * lowRpmBonus;
            double steadySpeed = Math.Sqrt(exhaustEnergy / COMPRESSOR_DRAG_FACTOR);
            double boost = Math.Pow(steadySpeed, 2) * maxBoost;

            return Math.Min(boost, wastegate);
        }

        public double GetBoost() { 
            return boost;
        }

    }
}
