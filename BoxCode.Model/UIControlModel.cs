using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoxCode.Model
{
    public class UIControlModel
    {
        public static Label FindLabelByName(Control parent, string labelName)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label && c.Name == labelName)
                    return (Label)c;
                if (c.HasChildren)
                {
                    Label foundLabel = FindLabelByName(c, labelName);
                    if (foundLabel != null)
                        return foundLabel;
                }
            }
            return null;
        }

        public static void SetPanelStatus(Panel pl, bool Enable, bool Visible)
        {
            if (pl.InvokeRequired)
            {
                pl.Invoke(new Action(() => SetPanelStatus(pl, Enable, Visible)));
            }
            else
            {
                pl.Enabled = Enable;
                pl.Visible = Visible;
            }
        }
        public static void SetTextBoxStatus(TextBox tbox, bool Enable, bool Visible)
        {
            tbox.Enabled = Enable;
            tbox.Visible = Visible;
            if (Enable && Visible)
            {
                tbox.Focus();
                tbox.SelectAll();
            }
        }
        public static void SetRichTextBoxStatus(RichTextBox tbox, bool Enable, bool Visible)
        {
            tbox.Enabled = Enable;
            tbox.Visible = Visible;
            if (Enable && Visible)
            {
                tbox.Focus();
                tbox.SelectAll();
            }
        }
        public static void ResizeLabelFontToFitPanel(Label label, Panel panel, int compensate)
        {
            // 取得 Panel 的寬度和高度
            int panelWidth = panel.ClientSize.Width;
            int panelHeight = panel.ClientSize.Height;

            // 設定最小和最大的字體大小
            float minFontSize = 6f;
            float maxFontSize = 100f;

            // 開始以最大的字體大小進行測試
            float fontSize = maxFontSize;

            // 獲取文本繪製時的大小
            using (Graphics g = panel.CreateGraphics())
            {
                // 進行縮小字體大小測試，直到文字寬度和高度適合 Panel
                while (fontSize > minFontSize)
                {
                    Font testFont = new Font(label.Font.FontFamily, fontSize, label.Font.Style);

                    // 測量文本的大小
                    SizeF textSize = g.MeasureString(label.Text, testFont);

                    // 如果文本寬度和高度都小於等於 Panel 的寬度和高度，則設置字體大小
                    if (textSize.Width <= panelWidth && textSize.Height <= panelHeight)
                    {
                        label.Font = testFont;
                        testFont.Dispose();
                        break;
                    }
                    testFont.Dispose();
                    // 減少字體大小並重試
                    fontSize -= 1f;
                }
            }

            // 最後確保字體大小不會低於最小字體大小
            if (fontSize < minFontSize)
            {
                label.Font = new Font(label.Font.FontFamily, minFontSize, label.Font.Style);
            }
            else 
                label.Font = new Font(label.Font.FontFamily, fontSize + compensate, label.Font.Style); ;
        }
        public static void ResizeTextBoxFontToFitPanel(TextBox textbox, Panel panel, int compensate)
        {
            // 取得 Panel 的寬度和高度
            int panelWidth = panel.ClientSize.Width;
            int panelHeight = panel.ClientSize.Height;

            // 設定最小和最大的字體大小
            float minFontSize = 6f;
            float maxFontSize = 100f;

            // 開始以最大的字體大小進行測試
            float fontSize = maxFontSize;

            // 獲取文本繪製時的大小
            using (Graphics g = panel.CreateGraphics())
            {
                // 進行縮小字體大小測試，直到文字寬度和高度適合 Panel
                while (fontSize > minFontSize)
                {
                    Font testFont = new Font(textbox.Font.FontFamily, fontSize, textbox.Font.Style);

                    // 測量文本的大小
                    SizeF textSize = g.MeasureString(textbox.Text, testFont);

                    // 如果文本寬度和高度都小於等於 Panel 的寬度和高度，則設置字體大小
                    if (textSize.Width <= panelWidth && textSize.Height <= panelHeight)
                    {
                        textbox.Font = testFont;
                        testFont.Dispose();
                        break;
                    }
                    testFont.Dispose();
                    // 減少字體大小並重試
                    fontSize -= 1f;
                }
            }

            // 最後確保字體大小不會低於最小字體大小
            if (fontSize < minFontSize)
            {
                textbox.Font = new Font(textbox.Font.FontFamily, minFontSize, textbox.Font.Style);
            }
            else
                textbox.Font = new Font(textbox.Font.FontFamily, fontSize + compensate, textbox.Font.Style); ;
        }
        public static void ResizePictureBoxToFitPanel(PictureBox picturebox, Panel panel)
        {
            // 計算 17:10 的寬高比
            double targetAspectRatio = 10.0 / 17.0;

            // 計算基於寬度的高度
            int newHeight = (int)(panel.Width / targetAspectRatio);

            // 計算基於高度的寬度
            int newWidth = (int)(panel.Height * targetAspectRatio);

            // 根據 Panel 大小來調整 PictureBox 的寬高
            if (newHeight <= panel.Height)
            {
                // 如果基於寬度計算的高度在範圍內，則使用它
                picturebox.Size = new Size(panel.Width, newHeight);
            }
            else
            {
                // 否則，使用基於高度計算的寬度
                picturebox.Size = new Size(newWidth, panel.Height);
            }

            // 將 PictureBox 居中在 Panel 中（可選）
            picturebox.Location = new Point(
                (panel.Width - picturebox.Width) / 2,
                (panel.Height - picturebox.Height) / 2);
        }
        public static Font ResizeUiPanel(Font panelFont, int panelHeight, int panelWidth, Graphics grap,string Text,int maxsize, int compensate)
        {
            if (panelFont != null)
                panelFont.Dispose();
            // 設定最小和最大的字體大小
            float minFontSize = 6f;
            float maxFontSize = 100f;

            // 開始以最大的字體大小進行測試
            float fontSize = maxFontSize;

            // 獲取文本繪製時的大小
            using (grap)
            {
                // 進行縮小字體大小測試，直到文字寬度和高度適合 Panel
                while (fontSize > minFontSize)
                {
                    Font testFont = new Font(panelFont.FontFamily, fontSize, panelFont.Style);

                    // 測量文本的大小
                    SizeF textSize = grap.MeasureString(Text, testFont);

                    // 如果文本寬度和高度都小於等於 Panel 的寬度和高度，則設置字體大小
                    if (textSize.Width <= panelWidth && textSize.Height <= panelHeight)
                    {
                        panelFont = testFont;
                        testFont.Dispose();
                        break;
                    }
                    testFont.Dispose();
                    // 減少字體大小並重試
                    fontSize -= 1f;
                }
            }

            // 最後確保字體大小不會低於最小字體大小
            if (fontSize < minFontSize)
            {
                panelFont = new Font(panelFont.FontFamily, minFontSize, panelFont.Style);
            }
            else if(fontSize > maxsize)
                panelFont = new Font(panelFont.FontFamily, maxsize, panelFont.Style);
            else
            {
                fontSize = fontSize + compensate;
                panelFont = new Font(panelFont.FontFamily, fontSize, panelFont.Style);
            }
            return panelFont;
        }

    }
}
