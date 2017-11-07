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
        public static string CustomerTypeRequired = "Yêu cầu loại khách hàng:";
        public static string TaxCodeIsUsed = "Mã số thuế này đã được sử dụng";

        public static string OpportunityNotFound = "Không tìm thấy cơ hội bán hàng";
        public static string OppStageRequired = "Yêu cầu cơ hội bán hàng ở giai đoạn:";
        public static string OpportunityClosed = "Cơ hội bán hàng đã kết thúc";
        public static string OpportunitySalesCategoriesRequired = "Tối thiểu 1 loại dịch vụ";
        public static string OppCategoryNotDuplicateRequired = "Các loại dịch vụ không được trùng lặp";
        public static string OppCategoriesNotFound = "Không tìm thấy một vài loại dịch vụ";
        public static string ChangeInfoStageRequired = "Chỉ được đổi thông tin cơ bản ở một trong các giai đoạn sau:";
        public static string ChangeCategoryStageRequired = "Chỉ được đổi các loại dịch vụ ở một trong các giai đoạn sau:";

        public static string WrongAuthorizedStaff = "Nhân viên không có quyền thực hiện thao tác này";
        public static string StaffRoleRequired = "Yêu cầu nhân viên có chức vụ:";
        public static string StaffNotFound = "Không tìm thấy nhân viên";

        public static string ActivityTypesRequired = "Yêu cầu loại lịch gặp khách hàng:";
        public static string ActivityMethodsRequired = "Yêu cầu cách thức gặp khách hàng:";
        public static string ActivityTodoNotPassCurrent = "Thời gian không quá hiện tại";
        public static string ActivityTodoMustPassCurrent = "Thời gian không được trước hiện tại";
        public static string TypeToCustomerNotHaveCategories = "Lịch hẹn tương lai chưa xác định được cơ hội bán hàng";
        public static string ActivityStatusRequired = "Yêu cầu trạng thái lịch hẹn:";
        public static string ActivityNotFound = "Không tìm thấy lịch hẹn";
        public static string ActivityNotForCreateOpportunity = "Lịch hẹn này không thể sinh ra cơ hội bán hàng";
        public static string ActivityOnlyOneAtATime = "Đang có một lịch hẹn";

        public static string DuplicateIDs = "ID không được trùng lặp";

        public static string CreateQuoteRequired = "Phải tạo báo giá trước";
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

        public static string InvalidSalesCategories = "Danh sách loại dịch vụ không hợp lệ";

        public static string IssueSolveDateMustPassCurrent = "Ngày giải quyết không được trước thời điểm hiện tại";
        public static string IssueCategoriesRequired = "Yêu cầu ít nhất 1 loại dịch vụ";
        public static string IssueNotFound = "Không tìm thấy khiếu nại";
        public static string IssueStatusRequired = "Yêu cầu trạng thái của khiếu nại:";

        public static string ContractNotFound = "Không tìm thấy hợp đồng";
        public static string ContractRequired = "Ít nhất 1 loại dịch vụ để làm hợp đồng";
        public static string ContractItemRequired = "Ít nhất 1 hạng mục trong dịch vụ để làm hợp đồng";
        public static string ContractItemStartDateMustPassCurrent = "Ngày bắt đầu sử dụng hạng mục không được trước thời điểm hiện tại";
        public static string ContractItemStartDateMustNotPassEndDate = "Ngày bắt đầu sử dụng hạng mục không được sau ngày kết thúc hạng mục";

        public static string MarketingPlanStatusRequired = "Yêu cầu chiến dịch đang ở trạng thái:";
    }
}
