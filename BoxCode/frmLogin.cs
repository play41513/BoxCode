using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoxCode.Model;

namespace BoxCode
{
    public partial class frmLogin : Form
    {
        protected Point MousePt; // 紀錄移動前和移動後的滑鼠座標
        protected bool canMove = false; // 紀錄表單可否被拖曳
        protected int LeftVar = 0, TopVar = 0; // 紀錄form的移動量
        public frmLogin()
        {
            InitializeComponent();
        }

        private void plFrmLoginTop_MouseDown(object sender, MouseEventArgs e)
        {
            // 設定滑鼠移動前的座標
            MousePt = new Point(e.X, e.Y);
            canMove = true; // 如果按下滑鼠左鍵時 可以移動表單
        }

        private void plFrmLoginTop_MouseMove(object sender, MouseEventArgs e)
        {
            // 拖曳form
            if (canMove)
            {
                this.Left += e.X - MousePt.X;
                this.Top += e.Y - MousePt.Y;
            }
        }

        private void plFrmLoginTop_MouseUp(object sender, MouseEventArgs e)
        {
            canMove = false; // 如果放開滑鼠左鍵時 暫停移動表單
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            WorkOrderModel.WorkOrder        = TBoxWorkOrder.Text;
            WorkOrderModel.INIT_MAC         = TBoxInital_Serial.Text;
            WorkOrderModel.FINAL_MAC        = TBoxFInal_Serial.Text;
            WorkOrderModel.TOTAL_BOX_COUNT  = TBoxTotal_Box.Text;
            WorkOrderModel.PACKING_NUMBER   = TBoxPackingNumber.Text;
            WorkOrderModel.EmployeeID       = TBoxEmployeeID.Text;

            frmMain FrmMain = new frmMain();
            FrmMain.Shown+= (s, args) =>
            {
                // 在主表單載入完成後，關閉Loading表單
                //loadingForm.Close();
            };
            FrmMain.Show();
            this.Hide();
        }

        private void frmLogin_Shown(object sender, EventArgs e)
        {
            TBoxWorkOrder.Focus();
            TBoxWorkOrder.SelectAll();
        }

        private void TBoxWorkOrder_MouseClick(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void TBoxWorkOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.KeyChar == 13)
            {
                if (tb.Name.Contains("WorkOrder"))
                    TBoxInital_Serial.Focus();
                else if (tb.Name.Contains("Inital_Serial"))
                    TBoxFInal_Serial.Focus();
                else if (tb.Name.Contains("FInal_Serial"))
                    TBoxPackingNumber.Focus();
                else if (tb.Name.Contains("PackingNumber"))
                    TBoxTotal_Box.Focus();
                else if (tb.Name.Contains("Total_Box"))
                    TBoxEmployeeID.Focus();
                else if (tb.Name.Contains("EmployeeID"))
                    btnLogin.PerformClick();
            }
        }
    }
}
