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
        // --- 路徑設定 ---
        private static string localLogPath = "C:\\ASMP\\log\\BoxCode\\";
        private static string logFileName = WorkOrderModel.WorkOrder + ".csv";

        // --- NAS 設定 ---
        private static readonly string nasBasePath = @"\\192.168.14.26\swtool\logs\OE-TR01";
        private static readonly string nasUser = "user1234";
        private static readonly string nasPassword = "user1234";
        private static readonly string nasDomain = "192.168.14.26";


        private static string GetActiveFullPath()
        {
            string nasFullPath = Path.Combine(nasBasePath, logFileName);
            string localFullPath = Path.Combine(localLogPath, logFileName);

            try
            {
                if (!Directory.Exists(nasBasePath))
                {
                    using (new NetworkImpersonator(nasUser, nasPassword, nasDomain))
                    {
                        Directory.CreateDirectory(nasBasePath);
                    }
                }
                return nasFullPath; // 成功則回傳 NAS 路徑
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NAS 無法存取，切換至本地模式。錯誤: {ex.Message}");
                Directory.CreateDirectory(localLogPath);
                return localFullPath; // 失敗則回傳本地路徑
            }
        }

        /// <summary>
        /// 同時更新 NAS 與 本地
        /// </summary>
        private static void InternalWriteAllLines(IEnumerable<string> lines)
        {
            try
            {
                // 直接嘗試建立目錄與寫入
                Directory.CreateDirectory(nasBasePath);
                File.WriteAllLines(Path.Combine(nasBasePath, logFileName), lines);
                // 若需詳細追蹤，可解除註解下行
                // NLogDAL.Instance.LogInfo(new NLogModel("NAS 直接寫入成功", "INFO"));
            }
            catch (UnauthorizedAccessException uex)
            {
                NLogDAL.Instance.LogWarning(new NLogModel($"NAS 權限不足，準備切換帳密登入模式。訊息: {uex.Message}", "WARNING"));

                // 若直接寫入失敗，改用帳密
                try
                {
                    using (new NetworkImpersonator(nasUser, nasPassword, nasDomain))
                    {
                        Directory.CreateDirectory(nasBasePath);
                        File.WriteAllLines(Path.Combine(nasBasePath, logFileName), lines);
                        NLogDAL.Instance.LogInfo(new NLogModel("NAS 帳號(NetworkImpersonator)寫入成功", "INFO"));
                    }
                }
                catch (System.ComponentModel.Win32Exception win32Ex)
                {
                    // 捕捉 LogonUser 失敗的 Win32 API 錯誤碼 (例如 1326 帳密錯誤, 1219 多重連線衝突)
                    NLogDAL.Instance.LogError(new NLogModel(
                        $"NetworkImpersonator 登入 NAS 失敗。Win32ErrorCode: {win32Ex.NativeErrorCode}, 訊息: {win32Ex.Message}",
                        "ERROR",
                        win32Ex));
                }
                catch (Exception ex)
                {
                    // 其他未知的寫入失敗 (例如檔案被佔用)
                    NLogDAL.Instance.LogError(new NLogModel(
                        $"NetworkImpersonator 登入後寫入仍失敗。HResult: 0x{ex.HResult:X8}, 訊息: {ex.Message}",
                        "ERROR",
                        ex));
                }
            }
            catch (Exception ex)
            {
                // 捕捉非權限問題的 I/O 錯誤 (如路徑找不到、檔案被 Excel 鎖定)
                // 檔案被佔用的典型 HResult 為 0x80070020
                NLogDAL.Instance.LogError(new NLogModel(
                    $"NAS 直接寫入發生網路或 I/O 錯誤。HResult: 0x{ex.HResult:X8}, 訊息: {ex.Message}",
                    "ERROR",
                    ex));
            }

            // 寫入本地備份
            try
            {
                Directory.CreateDirectory(localLogPath);
                File.WriteAllLines(Path.Combine(localLogPath, logFileName), lines);
            }
            catch (Exception ex)
            {
                NLogDAL.Instance.LogError(new NLogModel($"本地備份寫入失敗。訊息: {ex.Message}", "ERROR", ex));
            }
        }

        /// <summary>
        /// 讀取主要路徑 NAS 優先
        /// </summary>
        private static List<string> InternalReadAllLines()
        {
            string nasFile = Path.Combine(nasBasePath, logFileName);
            string localFile = Path.Combine(localLogPath, logFileName);

            try
            {
                if (File.Exists(nasFile))
                {
                    return File.ReadAllLines(nasFile).ToList();
                }
            }
            catch (UnauthorizedAccessException)
            {
                try
                {
                    using (new NetworkImpersonator(nasUser, nasPassword, nasDomain))
                    {
                        if (File.Exists(nasFile))
                            return File.ReadAllLines(nasFile).ToList();
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NAS 讀取發生非權限錯誤: {ex.Message}");
            }

            // 若 NAS 完全無法存取，嘗試本地端
            if (File.Exists(localFile))
                return File.ReadAllLines(localFile).ToList();

            return new List<string>();
        }

        public static void WriteLog(string logMsg)
        {
            // 讀取現有內容 (NAS 優先)
            var lines = InternalReadAllLines();

            // 清理末尾空行
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            int countInLastLine = 0;
            if (lines.Count > 0 && !lines.Last().StartsWith("Box") && !string.IsNullOrWhiteSpace(lines.Last()))
            {
                countInLastLine = lines.Last().Split(',').Length;
            }

            int LineCount = BarTenderModel.PACKING_NUMBER == "50" ? 5 : 8;

            // 附加資料或換行
            if (countInLastLine > 0 && countInLastLine < LineCount)
            {
                lines[lines.Count - 1] += "," + logMsg;
            }
            else
            {
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
                lines.Add(logMsg);
            }

            // 同步寫入兩端
            InternalWriteAllLines(lines);
            SaveToCSV(logMsg);
        }

        public static bool DeleteLog(string logMsgToDelete)
        {
            var allLines = InternalReadAllLines();
            if (!allLines.Any()) return true;

            try
            {
                int lastBoxStartIndex = allLines.FindLastIndex(line => line.StartsWith("Box"));
                if (lastBoxStartIndex == -1) lastBoxStartIndex = 0;

                var untouchedLines = allLines.GetRange(0, lastBoxStartIndex);
                var lastBoxLines = allLines.GetRange(lastBoxStartIndex, allLines.Count - lastBoxStartIndex);

                var lastBoxMessages = new List<string>();
                foreach (var line in lastBoxLines)
                {
                    if (!line.StartsWith("Box") && !string.IsNullOrWhiteSpace(line))
                        lastBoxMessages.AddRange(line.Split(',').Select(m => m.Trim()));
                }

                int removedCount = lastBoxMessages.RemoveAll(msg => msg.Trim() == logMsgToDelete.Trim());
                if (removedCount == 0) return true;

                var rebuiltLastBoxLines = new List<string>();
                int lineCount = BarTenderModel.PACKING_NUMBER == "50" ? 5 : 8;

                if (lastBoxMessages.Any())
                {
                    var originalHeader = lastBoxLines.FirstOrDefault(l => l.StartsWith("Box"));
                    if (originalHeader != null) rebuiltLastBoxLines.Add(originalHeader);

                    for (int i = 0; i < lastBoxMessages.Count; i += lineCount)
                    {
                        var chunk = lastBoxMessages.Skip(i).Take(lineCount);
                        rebuiltLastBoxLines.Add(string.Join(",", chunk));
                    }
                }

                untouchedLines.AddRange(rebuiltLastBoxLines);
                InternalWriteAllLines(untouchedLines);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刪除失敗: {ex.Message}");
                return false;
            }
        }

        public static void WriteBoxIdToLog(string BoxId, string targetBoxNumber = null)
        {
            var lines = InternalReadAllLines();
            if (lines.Count == 0) return;

            int BoxNumberLineIndex = -1;

            if (string.IsNullOrEmpty(targetBoxNumber))
            {
                // 正常產線模式 從最後一行往前找第一個包含 "Box" 的行
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    if (lines[i].Contains("Box"))
                    {
                        BoxNumberLineIndex = i;
                        break;
                    }
                }

                if (BoxNumberLineIndex >= 0)
                {
                    // 直接在末尾串接 BoxId
                    lines[BoxNumberLineIndex] = string.Concat(lines[BoxNumberLineIndex], "-" + BoxId);
                    InternalWriteAllLines(lines);
                }
            }
            else
            {
                string pattern = $@"^{Regex.Escape(targetBoxNumber.Trim())}(-$|$)";

                for (int i = 0; i < lines.Count; i++)
                {
                    // 去除可能存在的 BoxID 後進行比對，或者直接用正則判斷開頭
                    if (Regex.IsMatch(lines[i].Split('-')[0].Trim(), $@"^{Regex.Escape(targetBoxNumber.Trim())}$", RegexOptions.IgnoreCase))
                    {
                        BoxNumberLineIndex = i;
                        break;
                    }
                }

                if (BoxNumberLineIndex >= 0)
                {
                    string pureBoxHeader = lines[BoxNumberLineIndex].Split('-')[0].Trim();
                    lines[BoxNumberLineIndex] = $"{pureBoxHeader}-{BoxId}";

                    InternalWriteAllLines(lines);
                    Debug.WriteLine($"重工模式：已成功取代 {targetBoxNumber} 的 BoxID 為 {BoxId}");
                }
                else
                {
                    Debug.WriteLine($"重工錯誤：在 Log 中找不到指定的箱號 {targetBoxNumber}");
                }
            }
        }

        public static bool IsValueDuplicated(string valueToCheck)
        {
            var lines = InternalReadAllLines();
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                if (values.Contains(valueToCheck)) return true;
            }
            return false;
        }

        public static void CheckPackingStatus()
        {
            var lines = InternalReadAllLines();

            if (lines.Count > 0)
            {
                while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines.Last()))
                {
                    lines.RemoveAt(lines.Count - 1);
                }

                int InputNumber = 0;
                int iLineLength = lines.Count - 1;
                while (iLineLength >= 0)
                {
                    string lastLine = lines[iLineLength];
                    if (lastLine.Contains("Box"))
                    {
                        int iPos = lastLine.IndexOf('-');
                        BarTenderModel.NOW_BOX_COUNT = iPos > 0 ? lastLine.Substring(3, iPos - 3) : lastLine.Substring(3, lastLine.Length - 3);

                        if (InputNumber >= int.Parse(WorkOrderModel.PACKING_NUMBER))
                        {
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
                    }
                }
            }
            else
            {
                InputModel.ListInputValue.Clear();
                InputModel.InputValueCount = 0;
                BarTenderModel.NOW_BOX_COUNT = "1";
            }
        }
        public static void UpdateLogContent(string oldText, string newText)
        {
            var lines = InternalReadAllLines();

            if (lines.Count > 0)
            {
                var updatedLines = lines.Select(line => line.Replace(oldText, newText)).ToList();

                InternalWriteAllLines(updatedLines);
            }
        }
        public static string GetAllBoxNumber()
        {
            var lines = InternalReadAllLines();
            StringBuilder sb = new StringBuilder();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (Regex.IsMatch(trimmedLine, @"^Box\d+"))
                {
                    string boxNum = trimmedLine;
                    int lastDashIndex = trimmedLine.LastIndexOf('-');

                    if (lastDashIndex > 0)
                    {
                        string firstPart = trimmedLine.Split('-')[0].Trim();
                        string lastPart = trimmedLine.Substring(lastDashIndex + 1).Trim();

                        boxNum = $"{firstPart}-{lastPart}";
                    }

                    sb.AppendLine(boxNum);
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBoxContent(string BoxNumber)
        {
            if (BoxNumber.IndexOf('-') > 0)
                BoxNumber = BoxNumber.Substring(0, BoxNumber.IndexOf('-'));
            var lines = InternalReadAllLines();
            bool isBoxFound = false;
            string boxContent = "";

            foreach (string line in lines)
            {
                string LineBoxNumber = line;
                if (line.IndexOf('-') > 0)
                    LineBoxNumber = line.Substring(0, line.IndexOf('-'));

                if (LineBoxNumber.Trim().Equals(BoxNumber, StringComparison.OrdinalIgnoreCase))
                {
                    if (isBoxFound) break;
                    isBoxFound = true;
                    continue;
                }

                if (isBoxFound)
                {
                    if (LineBoxNumber.StartsWith("Box", StringComparison.OrdinalIgnoreCase)) break;
                    boxContent += (boxContent == "" ? "" : ",") + LineBoxNumber.Trim();
                }
            }
            return boxContent;
        }

        public static bool SaveToCSV(string mac)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string localLogFolder = @"C:\log";
                Directory.CreateDirectory(localLogFolder);

                string logString = $"{{[BOX_{BarTenderModel.NOW_BOX_COUNT}_OF_{BarTenderModel.TOTAL_BOX_COUNT}][COUNT_{InputModel.InputValueCount}_OF_{BarTenderModel.PACKING_NUMBER}]}}";
                string finalResult = "PASS";

                string[] headers = new string[] { "ProductName", "EmployeeID", "Version", "Barcode", "UnitNumber", "Date", "Result", "ErrorCode", "WorkOrder", "SN", "MAC1", "MAC2", "MAC3", "LOG" };
                string csvPath = Path.Combine(localLogFolder, $"[{date}][{WorkOrderModel.WorkOrder}]_{mac}.csv");

                using (StreamWriter writer = new StreamWriter(csvPath))
                {
                    string ProductName = BarTenderModel.TB_Part == "720210722-03" ? "OE-TR01" : "OE-TR02";
                    writer.WriteLine(string.Join(",", headers));
                    writer.WriteLine($"{ProductName},{WorkOrderModel.EmployeeID},,{mac},,{date},{finalResult},,{WorkOrderModel.WorkOrder},{BarTenderModel.TB_Part},{mac},,,{logString}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveToCSV Error: {ex.Message}");
                return false;
            }
        }
    }
}