using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxCode.Model;
using BoxCode.DAL;

namespace BoxCode.BLL
{
    public class CheckInputValueBLL
    {
        /// <summary>
        /// 確認輸入值是否符合格式
        /// </summary>
        public static int CheckInputValue(String InputValue)
        {

            // 檢查是否符合 MAC 格式
            if (!IsValidMACFormat(InputValue))
            {
                return ConstantModel.ERROR_MAC_FORMAT_IS_FAIL;
            }

            // 檢查是否在範圍內
            if (string.Compare(InputValue, WorkOrderModel.INIT_MAC, StringComparison.OrdinalIgnoreCase) < 0 ||
                string.Compare(InputValue, WorkOrderModel.FINAL_MAC, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return ConstantModel.ERROR_MAC_IS_OVER_RANGE;
            }
            else
            {
                if (!LoggingDAL.IsValueDuplicated(InputValue))
                    return 0;
                else
                    return ConstantModel.ERROR_VALUE_IS_DUPLICATED;
            }
        }
        /// <summary>
        /// 檢查 MAC 格式是否有效 (必須是 12 位的十六進位字串)
        /// </summary>
        private static bool IsValidMACFormat(string mac)
        {
            return mac.Length == 12 && System.Text.RegularExpressions.Regex.IsMatch(mac, @"^[0-9A-Fa-f]{12}$");
        }
    }
}
