using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxCode.Model;
using BoxCode.DAL;
using System.IO;
using Seagull.BarTender.Print;
using System.Security.Cryptography;
using System.Threading;


namespace BoxCode.BLL
{
    public class BoxCodeBLL
    {
        public static bool bPrint = true;
        static readonly Engine engine = new Engine(); //打印機引擎
        public static LabelFormatDocument format1; //獲取 模板內容
        public static LabelFormatDocument format2; //獲取 模板內容

        public static int Pint_PackingModel(int printnum)
        {
            //取得該箱最小最大號值
            BoxCodeBLL.GetMinMaxValue(InputModel.ListInputValue);

            if (BarTenderModel.PACKING_NUMBER == "24")
                return Pint_24PackingModel(printnum);
            if(BarTenderModel.PACKING_NUMBER == "50")
                return Pint_50PackingModel(printnum);
            return Pint_80PackingModel(printnum);
        }
        public static int PrintEngineEnable(bool Enable, Action<int> reportProgress)
        {
            if (Enable)
            {
                NLogDAL.Instance.LogInfo(new NLogModel("Control Print Engine", "INFO"));
                try
                {
                    reportProgress?.Invoke(500); // 報告大約10%的進度
                    engine.Start();
                    if (engine.IsAlive)
                    {
                        try
                        {
                            if (BarTenderModel.PACKING_NUMBER == "24")
                            {
                                reportProgress?.Invoke(900); // 報告大約90%的進度
                                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72吋外箱貼-橫版.btw");
                                reportProgress?.Invoke(1000); // 報告100%的進度                    
                                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72外箱貼MAC_橫版.btw");
                            }
                            else if(BarTenderModel.PACKING_NUMBER == "50")
                            {
                                reportProgress?.Invoke(1000); // 報告100%的進度                    
                                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\TIVE貨櫃鎖-外箱貼MAC.btw");
                            }
                            else
                            {
                                reportProgress?.Invoke(900);
                                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺12吋外箱貼-橫版.btw");
                                reportProgress?.Invoke(1000);
                                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺外12箱貼MAC_橫版.btw");
                            }
                        }
                        catch
                        {
                            return ConstantModel.ERROR_OPEN_PRINT_FILE_ERROR;
                        }
                    }
                    else
                        return ConstantModel.ERROR_OPEN_PRINT_ERROR;
                }
                catch (Exception ex)
                {
                    NLogDAL.Instance.LogError(new NLogModel("Failed to Start Print Engine", "ERROR",ex));
                    return ConstantModel.ERROR_OPEN_PRINT_ERROR;
                }
            }
            else
            {
                NLogDAL.Instance.LogInfo(new NLogModel("Close Print Engine", "INFO"));
                try
                {
                    engine.Stop();
                    engine.Dispose();
                }
                catch (Exception ex)
                {
                    NLogDAL.Instance.LogError(new NLogModel("Failed to Close Print Engine", "ERROR", ex));
                }
            }
            NLogDAL.Instance.LogInfo(new NLogModel("Control Print Engine Finish", "INFO"));
            return 0;
        }
        public static void RePrint(LabelFormatDocument format)
        {
            NLogDAL.Instance.LogInfo(new NLogModel("[CheckLabel Page] Start to RePrint", "INFO"));
            if (bPrint) 
                format.Print();
        }
        public static int Pint_24PackingModel(int printnum)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72吋外箱貼-橫版.btw");
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72外箱貼MAC_橫版.btw");
            }

            format1.SubStrings["TB_PART"].Value = BarTenderModel.TB_Part;
            format1.SubStrings["MAC序號首"].Value   = BarTenderModel.INIT_MAC;
            format1.SubStrings["MAC序號尾"].Value   = BarTenderModel.FINAL_MAC;
            format1.SubStrings["總箱數"].Value = BarTenderModel.TOTAL_BOX_COUNT;
            format1.SubStrings["目前箱數"].Value    = BarTenderModel.NOW_BOX_COUNT;
            format1.SubStrings["裝箱數"].Value = InputModel.InputValueCount.ToString();//這邊使用實際的裝箱數
            NLogDAL.Instance.LogInfo(new NLogModel("Start to Print 1", "INFO"));
            Result rel = 0;
            for (int i = 0; i < printnum; i++)
            {
                if (bPrint)
                    rel = format1.Print();//列印
                else
                    rel = Result.Success;
                if (rel != Result.Success)
                    break;
            }
            if (rel == Result.Success)
            {
                string fruitsString = String.Join("\r\n", InputModel.ListInputValue);
                format2.SubStrings["MAC"].Value = fruitsString;
                NLogDAL.Instance.LogInfo(new NLogModel("Start to Print 2", "INFO"));
                for (int i = 0; i < printnum; i++)
                {
                    if (bPrint)
                        rel = format2.Print();//列印
                    else
                        rel = Result.Success;
                    if (rel != Result.Success)
                        break;
                }
                if (rel != Result.Success)
                {
                    NLogDAL.Instance.LogWarning(new NLogModel("Failed to Print 2", "WARNING"));
                    return ConstantModel.ERROR_PRINT_FAIL;
                }
            }
            else
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to Print 1", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }

            return 0;
        }
        public static int Pint_80PackingModel(int printnum)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺12吋外箱貼-橫版.btw");
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺外12箱貼MAC_橫版.btw");
            }

            format1.SubStrings["TB_PART"].Value = BarTenderModel.TB_Part;
            format1.SubStrings["MAC序號首"].Value = BarTenderModel.INIT_MAC;
            format1.SubStrings["MAC序號尾"].Value = BarTenderModel.FINAL_MAC;
            format1.SubStrings["總箱數"].Value = BarTenderModel.TOTAL_BOX_COUNT;
            format1.SubStrings["目前箱數"].Value = BarTenderModel.NOW_BOX_COUNT;
            format1.SubStrings["裝箱數"].Value = InputModel.InputValueCount.ToString();//這邊使用實際的裝箱數
            NLogDAL.Instance.LogInfo(new NLogModel("Start to Print 1", "INFO"));
            Result rel = 0;
            for (int i = 0; i < printnum; i++)
            {
                if (bPrint)
                    rel = format1.Print();//列印
                else
                    rel = Result.Success;
                if (rel != Result.Success)
                    break;
            }
            if (rel == Result.Success)
            {
                // 確保前 40 筆資料放入 MAC
                List<string> first40 = InputModel.ListInputValue.Take(40).ToList();
                string fruitsStringFirst = String.Join("\r\n", first40);
                format2.SubStrings["MAC序號1"].Value = fruitsStringFirst;

                // 取出 40 筆之後的資料放入 MAC2
                if (InputModel.ListInputValue.Count > 40)
                {
                    List<string> after40 = InputModel.ListInputValue.Skip(40).ToList();
                    string fruitsStringAfter = String.Join("\r\n", after40);
                    format2.SubStrings["MAC序號2"].Value = fruitsStringAfter;
                }
                else
                    format2.SubStrings["MAC序號2"].Value = "";
                NLogDAL.Instance.LogInfo(new NLogModel("Start to Print 2", "INFO"));
                for (int i = 0; i < printnum; i++)
                {
                    if (bPrint)
                        rel = format2.Print();//列印
                    else
                        rel = Result.Success;
                    if (rel != Result.Success)
                        break;
                }
                if (rel != Result.Success)
                {
                    NLogDAL.Instance.LogWarning(new NLogModel("Failed to Print 2", "WARNING"));
                    return ConstantModel.ERROR_PRINT_FAIL;
                }
            }
            else
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to Print 1", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }

            return 0;
        }
        public static int Pint_50PackingModel(int printnum)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\TIVE貨櫃鎖-外箱貼MAC.btw");
            }

            Result rel = 0;

            // 前 25 筆資料放入 MAC
            List<string> PreMACList25 = InputModel.ListInputValue.Take(25).ToList();
            string PreMACValue = String.Join("\r\n", PreMACList25);
            format2.SubStrings["MAC序號1"].Value = PreMACValue;

            // 取出 25 筆之後的資料放入 MAC2
            if (InputModel.ListInputValue.Count > 25)
            {
                List<string> after25 = InputModel.ListInputValue.Skip(25).ToList();
                string fruitsStringAfter = String.Join("\r\n", after25);
                format2.SubStrings["MAC序號2"].Value = fruitsStringAfter;
            }
            else
                format2.SubStrings["MAC序號2"].Value = "";

            NLogDAL.Instance.LogInfo(new NLogModel("Start to Print 2", "INFO"));
            for (int i = 0; i < printnum; i++)
            {
                if (bPrint)
                    rel = format2.Print();//列印
                else
                    rel = Result.Success;
                if (rel != Result.Success)
                    break;
            }
            if (rel != Result.Success)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to Print 2", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }

            return 0;
        }
        public static int RePint_PackingModel(string BoxNumber)
        {
            //取得該箱最小最大號值
            BoxCodeBLL.GetMinMaxValue(InputModel.ListReprintValue);

            if (BarTenderModel.PACKING_NUMBER == "24")
                return ReprintView24PackingModel(BoxNumber);
            else if(BarTenderModel.PACKING_NUMBER == "50")
                return ReprintView50PackingModel(BoxNumber);
            return ReprintView80PackingModel(BoxNumber);
        }
        public static int ReprintView24PackingModel(string BoxNumber)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72吋外箱貼-橫版.btw");
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺72外箱貼MAC_橫版.btw");
            }


            format1.SubStrings["TB_PART"].Value = BarTenderModel.TB_Part;
            format1.SubStrings["MAC序號首"].Value = BarTenderModel.INIT_MAC;
            format1.SubStrings["MAC序號尾"].Value = BarTenderModel.FINAL_MAC;
            format1.SubStrings["總箱數"].Value = BarTenderModel.TOTAL_BOX_COUNT;
            format1.SubStrings["目前箱數"].Value = BoxNumber;
            format1.SubStrings["裝箱數"].Value = InputModel.ListReprintValue.Count.ToString();//這邊使用實際的裝箱數
            NLogDAL.Instance.LogInfo(new NLogModel("Start to ReView 1 Box"+ BoxNumber, "INFO"));
            Result rel;
            if (bPrint)
                rel = format1.Print();//列印
            else
                rel = Result.Success;
            if (rel == Result.Success)
            {
                string fruitsString = String.Join("\n", InputModel.ListInputValue);
                format2.SubStrings["MAC"].Value = fruitsString;
                NLogDAL.Instance.LogInfo(new NLogModel("Start to ReView 2 Box" + BoxNumber, "INFO"));
                if (bPrint)
                    rel = format2.Print();//列印
                else
                    rel = Result.Success;
                if (rel != Result.Success)
                {
                    NLogDAL.Instance.LogWarning(new NLogModel("Failed to ReView 2", "WARNING"));
                    return ConstantModel.ERROR_PRINT_FAIL;
                }
            }
            else
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to ReView 1", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }
            return 0;
        }
        public static int ReprintView80PackingModel(string BoxNumber)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format1 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺12吋外箱貼-橫版.btw");
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺外12箱貼MAC_橫版.btw");
            }

            format1.SubStrings["TB_PART"].Value = BarTenderModel.TB_Part;
            format1.SubStrings["MAC序號首"].Value = BarTenderModel.INIT_MAC;
            format1.SubStrings["MAC序號尾"].Value = BarTenderModel.FINAL_MAC;
            format1.SubStrings["總箱數"].Value = BarTenderModel.TOTAL_BOX_COUNT;
            format1.SubStrings["目前箱數"].Value = BoxNumber;
            format1.SubStrings["裝箱數"].Value = InputModel.ListReprintValue.Count.ToString();//這邊使用實際的裝箱數
            NLogDAL.Instance.LogInfo(new NLogModel("Start to ReView 1 Box" + BoxNumber, "INFO"));
            Result rel;
            if (bPrint)
                rel = format1.Print();//列印
            else
                rel = Result.Success;
            if (rel == Result.Success)
            {
                // 前 40 筆資料放入 MAC
                List<string> first40 = InputModel.ListReprintValue.Take(40).ToList();
                string fruitsStringFirst = String.Join("\r\n", first40);
                format2.SubStrings["MAC序號1"].Value = fruitsStringFirst;

                // 取出 40 筆之後的資料放入 MAC2
                if (InputModel.ListReprintValue.Count > 40)
                {
                    List<string> after40 = InputModel.ListReprintValue.Skip(40).ToList();
                    string fruitsStringAfter = String.Join("\r\n", after40);
                    format2.SubStrings["MAC序號2"].Value = fruitsStringAfter;
                }
                else
                    format2.SubStrings["MAC序號2"].Value = "";
                NLogDAL.Instance.LogInfo(new NLogModel("Start to ReView 2 Box" + BoxNumber, "INFO"));
                if (bPrint)
                    rel = format2.Print();//列印
                else
                    rel = Result.Success;
                if (rel != Result.Success)
                {
                    NLogDAL.Instance.LogWarning(new NLogModel("Failed to ReView 2", "WARNING"));
                    return ConstantModel.ERROR_PRINT_FAIL;
                }
            }
            else
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to ReView 1", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }
            return 0;
        }
        public static int ReprintView50PackingModel(string BoxNumber)
        {
            if (engine.IsAlive == false)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Print Engine Is Not Alive,Reset Engine", "WARNING"));
                engine.Start();
                format2 = engine.Documents.Open(ConstantModel.AppPath + $"\\Model\\準旺外12箱貼MAC_橫版.btw");
            }

            Result rel;
 
            // 確保前 25 筆資料放入 MAC
            List<string> first25 = InputModel.ListReprintValue.Take(25).ToList();
            string fruitsStringFirst = String.Join("\r\n", first25);
            format2.SubStrings["MAC序號1"].Value = fruitsStringFirst;

            // 取出 40 筆之後的資料放入 MAC2
            if (InputModel.ListReprintValue.Count > 25)
            {
                List<string> after25 = InputModel.ListReprintValue.Skip(25).ToList();
                string fruitsStringAfter = String.Join("\r\n", after25);
                format2.SubStrings["MAC序號2"].Value = fruitsStringAfter;
            }
            else
                format2.SubStrings["MAC序號2"].Value = "";
            NLogDAL.Instance.LogInfo(new NLogModel("Start to ReView 2 Box" + BoxNumber, "INFO"));
            if (bPrint)
                rel = format2.Print();//列印
            else
                rel = Result.Success;
            if (rel != Result.Success)
            {
                NLogDAL.Instance.LogWarning(new NLogModel("Failed to ReView 2", "WARNING"));
                return ConstantModel.ERROR_PRINT_FAIL;
            }

            return 0;
        }
        public static void CheckPackingStatus()
        {
            LoggingDAL.CheckPackingStatus();
        }
        public static void WriteLog(String logMsg)
        {
            LoggingDAL.WriteLog(logMsg);
        }
        public static void DeleteLog(String logMsg)
        {
            LoggingDAL.DeleteLog(logMsg);
        }
        public static void GetMinMaxValue(List<string> list)
        {
            // 假設第一個值是最小與最大值
            string minMAC = list[0];
            string maxMAC = list[0];

            // 遍歷列表，逐一比較
            foreach (var mac in list)
            {
                // 比較最小值
                if (string.Compare(mac, minMAC, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    minMAC = mac;
                }

                // 比較最大值
                if (string.Compare(mac, maxMAC, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    maxMAC = mac;
                }
            }
            BarTenderModel.INIT_MAC = minMAC;
            BarTenderModel.FINAL_MAC = maxMAC;
        }
        public static string GetReprintBoxNumber()
        {
            return LoggingDAL.GetAllBoxNumber();
        }
        public static string GetBoxContent(string boxnumber)
        {
            return LoggingDAL.GetBoxContent(boxnumber);
        }
    }
}
