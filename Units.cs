using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public static class Units {

        public const double GAS_CONSTANT = 287.05;
        
        public const double HP = 1 / 745.7;
        public const double kPa = 1 / 1000.0;
        public const double L = 1000.0;
        public const double km = 1 / 1000.0;
        public const double h = 1 / 3600.0;

        public static double C_to_K(double c) {
            return c + 274.15;
        }

        public static double kgs_to_Lh(double kgs, double density) {
            return (kgs / density) * (Units.L / Units.h);
        }

    }
}
