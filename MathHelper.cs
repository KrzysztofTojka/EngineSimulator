using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public static class MathHelper {

        static Random random = new Random();

        static bool useRandom = true;

        public static double Clamp(double value, double min, double max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Clamp(float value, float min, float max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static double PowerFunction(double x, double yMin, double yMax, double power) {
            return yMin + (yMax - yMin) * Math.Pow(x, power);
        }

        public static double Random(double min, double max, bool enabled) {
            if (!enabled) return 1.0;
            if (!useRandom) return (min + max) / 2.0;
            return min + (max - min) * random.NextDouble();
        }

        public static double Lerp(double a, double b, double t) {
            t = Clamp(t, 0.0, 1.0);
            return a + (b - a) * t;
        }

        public static List<double> Linspace(double start, double end, int num) {
            List<double> result = new List<double>();
            double step = (end - start) / (num - 1);
            for (int i = 0; i < num; i++) {
                result.Add(start + step * i);
            }
            return result;
        }

        public static double SigmoidFunction(double x, double steepness = 10.0, double midpoint = 0.5) {
            double sigmoid = 1.0 / (1.0 + Math.Exp(-steepness * (x - midpoint)));
            return sigmoid;
        }

        public static void UseRandom(bool use) {
            useRandom = use;
        }

    }
}
