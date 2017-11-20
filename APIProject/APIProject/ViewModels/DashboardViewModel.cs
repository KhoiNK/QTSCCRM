using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardOpportunity Opportunity { get; set; }
        public DashboardActivity Activity { get; set; }
        public DashboardContract Contract { get; set; }
        public DashboardCustomer Customer { get; set; }
        public DashboardIssue Issue { get; set; }
        public DashboardMarketing Marketing { get; set; }
    }
    public class DashboardActivity
    {
        public List<ActivityViewModel> OverdueActivities { get; set; }
        public List<ActivityViewModel> TodayActivities { get; set; }
        public List<ActivityViewModel> FutureActivities { get; set; }
    }
    public class DashboardContract
    {
        public int TotalCount { get; set; }
        public int NeedActionCount { get; set; }
        public ChartViewModel AllUsingRates { get; set; }
        public List<UsingRateCharts> UsingRatesList { get; set; }
    }
    public class UsingRateCharts
    {
        public string CategoryName { get; set; }
        public ChartViewModel Chart { get; set; }
    }
    public class DashboardOpportunity
    {
        public int ConsiderCount { get; set; }
        public int MakeQuoteCount { get; set; }
        public int ValidateCount { get; set; }
        public int SendCount { get; set; }
        public int NegotiateCount { get; set; }
        public ChartViewModel WonLostRate { get; set; }
        public DoubleTypeChartViewModel StageDurationRates { get; set; }
        public ChartViewModel CreateRates { get; set; }
    }
    public class DoubleTypeChartViewModel
    {
        public List<string> Labels { get; set; }
        public List<double> Values { get; set; }
    }
    public class LineChartViewModel
    {
        public List<string> XLabels { get; set; }
        public List<int> Values { get; set; }
    }
    public class ChartViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> Values { get; set; }
    }
    public class PieChartViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> Values { get; set; }
    }
    public class DashboardCustomer
    {
        public int CustomerCount { get; set; }
        public int LeadCount { get; set; }
        public ChartViewModel ConvertRates { get; set; }
    }
    public class DashboardIssue
    {
        public int DoingIssuesCount { get; set; }
        public ChartViewModel DoneFailedRate { get; set; }
        public ChartViewModel CreateRates { get; set; }
        public ChartViewModel IssueCategoryRates { get; set; }
    }
    public class DashboardMarketing
    {
        public int ExecutingCount { get; set; }
        public ChartViewModel Rates { get; set; }
        public ChartViewModel LeadCountChart { get; set; }
        public ChartViewModel LeadSourceCountChart { get; set; }
        public double MarketingRatingMaxValue { get; set; }
        public DoubleTypeChartViewModel MarketingRatings { get; set; }
    }
    
    public class IssueCountChart
    {
        public int FailedCount { get; set; }
        public int DoneCount { get; set; }
        public IssueCategoryCount CategoryCount { get; set; }
    }
    public class CustomerRateChart
    {
        public List<string> XLabels { get; set; }
        public List<int> CustomerRates { get; set; }
        public List<int> LeadRates { get; set; }
    }

    public class IssueRateChart
    {
        public List<string> XLabels { get; set; }
        public List<int> IssueRates { get; set; }
    }
    public class IssueCategoryCount
    {
        public List<string> Labels { get; set; }
        public List<int> Counts { get; set; }
    }
}