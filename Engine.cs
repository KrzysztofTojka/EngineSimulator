using System;
using System.Text.Json;
using System.Collections.Generic;

namespace EngineSimulator {
    public abstract class Engine {

        public ECU ecu;
        private Turbocharger turbocharger;

        public virtual double AFR_STOICH => 0;
        public virtual double FUEL_DENSITY => 0; // kg/m3
        public virtual double LHV => 0; // J/kg
        public virtual double BASE_THERMAL_EFFICIENCY => 0;

        private readonly double temperature = Units.C_to_K(Program.temperatureC);
        protected readonly double pressureAtm = Program.pressureHPA * 100.0;

        protected double displacement; // m3
        protected double inertia; // kg*m2
        protected double maxRpm;
        protected double maxVe;
        protected double optimalIntakeRpm;
        protected double veRangeScale;
        protected double maxAirflowRpm;

        protected double throttle = 0.0;
        protected double rpm = 0.0;

        protected double afr;
        protected double maf;
        protected double map;
        protected double ve;
        protected double fuelRate;
        protected double fuelPower;
        protected double fuelTorque;
        protected double load;
        protected double brakePower;
        protected double brakeTorque;
        protected double loadTorque;
        protected double netTorque;

        public Engine(double displacementL, double maxRpm, double inertia) {
            this.ecu = new ECU(this);
            //this.turbocharger = new Turbocharger(this, 1.0);

            this.displacement = displacementL / 1000;
            this.maxRpm = maxRpm;
            this.inertia = inertia;
        }

        public Engine(Engine other) {
            this.ecu = new ECU(this);
            if (other.GetTurbocharger() != null) {
                this.turbocharger = new Turbocharger(this, other.turbocharger);
            }
            
            this.displacement = other.displacement;
            this.inertia = other.inertia;
            this.maxRpm = other.maxRpm;
            this.maxVe = other.maxVe;
            this.optimalIntakeRpm = other.optimalIntakeRpm;
            this.veRangeScale = other.veRangeScale;
            this.maxAirflowRpm = other.maxAirflowRpm;
            this.rpm = other.rpm;
            this.throttle = other.throttle;
            this.loadTorque = other.loadTorque;
        }

        public abstract Engine Clone();

        public void Update(double dt) {
            throttle = ecu.GetThrottle();

            maf = GetMAF(throttle, rpm);
            map = GetMAP(maf, rpm, throttle);
            afr = ecu.GetAFR(rpm, map);
            ve = GetVolumetricEfficiency(rpm);
            fuelRate = GetFuelRate(maf, afr);
            fuelPower = GetFuelPower(fuelRate, rpm, afr);
            fuelTorque = PowerToTorque(fuelPower, rpm);
            load = GetEngineLoad(fuelPower, rpm, afr);
            brakePower = GetThermalEfficiency() * fuelPower - TorqueToPower(GetBrakingTorque(rpm, throttle), rpm);
            brakeTorque = PowerToTorque(brakePower, rpm);

            if (turbocharger != null) {
                turbocharger.Update(dt);
            }
        }

        public void UpdateRpm(double dt) {
            netTorque = brakeTorque - loadTorque;
            double newRpm = GetNewRPM(rpm, dt, netTorque);

            if (double.IsNaN(newRpm) || double.IsInfinity(newRpm)) {
                Console.WriteLine("RPM error: " + newRpm);
                return;
                //newRpm = 0;
            }
            rpm = Math.Max(0, newRpm);
        }

        public void ShowInfo() {
            Console.WriteLine(
                $"{(DateTimeOffset.Now.ToUnixTimeMilliseconds() - Program.startTime)/1000:F3} s | " +
                $"{rpm:F0} RPM, " +
                $"{Program.GetGearbox().GetCarSpeed() * (Units.km / Units.h):F2} kmh - " +
                $"THR: {throttle:F2}, " +
                $"MAF: {maf * 1000:F2} g/s, " +
                $"MAP: {map / 1000:F2} kPa, " +
                $"L: {load:F2}, " +
                $"AFR: {afr:F2}, " +
                $"VE: {ve:F2}, " +
                $"FR: {fuelRate * 1000 * 3600 / FUEL_DENSITY:F2} L/h, " +
                $"P: {brakePower * Units.HP:F2} HP, " +
                $"TQ: {brakeTorque:F2} Nm, " +
                $"E: {brakePower / GetFuelPower(fuelRate, rpm, 14.7, false):F2}"
            );
        }

        public string GetCsvHeader() {
            return "Time (s);RPM;Speed (kmh);Throttle;MAF (g/s);MAP (kPa);Load;AFR;VE;Fuel Rate (g/s);Power (HP);Torque (Nm);Efficiency";
        }

        public string GetCsvLine() {
            return string.Join(";", new string[] {
                $"{(DateTimeOffset.Now.ToUnixTimeMilliseconds() - Program.startTime)/1000.0:F3}",
                $"{rpm:F0}",
                $"{Program.GetGearbox().GetCarSpeed() * (Units.km / Units.h):F2}",
                $"{throttle:F3}",
                $"{maf * 1000:F3}",
                $"{map / 1000:F3}",
                $"{load:F3}",
                $"{afr:F2}",
                $"{ve:F2}",
                $"{fuelRate * 1000/* * 3600 / FUEL_DENSITY*/:F3}",
                $"{brakePower * Units.HP:F3}",
                $"{brakeTorque:F2}",
                $"{brakePower / GetFuelPower(fuelRate, rpm, 14.7, false):F3}"
            });
        }

        public void Ignite() {
            SetRPM(500);
        }

        public double GetTorque(double rpm) {
            return GetTorque(GetThrottle(), rpm);
        }

        public double GetTorque(double throttle, double rpm, bool random = false) {
            double maf = GetMAF(throttle, rpm, random);
            double afr = ecu.GetAFR(rpm, throttle, random);
            double fuelRate = GetFuelRate(maf, afr);
            double fuelPower = GetFuelPower(fuelRate, rpm, afr, random);
            double brakePower = GetThermalEfficiency() * fuelPower - TorqueToPower(GetBrakingTorque(rpm, throttle), rpm);
            return PowerToTorque(brakePower, rpm);
        }

        public double GetVolumetricEfficiency_Old(double rpm) {
            return maxVe * Math.Exp(-Math.Pow((rpm - optimalIntakeRpm) * (rpm > optimalIntakeRpm ? 1.2 : 1.0) / (optimalIntakeRpm * veRangeScale), 2));
        }

        public double GetVolumetricEfficiency(double rpm) {
            double lowRpmFactor = 1.0;
            if (rpm < 3000) {
                lowRpmFactor = Math.Pow(rpm / 3000, 0.2) * (0.8 + 0.2 * MathHelper.Clamp((rpm) / 1300, 0.0, 1.0));
            }

            double baseRpmFactor = Math.Exp(-Math.Pow((rpm - optimalIntakeRpm) / (optimalIntakeRpm * veRangeScale), 2));
            double rpmFactor = lowRpmFactor * baseRpmFactor;

            double mapFactor = 0.5 + 0.5 * Math.Pow(map / pressureAtm, 0.7);

            return MathHelper.Clamp(maxVe * rpmFactor * mapFactor, 0.2, 1.0);
        }

        private double GetAirDensity(double temperature, double pressure) {
            return pressure / (temperature * Units.GAS_CONSTANT); // kg/m3
        }

        private double GetMaxMAF(double rpm) {
            double pressure = pressureAtm;
            if (turbocharger != null) {
                pressure += turbocharger.GetBoost() * pressureAtm;
            }
            return GetAirDensity(temperature, pressure) * (displacement / 2) * GetVolumetricEfficiency(rpm) * (rpm / 60); // kg/s
        }

        public double GetMAF(double throttle, double rpm, bool random = true) {
            double calculatedMaf = GetMaxMAF(maxAirflowRpm) * Math.Sin(throttle * (Math.PI / 2)) * MathHelper.Random(0.98, 1.02, random);
            return Math.Min(calculatedMaf, GetMaxMAF(rpm)); // kg/s
        }

        public double GetMAP(double maf, double rpm, double throttle, bool random = true) {
            if (rpm < 50) return pressureAtm;
            
            double calculatedMap = (maf * 2 * 60 * Units.GAS_CONSTANT * temperature) / (displacement * GetVolumetricEfficiency(rpm) * rpm);
            if (calculatedMap < 15_000) {
                return 15_000 * MathHelper.Random(0.98, 1.02, random);
            }

            return calculatedMap;
        }

        public double GetFuelRate(double maf, double afr) {
            if (ecu.ShouldCutFuel()) return 0.0;

            return maf / afr; // kg/s
        }

        public double GetEngineLoad(double fuelPower, double rpm, double afr) {
            double maxFuelPower = GetFuelPower(GetFuelRate(GetMAF(1.0, rpm), ecu.GetAFR(rpm, 1.0)), rpm, afr);

            if (maxFuelPower == 0.0) return 0.0;

            return fuelPower / maxFuelPower;
        }


        public double GetFuelPower(double fuelRate, double rpm, double afr, bool random = true) {
            if (rpm < 200) return 0;
            return fuelRate * (Math.Min(afr, 14.7) / 14.7) * LHV * MathHelper.Random(0.97, 1.03, random) * (Math.Sin(Math.PI * Math.Min(1.0, rpm / 500) - Math.PI / 2) + 1) / 2; // W
        }

        public double PowerToTorque(double power, double rpm) {
            return (power * 60) / (2 * Math.PI * rpm); // Nm
        }

        public double TorqueToPower(double torque, double rpm) {
            return (torque * 2 * Math.PI * rpm) / 60; // W
        }

        public abstract double GetBrakingTorque(double rpm, double throttle);

        public double GetAcceleration(double torque) {
            return torque / inertia; // rad/s2
        }

        public double GetNewRPM(double rpm, double dt, double torque) {
            double accel = GetAcceleration(torque);

            double omega = rpm * 2 * Math.PI / 60; // rad/s
            omega += accel * dt; // rad/s

            return omega * 60 / (2 * Math.PI); // rpm
        }

        public ECU GetECU() {
            return this.ecu;
        }

        public Turbocharger GetTurbocharger() {
            return this.turbocharger;
        }

        public void SetTurbocharger(Turbocharger turbo) {
            this.turbocharger = turbo;
        }

        public double GetDisplacement() {
            return this.displacement;
        }

        public void SetThrottle(double throttle) {
            this.throttle = throttle;
        }

        public double GetThrottle() {
            return this.throttle;
        }

        public void SetRPM(double rpm) {
            this.rpm = rpm;
        }

        public double GetRPM() {
            return this.rpm;
        }

        public double GetMaxRPM() {
            return this.maxRpm;
        }

        public double GetLoad() {
            return this.load;
        }

        public double GetMAP() {
            return this.map;
        }

        public double GetMAF() {
            return this.maf;
        }

        public double GetFuelRate() {
            return this.fuelRate;
        }

        public double GetAFR() {
            return this.afr;
        }

        public double GetTorque() {
            return fuelTorque;
        }

        public double GetPower() {
            return fuelPower;
        }

        public double GetBrakeTorque() {
            return brakeTorque;
        }

        public void SetBrakeTorque(double torque) {
            this.brakeTorque = torque;
        }

        public double GetBrakePower() {
            return brakePower;
        }

        public void SetDisplacement(double displacementL) {
            this.displacement = displacementL / 1000;
        }

        public void SetInertia(double inertia) {
            this.inertia = inertia;
        }

        public double GetInertia() {
            return this.inertia;
        }

        public void SetMaxRPM(double maxRpm) {
            this.maxRpm = maxRpm;
        }

        public void SetLoadTorque(double loadTorque) {
            this.loadTorque = loadTorque;
        }

        public double GetMaxVe() {
            return this.maxVe;
        }

        public void SetMaxVe(double maxVe) {
            this.maxVe = maxVe;
        }

        public double GetMaxVeRpm() {
            return this.optimalIntakeRpm;
        }

        public void SetMaxVeRpm(double rpm) {
            this.optimalIntakeRpm = rpm;
        }

        public double GetVeRangeScale() {
            return this.veRangeScale;
        }

        public void SetVeRangeScale(double scale) {
            this.veRangeScale = scale;
        }

        public double GetMaxAirflowRpm() {
            return this.maxAirflowRpm;
        }

        public void SetMaxAirflowRpm(double rpm) {
            this.maxAirflowRpm = rpm;
        }

        public double GetPressureAtm() {
            return this.pressureAtm;
        }

        public abstract double GetThermalEfficiency();

        public string Serialize() {
            return JsonSerializer.Serialize(this);
        }

        public static Engine Deserialize(string json) {
            return JsonSerializer.Deserialize<Engine>(json);
        }

        
    }
}
