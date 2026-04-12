using System;

namespace EngineSimulator {
    public class DualClutchGearbox : AutomaticGearbox {

        public const double DISENGAGE_TIME = 50;
        public const double ENGAGE_TIME = 50;
        public const double SELECT_TIME = 100;
        public const double SHIFT_COOLDOWN = 1000;

        private Clutch clutchEven;
        private Clutch clutchOdd;
        private int gearEven;
        private int gearOdd;
        private ShiftPhase shiftPhase;
        private bool scheduledShift;
        private bool scheduledSelect;
        private int scheduledGear;
        private double targetLaunchRpm;
        private double launchProgress;
        private double launchThrottle;
        private double launchClutchEngangement;

        public DualClutchGearbox(Engine engine, int gears, double[] gearRatios, double finalGearRatio) : base(engine, gears, gearRatios, finalGearRatio) {
            this.clutchEven = new Clutch(engine, this);
            this.clutchOdd = new Clutch(engine, this);
            this.gearEven = 2;
            this.gearOdd = 1;
            this.shiftPhase = ShiftPhase.STALL;
            this.shiftTimer = 0;
            this.scheduledShift = false;
            this.scheduledSelect = false;
            this.scheduledGear = 0;
            this.targetLaunchRpm = 0;
            this.launchProgress = 0;
            this.launchThrottle = 0;
            this.launchClutchEngangement = 0;
        }

        public override void Update(double dt) {
            clutchEven.Update(dt);
            clutchOdd.Update(dt);

            SetInputTorque(clutchEven.GetOutputTorque() + clutchOdd.GetOutputTorque());

            UpdatePhysics(dt);

            UpdateShiftTimer(dt);

            UpdateShiftLogic(dt);
        }

        protected override void UpdateShiftTimer(double dt) {
            if (currentGear == 0) {
                return;
            }

            switch (shiftPhase) {
                case ShiftPhase.IDLE: {
                    // TODO better preselect
                    if (Program.GetBrakePedalPosition() > 0.0) {
                        int downshift = GetMinDownshiftGear(Math.Pow(Program.GetBrakePedalPosition(), 2.0));

                        if (downshift < currentGear) {
                            StartShift(downshift);
                            break;
                        }
                    }

                    if (currentGear != gears && GetOppositeGear() != currentGear + 1) {
                        StartPreselect(currentGear + 1);
                        break;
                    }

                    if (currentGear == 1) {
                        double minSpeed = RpmToCarSpeed(900, wheelRadius, GetTotalRatio(1));
                        if (GetCarSpeed() < minSpeed) {
                            shiftPhase = ShiftPhase.SLIP;
                            break;
                        }
                    }

                    break;
                }
                    
                case ShiftPhase.SLIP: {
                    double minSpeed = RpmToCarSpeed(900, wheelRadius, GetTotalRatio(1));
                    if (GetCarSpeed() < 0.1) {
                        SetCurrentClutchEngangement(0.0);
                        shiftPhase = ShiftPhase.STALL;
                    } else if (GetCarSpeed() < minSpeed) {
                        SetCurrentClutchEngangement(Math.Pow(GetCarSpeed() / minSpeed, 1.5));
                    } else {
                        SetCurrentClutchEngangement(1.0);
                        shiftPhase = ShiftPhase.IDLE;
                    }
                    break;
                }
                    
                case ShiftPhase.STALL:
                    if (Program.GetBrakePedalPosition() < 0.5) {
                        shiftPhase = ShiftPhase.LAUNCH;
                    }
                    break;
                case ShiftPhase.LAUNCH:
                    if (GetCurrentClutchEngangement() == 1.0) {
                        shiftPhase = ShiftPhase.IDLE;
                        engine.GetECU().SetThrottleOverride(-1);
                        break;
                    }

                    targetLaunchRpm = SelectLaunchRpm(Program.GetThrottlePedalPosition());
                    launchProgress = GetCarSpeed() / RpmToCarSpeed(targetLaunchRpm, wheelRadius, GetTotalRatio());
                    launchProgress = MathHelper.Clamp(launchProgress, 0.0, 1.0);
                    launchThrottle = GetLaunchThrottle(Program.GetThrottlePedalPosition());
                    engine.GetECU().SetThrottleOverride(launchThrottle);
                    launchClutchEngangement = GetLaunchClutchEngangement();
                    SetCurrentClutchEngangement(launchClutchEngangement);
                    /*Console.WriteLine(
                        $"{(DateTimeOffset.Now.ToUnixTimeMilliseconds() - Program.startTime) / 1000:F3} s | " +
                        $"{engine.GetRPM():F0} RPM, " +
                        $"{Program.GetGearbox().GetCarSpeed() * (Units.km / Units.h):F2} kmh - " +
                        $"LP: {launchProgress:F3}, " +
                        $"LCE: {launchClutchEngangement:F4}, " +
                        $"LR: {targetLaunchRpm:F2}, " +
                        $"LT: {launchThrottle:F4}"
                    );*/
                    break;
                case ShiftPhase.PRESELECT:
                    if (shiftTimer > SELECT_TIME) {
                        shiftTimer = 0;
                        SetSelected(scheduledGear);
                        if (scheduledShift) {
                            scheduledShift = false;
                            StartShift();
                        } else {
                            shiftPhase = ShiftPhase.IDLE;
                        }
                    }
                    break;
                case ShiftPhase.SELECT:
                    if (shiftTimer > SELECT_TIME) {
                        shiftTimer = 0;
                        SetSelected(scheduledGear);
                        currentGear = scheduledGear;
                        shiftPhase = ShiftPhase.ENGAGE;
                    }
                    break;
                case ShiftPhase.DISENGAGE:
                    SetCurrentClutchEngangement(1.0 - (shiftTimer / DISENGAGE_TIME));
                    if (shiftTimer > DISENGAGE_TIME) {
                        shiftTimer = 0;
                        if (scheduledSelect) {
                            scheduledSelect = false;
                            shiftPhase = ShiftPhase.SELECT;
                        } else if (scheduledShift) {
                            scheduledShift = false;
                            currentGear = scheduledGear;
                            shiftPhase = ShiftPhase.ENGAGE;
                        }
                    }
                    break;
                case ShiftPhase.ENGAGE:
                    SetCurrentClutchEngangement(shiftTimer / ENGAGE_TIME);
                    if (shiftTimer > ENGAGE_TIME) {
                        shiftTimer = 0;
                        shiftPhase = ShiftPhase.IDLE;
                        engine.RetardIgnition(false);
                    }
                    break;
            }

            shiftTimer += dt * 1000.0;
        }

        private double SelectLaunchRpm(double throttlePedal) {
            return MathHelper.Lerp(900, 4000, throttlePedal);
        }

        private double GetLaunchThrottle(double throttlePedal) {
            double minTargetThrottle = engine.GetECU().GetIdleThrottle();
            double targetThrottle = Math.Max(minTargetThrottle, engine.GetECU().GetThrottleMap(Program.GetThrottlePedalPosition()));
            //return targetThrottle * MathHelper.Lerp(0.3, 1.0, Math.Pow(launchProgress, 0.5));
            return engine.GetECU().GetThrottleMap(Math.Max(throttlePedal, 0.13));
        }

        private double GetLaunchClutchEngangement() {
            return MathHelper.Lerp(0.01 + 0.3 * Math.Max(Program.GetThrottlePedalPosition(), 0.13), 1.2, Math.Pow(launchProgress, 1.0));
        }

        protected override void UpdateShiftLogic(double dt) {
            if (currentGear == 0) {
                return;
            }

            if (shiftPhase != ShiftPhase.IDLE) {
                return;
            }

            int minDownshiftGear = GetMinDownshiftGear();

            if (minDownshiftGear < currentGear && (Program.GetThrottlePedalPosition() > 0.9 || TimeSince(lastShiftTime) > SHIFT_COOLDOWN)) {
                StartShift(minDownshiftGear);
            }

            if (ShouldUpshift() && TimeSince(lastShiftTime) > SHIFT_COOLDOWN) {
                StartShift(currentGear + 1);
            }
        }

        protected override void StartShift(int targetGear) {
            if (shiftPhase != ShiftPhase.IDLE) {
                return;
            }

            if (GetOppositeGear() == targetGear) {
                StartShift();
            } else {
                StartSelect(targetGear);
                ScheduleShift();
            }
        }

        private int GetOppositeGear() {
            if (currentGear % 2 == 0) {
                return gearOdd;
            } else {
                return gearEven;
            }
        }

        private void StartShift() {
            shiftPhase = ShiftPhase.DISENGAGE;
            lastShiftTime = Now();
            engine.RetardIgnition(true);
            ScheduleShift();
            scheduledGear = GetOppositeGear();
        }

        private void StartPreselect(int targetGear) {
            if (currentGear % 2 == targetGear % 2) {
                return;
            }

            shiftPhase = ShiftPhase.PRESELECT;
            scheduledGear = targetGear;
        }

        private void StartSelect(int targetGear) {
            if (currentGear % 2 != targetGear % 2) {
                StartPreselect(targetGear);
            } else {
                shiftPhase = ShiftPhase.DISENGAGE;
                ScheduleSelect(targetGear);
            }
        }

        private void ScheduleShift() {
            scheduledShift = true;
        } 

        private void ScheduleSelect(int targetGear) {
            scheduledSelect = true;
            scheduledGear = targetGear;
        }

        private void SetSelected(int targetGear) {
            if (targetGear % 2 == 0) {
                gearEven = targetGear;
            } else {
                gearOdd = targetGear;
            }
        }

        private void SetCurrentClutchEngangement(double engangement) {
            if (currentGear % 2 == 0) {
                clutchEven.SetEngagement(engangement);
            } else {
                clutchOdd.SetEngagement(engangement);
            }
        }
        
        private double GetCurrentClutchEngangement() {
            if (currentGear % 2 == 0) {
                return clutchEven.GetEngangement();
            } else {
                return clutchOdd.GetEngangement();
            }
        }

        public override double GetTotalRatio() {
            return GetTotalRatio(currentGear);
        }

        public double GetTotalRatio(int gear) {
            return gearRatios[gear] * finalDriveRatio;
        }

        public new enum ShiftPhase {
            IDLE,
            STALL,
            LAUNCH,
            SLIP,
            PRESELECT,
            SELECT,
            DISENGAGE,
            ENGAGE
        }

    }
}
