using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class CarParts {

        public static List<EnginePreset> GetEngines() {
            return new List<EnginePreset> {
                new EnginePreset {
                    Name = "Mazda SKYACTIV-G 2.5 (194 HP)",
                    Create = () => new GasolineEngine(2.5, 6200, 0.155)
                },
                new EnginePreset {
                    Name = "Opel Ecotec 1.9 CDTI (150 HP)",
                    Create = () => new DieselEngine(1.9, 4500)
                }
            };
        }

        public static List<GearboxPreset> GetGearboxes() {
            return new List<GearboxPreset> {
                new GearboxPreset {
                    Name = "Mazda 6-Speed Automatic",
                    Create = () => {
                        var gears = Gearbox.GearSet(3.552, 2.022, 1.452, 1.000, 0.708, 0.599);
                        return new DualClutchGearbox(6, gears, 4.056);
                    }
                },
                new GearboxPreset {
                    Name = "M32 6-Speed Manual",
                    Create = () => {
                        var gears = Gearbox.GearSet(3.82, 2.05, 1.30, 0.96, 0.74, 0.61);
                        return new ManualGearbox(6, gears, 3.65);
                    }
                },
                new GearboxPreset {
                    Name = "BMW Dual-Clutch 8-Speed (GA8S45DW)",
                    Create = () => {
                        var gears = Gearbox.GearSet(5.519, 3.184, 2.05, 1.492, 1.235, 1, 0.801, 0.673);
                        return new DualClutchGearbox(8, gears, 3.075);
                    }
                }
            };
        }

    }
}
