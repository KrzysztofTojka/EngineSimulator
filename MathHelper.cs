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

        public static double Random(double min, double max) {
            if (!useRandom) return (min + max) / 2.0;
            return min + (max - min) * random.NextDouble();
        }

        public static double Lerp(double a, double b, double t) {
            return a + (b - a) * t;
        }

        public static void UseRandom(bool use) {
            useRandom = use;
        }

    }
}
