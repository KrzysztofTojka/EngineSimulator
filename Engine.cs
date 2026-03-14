using System;
using System.Collections.Generic;

namespace EngineSimulator {
    public class Engine {

        public ECU ecu;

        public const double AFR = 14.7;
        public const double FUEL_DENSITY = 748.9; // kg/m3
        public const double LHV = 43_000_000; // J/kg

        public const double MAX_POWER_RPM = 5500;

        private readonly double temperature = Units.C_to_K(Program.temperatureC);
        private readonly double pressureAtm = Program.pressureHPA * 100.0;

        private double displacement; // m3
        private double inertia; // kg*m2
        private double maxRpm;

        private double throttle = 0.0;
        private double rpm = 0.0;

        double afr;
        double maf;
        double map;
        double ve;
        double fuelRate;
        double fuelPower;
        double fuelTorque;
        double load;
        double brakePower;
        double brakeTorque;
        double loadTorque;
        double netTorque;

        public Engine(double displacementL, double maxRpm = 6000, double inertia = 0.12) {
            this.displacement = displacementL / 1000;
            this.maxRpm = maxRpm;
            this.inertia = inertia;

            this.ecu = new ECU(this);
        }

        public void Update(double dt) {
            this.Update();
            rpm = GetNewRPM(rpm, dt, netTorque);
        }

        public void Update() {
            throttle = ecu.GetThrottle();

            afr = ecu.GetAFR(rpm, throttle);
            maf = GetMAF(throttle, rpm);
            map = GetMAP(maf, rpm, throttle);
            ve = GetVolumetricEfficiency(rpm);
            fuelRate = GetFuelRate(maf, afr);
            fuelPower = GetFuelPower(fuelRate, afr);
            fuelTorque = PowerToTorque(fuelPower, rpm);
            load = GetEngineLoad(fuelPower, rpm, afr);
            brakePower = 0.5 * fuelPower - GetBrakingPower(rpm);
            brakeTorque = PowerToTorque(brakePower, rpm);

            netTorque = brakeTorque - loadTorque;
        }

        public void ShowInfo() {
            Console.WriteLine(
                $"{rpm:F0} RPM - " +
                $"THR: {throttle:F2}, " +
                $"MAF: {maf * 1000:F2} g/s, " +
                $"MAP: {map / 1000:F2} kPa, " +
                $"L: {load:F2}, " +
                $"AFR: {afr:F2}, " +
                $"VE: {ve:F2}, " +
                $"FR: {fuelRate * 1000 * 3600 / Engine.FUEL_DENSITY:F2} L/h, " +
                $"P: {brakePower * Units.HP:F2} HP, " +
                $"TQ: {brakeTorque:F2} Nm, " +
                $"E: {brakePower / fuelPower:F2}"
            );
        }

        public void Ignite() {
            SetRPM(500);
        }

        public double GetVolumetricEfficiency(double rpm) {
            double veMax = 1.0;
            double rpmOpt = 4000;
            double rpmScale = 2.0;

            return veMax * Math.Exp(-Math.Pow((rpm - rpmOpt) * (rpm > rpmOpt ? 1.2 : 1.0) / (rpmOpt * rpmScale), 2));
        }

        private double GetAirDensity(double temperature, double pressure) {
            return pressure / (temperature * Units.GAS_CONSTANT); // kg/m3
        }

        private double GetMaxMAF(double rpm) {
            return GetAirDensity(temperature, pressureAtm) * (displacement / 2) * GetVolumetricEfficiency(rpm) * (rpm / 60); // kg/s
        }

        public double GetMAF(double throttle, double rpm) {
            double calculatedMaf = GetMaxMAF(MAX_POWER_RPM) * Math.Sin(throttle * (Math.PI / 2)) * MathHelper.Random(0.97, 1.03);
            return Math.Min(calculatedMaf, GetMaxMAF(rpm)); // kg/s
        }

        public double GetMAP(double maf, double rpm, double throttle) {
            return (maf * 2 * 60 * Units.GAS_CONSTANT * temperature) / (displacement * GetVolumetricEfficiency(rpm) * rpm); // Pa
        }

        public double GetFuelRate(double maf, double afr) {
            if (ecu.ShouldCutFuel()) return 0.0;

            return maf / afr; // kg/s
        }

        public double GetEngineLoad(double fuelPower, double rpm, double afr) {
            double maxFuelPower = GetFuelPower(GetFuelRate(GetMAF(1.0, rpm), ecu.GetAFR(rpm, 1.0)), afr);

            if (maxFuelPower == 0.0) return 0.0;

            return fuelPower / maxFuelPower;
        }


        public double GetFuelPower(double fuelRate, double afr) {
            return fuelRate * (Math.Min(afr, 14.7) / 14.7) * LHV * MathHelper.Random(0.95, 1.05); // W
        }

        public double PowerToTorque(double power, double rpm) {
            return (power * 60) / (2 * Math.PI * rpm); // Nm
        }

        public double TorqueToPower(double torque, double rpm) {
            return (torque * 2 * Math.PI * rpm) / 60; // W
        }

        public double GetBrakingPower(double rpm) {
            double referenceTorque = 40.0;
            double referenceRpm = 1000;
            double maxRpm = 6000;
            double dropRate = 0.75;

            return TorqueToPower(referenceTorque * (1 + ((rpm - referenceRpm) / (maxRpm / dropRate - referenceRpm))), rpm);
        }

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
    }
}
