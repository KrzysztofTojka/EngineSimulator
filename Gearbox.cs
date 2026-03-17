using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class Gearbox {

        protected Engine engine;

        protected double wheelRadius = 0.323;
        protected double mass = 1400;
        protected double Cd = 0.32;
        protected double area = 2.2;
        protected double rollingResistance = 0.015; // 0.015
        protected double airDensity = 1.225;
        protected double brakesTorque = 12000.0;

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

        public Gearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) {
            this.engine = engine;

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
            double rollingForce = rollingResistance * mass * 9.81;

            if (Math.Abs(carSpeed) < 0.1) {
                rollingForce *= (carSpeed / 0.1);
            } else {
                rollingForce *= Math.Sign(carSpeed);
            }

            double dragForce = 0.5 * airDensity * Cd * area * carSpeed * carSpeed * Math.Sign(carSpeed);

            double drivetrainFriction = 80.0; // Nm
            double drivetrainLosses = 0;

            // TODO
            //if (currentGear > 0 && Program.GetClutch().GetEngangement() > 0.0) {
            //    drivetrainLosses = (drivetrainFriction * GetTotalRatio()) / wheelRadius;
            //}


            return rollingForce + dragForce + drivetrainLosses;
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
