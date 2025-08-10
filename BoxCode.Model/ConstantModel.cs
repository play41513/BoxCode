using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxCode.Model
{
    public class ConstantModel
    {
        public const int ERROR_VALUE_IS_DUPLICATED = 1;
        public const int ERROR_MAC_FORMAT_IS_FAIL = 2;
        public const int ERROR_MAC_IS_OVER_RANGE = 3;
        public const int ERROR_PRINT_FAIL = 4;
        public const int MESSAGE_PRINTING = 5;
        public const int MESSAGE_CHANGE_NEXT_BOX = 6;

        public const int ERROR_OPEN_PRINT_ERROR = 7;
        public const int ERROR_OPEN_PRINT_FILE_ERROR = 8;
        public const int MESSAGE_ENTER_PAGE_REPRINTING = 9;
        public const int MESSAGE_ENTER_PAGE_HOMEPAGE = 10;
        public const int ERROR_SERVER_VALUE_IS_EXISTED = 11;

        public static String AppPath;
        public string CMD_READ_PASS { get; } = "0313";

        public const int PAGE_CONTROL_HOME_PANEL = 0;
        public const int PAGE_CONTROL_REPRINT = 1;
        public const int PAGE_CONTROL_LOADING_PANEL = 2;
        public const int PAGE_CONTROL_PREVIEW_PANEL = 3;

        public const int MESSAGE_TIVE_UPLOADER_PASS  = 1;
        public const int MESSAGE_TIVE_UPLOADER_FAIL = 2;
        public const int MESSAGE_TIVE_UPLOADER_EXIST = 3;
    }
}
