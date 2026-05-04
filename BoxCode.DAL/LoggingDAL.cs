using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BoxCode.Model;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;

namespace BoxCode.DAL
{
    public class LoggingDAL
    {
        private static String logPath = "C:\\ASMP\\log\\BoxCode\\"; //Log目錄
        private static String logFileName = WorkOrderModel.WorkOrder + ".csv";
        private static String fullPath = Path.Combine(logPath, logFileName);

        // --- NAS Log 設定 (新增) ---
        private static readonly string nasBasePath = @"\\192.168.14.26\swtool\logs\OE-TR01";
        private static readonly string nasUser = "user1234";
        private static readonly string nasPassword = "user1234";
        // 對於非網域環境的 NAS，domain 通常是其主機名稱或 IP
        private static readonly string nasDomain = "192.168.14.26";



        private static void SyncLogToNas(IEnumerable<string> linesToSync)
        {
            string nasFullPath = Path.Combine(nasBasePath, logFileName);

            try
            {
                // --- 嘗試 1: 使用當前使用者權限直接寫入 ---
                // 這種方式適用於執行程式的電腦已登入網域，且該網域帳號有權限的狀況
                Debug.WriteLine("NAS Sync: 嘗試使用當前使用者權限直接存取...");
                Directory.CreateDirectory(nasBasePath);
                File.WriteAllLines(nasFullPath, linesToSync);
                Debug.WriteLine("NAS Sync: 直接存取成功。");
                return; // 成功就結束
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("NAS Sync: 直接存取權限不足。將嘗試使用提供的後備帳密...");
                // 如果沒有提供後備帳密，就直接結束
                if (string.IsNullOrEmpty(nasUser))
                {
                    Debug.WriteLine("NAS Sync: 未設定後備帳密，同步中止。");
                    return;
                }
                // 若權限不足，則繼續執行下面的後備方案
            }
            catch (IOException ioEx)
            {
                // 如果是網路路徑找不到等 I/O 問題，更換帳密也沒用，直接報錯並返回
                Debug.WriteLine($"NAS Sync: 發生網路 I/O 錯誤，NAS 可能已離線。錯誤: {ioEx.Message}");
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NAS Sync: 嘗試直接存取時發生未預期的錯誤。錯誤: {ex.Message}");
                return;
            }

            // --- 嘗試 2: 使用指定的後備帳密進行模擬 (Impersonation) ---
            // 只有在第一次嘗試因「權限不足」失敗時，才會執行到這裡
            try
            {
                Debug.WriteLine("NAS Sync: 嘗試使用後備帳密進行存取...");
                using (new NetworkImpersonator(nasUser, nasPassword, nasDomain))
                {
                    Directory.CreateDirectory(nasBasePath);
                    File.WriteAllLines(nasFullPath, linesToSync);
                    Debug.WriteLine("NAS Sync: 使用後備帳密存取成功。");
                }
            }
            catch (Exception ex)
            {
                // 如果連使用後備帳密都失敗，記錄錯誤
                Debug.WriteLine($"NAS Sync: 使用後備帳密存取失敗。請檢查後備帳密是否正確。錯誤: {ex.Message}");
            }
        }

        public static void WriteLog(String logMsg)
        {
            // 確認資料夾和檔案是否存在，不存在則建立
            Directory.CreateDirectory(logPath);
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
            }

            // 讀取檔案中的內容來檢查最後一行的數據個數

            var lines = new List<string>();
            if (File.Exists(fullPath))
            {
                lines = File.ReadAllLines(fullPath).ToList();
            }
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines.RemoveAt(lines.Count - 1);
            }
            int countInLastLine = 0;
            if (lines.Count > 0 && !lines.Last().StartsWith("Box") && !string.IsNullOrWhiteSpace(lines.Last()))
            {
                countInLastLine = lines.Last().Split(',').Length;
            }
            // 一列多少數據
            int LineCount = BarTenderModel.PACKING_NUMBER == "50" ? 5 : 8;
            // 判斷列是否已經數據已滿，若已滿，則換行
            // 條件A: 最後一行是資料行，且尚未寫滿
            if (countInLastLine > 0 && countInLastLine < LineCount)
            {
                // 直接在最後一行後面附加資料
                lines[lines.Count - 1] += "," + logMsg;
            }
            // 條件B: 需要新增一行來寫入資料 (包含檔案是空的、最後一行是 "Box"、或最後一行資料已滿)
            else
            {
                // 在此處處理新增 "Box" 標頭的邏輯 (保留您原始的判斷)
                if (countInLastLine % LineCount == 0 && countInLastLine != 0)
                {
                    if (InputModel.InputValueCount == int.Parse(WorkOrderModel.PACKING_NUMBER) || InputModel.InputValueCount == 0)
                    {
                        lines.Add("Box" + BarTenderModel.NOW_BOX_COUNT);
                    }
                }

                if (InputModel.InputValueCount == 1)
                {
                    if (lines.Count == 0 || !lines.Last().StartsWith("Box" + BarTenderModel.NOW_BOX_COUNT))
                    {
                        lines.Add("Box" + BarTenderModel.NOW_BOX_COUNT);
                    }
                }

                // 最後，將新的 logMsg 作為一個新行加入
                lines.Add(logMsg);
            }
            File.WriteAllLines(fullPath, lines);
            SyncLogToNas(lines);
            SaveToCSV(logMsg);
        }
        public static bool DeleteLog(string logMsgToDelete)
        {
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("日誌檔案不存在，無需刪除。");
                return true;
            }

            try
            {
                var allLines = File.ReadAllLines(fullPath).ToList();
                if (!allLines.Any())
                {
                    return true;
                }

                // 1. 找到最後一個 "Box" 標頭的索引
                int lastBoxStartIndex = allLines.FindLastIndex(line => line.StartsWith("Box"));

                // 如果找不到任何 Box，則將整個檔案視為一個 Box 處理
                if (lastBoxStartIndex == -1)
                {
                    lastBoxStartIndex = 0;
                }

                // 2. 分離出最後一個 box 的內容和之前不變的內容
                var untouchedLines = allLines.GetRange(0, lastBoxStartIndex);
                var lastBoxLines = allLines.GetRange(lastBoxStartIndex, allLines.Count - lastBoxStartIndex);

                // 3. 從最後一個 box 中提取所有訊息
                var lastBoxMessages = new List<string>();
                foreach (var line in lastBoxLines)
                {
                    // 忽略 Box 標頭和空行
                    if (!line.StartsWith("Box") && !string.IsNullOrWhiteSpace(line))
                    {
                        lastBoxMessages.AddRange(line.Split(',').Select(m => m.Trim()));
                    }
                }

                // 4. 刪除指定的訊息
                int removedCount = lastBoxMessages.RemoveAll(msg => msg.Trim() == logMsgToDelete.Trim());
                if (removedCount == 0)
                {
                    // 在最後一個 box 中沒有找到要刪除的訊息，直接返回
                    return true;
                }

                // 5. 重建最後一個 box 的內容
                var rebuiltLastBoxLines = new List<string>();
                int lineCount = BarTenderModel.PACKING_NUMBER == "50" ? 5 : 8;

                if (lastBoxMessages.Any())
                {
                    // 如果原始的 Box 有標頭，就加回去
                    var originalHeader = lastBoxLines.FirstOrDefault(l => l.StartsWith("Box"));
                    if (originalHeader != null)
                    {
                        rebuiltLastBoxLines.Add(originalHeader);
                    }

                    // 重新排列資料
                    for (int i = 0; i < lastBoxMessages.Count; i += lineCount)
                    {
                        var chunk = lastBoxMessages.Skip(i).Take(lineCount);
                        rebuiltLastBoxLines.Add(string.Join(",", chunk));
                    }
                }
                // 如果 lastBoxMessages 為空，rebuiltLastBoxLines 也會是空的，
                // 這意味著最後一個 box 在刪除後變空了，它將被移除。

                // 6. 合併不變的部分和重建後的部分
                untouchedLines.AddRange(rebuiltLastBoxLines);

                // 7. 將最終內容寫回檔案
                File.WriteAllLines(fullPath, untouchedLines);
                SyncLogToNas(untouchedLines);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刪除日誌時發生錯誤: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 將 BoxId 寫入記錄檔。
        /// </summary>
        /// <param name="BoxId">要寫入記錄檔的 BoxId。</param>
        public static void WriteBoxIdToLog(String BoxId)
        {
            // 檢查記錄檔是否存在，若不存在則直接返回。
            if (!File.Exists(fullPath)) return;

            // 將記錄檔的所有行讀入陣列。
            string[] lines = File.ReadAllLines(fullPath);

            // 創建新的清單來儲存更新後的行
            List<string> updatedLines = new List<string>(lines);

            // 初始化最後一個包含 "Box" 的行索引為 -1
            int BoxNumberLineIndex = -1;

            // 逆向來尋找最後一個包含 "Box" 的行
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                // 檢查當前行是否包含 "Box"。
                if (lines[i].Contains("Box"))
                {
                    // 如果找到，儲存索引並跳出迴圈。
                    BoxNumberLineIndex = i;
                    break;
                }
            }

            // 如果找到包含 "Box" 的行，則在其後插入 BoxId。
            if (BoxNumberLineIndex >= 0)
            {
                // 將 BoxId 附加到包含 "Box" 的行後面。
                updatedLines[BoxNumberLineIndex] = string.Concat(updatedLines[BoxNumberLineIndex], "-"+BoxId);
                // 將更新後的行寫回記錄檔。
                File.WriteAllLines(fullPath, updatedLines);
                SyncLogToNas(updatedLines);
            }
        }
        public static bool SaveToCSV(String mac)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string localLogFolder = @"C:\log";
                Directory.CreateDirectory(localLogFolder);

                // --- 組合 Log 字串 ---
                string logString = $"{{[BOX_{BarTenderModel.NOW_BOX_COUNT}_OF_{BarTenderModel.TOTAL_BOX_COUNT}][COUNT_{InputModel.InputValueCount}_OF_{BarTenderModel.PACKING_NUMBER}]}}";
                string finalResult = "PASS";

                // --- 寫入本地 ---

                string[] headers = new string[] {
                "ProductName", "EmployeeID", "Version", "Barcode", "UnitNumber", "Date",
                "Result", "ErrorCode", "WorkOrder", "SN", "MAC1", "MAC2", "MAC3", "LOG"
            };

                string csvPath = Path.Combine(localLogFolder, $"[{date}][{WorkOrderModel.WorkOrder}]_{mac}.csv");

                using (StreamWriter writer = new StreamWriter(csvPath))
                {
                    string ProductName = BarTenderModel.TB_Part == "720210722-03" ? "OE-TR01": "OE-TR02";
                    writer.WriteLine(string.Join(",", headers));
                    writer.WriteLine($"{ProductName},{WorkOrderModel.EmployeeID}," +
                                     $",{mac}," + // Version 和 Barcode 欄位是空的
                                     $",{date}," + // UnitNumber 欄位是空的
                                     $"{finalResult},," + // ErrorCode 欄位是空的
                                     $"{WorkOrderModel.WorkOrder},{BarTenderModel.TB_Part}," + // SN 欄位使用了 TB_Part
                                     $"{mac},," + // MAC2 和 MAC3 欄位是空的
                                     $",{logString}"); // 最後一個 MAC3 欄位後面的逗號
                }
                return true;
            }
            catch (Exception ex)
            {
                // 建議加入錯誤處理，方便追蹤問題
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
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
            // 將檔案所有行讀取到一個 List 中
            var lines = File.ReadAllLines(fullPath).ToList();

            // 移除所有在檔案結尾的空行或僅包含空白的行
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            // 根據清理後的行數來判斷
            if (lines.Count > 0)
            {
                int InputNumber = 0;
                // 從List 的結尾開始循環
                int iLineLength = lines.Count - 1;
                while (true)
                {
                    string lastLine = lines[iLineLength];
                    if (lastLine.Contains("Box"))
                    {
                        int iPos = lastLine.IndexOf('-');

                        BarTenderModel.NOW_BOX_COUNT
                            = iPos > 0 ? lastLine.Substring(3, iPos - 3) : lastLine.Substring(3, lastLine.Length - 3);

                        if (InputNumber >= int.Parse(WorkOrderModel.PACKING_NUMBER))
                        {//讀到Box段之前 數量已達到該Box滿  故Box數+1
                            InputModel.ListInputValue.Clear();
                            InputModel.InputValueCount = 0;
                            BarTenderModel.NOW_BOX_COUNT = (int.Parse(BarTenderModel.NOW_BOX_COUNT) + 1).ToString();
                        }
                        break;
                    }
                    else
                    {
                        int countInLastLine = lastLine.Split(',').Length;
                        InputNumber += countInLastLine;
                        string[] fields = lastLine.Split(',');
                        InputModel.ListInputValue.InsertRange(0, fields);
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
            else // 如果檔案是空的，或只包含空行
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

            // 匹配 "Box" 開頭的字串
            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, @"^Box\d+"))
                {
                    string boxNumber = line;
                    if (line.IndexOf('-') > 0)
                        boxNumber = line.Substring(0, line.IndexOf('-'));
                    BoxNumber += boxNumber + "\n";
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
                string LineBoxNumber = line;
                if(line.IndexOf('-') > 0)
                    LineBoxNumber = line.Substring(0, line.IndexOf('-'));
                // 找到目標Box
                if (LineBoxNumber.Trim().Equals(BoxNumber, StringComparison.OrdinalIgnoreCase))
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
                    if (LineBoxNumber.StartsWith("Box", StringComparison.OrdinalIgnoreCase))
                    {
                        break; // 碰到下一個Box，結束內容擷取
                    }

                    if(boxContent == "")
                        boxContent += LineBoxNumber.Trim();
                    else
                        boxContent += ","+ LineBoxNumber.Trim();
                }
            }

            // 回傳找到的Box內容，如果沒有找到則回傳空字串
            return boxContent;
        }
    }
}
