using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class Gearbox {

        protected Engine engine;

        protected double wheelRadius = 0.323;
        private double mass = 1400;
        private double Cd = 0.32;
        private double area = 2.2;
        private double rollingResistance = 0.015; // 0.015
        private double airDensity = 1.225;
        private double brakesTorque = 12000.0;

        private Type type;
        protected int gears;
        protected double finalDriveRatio;
        protected Dictionary<int, double> gearRatios = new Dictionary<int, double>();

        private double brakesEngangement;
        protected int currentGear;
        protected double inputTorque; // Nm
        protected double inputRpm; // rpm
        private double inputOmega; // rad/s
        private double wheelTorque; // Nm
        protected double wheelRpm; // rpm
        protected double carSpeed; // m/s

        public Gearbox(Engine engine, Type type, int gears, double[] gearRatios, double finalGearRatio) {
            this.engine = engine;

            this.type = type;
            this.gears = gears;
            this.finalDriveRatio = finalGearRatio;

            this.currentGear = 1;
            this.carSpeed = 0.0;
            this.brakesEngangement = 0.0;

            for (int i = 0; i < gearRatios.Length; i++) {
                this.gearRatios.Add(i + 1, gearRatios[i]);
            }
        }

        public virtual void Update(double dt) {
            if (currentGear == 0) {
                inputRpm = engine.GetRPM();
                return;
            }

            double gearRatio = gearRatios[currentGear] * finalDriveRatio;
            double driveForce = (inputTorque * gearRatio) / wheelRadius;
            double netForce = driveForce - GetTotalResistance() - brakesTorque * brakesEngangement;

            double accel = netForce / mass;
            carSpeed += accel * dt;

            wheelRpm = (carSpeed / (2 * Math.PI * wheelRadius)) * 60;
            inputRpm = wheelRpm * gearRatio;
        }

        public double GetTotalResistance() {
            double rollingForce = Math.Sign(carSpeed) * rollingResistance * mass * 9.81; // 9.81
            double dragForce = 0.5 * airDensity * Cd * area * carSpeed * carSpeed * Math.Sign(carSpeed);

            return rollingForce + dragForce;
        }

        public void SetGear(int gear) {
            this.currentGear = gear;
        }

        public void GearUp() {
            if (currentGear == gears) {
                return;
            }
            currentGear++;
        }

        public void GearDown() {
            if (currentGear == 0) {
                return;
            }
            currentGear--;
        }

        public double WheelRpmToCarSpeed(double wheelRpm, double wheelRadius) {
            return 2 * Math.PI * wheelRadius * wheelRpm / 60.0;
        }

        public double GetCarInertia() {
            if (currentGear == 0) return 0;
            double totalRatio = gearRatios[currentGear] * finalDriveRatio;
            return mass * Math.Pow(wheelRadius / totalRatio, 2);
        }

        public double GetCarSpeed() {
            return carSpeed;
        }

        public int GetCurrentGear() {
            return currentGear;
        }

        public double GetTotalRatio() {
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

        public enum Type {
            Manual,
            Automatic
        }

        public static double[] GearSet(params double[] values) {
            return values;
        }
    }
}
