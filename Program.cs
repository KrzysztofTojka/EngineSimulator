using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace EngineSimulator {
    public class Program {

        private static Car car;

        public const double TEMPERATURE_C = 20.0;
        public const double PRESSURE_HPA = 1024.0;
        public const double AIR_DENSITY = 1.225;

        private static SteeringWheel steeringWheel;
        private static Keyboard keyboard;

        private static Thread simulationThread;
        private static Thread soundThread;
        private static bool isRunning = false;
        private static long lastUpdateTime = 0;

        private static Engine engine;
        private static Gearbox gearbox;
        private static Dyno dyno;

        public static bool AUTO_START = true;

        private static double throttlePedalPosition = 0.0;
        private static double brakePedalPosition = 0.0;
        private static double clutchPedalPosition = 0.0;

        public static double startTime;

        public static void Main() {
            Console.WindowWidth = 160;

            keyboard = new Keyboard();

            if (SteeringWheel.IsPresent()) {
                steeringWheel = new SteeringWheel();
                Console.WriteLine("Steering wheel initialized: " + steeringWheel.GetName());
                steeringWheel.Poll();
            } else {
                Console.WriteLine("No steering wheel found");
            }

            var gearRatios = Gearbox.GearSet(3.552, 2.022, 1.452, 1.000, 0.708, 0.599);
            double finalGearRatio = 4.056;//4.325; 4.056

            //var gearRatios = Gearbox.GearSet(3.82, 2.05, 1.30, 0.96, 0.74, 0.61);
            //double finalGearRatio = 3.65;

            car = new Car();
            car.SetEngine(new GasolineEngine(2.5, 6500, 0.17));
            //car.SetEngine(new DieselEngine(1.9, 4500));
            car.SetGearbox(new ManualGearbox(car.GetEngine(), 6, gearRatios, finalGearRatio));
            //car.SetGearbox(new AutomaticGearbox(engine, 6, gearRatios, finalGearRatio));
            //car.SetGearbox(new DualClutchGearbox(engine, 6, gearRatios, finalGearRatio));

            UseCar(car);

            isRunning = true;
            startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            simulationThread = new Thread(Run);
            simulationThread.Start();
            soundThread = new Thread(SoundThread);
            soundThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

        public static void Run() {
            dyno = new Dyno();
            dyno.DoMaxTorqueRun(1.0);

            if (AUTO_START) {
                engine.SetIgnition(true);
                engine.SetRPM(500);
            }

            double dt = 0.01;
            double accumulator = 0.0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            double lastTime = stopwatch.Elapsed.TotalSeconds;
            int i = 1;

            while (isRunning) {
                double currentTime = stopwatch.Elapsed.TotalSeconds;
                double frameTime = currentTime - lastTime;
                lastTime = currentTime;

                if (frameTime > 0.25) frameTime = 0.25;
                accumulator += frameTime;

                UpdateInput();

                while (accumulator >= dt) {
                    car.Update(dt);
                    PerformanceMeter.Update(gearbox.GetCarSpeed() * (Units.km / Units.h));

                    if (i == 1) {
                        engine.ShowInfo();
                        i = 0;
                    }

                    accumulator -= dt;
                    i++;
                }

                Thread.Sleep(1);
            }
        }

        private static void SoundThread() {
            int sampleRate = 44100;
            int bufferSize = 16384; // 2^14, around 0.37s

            AudioEngine.Init(sampleRate, true, bufferSize);
            AudioEngine.SetVolume(0.0f);
            AudioEngine.StartEngine();

            while (true) {
                AudioEngine.Update(engine.GetRPM(), engine.GetLoad(), 20);
                Thread.Sleep(20);
            }
        }

        public static void UpdateInput() {
            throttlePedalPosition = 0;
            clutchPedalPosition = 0;
            brakePedalPosition = 0;

            keyboard.Update();

            if (!(steeringWheel is null)) {
                steeringWheel.Poll();
                throttlePedalPosition = steeringWheel.GetThrottle();
                brakePedalPosition = steeringWheel.GetBrake();
                clutchPedalPosition = steeringWheel.GetClutch();
            }

            if (keyboard.IsKeyDown(Keys.D1)) {
                throttlePedalPosition = 0.1;
            }
            if (keyboard.IsKeyDown(Keys.D2)) {
                throttlePedalPosition = 0.2;
            }
            if (keyboard.IsKeyDown(Keys.D3)) {
                throttlePedalPosition = 0.3;
            }
            if (keyboard.IsKeyDown(Keys.D4)) {
                throttlePedalPosition = 0.4;
            }
            if (keyboard.IsKeyDown(Keys.D5)) {
                throttlePedalPosition = 0.5;
            }
            if (keyboard.IsKeyDown(Keys.D6)) {
                throttlePedalPosition = 0.6;
            }
            if (keyboard.IsKeyDown(Keys.D7)) {
                throttlePedalPosition = 0.7;
            }
            if (keyboard.IsKeyDown(Keys.D8)) {
                throttlePedalPosition = 0.8;
            }
            if (keyboard.IsKeyDown(Keys.D9)) {
                throttlePedalPosition = 0.9;
            }
            if (keyboard.IsKeyDown(Keys.D0)) {
                throttlePedalPosition = 1.0;
            }

            if (keyboard.WasKeyPressed(Keys.A)) {
                gearbox.GearDown();
            }

            if (keyboard.WasKeyPressed(Keys.D)) {
                gearbox.GearUp();
            }

            if (keyboard.IsKeyDown(Keys.Space)) {
                clutchPedalPosition = 1.0;
            }

            if (keyboard.IsKeyDown(Keys.S)) {
                brakePedalPosition = 1.0;
            }

            if (keyboard.WasKeyPressed(Keys.Q)) {
                engine.SetIgnition(!engine.IsIgnitionOn());
            }

            if (keyboard.WasKeyPressed(Keys.P)) {
                PerformanceMeter.Reset();
                PerformanceMeter.SetTargetSpeeds(40, 50, 60, 80, 100, 120, 130, 140, 150, 160, 180);
                PerformanceMeter.Start();
            }

            engine.SetStarter(keyboard.IsKeyDown(Keys.E));

            engine.GetECU().SetThrottle(throttlePedalPosition);
        }

        public static void UseCar(Car car) {
            Program.car = car;
            Program.engine = car.GetEngine();
            Program.gearbox = car.GetGearbox();
        }

        public static Keyboard GetKeyboard() {
            return keyboard;
        }

        public static Engine GetEngine() {
            return engine;
        }

        public static Dyno GetDyno() {
            return dyno;
        }

        public static Gearbox GetGearbox() {
            return gearbox;
        }

        public static double GetThrottlePedalPosition() {
            return throttlePedalPosition;
        }

        public static double GetClutchPedalPosition() {
            return clutchPedalPosition;
        }

        public static double GetBrakePedalPosition() {
            return brakePedalPosition;
        }

        public static long GetLastUpdateTime() {
            return lastUpdateTime;
        }

    }
}
