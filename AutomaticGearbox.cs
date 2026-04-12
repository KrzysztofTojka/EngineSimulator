using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class AutomaticGearbox : Gearbox {

        private Clutch clutch; // for now
        private TorqueConverter torqueConverter;

        public const double SHIFT_TIME = 100;
        public const double GLOBAL_SHIFT_COOLDOWN = 500;
        public const double UPSHIFT_COOLDOWN = 1500;
        public const double DOWNSHIFT_COOLDOWN = 1000;
        public const double MIN_UPSHIFT_RPM = 1700;

        private DriveMode driveMode;
        private ShiftMode shiftMode;
        private ShiftPhase shiftPhase;
        protected double shiftTimer;
        private int targetGear;
        private int prevGear;
        protected long lastShiftTime;
        protected long lastUpshiftTime;
        protected long lastDownshiftTime;

        public AutomaticGearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) : base(engine, gears, gearRatios, finalGearRatio) {
            this.type = Type.Automatic;
            this.clutch = new Clutch(engine, this, 2000);
            this.torqueConverter = new TorqueConverter(engine, this);
            this.driveMode = DriveMode.NEUTRAL;
            this.shiftMode = ShiftMode.NORMAL;
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

            UpdateShiftLogic(dt);

            torqueConverter.Update(dt);
            //clutch.Update(dt); // for now
        }

        protected virtual void UpdateShiftLogic(double dt) {
            if (currentGear == 0) {
                return;
            }

            if (shiftPhase != ShiftPhase.IDLE) {
                return;
            }

            int minDownshiftGear = GetMinDownshiftGear();

            if (minDownshiftGear < currentGear/* && TimeSince(lastShiftTime) > DOWNSHIFT_COOLDOWN*/) {
                StartShift(minDownshiftGear);
            }

            if (ShouldUpshift()) {
                StartShift(currentGear + 1);
            }

            if (shiftPhase == ShiftPhase.IDLE) {
                clutch.SetPosition(1.0 - Program.GetClutchPedalPosition());
            }
        }

        protected virtual void UpdateShiftTimer(double dt) {
            if (shiftTimer == -1) {
                return;
            }

            if (shiftPhase == ShiftPhase.PRE_SHIFTING && shiftTimer >= SHIFT_TIME / 2.0) {
                prevGear = currentGear;
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
        
        public int GetMinDownshiftGear(double throttlePedal) {
            if (currentGear <= 1) {
                return currentGear;
            }

            if (engine.GetRPM() < 900) {
                return currentGear - 1;
            }

            double currentTorque = GetWheelTorque(engine.GetTorque(engine.GetECU().GetThrottleMap(throttlePedal), GetRpmForGear(currentGear)), currentGear);

            int minGear = currentGear;
            double maxTorque = currentTorque;
            for (int i = currentGear; i > 1; i--) {
                double prevGearRpm = GetRpmForGear(i - 1);

                if (prevGearRpm >= engine.GetMaxRPM() * 0.9) {
                    return minGear;
                }
                double prevGearTorque = GetWheelTorque(engine.GetTorque(engine.GetECU().GetThrottleMap(throttlePedal), prevGearRpm), i - 1);

                if (prevGearTorque > maxTorque) {
                    maxTorque = prevGearTorque;
                    minGear = i - 1;
                }
            }

            return minGear;
        }

        public int GetMinDownshiftGear() {
            return GetMinDownshiftGear(Program.GetThrottlePedalPosition());
        }

        public bool ShouldUpshift() {
            if (engine.GetECU().GetThrottlePedal() <= 0.05) {
                return false;
            }

            if (currentGear == gears) {
                return false;
            }

            if (TimeSince(lastShiftTime) < GLOBAL_SHIFT_COOLDOWN) {
                return false;
            }

            if (engine.GetRPM() >= engine.GetMaxRPM() * 0.98) {
                return true;
            }

            if (TimeSince(lastUpshiftTime) < UPSHIFT_COOLDOWN) {
                return false;
            }

            if (engine.GetRPM() < 1700) {
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

        protected virtual void StartShift(int gear) {
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

        public override void GearUp() {
            switch (driveMode) {
                case DriveMode.NEUTRAL:
                    driveMode = DriveMode.DRIVE;
                    SetGear(1);
                    break;
                case DriveMode.REVERSE:
                    driveMode = DriveMode.NEUTRAL;
                    break;
            }
        }

        public override void GearDown() {
            switch (driveMode) {
                case DriveMode.NEUTRAL:
                    driveMode = DriveMode.REVERSE;
                    break;
                case DriveMode.DRIVE:
                    driveMode = DriveMode.NEUTRAL;
                    SetGear(0);
                    break;
            }
        }

        public override double GetTotalRatio() {
            if (currentGear == 0) {
                return 0.0;
            }

            if (shiftPhase == ShiftPhase.IDLE) {
                return base.GetTotalRatio();
            }

            if (shiftPhase == ShiftPhase.PRE_SHIFTING) {
                return MathHelper.Lerp(gearRatios[currentGear], gearRatios[targetGear], shiftTimer / SHIFT_TIME) * finalDriveRatio;
            }

            if (shiftPhase == ShiftPhase.POST_SHIFTING) {
                return MathHelper.Lerp(gearRatios[prevGear], gearRatios[currentGear], 0.5 + shiftTimer / SHIFT_TIME) * finalDriveRatio;
            }
            
            return gearRatios[currentGear] * finalDriveRatio;
        }

        public ShiftPhase GetShiftPhase() {
            return shiftPhase;
        }

        public override string GetGearLabel() {
            switch (driveMode) {
                case DriveMode.NEUTRAL:
                    return "N";
                case DriveMode.DRIVE:
                    return currentGear.ToString();
                case DriveMode.REVERSE:
                    return "R";
                default:
                    return "";
            }
        }

        protected long Now() {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        protected long TimeSince(long timestamp) {
            return Now() - timestamp;
        }

        public enum DriveMode {
            NEUTRAL,
            DRIVE,
            REVERSE,
        }

        public enum ShiftMode {
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
