using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.GlobalVariables
{
    public static class MarketingStatus
    {
        public static string Executing = "Đang hoạt động";
        public static string Finished = "Kết thúc";
    }

    public static class MarketingResultStatus
    {
        public static string HasSimilar = "Có tương đồng";
        public static string New = "Khách hàng mới";
        public static string BecameNewLead = "Đã cập nhật khách hàng tiềm năng";
        public static string BecameNewContact = "Đã cập nhật liên lạc";
    }

    public static class MarketingResultIsFrom
    {
        public static string IsFromMedia = "IsFromMedia";
        public static string IsFromInvitation = "IsFromInvitation";
        public static string IsFromWebsite = "IsFromWebsite";
        public static string IsFromFriend = "IsFromFriend";
        public static string IsFromOthers = "IsFromOthers";
    }

    public static class MarketingResultSource
    {
        public static string IsFromMedia = "Phương tiện truyền thông";
        public static string IsFromInvitation = "Thư mời";
        public static string IsFromWebsite = "Website QTSC";
        public static string IsFromFriend = "Bạn bè";
        public static string IsFromOthers = "Khác";
    }
    public static class MarketingRatingName
    {
        public static string FacilityRate = "Cơ sở vật chất";
        public static string ArrangingRate = "Công tác tổ chức";
        public static string ServicingRate = "Công tác phục vụ";
        public static string IndicatorRate = "Người trình bày";
        public static string OthersRate = "Khác";
    }
}
