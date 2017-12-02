﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class RoleName
    {
        public static string Sales = "Kinh doanh";
        public static string Marketing = "Marketing";
        public static string Support = "Hỗ trợ";
        public static string Director = "Giám đốc";
        public static string Officer = "Trưởng phòng kinh doanh";
        public static string Admin = "Admin";
        public static string Mainternance = "Khảo sát chất lượng";

        public static List<string> GetList()
        {
            return new List<string>
            {
                Sales,
                Marketing,
                Support,
                Director,
                Officer,
                Admin
            };
        }
    }
}
