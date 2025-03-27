namespace BoxCode
{
    partial class frmLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.plFrmLoginTop = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbWorkOrder = new System.Windows.Forms.Label();
            this.TBoxWorkOrder = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.TBoxInital_Serial = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TBoxFInal_Serial = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TBoxTotal_Box = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TBoxEmployeeID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TBoxPackingNumber = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.plFrmLoginTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // plFrmLoginTop
            // 
            this.plFrmLoginTop.BackColor = System.Drawing.Color.SteelBlue;
            this.plFrmLoginTop.Controls.Add(this.btnClose);
            this.plFrmLoginTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plFrmLoginTop.Location = new System.Drawing.Point(0, 0);
            this.plFrmLoginTop.Name = "plFrmLoginTop";
            this.plFrmLoginTop.Size = new System.Drawing.Size(828, 40);
            this.plFrmLoginTop.TabIndex = 9;
            this.plFrmLoginTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plFrmLoginTop_MouseDown);
            this.plFrmLoginTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plFrmLoginTop_MouseMove);
            this.plFrmLoginTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plFrmLoginTop_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.SteelBlue;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("新細明體", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(792, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(33, 31);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(684, 492);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 41);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // lbWorkOrder
            // 
            this.lbWorkOrder.AutoSize = true;
            this.lbWorkOrder.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbWorkOrder.ForeColor = System.Drawing.Color.DimGray;
            this.lbWorkOrder.Location = new System.Drawing.Point(42, 205);
            this.lbWorkOrder.Name = "lbWorkOrder";
            this.lbWorkOrder.Size = new System.Drawing.Size(203, 27);
            this.lbWorkOrder.TabIndex = 11;
            this.lbWorkOrder.Text = "工單號 Work Order :";
            // 
            // TBoxWorkOrder
            // 
            this.TBoxWorkOrder.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxWorkOrder.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxWorkOrder.ForeColor = System.Drawing.Color.Black;
            this.TBoxWorkOrder.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxWorkOrder.Location = new System.Drawing.Point(47, 245);
            this.TBoxWorkOrder.Name = "TBoxWorkOrder";
            this.TBoxWorkOrder.Size = new System.Drawing.Size(320, 34);
            this.TBoxWorkOrder.TabIndex = 1;
            this.TBoxWorkOrder.Text = "000-000000000000000";
            this.TBoxWorkOrder.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TBoxWorkOrder_MouseClick);
            this.TBoxWorkOrder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnLogin.Location = new System.Drawing.Point(191, 465);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(436, 37);
            this.btnLogin.TabIndex = 10;
            this.btnLogin.Text = "登    入   L o g i n";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(42, 291);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 27);
            this.label4.TabIndex = 20;
            this.label4.Text = "INITAL SERIAL :";
            // 
            // TBoxInital_Serial
            // 
            this.TBoxInital_Serial.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxInital_Serial.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxInital_Serial.ForeColor = System.Drawing.Color.Black;
            this.TBoxInital_Serial.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxInital_Serial.Location = new System.Drawing.Point(47, 321);
            this.TBoxInital_Serial.Name = "TBoxInital_Serial";
            this.TBoxInital_Serial.Size = new System.Drawing.Size(320, 34);
            this.TBoxInital_Serial.TabIndex = 5;
            this.TBoxInital_Serial.Text = "000000000000";
            this.TBoxInital_Serial.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TBoxWorkOrder_MouseClick);
            this.TBoxInital_Serial.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(48, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 27);
            this.label5.TabIndex = 22;
            this.label5.Text = "FINAL SERIAL :";
            // 
            // TBoxFInal_Serial
            // 
            this.TBoxFInal_Serial.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxFInal_Serial.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxFInal_Serial.ForeColor = System.Drawing.Color.Black;
            this.TBoxFInal_Serial.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxFInal_Serial.Location = new System.Drawing.Point(47, 399);
            this.TBoxFInal_Serial.Name = "TBoxFInal_Serial";
            this.TBoxFInal_Serial.Size = new System.Drawing.Size(320, 34);
            this.TBoxFInal_Serial.TabIndex = 6;
            this.TBoxFInal_Serial.Text = "FFFFFFFFFFFF";
            this.TBoxFInal_Serial.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TBoxWorkOrder_MouseClick);
            this.TBoxFInal_Serial.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(437, 291);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(241, 27);
            this.label6.TabIndex = 24;
            this.label6.Text = "總箱數 Total Box Count :";
            // 
            // TBoxTotal_Box
            // 
            this.TBoxTotal_Box.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxTotal_Box.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxTotal_Box.ForeColor = System.Drawing.Color.Black;
            this.TBoxTotal_Box.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxTotal_Box.Location = new System.Drawing.Point(442, 321);
            this.TBoxTotal_Box.Name = "TBoxTotal_Box";
            this.TBoxTotal_Box.Size = new System.Drawing.Size(320, 34);
            this.TBoxTotal_Box.TabIndex = 8;
            this.TBoxTotal_Box.Text = "100";
            this.TBoxTotal_Box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(437, 369);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(190, 27);
            this.label7.TabIndex = 26;
            this.label7.Text = "工號 Employee ID :";
            // 
            // TBoxEmployeeID
            // 
            this.TBoxEmployeeID.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxEmployeeID.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxEmployeeID.ForeColor = System.Drawing.Color.Black;
            this.TBoxEmployeeID.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxEmployeeID.Location = new System.Drawing.Point(442, 399);
            this.TBoxEmployeeID.Name = "TBoxEmployeeID";
            this.TBoxEmployeeID.Size = new System.Drawing.Size(320, 34);
            this.TBoxEmployeeID.TabIndex = 9;
            this.TBoxEmployeeID.Text = "T00000";
            this.TBoxEmployeeID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.Location = new System.Drawing.Point(437, 205);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(256, 27);
            this.label8.TabIndex = 28;
            this.label8.Text = "裝箱數 Quantity per Box  :";
            // 
            // TBoxPackingNumber
            // 
            this.TBoxPackingNumber.BackColor = System.Drawing.Color.DarkGray;
            this.TBoxPackingNumber.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TBoxPackingNumber.ForeColor = System.Drawing.Color.Black;
            this.TBoxPackingNumber.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.TBoxPackingNumber.Location = new System.Drawing.Point(442, 245);
            this.TBoxPackingNumber.Name = "TBoxPackingNumber";
            this.TBoxPackingNumber.Size = new System.Drawing.Size(320, 34);
            this.TBoxPackingNumber.TabIndex = 7;
            this.TBoxPackingNumber.Text = "80";
            this.TBoxPackingNumber.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TBoxWorkOrder_MouseClick);
            this.TBoxPackingNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBoxWorkOrder_KeyPress);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(102, 67);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(591, 125);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 29;
            this.pictureBox2.TabStop = false;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(828, 531);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.TBoxPackingNumber);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TBoxEmployeeID);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TBoxTotal_Box);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TBoxFInal_Serial);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TBoxInital_Serial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.TBoxWorkOrder);
            this.Controls.Add(this.lbWorkOrder);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.plFrmLoginTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmLogin";
            this.Shown += new System.EventHandler(this.frmLogin_Shown);
            this.plFrmLoginTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel plFrmLoginTop;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbWorkOrder;
        private System.Windows.Forms.TextBox TBoxWorkOrder;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TBoxInital_Serial;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TBoxFInal_Serial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TBoxTotal_Box;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TBoxEmployeeID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TBoxPackingNumber;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}