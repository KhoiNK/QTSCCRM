using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class ActivityMethod
    {
        public static string Email = "Email";
        public static string Direct = "Trực tiếp";
        public static string Phone = "Gọi điện";

        public static List<string> GetList()
        {
            return new List<string>
            {
                Email,
                Direct,
                Phone
            };
        }
    }
}
