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
        public static string QuoteNotFound = "Không tìm thấy báo giá";
        public static string StaffNotFound = "Không tìm thấy nhân viên";
        

        public static string WrongAuthorizedStaff = "Nhân viên không có quyền thực hiện thao tác này";

        public static string QuoteRequired = "Chưa tạo báo giá";
        public static string NewQuoteRequired = "Cần tạo báo giá mới";
        public static string ValidateQuoteRequired = "Báo giá chưa được duyệt";
        public static string SendQuoteRequired = "Chưa gửi báo giá cho khách hàng";

        public static string PendingQuoteExisted = "Chỉ tồn tại 1 báo giá trong một thời điểm";
        public static string ValidatingQuoteRequired = "Báo giá chưa hoàn thành để kiểm duyệt";
        public static string DuplicateIDs = "ID không được trùng lặp";
        public static string InvalidSalesItems = "Danh sách item báo giá không hợp lệ";
        public static string OpportunityClosed = "Cơ hội bán hàng đã kết thúc";
    }
}
