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
        public static string OppStageRequired = "Yêu cầu cơ hội bán hàng ở giai đoạn:";
        public static string OpportunityClosed = "Cơ hội bán hàng đã kết thúc";
        public static string OpportunitySalesCategoriesRequired = "Tối thiểu 1 loại dịch vụ";
        public static string OppCategoryNotDuplicateRequired = "Các loại dịch vụ không được trùng lặp";
        public static string OppCategoriesNotFound = "Không tìm thấy một vài loại dịch vụ";
        public static string ChangeInfoStageRequired = "Chỉ được đổi thông tin cơ bản ở một trong các giai đoạn sau:";

        public static string WrongAuthorizedStaff = "Nhân viên không có quyền thực hiện thao tác này";
        public static string StaffRoleRequired = "Yêu cầu nhân viên có chức vụ:";
        public static string StaffNotFound = "Không tìm thấy nhân viên";


        public static string DuplicateIDs = "ID không được trùng lặp";


        public static string QuoteStatusRequired = "Báo giá phải ở trạng thái:";
        public static string QuoteExisted = "Đã tạo một báo giá trước đó";
        public static string QuoteNotFound = "Không tìm thấy báo giá";
        public static string QuoteRequired = "Chưa tạo báo giá";
        public static string QuoteItemNotDuplicateRequired = "Các mục không được trùng lặp";
        public static string NewQuoteRequired = "Cần tạo báo giá mới";
        public static string ValidateQuoteRequired = "Báo giá chưa được duyệt";
        public static string SendQuoteRequired = "Chưa gửi báo giá cho khách hàng";
        public static string ValidatingQuoteRequired = "Báo giá chưa hoàn thành để kiểm duyệt";
        public static string PendingQuoteExisted = "Chỉ tồn tại 1 báo giá trong một thời điểm";
        public static string QuoteItemsNotFound = "Không tìm thấy một vài mục trong báo giá";
        public static string QuoteItemRequired = "Phải ít nhất 1 hạng mục báo giá";

        public static string InvalidSalesItems = "Danh sách mục báo giá không hợp lệ";



    }
}
