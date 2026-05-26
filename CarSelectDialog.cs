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

        public CarSelectDialog() {
            InitializeComponent();

            engineList.DataSource = CarParts.GetEngines();
            gearboxList.DataSource = CarParts.GetGearboxes();
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

    }
}
