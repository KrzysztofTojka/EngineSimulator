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

        private readonly CarPreset customPreset = new CarPreset { Name = "Custom" };

        private bool updatingFromPreset = false;

        public CarSelectDialog() {
            InitializeComponent();

            cars = CarParts.GetCars();
            engines = CarParts.GetEngines();
            gearboxes = CarParts.GetGearboxes();

            cars.Add(customPreset);

            carList.DataSource = cars;
            engineList.DataSource = engines;
            gearboxList.DataSource = gearboxes;

            engineList.SelectedIndexChanged += component_SelectedIndexChanged;
            gearboxList.SelectedIndexChanged += component_SelectedIndexChanged;

            carList_SelectedIndexChanged(null, null);
        }

        private void startButton_Click(object sender, EventArgs e) {
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
                carList.SelectedItem = customPreset;
            }
        }

        private void carList_SelectedIndexChanged(object sender, EventArgs e) {
            if (carList.SelectedItem is CarPreset carPreset && carPreset != customPreset) {
                updatingFromPreset = true;
                
                EnginePreset enginePreset = engines.FirstOrDefault(eng =>  eng.Name == carPreset.EnginePresetName);
                if (enginePreset != null) {
                    engineList.SelectedItem = enginePreset;
                }

                GearboxPreset gearboxPreset = gearboxes.FirstOrDefault(gb => gb.Name == carPreset.GearboxPresetName);
                if (gearboxPreset != null) {
                    gearboxList.SelectedItem = gearboxPreset;
                }

                updatingFromPreset = false;
            }
        }



    }
}
