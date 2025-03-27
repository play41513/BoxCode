using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxCode.Model
{
    public class WorkOrderModel
    {
        public static String WorkOrder { get; set; }
        public static String VMI_Order { get; set; }
        public static String TB_Part { get; set; }
        public static String CUST_Part { get; set; }
        public static String INIT_MAC { get; set; }
        public static String FINAL_MAC { get; set; }
        public static String TOTAL_BOX_COUNT { get; set; }
        public static String PACKING_NUMBER { get; set; }
        public static String EmployeeID { get; set; }
    }
    public class BarTenderModel
    {
        public static String VMI_Order { get; set; }
        public static String TB_Part { get; set; }
        public static String CUST_Part { get; set; }
        public static String INIT_MAC { get; set; }
        public static String FINAL_MAC { get; set; }
        public static String TOTAL_BOX_COUNT { get; set; }
        public static String PACKING_NUMBER { get; set; }
        public static String NOW_BOX_COUNT { get; set; }
    }
    public class InputModel
    {
        public static List<String> ListInputValue { get; set; }
        public static int InputValueCount { get; set; }
        public static List<String> ListReprintValue { get; set; }
    }
    public class PreViewModel
    {
        public static int PreViewPageIndex { get; set; }
    }
}
