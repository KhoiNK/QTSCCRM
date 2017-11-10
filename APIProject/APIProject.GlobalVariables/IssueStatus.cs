using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class IssueStatus
    {
        //Open Stage
        //public static string Open = "Chưa có ngày giải quyết";

        //Solving Stage
        public static string Doing = "Đang xử lý";
        public static string Overdue = "Trễ";

        //Closed Stage
        public static string Done = "Hoàn thành";
        public static string Failed = "Không thể đáp ứng";
    }
}
