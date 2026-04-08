using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {

    public class PerformanceMeter {

        private static Dictionary<double, double> accelerationTimes = new Dictionary<double, double>();
        private static long startTime = 0;
        private static bool ready = false;
        private static bool isRunning = false;

        public static void SetTargetSpeeds(params double[] targetSpeeds) {
            foreach (var targetSpeed in targetSpeeds) {
                accelerationTimes[targetSpeed] = 0.0;
            }
        }

        public static void Update(double speed) {
            if (ready && speed > 0.1) {
                ready = false;
                isRunning = true;
                startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Log("Start!");
            }

            if (!isRunning) {
                return;
            }

            if (accelerationTimes.Count() == 0) {
                return;
            }

            bool left = true;

            foreach (var targetSpeed in accelerationTimes.Keys.ToList()) {
                if (speed > targetSpeed && accelerationTimes[targetSpeed] == 0.0) {
                    double time = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime) / 1000.0;
                    accelerationTimes[targetSpeed] = time;
                    Log($"0 - {targetSpeed}: {time:F4}s");

                    if (targetSpeed == accelerationTimes.Keys.Last()) {
                        left = false;
                    }
                }
            }

            if (!left) {
                Log("Completed");
                Stop();
            }
        }

        public static void Reset() {
            accelerationTimes.Clear();
        }

        public static void Start() {
            ready = true;
            Log("Ready...");
        }

        public static void StartZeroTo100() {
            Reset();
            SetTargetSpeeds(100.0);
            Start();
        }

        public static void Stop() {
            isRunning = false;
        }

        public static void PrintStats() {
            foreach (var entry in accelerationTimes) {
                double speed = entry.Key;
                double time = entry.Value;

                Console.WriteLine($"0 - {Math.Round(speed)}: {time:F4}s");
            }
        }

        private static void Log(string message) {
            Console.WriteLine($"[Performance Meter] {message}");
        }

    }
}
