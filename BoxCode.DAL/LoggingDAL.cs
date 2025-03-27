using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BoxCode.Model;
using System.Text.RegularExpressions;

namespace BoxCode.DAL
{
    public class LoggingDAL
    {
        private static String logPath = "C:\\ASMP\\log\\BoxCode\\"; //Log目錄
        private static String logFileName = WorkOrderModel.WorkOrder + ".csv";
        private static String fullPath = Path.Combine(logPath, logFileName);
        public static void WriteLog(String logMsg)
        {
            // 確認資料夾和檔案是否存在，不存在則建立
            Directory.CreateDirectory(logPath);
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
            }

            // 讀取檔案中的內容來檢查最後一行的數據個數
            int countInLastLine = 0;
            string[] lines = File.ReadAllLines(fullPath);
            if (lines.Length > 0)
            {
                string lastLine = lines[lines.Length - 1];
                countInLastLine = lastLine.Split(',').Length;
            }
            // 判斷是否已經有八個數據，若已滿，則換行
            using (
                StreamWriter sw = File.AppendText(fullPath))
            {
                if (countInLastLine % 8 == 0 && countInLastLine != 0)
                {
                    sw.WriteLine(); // 換行
                    if (InputModel.InputValueCount == Int32.Parse(WorkOrderModel.PACKING_NUMBER)
                        || InputModel.InputValueCount == 0)
                        sw.WriteLine("Box" + BarTenderModel.NOW_BOX_COUNT);                      
                }
                if (countInLastLine % 8 == 0)
                {
                    if (InputModel.InputValueCount == 1)
                        sw.WriteLine("Box"+BarTenderModel.NOW_BOX_COUNT);
                    sw.Write(logMsg);
                }
                else
                    sw.Write("," + logMsg);

            }
        }
        public static bool IsValueDuplicated(string valueToCheck)
        {
            // 如果檔案不存在，則不可能有重複值
            if (!File.Exists(fullPath))
            {
                return false;
            }

            // 讀取整個檔案的內容
            string[] lines = File.ReadAllLines(fullPath);

            foreach (string line in lines)
            {
                // 檢查每一行是否包含重複的值
                string[] values = line.Split(',');
                if (values.Contains(valueToCheck))
                {
                    return true; // 找到重複的值，回傳 true
                }
            }
            return false; // 沒有找到重複的值，回傳 false
        }
        public static void CheckPackingStatus()
        {
            // 確認資料夾是否存在，不存在則建立
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            // 確認檔案是否存在，不存在則建立
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
                BarTenderModel.NOW_BOX_COUNT = "1";
                InputModel.ListInputValue.Clear();
                InputModel.InputValueCount = 0;
                return;
            }

            // 讀取檔案中的內容來檢查最後一行的數據個數
            int countInLastLine = 0;
            string[] lines = File.ReadAllLines(fullPath);
            if (lines.Length > 0)
            {
                int InputNumber = 0;
                string lastLine;
                int iLineLength = lines.Length - 1;
                while (true)
                {
                    lastLine = lines[iLineLength];
                    if (lastLine.Contains("Box"))
                    {
                        BarTenderModel.NOW_BOX_COUNT = lastLine.Substring(3, lastLine.Length - 3);
                        if (InputNumber == Int32.Parse(WorkOrderModel.PACKING_NUMBER))
                        {//讀到Box段之前 數量已達到該Box滿  故Box數+1
                            InputModel.ListInputValue.Clear();
                            InputModel.InputValueCount = 0;
                            BarTenderModel.NOW_BOX_COUNT = (Int32.Parse(BarTenderModel.NOW_BOX_COUNT) + 1).ToString();
                        }
                        break;
                    }
                    else
                    {
                        countInLastLine = lastLine.Split(',').Length;
                        InputNumber += countInLastLine;
                        string[] fields = lastLine.Split(',');
                        InputModel.ListInputValue.InsertRange(0,fields);
                        InputModel.InputValueCount = InputNumber;
                        iLineLength--;
                        if (iLineLength < 0)
                        {   //讀到最後都沒Box字段 重新創建box1
                            InputModel.ListInputValue.Clear();
                            InputModel.InputValueCount = 0;
                            BarTenderModel.NOW_BOX_COUNT = "1";
                            break;
                        }
                    }
                }
            }
            else
            {
                InputModel.ListInputValue.Clear();
                InputModel.InputValueCount = 0;
                BarTenderModel.NOW_BOX_COUNT = "1";
                return;
            }
        }
        public static string GetAllBoxNumber()
        {
            // 確認檔案是否存在
            if (!File.Exists(fullPath))
            {
                return "";
            }

            // 讀取檔案的每一行
            string[] lines = File.ReadAllLines(fullPath);
            string BoxNumber ="";

            // 使用正則表達式來匹配 "Box" 開頭的字串
            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, @"^Box\d+"))
                {
                    BoxNumber += line + "\n";
                }
            }

            // 移除最後一個換行符號
            if (BoxNumber.EndsWith("\n"))
            {
                BoxNumber = BoxNumber.TrimEnd('\n');
            }

            return BoxNumber;
        }
        public static string GetBoxContent(string BoxNumber)
        {
            // 確認檔案是否存在
            if (!File.Exists(fullPath))
            {
                return "";
            }

            // 讀取檔案的每一行
            string[] lines = File.ReadAllLines(fullPath);
            bool isBoxFound = false;
            string boxContent = "";

            // 尋找匹配的Box
            foreach (string line in lines)
            {
                // 找到目標Box
                if (line.Trim().Equals(BoxNumber, StringComparison.OrdinalIgnoreCase))
                {
                    if (isBoxFound)
                    {
                        break; // 已經找到過一次該Box，結束內容擷取
                    }
                    isBoxFound = true;
                    continue;
                }

                // 如果找到了目標Box，並且下一行不是另一個Box，則將內容添加到boxContent
                if (isBoxFound)
                {
                    if (line.StartsWith("Box", StringComparison.OrdinalIgnoreCase))
                    {
                        break; // 碰到下一個Box，結束內容擷取
                    }

                    if(boxContent == "")
                        boxContent += line.Trim();
                    else
                        boxContent += ","+line.Trim();
                }
            }

            // 回傳找到的Box內容，如果沒有找到則回傳空字串
            return boxContent;
        }
    }
}
