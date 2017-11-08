using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class MarketingStatus
    {
        public static string Drafting = "Chỉnh sửa";
        public static string Waiting = "Chờ";
        public static string Executing = "Đang hoạt động";
        public static string Reporting = "Đang tổng kết";
        public static string Finished = "Kết thúc";
    }

    public static class MarketingResultStatus
    {
        public static string HasSimilar = "Có tương đồng";
        public static string New = "Khách hàng mới";
        public static string BecameNewLead = "Đã cập nhật khách hàng tiềm năng";
    }
}
