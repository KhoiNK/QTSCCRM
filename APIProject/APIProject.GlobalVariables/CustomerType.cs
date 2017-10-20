using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class CustomerType
    {
        public static string Lead = "Tiềm năng";

        //radio buttons
        public static string Official = "Chính thức";
        public static string Inside = "Nội";
        public static string Outside = "Ngoại";

        public static List<string> GetList()
        {
            return new List<string>
            {
                Lead,
                Official,
                Inside,
                Outside
            };
        }
    }
}
