using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class OpportunityStage
    {
        //public static string Open = "Mới";
        public static string Consider = "Xem xét";
        public static string MakeQuote = "Tạo báo giá";
        public static string ValidateQuote = "Duyệt báo giá";
        public static string SendQuote = "Gửi báo giá";
        public static string Negotiation = "Đàm phán";
        public static string Won = "Thành công";
        public static string Lost = "Thất bại";

        public static string ConsiderDetails = "Đã đủ khả năng đáp ứng?";
        public static string MakeQuoteDetails = "Đã tạo báo giá?";
        public static string ValidateQuoteDetails = "Đã được giám đốc phê duyệt?";
        public static string SendQuoteDetails = "Đã gửi báo giá cho khách hàng?";
        public static string NegotiationDetails = "Đã đàm phán / trao đổi với khách hàng?";
        public static string WonDetails = "Khách hàng đã đồng ý sử dụng dịch vụ";
        public static string LostDetails = "Khách hàng đã từ chối sử dụng dịch vụ";
        public static Dictionary<string, string> GetList()
        {
            var diction = new Dictionary<string, string>();
            diction.Add(Consider, ConsiderDetails);
            diction.Add(MakeQuote, MakeQuoteDetails);
            diction.Add(ValidateQuote, ValidateQuoteDetails);
            diction.Add(SendQuote, SendQuoteDetails);
            diction.Add(Negotiation, NegotiationDetails);
            diction.Add(Won, WonDetails);
            diction.Add(Lost, LostDetails);
            return diction;
        }
    }
}
