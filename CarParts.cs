using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class CarParts {

        public static List<CarPreset> GetCars() {
            return new List<CarPreset> {
                new CarPreset {
                    Name = "Mazda CX-5 II 2.5 SKYACTIV-G",
                    EnginePresetName = "Mazda 2.5 SKYACTIV-G (194 HP)",
                    GearboxPresetName = "Mazda 6-Speed Automatic"
                },
                new CarPreset {
                    Name = "Opel Astra H 1.9 CDTI",
                    EnginePresetName = "Opel 1.9 CDTI Ecotec (150 HP)",
                    GearboxPresetName = "M32 6-Speed Manual"
                },
                new CarPreset {
                    Name = "BMW M235i xDrive",
                    EnginePresetName = "BMW 2.0 B48A20T1 (306 HP)",
                    GearboxPresetName = "BMW Dual-Clutch 8-Speed (GA8S45DW)"
                }
            };
        }

        public static List<EnginePreset> GetEngines() {
            return new List<EnginePreset> {
                new EnginePreset {
                    Name = "Mazda 2.5 SKYACTIV-G (194 HP)",
                    Create = () => new GasolineEngine(2.5, 6200, 0.185)
                },
                new EnginePreset {
                    Name = "Opel 1.9 CDTI Ecotec (150 HP)",
                    Create = () => new DieselEngine(1.9, 4500)
                },
                new EnginePreset {
                    Name = "BMW 2.0 B48A20T1 (306 HP)",
                    Create = () => new GasolineEngine(2.0, 6800, 0.16, maxAirflowRpm: 6100, turbo: new Turbocharger(0.65))
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
