using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public abstract class Gearbox {

        protected Engine engine;

        protected double wheelRadius = 0.340; // 0.323, 0.365
        protected double mass = 1520; // 1400
        protected double Cd = 0.32;
        protected double area = 2.2;
        protected double rollingResistance = 0.015; // 0.015
        protected double airDensity = 1.225;
        protected double brakesTorque = 8000.0;

        protected Type type;
        protected int gears;
        protected double finalDriveRatio;
        protected Dictionary<int, double> gearRatios = new Dictionary<int, double>();

        protected double brakesEngangement;
        protected int currentGear;
        protected double inputTorque; // Nm
        protected double inputRpm; // rpm
        protected double wheelRpm; // rpm
        protected double carSpeed; // m/s

        public Gearbox(int gears, double[] gearRatios, double finalGearRatio) {
            this.type = Type.Manual;
            this.gears = gears;
            this.finalDriveRatio = finalGearRatio;

            this.currentGear = 0;
            this.carSpeed = 0.0;
            this.brakesEngangement = 0.0;

            for (int i = 0; i < gearRatios.Length; i++) {
                this.gearRatios.Add(i + 1, gearRatios[i]);
            }
        }

        public virtual void Update(double dt) {
            UpdatePhysics(dt);
        }

        public void UpdatePhysics(double dt) {
            if (currentGear == 0) {
                inputRpm = engine.GetRPM();
                return;
            }

            brakesEngangement = Math.Pow(Program.GetBrakePedalPosition(), 2.0);

            double gearRatio = GetTotalRatio();
            double driveForce = (inputTorque * gearRatio) / wheelRadius;
            double brakesForce = brakesTorque * brakesEngangement / wheelRadius;

            double stoppingForce = GetTotalResistance() + Math.Sign(carSpeed) * brakesForce;

            double netForce = driveForce - stoppingForce;

            double accel = netForce / mass;
            double speedDelta = accel * dt;

            if (Math.Abs(carSpeed) < Math.Abs(speedDelta) && Math.Abs(driveForce) < Math.Abs(stoppingForce)) {
                carSpeed = 0;
            } else {
                carSpeed += speedDelta;
            }

            wheelRpm = (carSpeed / (2 * Math.PI * wheelRadius)) * 60;
            inputRpm = wheelRpm * gearRatio;
        }

        public double GetTotalResistance() {
            double rollingForce = rollingResistance * mass * 9.81;

            if (Math.Abs(carSpeed) < 0.2) {
                rollingForce *= (carSpeed / 0.2);
            } else {
                rollingForce *= Math.Sign(carSpeed);
            }

            double dragForce = 0.5 * airDensity * Cd * area * carSpeed * carSpeed * Math.Sign(carSpeed);

            double drivetrainFriction = MathHelper.Lerp(10.0, 50.0, engine.GetRPM() / engine.GetMaxRPM()); // Nm
            double drivetrainLosses = 0;

            if (currentGear > 0) {
                drivetrainLosses = (drivetrainFriction * GetTotalRatio()) / wheelRadius;
            }

            return rollingForce + dragForce + drivetrainLosses;
        }

        public void SetGear(int gear) {
            this.currentGear = gear;
        }

        public virtual void GearUp() {
            if (currentGear == gears) {
                return;
            }
            currentGear++;
        }

        public virtual void GearDown() {
            if (currentGear == 0) {
                return;
            }
            currentGear--;
        }

        public double WheelRpmToCarSpeed(double wheelRpm, double wheelRadius) {
            return 2 * Math.PI * wheelRadius * wheelRpm / 60.0;
        }

        public double CarSpeedToRpm(double carSpeed, double wheelRadius, double totalRatio) {
            return (carSpeed / (2.0 * Math.PI * wheelRadius)) * totalRatio * 60;
        }

        public double RpmToCarSpeed(double engineRpm, double wheelRadius, double totalRatio) {
            return (engineRpm / 60.0) / totalRatio * (2.0 * Math.PI * wheelRadius);
        }

        public double GetCarInertia() {
            if (currentGear == 0) return 0;
            double totalRatio = gearRatios[currentGear] * finalDriveRatio;
            return mass * Math.Pow(wheelRadius / totalRatio, 2);
        }

        public double GetWheelTorque(double engineTorque, int gear) {
            return engineTorque * gearRatios[gear] * finalDriveRatio - GetTotalResistance() * wheelRadius;
        }

        public double GetRpmForGear(int gear) {
            if (gear == 0) return engine.GetRPM();
            return wheelRpm * gearRatios[gear] * finalDriveRatio;
        }

        public double GetCarSpeed() {
            return carSpeed;
        }

        public int GetCurrentGear() {
            return currentGear;
        }

        public virtual string GetGearLabel() {
            if (currentGear == 0) return "N";
            return currentGear.ToString();
        }

        public virtual double GetTotalRatio() {
            return gearRatios[currentGear] * finalDriveRatio;
        }

        public void SetInputTorque(double torque) {
            this.inputTorque = torque;
        }

        public double GetInputRPM() {
            return inputRpm;
        }

        public void SetInputRPM(double rpm) {
            this.inputRpm = rpm;
        }

        public void SetBrakesEngangement(double engangement) {
            this.brakesEngangement = engangement;
        }

        public double GetWheelRadius() {
            return wheelRadius;
        }

        public double GetMaxSpeed(int gear) {
            return WheelRpmToCarSpeed(engine.GetMaxRPM() / (gearRatios[gear] * finalDriveRatio), wheelRadius);
        }

        public void SetWeight(double weight) {
            this.mass = weight;
        }

        public void SetWheelRadius(double wheelRadius) {
            this.wheelRadius = wheelRadius;
        }

        public enum Type {
            Manual,
            Automatic
        }

        public static double[] GearSet(params double[] values) {
            return values;
        }

        public virtual void SetEngine(Engine engine) {
            this.engine = engine;
        }

        public Engine GetEngine() {
            return engine;
        }

        public class ShiftData {

            public List<double> speedValues = new List<double>();
            public Dictionary<int, List<double>> throttleValues = new Dictionary<int, List<double>>();

            public ShiftData(int gears) {
                for (int i = 1; i < gears; i++) {
                    throttleValues[i] = new List<double>();
                }
            }

            public ShiftData(List<double> speedValues, Dictionary<int, List<double>> throttleValues) {
                this.speedValues = speedValues;
                this.throttleValues = throttleValues;
            }

        }

        public ShiftData GetShiftData() {
            List<double> throttleValues = MathHelper.Linspace(0.01, 1.0, 100);
            List<double> speedValues = MathHelper.Linspace(0.1, GetMaxSpeed(gears), 200);

            ShiftData shiftData = new ShiftData(gears);

            for (int gear = 1; gear < gears; gear++) {
                //Console.WriteLine($"Gear {gear} -> {gear + 1}");
                double currentRatio = gearRatios[gear] * finalDriveRatio;
                double nextRatio = gearRatios[gear + 1] * finalDriveRatio;

                foreach (double speed in speedValues) {
                    double rpm = CarSpeedToRpm(speed, wheelRadius, currentRatio);

                    if (rpm < 500) {
                        shiftData.throttleValues[gear].Add(0.0);
                        continue;
                    }

                    if (rpm > engine.GetMaxRPM()) {
                        break;
                    }

                    //Console.WriteLine($"Speed: {speed * (Units.km / Units.h):F2} km/h");

                    double nextGearRpm = CarSpeedToRpm(speed, wheelRadius, nextRatio);

                    foreach (double throttle in throttleValues) {
                        double throttleMapped = engine.GetECU().GetThrottleMap(throttle);
                        double currentTorque = GetWheelTorque(engine.GetTorque(throttleMapped, rpm), gear);
                        double nextGearTorque = GetWheelTorque(engine.GetTorque(throttleMapped, nextGearRpm), gear + 1);

                        //if (currentTorque < 0 && nextGearTorque < 0) {
                        //    continue;
                        //}   

                        if (nextGearTorque < currentTorque) {
                            //Console.WriteLine($"  Shift Point: {speed * (Units.km / Units.h):F2} km/h | Throttle: {throttle:F2} | RPM: {rpm:F0} | TQ_CURR: {currentTorque,6:F1} Nm | TQ_NEXT: {nextGearTorque,6:F1} Nm");
                            shiftData.throttleValues[gear].Add(throttle);
                            break;
                        }
                    }
                }

                shiftData.throttleValues[gear].Add(1.0);
            }

            shiftData.speedValues = speedValues.Select(x => x * (Units.km / Units.h)).ToList();

            return shiftData;
        }
    }
}
