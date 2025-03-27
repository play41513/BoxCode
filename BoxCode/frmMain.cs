using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Seagull.BarTender.Print;
using BoxCode.BLL;
using BoxCode.Model;
using System.Threading;

namespace BoxCode
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            BarTenderModel.TB_Part = WorkOrderModel.TB_Part;
            BarTenderModel.PACKING_NUMBER = WorkOrderModel.PACKING_NUMBER;
            BarTenderModel.TOTAL_BOX_COUNT = WorkOrderModel.TOTAL_BOX_COUNT;
            this.AutoScaleMode = AutoScaleMode.Font;
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalExceptionHandler);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            ConstantModel.AppPath = Application.StartupPath;//模板路徑
            InputModel.ListInputValue = new List<string>();
            InputModel.ListReprintValue = new List<string>();
            BoxCodeBLL.CheckPackingStatus();
            // 將 ListInputValue 的每個元素逐一添加到 ListBox 中
            int count = 1;
            foreach (string field in InputModel.ListInputValue)
            {
                listb_InputValue.Items.Add($"[{count.ToString("D6")}]   {field}");
                listb_InputValue.SelectedIndex = listb_InputValue.Items.Count - 1;
                count++;
            }
            plNowBoxCount.Text = "BOX "+BarTenderModel.NOW_BOX_COUNT.ToString()+"  OF  " + BarTenderModel.TOTAL_BOX_COUNT;
            plQuantityPerBox.Text =InputModel.InputValueCount.ToString() + "  OF  " + BarTenderModel.PACKING_NUMBER;
            BarTenderModel.TB_Part = BarTenderModel.PACKING_NUMBER.Contains("80") ? "720210122-03" : "720210722-03";
            plTB_PART.Text = BarTenderModel.TB_Part;
            lbWorkOrderInfo.Text = " 工單號 :   "+ WorkOrderModel.WorkOrder + "      工號: "+ WorkOrderModel.EmployeeID; 
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            BoxCodeBLL.PrintEngineEnable(false,null);
            System.Environment.Exit(0);
        }
        /// <summary>
        /// 判斷主頁面輸入欄位的值
        /// Validate the Input Fields on the Main Page.
        /// </summary>
        private void tb_model_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                UIControlModel.SetTextBoxStatus(tb_model, false, true);
                plResult.FillColor = Color.White;
                if (tb_model.Text.Contains("[LastBox]"))
                {
                    //列印
                    plResult.Text = "列印中...";
                    UI_Update(tb_model.Text, ConstantModel.MESSAGE_PRINTING);
                    if (BoxCodeBLL.Pint_PackingModel(2) == 0)
                    {
                        plResult.Text = "BOX " + BarTenderModel.NOW_BOX_COUNT.ToString() + " 列印完成";
                        plResult.FillColor = Color.Green;
                        UI_Update(tb_model.Text, ConstantModel.MESSAGE_CHANGE_NEXT_BOX);
                        PreViewModel.PreViewPageIndex = 1;
                        Pint_PreView();
                        BarTenderModel.NOW_BOX_COUNT = (Int32.Parse(BarTenderModel.NOW_BOX_COUNT) + 1).ToString();
                    }
                    else
                    {
                        plResult.Text = "列印失敗";
                        plResult.FillColor = Color.Red;
                        UI_Update(tb_model.Text, ConstantModel.ERROR_PRINT_FAIL);
                    }
                }
                else if (tb_model.Text.Contains("[RePrint]"))
                {
                    //列印
                    UI_Update(tb_model.Text, ConstantModel.MESSAGE_ENTER_PAGE_REPRINTING);
                    UIControlModel.SetTextBoxStatus(tb_model, false, true);
                    currentProgress = 0;
                    UpdateProgressBar(1000, 1);
                    tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_LOADING_PANEL);
                    EnmuReprintBoxNumber();
                    tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_REPRINT);
                    e.Handled = true;
                }
                else if (Int32.Parse(BarTenderModel.NOW_BOX_COUNT) > Int32.Parse(WorkOrderModel.TOTAL_BOX_COUNT))
                {
                    plResult.Text = "FAIL";
                    plResult.FillColor = Color.Red;
                    string LogInfo = DateTime.Now.ToString() + "  輸入值 : " + tb_model.Text + " 輸入結果 : ";
                    RTBoxRecord.SelectionColor = Color.Red;
                    LogInfo += "錯誤 箱數已超過總箱數";
                    RTBoxRecord.AppendText(LogInfo + Environment.NewLine);
                    RTBoxRecord.SelectionStart = RTBoxRecord.TextLength;
                    RTBoxRecord.ScrollToCaret();
                }
                else
                {
                    //確認是否符合格式以及範圍內
                    //讀取LOG檔檢查是否重複
                    int result = CheckInputValueBLL.CheckInputValue(tb_model.Text);
                    if (result == 0)
                    {
                        InputModel.ListInputValue.Add(tb_model.Text);
                        InputModel.InputValueCount++;
                        plResult.Text = "輸入成功";
                        if (InputModel.InputValueCount == Int32.Parse(WorkOrderModel.PACKING_NUMBER))
                        {   //數量到達裝箱數，箱數+1
                            //列印
                            plResult.Text = "列印中...";
                            UI_Update(tb_model.Text, ConstantModel.MESSAGE_PRINTING);
                            if (BoxCodeBLL.Pint_PackingModel(2) == 0)                            
                            {
                                plResult.Text = "BOX " + BarTenderModel.NOW_BOX_COUNT.ToString() + " 列印完成";
                                plResult.FillColor = Color.Green;
                                UI_Update(tb_model.Text, ConstantModel.MESSAGE_CHANGE_NEXT_BOX);
                                PreViewModel.PreViewPageIndex = 1;
                                Pint_PreView();
                                BarTenderModel.NOW_BOX_COUNT = (Int32.Parse(BarTenderModel.NOW_BOX_COUNT) + 1).ToString();
                                BoxCodeBLL.WriteLog(tb_model.Text);
                            }
                            else
                            {
                                plResult.Text = "列印失敗";
                                plResult.FillColor = Color.Red;
                                InputModel.InputValueCount--;
                                InputModel.ListInputValue.Remove(tb_model.Text);
                                UI_Update(tb_model.Text, ConstantModel.ERROR_PRINT_FAIL);
                            }

                            
                        }
                        else
                        {
                            if (InputModel.InputValueCount == 1)
                            {
                                listb_InputValue.Items.Clear();
                                UI_Update(tb_model.Text, result);
                            }
                            else
                                UI_Update(tb_model.Text, result);
                            BoxCodeBLL.WriteLog(tb_model.Text);
                            plNowBoxCount.Text = "BOX " + BarTenderModel.NOW_BOX_COUNT.ToString() + "  OF  " + BarTenderModel.TOTAL_BOX_COUNT;
                        }
                    }
                    else
                    {
                        plResult.Text = "輸入失敗";
                        plResult.FillColor = Color.Red;
                        UI_Update(tb_model.Text, result);
                    }
                }
                if(tabControlPanel.SelectedIndex != ConstantModel.PAGE_CONTROL_PREVIEW_PANEL)
                    UIControlModel.SetTextBoxStatus(tb_model,true,true);
            }
        }
        /// <summary>
        /// 主頁面開啟時，啟動loading畫面，並開始啟動印表機引擎
        /// When the Main Page Opens, Display the Loading Screen and Start the Printer Engine.
        /// </summary>
        private void frmMain_Shown(object sender, EventArgs e)
        {
            ShowLoadingPanel(true);
            // 開啟執行緒
            Task.Run(() =>
            {
                int resultCode = BoxCodeBLL.PrintEngineEnable(true, progress =>
                {
                    // 更新進度條，必須在UI執行緒中執行
                    this.Invoke((Action)(() =>
                    {
                        UpdateProgressBar(progress,1);
                    }));
                });
                if (resultCode != 0) // 檢查是否有錯誤碼
                {
                    // 操作失敗，顯示錯誤訊息並關閉應用程式
                    this.Invoke((Action)(() =>
                    {
                        switch(resultCode)
                        {
                            case ConstantModel.ERROR_OPEN_PRINT_FILE_ERROR:
                                MessageBox.Show("操作失敗，列印來源檔無法開啟。\nOperation Failed: Unable to Open the Print Source File"
                                    , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                MessageBox.Show("操作失敗，無法啟用BarTender。\nOperation Failed: Unable to Enable BarTender"
                                    , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                        Application.Exit(); // 關閉應用程式
                    }));
                }
            })
            .ContinueWith(t =>
            {
        // 隱藏Loading Panel
                this.Invoke((Action)(() =>
                {
                    ShowLoadingPanel(false);
                    tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_HOME_PANEL);
                    UIControlModel.SetTextBoxStatus(tb_model, true, true);
                }));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void UI_Update(string InputValue,int ErrorCode)
        {
            string LogInfo = DateTime.Now.ToString()+"  輸入值 : "+ InputValue.PadRight(15) + " 輸入結果 : ";
            switch (ErrorCode)
            {
                case ConstantModel.ERROR_VALUE_IS_DUPLICATED:
                    RTBoxRecord.SelectionColor = Color.Red;
                    LogInfo += "錯誤 輸入值重複";
                    break;
                case ConstantModel.ERROR_MAC_FORMAT_IS_FAIL:
                    RTBoxRecord.SelectionColor = Color.Red;
                    LogInfo += "錯誤 輸入值格式錯誤";
                    break;
                case ConstantModel.ERROR_MAC_IS_OVER_RANGE:
                    RTBoxRecord.SelectionColor = Color.Red;
                    LogInfo += "錯誤 輸入值超出範圍";
                    break;
                case ConstantModel.ERROR_PRINT_FAIL:
                    RTBoxRecord.SelectionColor = Color.Red;
                    LogInfo += "錯誤 列印失敗";
                    break;
                case ConstantModel.MESSAGE_PRINTING:
                    RTBoxRecord.SelectionColor = Color.Black;
                    LogInfo += "列印中...";
                    break;
                case ConstantModel.MESSAGE_CHANGE_NEXT_BOX:
                    RTBoxRecord.SelectionColor = Color.Blue;
                    LogInfo += "成功 此箱已完成 Box "+ BarTenderModel.NOW_BOX_COUNT;
                    listb_InputValue.Items.Add($"[{InputModel.InputValueCount.ToString("D6")}]   {InputValue}");
                    listb_InputValue.SelectedIndex = listb_InputValue.Items.Count - 1;
                    break;
                case ConstantModel.MESSAGE_ENTER_PAGE_REPRINTING:
                    RTBoxRecord.SelectionColor = Color.Blue;
                    LogInfo += "前往貼紙重印頁面";
                    break;
                case ConstantModel.MESSAGE_ENTER_PAGE_HOMEPAGE:
                    RTBoxRecord.SelectionColor = Color.Blue;
                    LogInfo += "返回首頁";
                    break;
                default:
                    RTBoxRecord.SelectionColor = Color.Black;
                    LogInfo += "成功"; 
                    listb_InputValue.Items.Add($"[{InputModel.InputValueCount.ToString("D6")}]   {InputValue}");
                    listb_InputValue.SelectedIndex = listb_InputValue.Items.Count - 1;
                    break;
            }
            RTBoxRecord.AppendText(LogInfo + Environment.NewLine);

            RTBoxRecord.SelectionStart = RTBoxRecord.TextLength;
            RTBoxRecord.ScrollToCaret();
            plQuantityPerBox.Text = InputModel.InputValueCount.ToString() + "  OF  " + BarTenderModel.PACKING_NUMBER;
        }

        private void tb_model_MouseClick(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }
        public void Pint_PreView()
        {
            tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_PREVIEW_PANEL);
            lbPreViewNumber.Text = PreViewModel.PreViewPageIndex.ToString()+"  /  4";

            string modelname = PreViewModel.PreViewPageIndex <= 2 ? "A-example.bmp" : "B-example.bmp";
            LabelFormatDocument format = PreViewModel.PreViewPageIndex <= 2 ? BoxCodeBLL.format1 : BoxCodeBLL.format2;
            if (PreViewModel.PreViewPageIndex <= 2)
            {
                lbTB_Part.ForeColor = Color.DimGray;
                lbInital_Serial.ForeColor = Color.DimGray;
                lbFinalSerial.ForeColor = Color.DimGray;
                TboxTB_Part.Clear();
                TBoxInital_Serial.Clear();
                TBoxFinalSerial.Clear();
                TBoxPackingNumber.Clear();
                UIControlModel.SetPanelStatus(plPreViewSample1,true,true);
                UIControlModel.SetPanelStatus(plPreViewSample2, false, false);
                UIControlModel.SetTextBoxStatus(TboxTB_Part,true,true);
                UIControlModel.SetTextBoxStatus(TBoxInital_Serial, false, true);
                UIControlModel.SetTextBoxStatus(TBoxFinalSerial, false, true);
                UIControlModel.SetTextBoxStatus(TBoxPackingNumber, false, true);
            }
            else
            {
                lbPreViewMACTitle1.ForeColor = Color.DimGray;
                TboxPreViewMAC1.Clear();
                TboxPreViewMAC2.Clear();
                lbPreViewMsg1.Text = "";
                lbPreViewMsg2.Text = "";
                if (InputModel.InputValueCount > 40)
                {
                    lbPreViewMACTitle2.Visible = true;
                    lbPreViewMsg1.Text = "0 / 40";
                    lbPreViewMsg2.Text = "0 / "+(InputModel.InputValueCount-40).ToString();
                    lbPreViewMAC2.Visible = true;
                    UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, true, true);
                    TboxPreViewMAC2.Clear();
                }
                else
                {
                    lbPreViewMACTitle2.Visible = false;
                    lbPreViewMsg1.Text = "0 / "+ InputModel.InputValueCount.ToString();
                    lbPreViewMAC2.Visible = false;
                    UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, false, false);
                }
                UIControlModel.SetPanelStatus(plPreViewSample1, false, false);
                UIControlModel.SetPanelStatus(plPreViewSample2, true, true);
                UIControlModel.SetTextBoxStatus(TboxPreViewMAC1, true, true);
            }
            //預覽貼紙
            Messages message = new Messages();
            string previewPath = Application.StartupPath + $"\\Model\\";
            format.ExportPrintPreviewToFile(previewPath, modelname, ImageType.JPEG,Seagull.BarTender.Print.ColorDepth.ColorDepth256
                ,new Resolution(1190,1684),System.Drawing.Color.White,OverwriteOptions.Overwrite,true,true,out message);
            if (PreViewModel.PreViewPageIndex < 3)
                previewPath = Application.StartupPath + $"\\Model\\A-example1.bmp";
            else
            {
                previewPath = Application.StartupPath + $"\\Model\\B-example1.bmp";
                tableLayoutPanel9_SizeChanged(null, null);
            }
            Bitmap bmp = new Bitmap(previewPath);
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBoxPreViewModel.Image = bmp;
        }
        private void TboxTB_Part_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TextBox tb = (TextBox)sender;
                string lbName = tb.Name.Substring(4, tb.Name.Length - 4);
                Label lb = UIControlModel.FindLabelByName(this, "lb" + lbName);
                lb.Text = lb.Tag.ToString();
                lb.ForeColor = Color.DimGray;
                bool bPass = true;
                if (tb.Text.Contains("[RePrint]"))
                {
                    e.Handled = true;
                    BoxCodeBLL.RePrint(BoxCodeBLL.format1);
                    TboxTB_Part.Clear();
                    TBoxInital_Serial.Clear();
                    TBoxFinalSerial.Clear();
                    TBoxPackingNumber.Clear();
                    UIControlModel.SetTextBoxStatus(TBoxInital_Serial, false, true);
                    UIControlModel.SetTextBoxStatus(TBoxFinalSerial, false, true);
                    UIControlModel.SetTextBoxStatus(TBoxPackingNumber, false, true);
                    UIControlModel.SetTextBoxStatus(TboxTB_Part, true, true);

                }
                else
                {
                    if (tb.Name.Contains("TB_Part"))
                    {
                        if (tb.Text == BarTenderModel.TB_Part)
                        {
                            lb.ForeColor = Color.Green;
                            TBoxInital_Serial.Focus();
                            UIControlModel.SetTextBoxStatus(TBoxInital_Serial, true, true);
                        }
                        else
                            bPass = false;
                    }
                    else if (tb.Name.Contains("Inital_Serial"))
                    {
                        if (tb.Text == BarTenderModel.INIT_MAC)
                        {
                            lb.ForeColor = Color.Green;
                            TBoxFinalSerial.Focus();
                            UIControlModel.SetTextBoxStatus(TBoxFinalSerial, true, true);
                        }
                        else
                            bPass = false;
                    }
                    else if (tb.Name.Contains("FinalSerial"))
                    {
                        if (tb.Text == BarTenderModel.FINAL_MAC)
                        {
                            lb.ForeColor = Color.Green;
                            TBoxPackingNumber.Focus();
                            UIControlModel.SetTextBoxStatus(TBoxPackingNumber, true, true);
                        }
                        else
                            bPass = false;
                    }
                    else if (tb.Name.Contains("PackingNumber"))
                    {
                        if (tb.Text == InputModel.InputValueCount.ToString())
                        {
                            if (PreViewModel.PreViewPageIndex < 4)
                                PreViewModel.PreViewPageIndex++;
                            Pint_PreView();
                        }
                        else
                            bPass = false;
                    }
                    if (bPass == false)
                    {
                        lb.ForeColor = Color.Red;
                        lb.Text += " X";
                        tb.Focus();
                        tb.SelectAll();
                    }
                    else
                        UIControlModel.SetTextBoxStatus(tb, false, true);
                }
            }
        }
        private void TboxPreViewMAC1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {//lbPreViewMACTitle1
                lbPreViewMsg1.Text = TboxPreViewMAC2.Lines.Length.ToString() + " / " + (InputModel.InputValueCount - 40).ToString();
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                if(TboxPreViewMAC1.Text.Contains("[RePrint]"))
                {
                    TboxPreViewMAC1.Clear();
                    e.Handled = true;
                    BoxCodeBLL.RePrint(BoxCodeBLL.format2);
                }
                else if(InputModel.InputValueCount <= 40)
                {
                    lbPreViewMsg1.Text = TboxPreViewMAC1.Lines.Length.ToString()+" / " + (InputModel.InputValueCount).ToString();
                    if (TboxPreViewMAC1.Lines.Length == InputModel.InputValueCount)
                    {
                        string fruitsString = String.Join("\r\n", InputModel.ListInputValue);
                        if (fruitsString == TboxPreViewMAC1.Text)
                        {
                            if (PreViewModel.PreViewPageIndex < 4)
                            {
                                PreViewModel.PreViewPageIndex++;
                                Pint_PreView();
                                e.Handled = true;
                            }
                            else
                            {
                                tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_HOME_PANEL);
                                //換箱清空數據
                                InputModel.InputValueCount = 0;
                                InputModel.ListInputValue.Clear();
                                UIControlModel.SetTextBoxStatus(tb_model, true, true);
                            }
                        }
                        else
                        {
                            string[] textBoxLines = TboxPreViewMAC1.Lines;
                            for (int i = 0; i < InputModel.InputValueCount; i++)
                            {
                                if (textBoxLines[i] != InputModel.ListInputValue[i])
                                {
                                    // 找到不符合的行，這裡是從 1 開始計算行數
                                    int mismatchLineIndex = i + 1;
                                    lbPreViewMsg1.Text += $" -第 {mismatchLineIndex} 行的值不符合";

                                    // 將光標設置到該行
                                    e.Handled = true;
                                    int startIndex = TboxPreViewMAC1.GetFirstCharIndexFromLine(i);
                                    TboxPreViewMAC1.Select(startIndex, textBoxLines[i].Length);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    lbPreViewMsg1.Text = TboxPreViewMAC1.Lines.Length.ToString() + " / 40";
                    if (TboxPreViewMAC1.Lines.Length >= 40)
                    {
                        string fruitsString = String.Join("\r\n", InputModel.ListInputValue.Take(40));
                        if (fruitsString == TboxPreViewMAC1.Text)
                        {
                            lbPreViewMACTitle1.ForeColor = Color.Green;
                            UIControlModel.SetTextBoxStatus(TboxPreViewMAC1,false,true);
                            UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, true, true);
                        }
                        else
                        {
                            string[] textBoxLines = TboxPreViewMAC1.Lines;
                            for (int i = 0; i < 40; i++)
                            {
                                if (textBoxLines[i] != InputModel.ListInputValue[i])
                                {
                                    // 找到不符合的行，這裡是從 1 開始計算行數
                                    int mismatchLineIndex = i + 1;
                                    lbPreViewMsg1.Text += $" -第 {mismatchLineIndex} 行的值不符合";

                                    // 將光標設置到該行+
                                    e.Handled = true;
                                    int startIndex = TboxPreViewMAC1.GetFirstCharIndexFromLine(i);
                                    //TboxPreViewMAC1.Select(startIndex, textBoxLines[i].Length);
                                    TboxPreViewMAC1.SelectAll();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void TboxPreViewMAC2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                lbPreViewMsg2.Text = TboxPreViewMAC2.Lines.Length.ToString() + " / " + (InputModel.InputValueCount - 40).ToString();
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                if (TboxPreViewMAC2.Text.Contains("[RePrint]"))
                {
                    lbPreViewMACTitle1.ForeColor = Color.DimGray;
                    TboxPreViewMAC1.Clear();
                    TboxPreViewMAC2.Clear();
                    e.Handled = true;
                    BoxCodeBLL.RePrint(BoxCodeBLL.format2);
                    UIControlModel.SetTextBoxStatus(TboxPreViewMAC1, true, true);
                    if(InputModel.InputValueCount > 40)
                        UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, false, true);
                }
                else
                {
                    lbPreViewMsg2.Text = TboxPreViewMAC2.Lines.Length.ToString() + " / " + (InputModel.InputValueCount - 40).ToString();
                    if ((TboxPreViewMAC2.Lines.Length == (InputModel.InputValueCount - 40)))
                    {
                        string fruitsString = String.Join("\r\n", InputModel.ListInputValue.Skip(40));
                        if (fruitsString == TboxPreViewMAC2.Text)
                        {
                            if (PreViewModel.PreViewPageIndex < 4)
                            {
                                PreViewModel.PreViewPageIndex++;
                                Pint_PreView();
                                e.Handled = true;
                            }
                            else
                            {
                                tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_HOME_PANEL);
                                //換箱清空數據
                                InputModel.InputValueCount = 0;
                                InputModel.ListInputValue.Clear();
                                UIControlModel.SetTextBoxStatus(tb_model, true, true);
                            }
                        }
                        else
                        {
                            string[] textBoxLines = TboxPreViewMAC2.Lines;
                            for (int i = 0; i < (InputModel.InputValueCount-40); i++)
                            {
                                if (textBoxLines[i] != InputModel.ListInputValue[40+i])
                                {
                                    // 找到不符合的行，這裡是從 1 開始計算行數
                                    int mismatchLineIndex = i + 1;
                                    lbPreViewMsg2.Text += $" -第 {mismatchLineIndex} 行的值不符合";

                                    // 將光標設置到該行+
                                    e.Handled = true;
                                    int startIndex = TboxPreViewMAC2.GetFirstCharIndexFromLine(i);
                                    TboxPreViewMAC2.Select(startIndex, textBoxLines[i].Length);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnLastPrint_Click(object sender, EventArgs e)
        {
            //列印
            plResult.Text = "列印中...";
            UI_Update(tb_model.Text, ConstantModel.MESSAGE_PRINTING);
            UIControlModel.SetTextBoxStatus(tb_model, false, true);
            if (BoxCodeBLL.Pint_PackingModel(2) == 0)
            {
                plResult.Text = "BOX "+ BarTenderModel.NOW_BOX_COUNT.ToString() + " 列印完成";
                plResult.FillColor = Color.Green;
                UI_Update(tb_model.Text, ConstantModel.MESSAGE_CHANGE_NEXT_BOX);
                PreViewModel.PreViewPageIndex = 1;
                Pint_PreView();
                BarTenderModel.NOW_BOX_COUNT = (Int32.Parse(BarTenderModel.NOW_BOX_COUNT) + 1).ToString();
            }
            else
            {
                plResult.Text = "列印失敗";
                plResult.FillColor = Color.Red;
                UI_Update(tb_model.Text, ConstantModel.ERROR_PRINT_FAIL);
                UIControlModel.SetTextBoxStatus(tb_model, true, true);
            }
        }
        private void EnmuReprintBoxNumber()
        {
            string[] boxes = BoxCodeBLL.GetReprintBoxNumber().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string box in boxes)
            {
                cbReprintBoxNumber.Items.Add(box);
            }
            cbReprintBoxNumber.SelectedIndex = 0;
        }
        private void cbReprintBoxNumber_TextChanged(object sender, EventArgs e)
        {
            InputModel.ListReprintValue.Clear();
            listBoxReprint.Items.Clear();
            int iCount = 1;
            string[] contents = BoxCodeBLL.GetBoxContent(cbReprintBoxNumber.Text).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string content in contents)
            {
                InputModel.ListReprintValue.Add(content);
                listBoxReprint.Items.Add($"[{iCount.ToString("D6")}]   {content}");
                iCount++;
            }
            listBoxReprint.SelectedIndex = 0;

            BoxCodeBLL.RePint_PackingModel(cbReprintBoxNumber.Text.Remove(0, 3));

            string modelname = "A-example.bmp";
            Messages message = new Messages();
            string previewPath = Application.StartupPath + $"\\Model\\";
            BoxCodeBLL.format1.ExportPrintPreviewToFile(previewPath, modelname, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth256
                , new Resolution(1190, 1684), System.Drawing.Color.White, OverwriteOptions.Overwrite, true, true, out message);
            previewPath = Application.StartupPath + $"\\Model\\A-example1.bmp";
            Bitmap bmp = new Bitmap(previewPath);
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBoxReprintView1.Image = bmp;

            modelname = "B-example.bmp";
            previewPath = Application.StartupPath + $"\\Model\\";
            BoxCodeBLL.format2.ExportPrintPreviewToFile(previewPath, modelname, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth256
                , new Resolution(1190, 1684), System.Drawing.Color.White, OverwriteOptions.Overwrite, true, true, out message);
            previewPath = Application.StartupPath + $"\\Model\\B-example1.bmp";
            bmp = new Bitmap(previewPath);
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBoxReprintView2.Image = bmp;
        }
        private void btnReprintModel1_Click(object sender, EventArgs e)
        {
            btnReprintModel1.Enabled = false;
            btnReprintModel2.Enabled = false;
            Button btn = (Button)sender;
            if (btn.Name.Contains("1"))
            {
                BoxCodeBLL.RePrint(BoxCodeBLL.format1);
            }
            else
            {
                BoxCodeBLL.RePrint(BoxCodeBLL.format2);
            }
            btnReprintModel1.Enabled = true;
            btnReprintModel2.Enabled = true;
        }
        private void ShowLoadingPanel(bool Show)
        {
            if(Show)
            {
                tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_LOADING_PANEL);
            }
        }
        private int currentProgress = 0; // 追蹤當前的進度
        private CancellationTokenSource cts = new CancellationTokenSource(); // 控制進度更新的取消
        private void UpdateProgressBar(int targetProgress, int durationInSeconds)
        {
            // 如果已有進度更新正在進行，則取消它
            cts.Cancel();
            cts = new CancellationTokenSource();

            double progressIncrement = (double)(targetProgress - currentProgress);
            int totalSteps = Math.Min(1000, durationInSeconds * 100); // 確保不超過1000步
            double progressPerStep = progressIncrement / totalSteps;
            int delay = durationInSeconds * 1000 / totalSteps;

            Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i < totalSteps; i++)
                    {
                        cts.Token.ThrowIfCancellationRequested(); // 如果有新目標，取消當前更新

                        currentProgress += (int)Math.Round(progressPerStep);

                        // 更新UI上的進度條
                        this.Invoke((Action)(() =>
                        {
                            LoadingProcessBar.Value = Math.Min(currentProgress, 1000); // 確保不超過1000
                        }));

                        await Task.Delay(delay);

                        // 如果已達到或超過目標進度，結束迴圈
                        if (currentProgress >= targetProgress)
                            break;
                    }

                    // 確保最終設定為目標進度
                    this.Invoke((Action)(() =>
                    {
                        LoadingProcessBar.Value = targetProgress;
                        currentProgress = targetProgress;
                    }));
                }
                catch (OperationCanceledException)
                {
                    // 處理進度更新被取消的情況
                }
            }, cts.Token);
        }
        private void plReprintRightPage_SizeChanged(object sender, EventArgs e)
        {
            plReprintTitle.Height = plReprintRightPage.Height * 75 / 646;
            UIControlModel.ResizeLabelFontToFitPanel(lbReprintCH, plReprintTitleUP,2);
            UIControlModel.ResizeLabelFontToFitPanel(lbReprintEN, plReprintTitleDOWN,0);
            UIControlModel.ResizePictureBoxToFitPanel(pictureBoxReprintView1, plRepritnPicturePanel1);
            UIControlModel.ResizePictureBoxToFitPanel(pictureBoxReprintView2, plRepritnPicturePanel2);
            plReprintModel1.Height = plReprintModel1.Width;
            plReprintModel2.Height = plReprintModel2.Width;
            plReturnHomePage.Height = plReturnHomePage.Width;
        }

        private void tableLayoutPanel9_SizeChanged(object sender, EventArgs e)
        {
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewNumber, plPreViewNumber, 2);
            plPreViewEN.Height = plPreViewTitle.Height / 3;
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewCH, plPreViewCH, 0);
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewEN, plPreViewEN, 0);
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewMACTitle1, plPreViewMACTitle1,-5);
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewMACTitle2, plPreViewMACTitle2, -5);
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewMsg1, plPreViewMACTitle1, -5);
            UIControlModel.ResizeLabelFontToFitPanel(lbPreViewMsg2, plPreViewMACTitle2, -5);

            UIControlModel.ResizeLabelFontToFitPanel(lbTB_Part, plPreViewTB_Part, 0);
            UIControlModel.ResizeLabelFontToFitPanel(lbInital_Serial, plPreViewInital_Serial, 0);
            UIControlModel.ResizeLabelFontToFitPanel(lbFinalSerial, plPreViewFinalSerial, 0);
            UIControlModel.ResizeLabelFontToFitPanel(lbPackingNumber, plPreViewPackingNumber, 0);//plTboxTB_Part

            TboxTB_Part.Font.Dispose();
            TboxTB_Part.Font = new Font(TboxTB_Part.Font.FontFamily, lbPackingNumber.Font.Size-4, TboxTB_Part.Font.Style);
            TBoxInital_Serial.Font.Dispose();
            TBoxInital_Serial.Font = new Font(TboxTB_Part.Font.FontFamily, lbPackingNumber.Font.Size - 4, TboxTB_Part.Font.Style);
            TBoxFinalSerial.Font.Dispose();
            TBoxFinalSerial.Font = new Font(TboxTB_Part.Font.FontFamily, lbPackingNumber.Font.Size - 4, TboxTB_Part.Font.Style);
            TBoxPackingNumber.Font.Dispose();
            TBoxPackingNumber.Font = new Font(TboxTB_Part.Font.FontFamily, lbPackingNumber.Font.Size - 4, TboxTB_Part.Font.Style);

            UIControlModel.ResizePictureBoxToFitPanel(pictureBoxPreViewModel, plPictureBoxPreViewModel);
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            UI_Update("[RePrint]", ConstantModel.MESSAGE_ENTER_PAGE_REPRINTING);
            UIControlModel.SetTextBoxStatus(tb_model, false, true);
            currentProgress = 0;
            UpdateProgressBar(1000, 1);
            tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_LOADING_PANEL);
            EnmuReprintBoxNumber();
            tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_REPRINT);
        }

        private void tabControlPanel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlPanel.SelectedIndex == ConstantModel.PAGE_CONTROL_LOADING_PANEL)
                LoadingProcessBar.Value = 0;
        }

        private void tabPageMain_Resize(object sender, EventArgs e)
        {
            plQuantityPerBox.Font
                = UIControlModel.ResizeUiPanel(plQuantityPerBox.Font, plQuantityPerBox.Height, plQuantityPerBox.Width, plQuantityPerBox.CreateGraphics(), plQuantityPerBox.Text, 30,-2);
            plNowBoxCount.Font
                = plQuantityPerBox.Font;
            plTB_PART.Font 
                = plQuantityPerBox.Font;
        }

        private void btnReturnHomePage_Click(object sender, EventArgs e)
        {
            tabControlPanel.SelectTab(ConstantModel.PAGE_CONTROL_HOME_PANEL);
            UIControlModel.SetTextBoxStatus(tb_model, true, true);
            UI_Update("[BackHomePage]", ConstantModel.MESSAGE_ENTER_PAGE_HOMEPAGE);
        }
        private static void GlobalExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            // 處理未捕捉的例外
            MessageBox.Show($"An error occurred: {e.Exception.Message}");
        }

        private void pictboxPreViewReprint_Click(object sender, EventArgs e)
        {
            pictboxPreViewReprint.Enabled = false;
            if (PreViewModel.PreViewPageIndex <= 2)
            {
                lbTB_Part.ForeColor = Color.DimGray;
                lbInital_Serial.ForeColor = Color.DimGray;
                lbFinalSerial.ForeColor = Color.DimGray;
                TboxTB_Part.Clear();
                TBoxInital_Serial.Clear();
                TBoxFinalSerial.Clear();
                TBoxPackingNumber.Clear();
                UIControlModel.SetTextBoxStatus(TboxTB_Part, true, true);
                UIControlModel.SetTextBoxStatus(TBoxInital_Serial, false, true);
                UIControlModel.SetTextBoxStatus(TBoxFinalSerial, false, true);
                UIControlModel.SetTextBoxStatus(TBoxPackingNumber, false, true);
                BoxCodeBLL.RePrint(BoxCodeBLL.format1);
            }
            else
            {
                lbPreViewMACTitle1.ForeColor = Color.DimGray;
                TboxPreViewMAC1.Clear();
                TboxPreViewMAC2.Clear();
                lbPreViewMsg1.Text = "";
                lbPreViewMsg2.Text = "";
                if (InputModel.InputValueCount > 40)
                {
                    lbPreViewMsg1.Text = "0 / 40";
                    lbPreViewMsg2.Text = "0 / " + (InputModel.InputValueCount - 40).ToString();
                    lbPreViewMAC2.Visible = true;
                    UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, true, true);

                    TboxPreViewMAC2.Clear();
                }
                else
                {
                    lbPreViewMsg1.Text = "0 / " + InputModel.InputValueCount.ToString();
                    lbPreViewMAC2.Visible = false;
                    UIControlModel.SetTextBoxStatus(TboxPreViewMAC2, false, false);
                }
                UIControlModel.SetPanelStatus(plPreViewSample1, false, false);
                UIControlModel.SetPanelStatus(plPreViewSample2, true, true);
                UIControlModel.SetTextBoxStatus(TboxPreViewMAC1, true, true);

                BoxCodeBLL.RePrint(BoxCodeBLL.format2);
            }
            pictboxPreViewReprint.Enabled = true;
        }
    }
}