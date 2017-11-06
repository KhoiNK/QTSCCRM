using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class ActivityType
    {
        public static string FromCustomer = "Ghi nhận";
        public static string ToCustomer = "Tạo mới";

        public static List<string> GetList()
        {
            return new List<string>
            {
                FromCustomer,
                ToCustomer
            };
        }
    }
}
