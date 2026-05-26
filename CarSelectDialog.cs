using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineSimulator {
    public partial class CarSelectDialog : Form {

        private Car selectedCar;

        private List<CarPreset> cars;
        private List<EnginePreset> engines;
        private List<GearboxPreset> gearboxes;

        private readonly CarPreset customPreset = new CarPreset { Name = "Custom", Weight = 1450, WheelRadius = 0.316 };

        private bool updatingFromPreset = false;

        public CarSelectDialog() {
            InitializeComponent();

            cars = CarDatabase.GetCars();
            engines = CarDatabase.GetEngines();
            gearboxes = CarDatabase.GetGearboxes();

            cars.Add(customPreset);

            carList.DataSource = cars;
            engineList.DataSource = engines;
            gearboxList.DataSource = gearboxes;

            engineList.SelectedIndexChanged += component_SelectedIndexChanged;
            gearboxList.SelectedIndexChanged += component_SelectedIndexChanged;

            carList_SelectedIndexChanged(null, null);
        }

        private void startButton_Click(object sender, EventArgs e) {
            CarPreset carPreset = carList.SelectedItem as CarPreset;
            EnginePreset enginePreset = engineList.SelectedItem as EnginePreset;
            GearboxPreset gearboxPreset = gearboxList.SelectedItem as GearboxPreset;

            if (enginePreset == null || gearboxPreset == null) {
                MessageBox.Show("Select engine and gearbox!");
                return;
            }

            selectedCar = new Car();

            Engine engine = enginePreset.Create();
            Gearbox gearbox = gearboxPreset.Create();

            selectedCar.SetEngine(engine);
            selectedCar.SetGearbox(gearbox);

            selectedCar.SetWeight(carPreset.Weight);
            selectedCar.SetWheelRadius(carPreset.WheelRadius);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public Car GetSelectedCar() {
            return selectedCar;
        }

        private void component_SelectedIndexChanged(object sender, EventArgs e) {
            if (updatingFromPreset) {
                return;
            }
            
            if (carList.SelectedItem is CarPreset carPreset && carPreset != customPreset) {
                EnginePreset selectedEngine = engineList.SelectedItem as EnginePreset;
                GearboxPreset selectedGearbox = gearboxList.SelectedItem as GearboxPreset;

                bool isEngineValid = selectedEngine != null && carPreset.Engines.Contains(selectedEngine.Name);
                bool isGearboxValid = selectedGearbox != null && carPreset.Gearbox == selectedGearbox.Name;

                if (!isEngineValid || !isGearboxValid) {
                    carList.SelectedItem = customPreset;
                }
            }
        }

        private void carList_SelectedIndexChanged(object sender, EventArgs e) {
            if (carList.SelectedItem is CarPreset carPreset && carPreset != customPreset) {
                EnginePreset currentEngine = engineList.SelectedItem as EnginePreset;

                updatingFromPreset = true;
                
                if (!carPreset.Engines.Contains(currentEngine.Name)) {
                    EnginePreset enginePreset = engines.FirstOrDefault(eng => eng.Name == carPreset.Engines.First());
                    if (enginePreset != null) {
                        engineList.SelectedItem = enginePreset;
                    }
                }

                GearboxPreset gearboxPreset = gearboxes.FirstOrDefault(gb => gb.Name == carPreset.Gearbox);
                if (gearboxPreset != null) {
                    gearboxList.SelectedItem = gearboxPreset;
                }

                updatingFromPreset = false;
            }
        }



    }
}
