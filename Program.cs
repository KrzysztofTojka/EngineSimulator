using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EngineSimulator {
    public class Program {

        public static double temperatureC = 20.0;
        public static double pressureHPA = 1024.0;

        static Thread simulationThread;
        static Thread soundThread;
        static EngineSoundPlayer engineSound2000;
        static EngineSoundPlayer engineSound3000;
        static bool isRunning = false;
        static long lastUpdateTime = 0;

        static Engine engine;
        static Gearbox gearbox;
        static Dyno dyno;

        static double clutchPedalPosition = 0.0;

        public static double startTime;

        public static void Main() {
            Console.WindowWidth = 160;

            simulationThread = new Thread(Run);

            engine = new GasolineEngine(2.5, 6500, 0.15);
            var gearRatios = Gearbox.GearSet(3.552, 2.022, 1.452, 1.000, 0.708, 0.599);
            double finalGearRatio = 4.056;//4.325; 4.056

            //engine = new DieselEngine(2.0, 4500);
            //var gearRatios = Gearbox.GearSet(3.82, 2.05, 1.30, 0.96, 0.74, 0.61);
            //double finalGearRatio = 3.65;

            gearbox = new ManualGearbox(engine, 6, gearRatios, finalGearRatio);
            //gearbox = new AutomaticGearbox(engine, 6, gearRatios, finalGearRatio);

            isRunning = true;

            startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            simulationThread.Start();
            //Test();

            //soundThread = new Thread(Sound);
            //soundThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

        public static void Test() {
            MathHelper.UseRandom(false);
            File.WriteAllLines("result.csv", new string[] { engine.GetCsvHeader() });

            engine.SetRPM(500);
            engine.GetECU().SetThrottle(engine.GetECU().GetIdleThrottle());
            engine.Update(0);

            for (int rpm = 500; rpm <= engine.GetMaxRPM(); rpm += 500) {
                engine.SetRPM(rpm);

                for (double throttle = 0.0; throttle <= 1.0; throttle += 0.05 * Math.Min(1.0, rpm / 6000.0)) {
                    if (throttle == 0.0) {
                        throttle = engine.GetECU().GetIdleThrottle();
                    }
                    
                    engine.GetECU().SetThrottle(throttle);

                    engine.Update(0);

                    string line = engine.GetCsvLine();
                    File.AppendAllLines("result.csv", new string[] { line });
                }
            }
        }

        public static void Run() {
            engine.Ignite();

            dyno = new Dyno();
            dyno.Run(1.0);

            int i = 0;

            while (isRunning) {
                HandleInput();

                lastUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                engine.Update(0.01);

                gearbox.Update(0.01);

                engine.UpdateRpm(0.01);

                if (i == 2) {
                    engine.ShowInfo();
                    i = 0;
                }
                
                Thread.Sleep(10);

                i++;
            }
        }

        private static void Sound() {
            engineSound2000 = new EngineSoundPlayer("assets/2000RPM.wav", 2000);
            //engineSound2000.Play();
            engineSound3000 = new EngineSoundPlayer("assets/3000RPM.wav", 3000);
            engineSound3000.Play();

            while (true) {
                engineSound2000.SetRPM((float) engine.GetRPM());
                engineSound2000.SetVolume((float)(engine.GetRPM() / 3000));
                engineSound3000.SetRPM((float) engine.GetRPM());
                engineSound3000.SetVolume((float)(engine.GetRPM() / 4000));
                Thread.Sleep(10);
            }
        }

        public static void HandleInput() {
            double currentThrottle = 0.0;

            Keyboard.Update();

            if (Keyboard.IsKeyDown(Keys.D1)) {
                currentThrottle = 0.1;
            }
            if (Keyboard.IsKeyDown(Keys.D2)) {
                currentThrottle = 0.2;
            }
            if (Keyboard.IsKeyDown(Keys.D3)) {
                currentThrottle = 0.3;
            }
            if (Keyboard.IsKeyDown(Keys.D4)) {
                currentThrottle = 0.4;
            }
            if (Keyboard.IsKeyDown(Keys.D5)) {
                currentThrottle = 0.5;
            }
            if (Keyboard.IsKeyDown(Keys.D6)) {
                currentThrottle = 0.6;
            }
            if (Keyboard.IsKeyDown(Keys.D7)) {
                currentThrottle = 0.7;
            }
            if (Keyboard.IsKeyDown(Keys.D8)) {
                currentThrottle = 0.8;
            }
            if (Keyboard.IsKeyDown(Keys.D9)) {
                currentThrottle = 0.9;
            }
            if (Keyboard.IsKeyDown(Keys.D0)) {
                currentThrottle = 1.0;
            }

            if (Keyboard.WasKeyPressed(Keys.A)) {
                gearbox.GearDown();
            }

            if (Keyboard.WasKeyPressed(Keys.D)) {
                gearbox.GearUp();
            }

            if (Keyboard.IsKeyDown(Keys.Space)) {
                clutchPedalPosition = 0.0;
            } else {
                clutchPedalPosition = 1.0;
            }

            if (Keyboard.IsKeyDown(Keys.S)) {
                gearbox.SetBrakesEngangement(1.0);
            } else {
                gearbox.SetBrakesEngangement(0.0);
            }

            engine.GetECU().SetThrottle(currentThrottle);
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

        public static double GetClutchPedalPosition() {
            return clutchPedalPosition;
        }

        public static long GetLastUpdateTime() {
            return lastUpdateTime;
        }

    }
}
