using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class AutomaticGearbox : Gearbox {

        public const double SHIFT_TIME = 50;
        public const double GLOBAL_SHIFT_COOLDOWN = 1000;
        public const double UPSHIFT_COOLDOWN = 1000;
        public const double DOWNSHIFT_COOLDOWN = 1000;
        public const double MIN_UPSHIFT_RPM = 1700;

        private DriveMode driveMode;
        private ShiftPhase shiftPhase;
        private double shiftTimer;
        private int targetGear;
        private long lastShiftTime;
        private long lastUpshiftTime;
        private long lastDownshiftTime;

        public AutomaticGearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) : base(engine, gears, gearRatios, finalGearRatio) {
            this.type = Type.Automatic;
            this.driveMode = DriveMode.NORMAL;
            this.shiftPhase = ShiftPhase.IDLE;
            this.shiftTimer = -1;
            this.targetGear = 0;
            this.lastShiftTime = 0;
            this.lastUpshiftTime = 0;
            this.lastDownshiftTime = 0;
        }

        public override void Update(double dt) {
            base.Update(dt);

            UpdateShiftTimer(dt);

            if (currentGear == 0) {
                return;
            }

            if (shiftPhase != ShiftPhase.IDLE) {
                return;
            }

            int minDownshiftGear = GetMinDownshiftGear();

            if (minDownshiftGear < currentGear && TimeSince(lastShiftTime) > DOWNSHIFT_COOLDOWN) {
                StartShift(minDownshiftGear);
            }

            if (ShouldUpshift()) {
                StartShift(currentGear + 1);
            }

            Program.GetClutch().Update(dt); // for now
        }

        public void UpdateShiftTimer(double dt) {
            if (shiftTimer == -1) {
                return;
            }

            if (shiftPhase == ShiftPhase.PRE_SHIFTING && shiftTimer >= SHIFT_TIME / 2.0) {
                SetGear(targetGear);
                shiftPhase = ShiftPhase.POST_SHIFTING;
                shiftTimer = 0.0;
            }
            
            if (shiftPhase == ShiftPhase.POST_SHIFTING && shiftTimer >= SHIFT_TIME / 2.0) {
                shiftPhase = ShiftPhase.IDLE;
                shiftTimer = 0.0;
            }

            shiftTimer += dt * 1000.0;
        }

        public int GetMinDownshiftGear() {
            if (currentGear <= 1) {
                return currentGear;
            }

            double currentTorque = GetWheelTorque(engine.GetTorque(GetRpmForGear(currentGear)), currentGear);

            int minGear = currentGear;
            for (int i = currentGear; i > 2; i--) {
                double prevGearRpm = GetRpmForGear(i - 1);

                if (prevGearRpm >= engine.GetMaxRPM() * 0.9) {
                    return minGear;
                }
                double prevGearTorque = GetWheelTorque(engine.GetTorque(prevGearRpm), i - 1);

                if (prevGearTorque > currentTorque) {
                    minGear = i - 1;
                }
            }

            return minGear;
        }

        public bool ShouldUpshift() {
            if (engine.GetECU().GetThrottlePedal() == 0.0) {
                return false;
            }

            if (currentGear == gears) {
                return false;
            }

            if (TimeSince(lastShiftTime) < GLOBAL_SHIFT_COOLDOWN) {
                return false;
            }

            if (engine.GetRPM() >= engine.GetMaxRPM() * 0.95) {
                return true;
            }

            if (TimeSince(lastUpshiftTime) < UPSHIFT_COOLDOWN) {
                return false;
            }

            double nextGearRpm = GetRpmForGear(currentGear + 1);

            if (nextGearRpm < 1000) {
                return false;
            }

            double currentTorque = GetWheelTorque(engine.GetTorque(GetRpmForGear(currentGear)), currentGear);
            double nextGearTorque = GetWheelTorque(engine.GetTorque(nextGearRpm), currentGear + 1);

            return nextGearTorque > currentTorque;
        }

        public void StartShift(int gear) {
            if (gear > currentGear) {
                lastUpshiftTime = Now();
            }
            if (gear < currentGear) {
                lastDownshiftTime = Now();
            }
            lastShiftTime = Now();
            shiftPhase = ShiftPhase.PRE_SHIFTING;
            shiftTimer = 0.0;
            targetGear = gear;
        }

        public double GetRpmForGear(int gear) {
            if (gear == 0) return engine.GetRPM();
            return wheelRpm * gearRatios[gear] * finalDriveRatio;
        }

        public double GetWheelTorque(double engineTorque, int gear) {
            return engineTorque * gearRatios[gear] * finalDriveRatio - GetTotalResistance() * wheelRadius;
        }

        public ShiftPhase GetShiftPhase() {
            return shiftPhase;
        }

        private long Now() {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private long TimeSince(long timestamp) {
            return Now() - timestamp;
        }

        public enum DriveMode {
            NORMAL,
            SPORT
        }

        public enum ShiftPhase {
            IDLE,
            PRE_SHIFTING,
            POST_SHIFTING
        }
    }
}
