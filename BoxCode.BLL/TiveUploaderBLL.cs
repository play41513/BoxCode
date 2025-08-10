using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using BoxCode.DAL;
using BoxCode.Model;

namespace BoxCode.BLL
{
    public class ApiResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public object data { get; set; }  // 若你知道 data 的具體格式可以用具體型別
        public string processId { get; set; }
    }
    public class TiveUploaderBLL
    {
        /* 正式用*/
        private static readonly string baseUrl = @"https://api.nebula.tive.com/api/v1/horsehead/";
        private static readonly string apiKey = @"MnLHzgr3acaDL0PsaLQen5rP1Wvz4lfD4QPxCN6Q";
        private static readonly string apiUser = @"mfr-tydenbrooks";
        private static readonly string apiUserKey = @"xAALy*j0Zx(BkG(%94ksgdiJlHSIjct%2pH!E^mYM&dH%&t@L*sjx!LO";
        /* 測試用
        private static readonly string baseUrl = @"https://api.uat.nebula.tive.com/api/v1/horsehead/";
        private static readonly string apiKey = @"Gv1tQa6WNS39IszyI296l9KyPZxFWom4DGoSss0j";
        private static readonly string apiUser = @"mfr-tydenbrooks";
        private static readonly string apiUserKey = @"D9@Bqc6nRp5SN@qHYn!ASkRMaz3$GYX%zPQIMdGa$Uo*Z7oF53C#lb!UU^Z";*/

        public static readonly HttpClient Client;


        static TiveUploaderBLL()
        {
            Client = new HttpClient();
            Client.Timeout = TimeSpan.FromSeconds(5);
            Client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            Client.DefaultRequestHeaders.Add("nebula-api-user", apiUser);
            Client.DefaultRequestHeaders.Add("nebula-api-key", apiUserKey);
            Client.Timeout = TimeSpan.FromSeconds(3);
        }
        public static async Task<bool> UploaderBoxIdToTiveSystem(string BoxID, List<string> bleMacList)
        {
            if (WorkOrderModel.TiveUpload == "false")
                return true;
            try
            {
                return await RegisterBoxAsync(
                    BoxID,
                    bleMacList
                );
            }
            catch
            {
                return false;
            }
        }
        public static async Task<int> UploaderMACToTiveSystem(string mac)
        {
            if (WorkOrderModel.TiveUpload == "false")
                return ConstantModel.MESSAGE_TIVE_UPLOADER_PASS;
            if (BarTenderModel.PACKING_NUMBER != "50")
                return ConstantModel.MESSAGE_TIVE_UPLOADER_PASS;
            try
            {
                return await RegisterBeaconAsync(
                    macAddress: mac,
                    deviceName: mac,
                    partNumber: "300079",
                    manufacturingDate: DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss+08:00"),
                    manufacturingLot: WorkOrderModel.WorkOrder
                );
            }
            catch
            {
                return ConstantModel.MESSAGE_TIVE_UPLOADER_FAIL;
            }
        }
        /// <summary>
        /// 註冊 BLE Seal (Beacon) 到 Tive
        /// </summary>
        public static async Task<int> RegisterBeaconAsync( 
          string macAddress,
          string deviceName,
          string partNumber,
          string manufacturingDate,
          string manufacturingLot)
        {
            var url = baseUrl + "register/beacon";

            var body = new
            {
                MacAddress = macAddress,
                DeviceName = deviceName,
                PartNumber = partNumber,
                ManufacturingDate = manufacturingDate,
                ManufacturingLot = manufacturingLot
            };

            string json = JsonConvert.SerializeObject(body);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    var response = await Client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    // 解析 JSON 字串為物件
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                    string status = apiResponse.status;
                    string message = apiResponse.message;
                    string processId = apiResponse.processId;
                    if(status == "error")
                    {
                        NLogDAL.Instance.LogWarning(new NLogModel("Status : " + status + "\r\n Message: " + message, "INFO"));
                        if (message.Contains("Device registration already exists"))
                        {
                            Console.WriteLine("已重複: Device registration already exists" );
                            return ConstantModel.MESSAGE_TIVE_UPLOADER_EXIST;
                            /*
                            url = baseUrl + "register/beacon/" + macAddress;
                            var content2 = new StringContent(json, Encoding.UTF8, "application/json");
                            response = await Client.PutAsync(url, content2);
                            result = await response.Content.ReadAsStringAsync();
                            // 解析 JSON 字串為物件
                            apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                            status = apiResponse.status;
                            message = apiResponse.message;
                            processId = apiResponse.processId;
                            Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                            Console.WriteLine("MAC: " + macAddress);
                            Console.WriteLine("回應內容: " + result);
                            NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("MAC: " + macAddress, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));
                            if (status == "error")
                                return ConstantModel.MESSAGE_TIVE_UPLOADER_FAIL;*/
                        }
                        else
                        {
                            Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                            Console.WriteLine("MAC: " + macAddress);
                            Console.WriteLine("回應內容: " + result);
                            NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("MAC: " + macAddress, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));
                            return ConstantModel.MESSAGE_TIVE_UPLOADER_FAIL;
                        }
                    }

                    Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                    Console.WriteLine("MAC: " + macAddress);
                    Console.WriteLine("回應內容: " + result);
                    NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                    NLogDAL.Instance.LogWarning(new NLogModel("MAC: " + macAddress, "INFO"));
                    NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));

                    // 根據 HTTP 狀態碼判斷是否成功
                    if(response.IsSuccessStatusCode)
                        return ConstantModel.MESSAGE_TIVE_UPLOADER_PASS;
                    return ConstantModel.MESSAGE_TIVE_UPLOADER_FAIL;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("發送失敗: " + ex.Message);
                    return ConstantModel.MESSAGE_TIVE_UPLOADER_FAIL;
                }
            }
        }
        public static async Task<bool> RegisterBoxAsync( 
           string BoxID,
           List<string> bleMacList)
        {
            var url = baseUrl + "register/box";

            var body = new
            {
                boxId = BoxID,
                contents = bleMacList
            };

            string json = JsonConvert.SerializeObject(body);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    var response = await Client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    // 解析 JSON 字串為物件
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                    string status = apiResponse.status;
                    string message = apiResponse.message;
                    string processId = apiResponse.processId;
                    NLogDAL.Instance.LogWarning(new NLogModel("Status : " + status + "\r\n Message: " + message, "INFO"));
                    if (status == "error")
                    {
                        if (message.Contains("already exists"))
                        {
                            url = baseUrl + "register/box/"+ BoxID;
                            var content2 = new StringContent(json, Encoding.UTF8, "application/json");
                            response = await Client.PutAsync(url, content2);
                            result = await response.Content.ReadAsStringAsync();
                            // 解析 JSON 字串為物件
                            apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                            status = apiResponse.status;
                            message = apiResponse.message;
                            processId = apiResponse.processId;
                            Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                            Console.WriteLine("回應內容: " + result);
                            NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));
                            if (status == "error")
                                return false;
                        }
                        else
                        {
                            Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                            Console.WriteLine("回應內容: " + result);
                            NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                            NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));
                            return false;
                        }
                    }

                    Console.WriteLine("伺服器回應狀態: " + response.StatusCode);
                    Console.WriteLine("回應內容: " + result);
                    NLogDAL.Instance.LogWarning(new NLogModel("伺服器回應狀態: " + response.StatusCode, "INFO"));
                    NLogDAL.Instance.LogWarning(new NLogModel("回應內容: " + result, "INFO"));

                    // 根據 HTTP 狀態碼判斷是否成功
                    LoggingDAL.WriteBoxIdToLog(BoxID);
                    return response.IsSuccessStatusCode; // 直接回傳 API 是否成功
                }
                catch (Exception ex)
                {
                    Console.WriteLine("發送失敗: " + ex.Message);
                    return false; // 發生例外時回傳失敗
                }
            }
        }
    }
}
