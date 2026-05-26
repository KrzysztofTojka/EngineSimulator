using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSimulator {
    public class CarDatabase {

        public static List<CarPreset> GetCars() {
            return new List<CarPreset> {
                new CarPreset {
                    Name = "Mazda CX-5 II",
                    Engines = new List<string> { 
                        "Mazda 2.5 SKYACTIV-G (194 HP)" ,
                        "Mazda 2.0 SKYACTIV-G (165 HP)"
                    },
                    Gearbox = "Mazda 6-Speed Automatic",
                    Weight = 1515,
                    WheelRadius = 0.365
                },
                new CarPreset {
                    Name = "Opel Astra H",
                    Engines = new List<string> { 
                        "Opel 1.9 CDTI Ecotec (150 HP)" 
                    },
                    Gearbox = "M32 6-Speed Manual",
                    Weight = 1450,
                    WheelRadius = 0.316
                },
                new CarPreset {
                    Name = "BMW M235i xDrive",
                    Engines = new List<string> { 
                        "BMW 2.0 B48A20T1 (306 HP)" 
                    },
                    Gearbox = "BMW Dual-Clutch 8-Speed Steptronic",
                    Weight = 1645,
                    WheelRadius = 0.31875
                },
                new CarPreset {
                    Name = "Dodge Challenger III",
                    Engines = new List<string> {
                        "Dodge 6.1 V8 HEMI (431 HP)"
                    },
                    Gearbox = "W5A580 5-Speed Automatic",
                    Weight = 1892,
                    WheelRadius = 0.365
                }
            };
        }

        public static List<EnginePreset> GetEngines() {
            return new List<EnginePreset> {
                new EnginePreset {
                    Name = "Mazda 2.5 SKYACTIV-G (194 HP)",
                    Create = () => new GasolineEngine(2.5, 6500, 0.185)
                },
                new EnginePreset {
                    Name = "Mazda 2.0 SKYACTIV-G (165 HP)",
                    Create = () => new GasolineEngine(2.0, 6500, 0.175)
                },
                new EnginePreset {
                    Name = "Opel 1.9 CDTI Ecotec (150 HP)",
                    Create = () => new DieselEngine(1.9, 4500)
                },
                new EnginePreset {
                    Name = "BMW 2.0 B48A20T1 (306 HP)",
                    Create = () => new GasolineEngine(2.0, 6800, 0.16, maxAirflowRpm: 6100, turbo: new Turbocharger(0.65))
                },
                new EnginePreset {
                    Name = "Dodge 6.1 V8 HEMI (431 HP)",
                    Create = () => new GasolineEngine(6.1, 6400, 0.3)
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
                    Name = "BMW Dual-Clutch 8-Speed Steptronic",
                    Create = () => {
                        var gears = Gearbox.GearSet(5.519, 3.184, 2.05, 1.492, 1.235, 1, 0.801, 0.673);
                        return new DualClutchGearbox(8, gears, 3.075);
                    }
                },
                new GearboxPreset {
                    Name = "W5A580 5-Speed Automatic",
                    Create = () => {
                        var gears = Gearbox.GearSet(3.59, 2.19, 1.41, 1.00, 0.83);
                        return new AutomaticGearbox(5, gears, 3.06);
                    }
                },
                new GearboxPreset {
                    Name = "Tremec TR-6060 6-Speed Manual",
                    Create = () => {
                        var gears = Gearbox.GearSet(2.97, 2.10, 1.46, 1.00, 0.75, 0.50);
                        return new ManualGearbox(6, gears, 3.91);
                    }
                }
            };
        }

    }
}
