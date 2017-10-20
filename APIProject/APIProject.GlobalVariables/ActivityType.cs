using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class ActivityType
    {
        public static string FromCustomer = "Từ khách hàng";
        public static string ToCustomer = "Đến khách hàng";

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
