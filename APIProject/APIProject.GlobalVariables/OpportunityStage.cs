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

        public static string ConsiderDetails = "Kiểm tra xem công ty có đủ khả năng đáp ứng nhu cầu của khách hàng hay không?";
        public static string MakeQuoteDetails = "Tạo báo giá với đủ hạng mục dịch vụ mà khách yêu cầu";
        public static string ValidateQuoteDetails = "Chờ giám đốc duyệt báo giá";
        public static string SendQuoteDetails = "Gửi báo giá cho khách hàng thông qua email khách đã cung cấp";
        public static string NegotiationDetails = "Trao đổi lần cuối với khách hàng, xác định rõ các hạng mục để lập hợp đồng";
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
