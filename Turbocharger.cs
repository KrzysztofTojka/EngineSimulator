using System;

namespace EngineSimulator {
    public class Turbocharger {

        private Engine engine;

        private const double COMPRESSOR_DRAG_FACTOR = 40.0;
        private const double AIRFLOW_ENERGY = 100.0;

        private double maxBoost;
        private double inertia;
        private double wastegate;
        private double currentWastegate;

        private double boost;
        private double speed;

        public Turbocharger(Engine engine, double maxBoost, double inertia = 3.0, double wastegate = 0.9) {
            this.engine = engine;
            this.maxBoost = maxBoost;
            this.inertia = inertia;
            this.wastegate = wastegate;
            this.currentWastegate = wastegate;

            this.boost = 0.0;
            this.speed = 0.0;
        }

        public Turbocharger(Engine engine, Turbocharger other) {
            this.engine = engine;
            this.maxBoost = other.maxBoost;
            this.inertia = other.inertia;
            this.wastegate = other.wastegate;
            this.currentWastegate = wastegate;

            this.boost = 0.0;
            this.speed = 0.0;
        }

        public void Update(double dt) {
            currentWastegate = GetCurrentWastegate();

            boost = CalculateBoost(engine.GetMAF(), engine.GetRPM(), dt);

            //Console.WriteLine($"RPM: {engine.GetRPM(),4:F0} | THR: {engine.GetECU().GetThrottle():F2} | MAF: {engine.GetMAF(),5:F1} g/s | BOOST: {boost,5:F2} kPa | WG: {currentWastegate,5:F2}");
        }

        public double CalculateBoost(double maf, double rpm, double dt) {
            if (dt == 0) {
                return CalculateSteadyStateBoost(maf, rpm);
            }

            //double lowRpmBonus = Math.Max(1.0, 3.0 - (rpm / 1500.0));
            double lowRpmBonus = 1.0;

            double exhaustEnergy = (maf * AIRFLOW_ENERGY) * (rpm / 2000.0) * lowRpmBonus;
            double compressorDrag = Math.Pow(speed, 2) * COMPRESSOR_DRAG_FACTOR;
            double acceleration = (exhaustEnergy - compressorDrag) / inertia;

            speed += acceleration * dt;
            speed = MathHelper.Clamp(speed, 0.0, 1.0);

            boost = Math.Pow(speed, 2) * maxBoost;

            return Math.Min(boost, currentWastegate);
        }

        public double CalculateSteadyStateBoost(double maf, double rpm) {
            //double lowRpmBonus = Math.Max(1.0, 3.0 - (rpm / 1500.0));
            double lowRpmBonus = 1.0;

            double exhaustEnergy = (maf * AIRFLOW_ENERGY) * (rpm / 2000.0) * lowRpmBonus;
            double steadySpeed = Math.Sqrt(exhaustEnergy / COMPRESSOR_DRAG_FACTOR);
            double boost = Math.Pow(steadySpeed, 2) * maxBoost;

            return Math.Min(boost, currentWastegate);
        }

        public double GetCurrentWastegate() {
            double throttleAbsolute = engine.GetECU().GetThrottleMap(engine.GetECU().GetThrottlePedal());

            double maxMafTurbo = engine.GetMaxMAF(engine.GetRPM(), true);
            double requestedAirflow = maxMafTurbo * throttleAbsolute; // TODO use Math.Sin

            double requestedBoost = engine.GetMAP(requestedAirflow, engine.GetRPM()) - engine.GetPressureAtm();

            double currentWastegate = MathHelper.Clamp(requestedBoost, 0.0, wastegate);
            return currentWastegate * (0.9 + 0.01 * ((3000 - Math.Abs(3000 - engine.GetRPM())) / 3000));
        }

        public double GetBoost() { 
            return boost;
        }

        public double GetMaxBoost() {
            return maxBoost;
        }

        public double GetActualMaxBoost() {
            return Math.Min(maxBoost, wastegate);
        }

        public double GetActualMaxBoost(double rpm) {
            double maxBoostRpm = 2500;

            return Math.Min(maxBoost * (0.5 + 0.5 * Math.Min(1.0, rpm / maxBoostRpm)), currentWastegate); // TODO better calculations
        }

        public void SetWastegate(double wastegate) {
            currentWastegate = Math.Min(wastegate, this.wastegate);
        }
    }
}
