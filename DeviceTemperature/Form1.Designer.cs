namespace DeviceTemperature {
    partial class Temperature {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Temperature));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.temperatureText = new System.Windows.Forms.Label();
            this.smartUpdatetick = new System.Windows.Forms.Timer(this.components);
            this.AirflowTemp = new System.Windows.Forms.Label();
            this.getPCbtn = new System.Windows.Forms.Button();
            this.converBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(4, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(246, 29);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // temperatureText
            // 
            this.temperatureText.AutoSize = true;
            this.temperatureText.Dock = System.Windows.Forms.DockStyle.Right;
            this.temperatureText.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.temperatureText.ForeColor = System.Drawing.Color.BlueViolet;
            this.temperatureText.Location = new System.Drawing.Point(366, 0);
            this.temperatureText.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.temperatureText.Name = "temperatureText";
            this.temperatureText.Size = new System.Drawing.Size(66, 39);
            this.temperatureText.TabIndex = 2;
            this.temperatureText.Text = "0℃";
            // 
            // smartUpdatetick
            // 
            this.smartUpdatetick.Interval = 500;
            this.smartUpdatetick.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AirflowTemp
            // 
            this.AirflowTemp.AutoSize = true;
            this.AirflowTemp.Dock = System.Windows.Forms.DockStyle.Right;
            this.AirflowTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.AirflowTemp.ForeColor = System.Drawing.Color.Crimson;
            this.AirflowTemp.Location = new System.Drawing.Point(300, 0);
            this.AirflowTemp.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.AirflowTemp.Name = "AirflowTemp";
            this.AirflowTemp.Size = new System.Drawing.Size(66, 39);
            this.AirflowTemp.TabIndex = 3;
            this.AirflowTemp.Text = "0℃";
            // 
            // getPCbtn
            // 
            this.getPCbtn.Font = new System.Drawing.Font("jf open 粉圓 1.1", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.getPCbtn.ForeColor = System.Drawing.Color.Yellow;
            this.getPCbtn.Image = ((System.Drawing.Image)(resources.GetObject("getPCbtn.Image")));
            this.getPCbtn.Location = new System.Drawing.Point(4, 42);
            this.getPCbtn.Name = "getPCbtn";
            this.getPCbtn.Size = new System.Drawing.Size(429, 40);
            this.getPCbtn.TabIndex = 4;
            this.getPCbtn.Text = "GET YOUR PC!!";
            this.getPCbtn.UseVisualStyleBackColor = true;
            this.getPCbtn.Click += new System.EventHandler(this.getPCbtn_Click);
            // 
            // converBtn
            // 
            this.converBtn.Location = new System.Drawing.Point(175, 99);
            this.converBtn.Name = "converBtn";
            this.converBtn.Size = new System.Drawing.Size(75, 23);
            this.converBtn.TabIndex = 5;
            this.converBtn.Text = "Conver!";
            this.converBtn.UseVisualStyleBackColor = true;
            this.converBtn.Click += new System.EventHandler(this.converBtn_Click);
            // 
            // Temperature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 127);
            this.Controls.Add(this.converBtn);
            this.Controls.Add(this.getPCbtn);
            this.Controls.Add(this.AirflowTemp);
            this.Controls.Add(this.temperatureText);
            this.Controls.Add(this.comboBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Temperature";
            this.Text = "Temperature";
            this.Load += new System.EventHandler(this.Temperature_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label temperatureText;
        private System.Windows.Forms.Timer smartUpdatetick;
        private System.Windows.Forms.Label AirflowTemp;
        private System.Windows.Forms.Button getPCbtn;
        private System.Windows.Forms.Button converBtn;
    }
}

