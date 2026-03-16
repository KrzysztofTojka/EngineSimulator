using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class AutomaticGearbox : Gearbox {

        public const double CLUTCH_TIME = 0.02;
        public const double SHIFT_TIME = 0.1;
        public const double UPSHIFT_COOLDOWN = 1.0;
        public const double DOWNSHIFT_COOLDOWN = 1.0;

        private DriveMode driveMode;
        private ShiftPhase shiftPhase;
        private double shiftTimer;
        private int targetGear;
        private long lastUpshiftTime;

        private Clutch clutch;

        public AutomaticGearbox(Engine engine, Type type, int gears, double[] gearRatios, double finalGearRatio) : base(engine, type, gears, gearRatios, finalGearRatio) {
            this.driveMode = DriveMode.NORMAL;
            this.shiftPhase = ShiftPhase.IDLE;
            this.shiftTimer = -1;
            this.targetGear = 0;
            this.lastUpshiftTime = 0;
            this.clutch = Program.GetClutch();
        }

        public override void Update(double dt) {
            base.Update(dt);

            UpdateShiftTimer(dt);

            if (currentGear == 0) {
                return;
            }

            if (shiftPhase != ShiftPhase.IDLE || clutch.GetEngangement() < 1.0) {
                return;
            }
            
            if (ShouldDownshift() && (DateTimeOffset.Now.ToUnixTimeSeconds() - lastUpshiftTime) > DOWNSHIFT_COOLDOWN) {
                StartShift(currentGear - 1);
            }

            if (ShouldUpshift() && (DateTimeOffset.Now.ToUnixTimeSeconds() - lastUpshiftTime) > UPSHIFT_COOLDOWN) {
                StartShift(currentGear + 1);
            }
            
        }

        public void UpdateShiftTimer(double dt) {
            if (shiftTimer == -1) {
                return;
            }

            if (shiftPhase == ShiftPhase.IDLE && shiftTimer == 0.0) {
                shiftPhase = ShiftPhase.DISENGAGING;
            }

            if (shiftPhase == ShiftPhase.DISENGAGING) {
                clutch.SetEngagement(1.0 - (shiftTimer / CLUTCH_TIME));
                if (shiftTimer >= CLUTCH_TIME) {
                    shiftPhase = ShiftPhase.SHIFTING;
                    shiftTimer = 0.0;
                }
            } else if (shiftPhase == ShiftPhase.SHIFTING) {
                if (shiftTimer >= SHIFT_TIME * 0.6 && targetGear > 0) {
                    SetGear(targetGear);
                    targetGear = 0;
                } else if (shiftTimer >= SHIFT_TIME) {
                    shiftPhase = ShiftPhase.ENGAGING;
                    shiftTimer = 0.0;
                }
            } else if (shiftPhase == ShiftPhase.ENGAGING) {
                clutch.SetEngagement(shiftTimer / CLUTCH_TIME);
                if (shiftTimer >= CLUTCH_TIME) {
                    shiftPhase = ShiftPhase.IDLE;
                    shiftTimer = -1;
                }
            }

            shiftTimer += dt;
        }

        public bool ShouldUpshift() {
            if (engine.GetECU().GetThrottlePedal() == 0.0) {
                return false;
            }

            if (currentGear == gears) {
                return false;
            }

            if (engine.GetRPM() >= engine.GetMaxRPM() * 0.95) {
                return true;
            }

            double nextGearRpm = GetRpmForGear(currentGear + 1);

            if (nextGearRpm < 1000) {
                return false;
            }

            double currentTorque = GetWheelTorque(engine.GetTorque(GetRpmForGear(currentGear)), currentGear);
            double nextGearTorque = GetWheelTorque(engine.GetTorque(nextGearRpm), currentGear + 1);

            return nextGearTorque > currentTorque;
        }

        public bool ShouldDownshift() {
            if (currentGear <= 1) {
                return false;
            }

            double prevGearRpm = GetRpmForGear(currentGear - 1);

            if (prevGearRpm >= engine.GetMaxRPM() * 0.95) {
                return false;
            }

            double currentTorque = GetWheelTorque(engine.GetTorque(GetRpmForGear(currentGear)), currentGear);
            double prevGearTorque = GetWheelTorque(engine.GetTorque(prevGearRpm), currentGear - 1);

            return prevGearTorque > currentTorque;
        }

        public void StartShift(int gear) {
            shiftTimer = 0.0;
            targetGear = gear;
            lastUpshiftTime = DateTimeOffset.Now.ToUnixTimeSeconds();
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

        public enum DriveMode {
            NORMAL,
            SPORT
        }

        public enum ShiftPhase {
            IDLE,
            DISENGAGING,
            SHIFTING,
            ENGAGING
        }
    }
}
