using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class CustomError
    {
        public static string CustomerNotFound = "Không tìm thấy khách";
        public static string ContactNotFound = "Không tìm thấy liên lạc";
        public static string OpportunityNotFound = "Không tìm thấy cơ hội bán hàng";

        public static string WrongAuthorizedStaff = "Chỉ nhân viên tạo mới được quyền sửa";
    }
}
