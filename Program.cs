using System;
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
        static Clutch clutch;
        static Dyno dyno;

        public static void Main() {
            Console.WindowWidth = 180;

            simulationThread = new Thread(Run);

            engine = new Engine(3.0, 6000);

            var gearRatios = Gearbox.GearSet(3.82, 2.05, 1.30, 0.96, 0.74, 0.61);
            double finalGearRatio = 3.65;
            clutch = new Clutch(engine);
            gearbox = new Gearbox(engine, 6, gearRatios, finalGearRatio);
            clutch.SetGearbox(gearbox);

            isRunning = true;

            simulationThread.Start();
            soundThread = new Thread(Sound);
            soundThread.Start();

            //Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

        public static void Test() {
            engine.GetECU().SetThrottle(0.0);
            double rpm = 0.0;

            for (int i = 0; i < 20; i++) {
                rpm += 300;
                engine.SetRPM(rpm);
                engine.Update(0);
                engine.ShowInfo();
            }
        }

        public static void Run() {
            engine.Ignite();

            dyno = new Dyno();
            dyno.Run();

            while (isRunning) {
                HandleInput();

                lastUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                engine.Update(0.01);

                gearbox.Update(0.01);

                clutch.Update(0.01);

                engine.ShowInfo();

                Thread.Sleep(10);
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

            if (!(gearbox is AutomaticGearbox auto) || auto.GetShiftPhase() == AutomaticGearbox.ShiftPhase.IDLE) {
                if (Keyboard.IsKeyDown(Keys.Space)) {
                    clutch.SetEngagement(1.0);
                } else {
                    clutch.SetEngagement(0.0);
                } 
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

        public static Clutch GetClutch() {
            return clutch;
        }

        public static long GetLastUpdateTime() {
            return lastUpdateTime;
        }

    }
}
