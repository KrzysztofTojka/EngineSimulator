namespace EngineSimulator {
    partial class CarSelectDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.startButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.engineList = new System.Windows.Forms.ComboBox();
            this.gearboxList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.startButton.Location = new System.Drawing.Point(413, 180);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(85, 28);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.exitButton.Location = new System.Drawing.Point(322, 180);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(85, 28);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(27, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Engine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(13, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(323, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select car components";
            // 
            // engineList
            // 
            this.engineList.FormattingEnabled = true;
            this.engineList.Location = new System.Drawing.Point(117, 75);
            this.engineList.Name = "engineList";
            this.engineList.Size = new System.Drawing.Size(277, 24);
            this.engineList.TabIndex = 4;
            // 
            // gearboxList
            // 
            this.gearboxList.FormattingEnabled = true;
            this.gearboxList.Items.AddRange(new object[] {
            "Mazda SKYACTIV-G 2.0L"});
            this.gearboxList.Location = new System.Drawing.Point(117, 115);
            this.gearboxList.Name = "gearboxList";
            this.gearboxList.Size = new System.Drawing.Size(277, 24);
            this.gearboxList.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label3.Location = new System.Drawing.Point(27, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Gearbox";
            // 
            // CarSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 217);
            this.Controls.Add(this.gearboxList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.engineList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.startButton);
            this.Name = "CarSelectDialog";
            this.Text = "Select car components";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox engineList;
        private System.Windows.Forms.ComboBox gearboxList;
        private System.Windows.Forms.Label label3;
    }
}