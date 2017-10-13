using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace APIProject.Service
{

    public interface IMarketingPlanService
    {
        IEnumerable<MarketingPlan> GetMarketingPlans();
        MarketingPlan GetMarketingPlan(int id);
        int CreateNewPlan(MarketingPlan marketingPlan, bool isFinished, string budgetB64, string taskAssignB64, string eventB64, string licenseB64);
        bool UpdatePlan(MarketingPlan marketingPlan, bool isFinished);
        bool ValidatePlan(MarketingPlan marketingPlan, bool validate);
        bool AcceptPlan(MarketingPlan marketingPlan, bool accept);
    }


    public class MarketingPlanService : IMarketingPlanService
    {
        private readonly IMarketingPlanRepository _marketingPlanRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUnitOfWork _unitOfWork;

        private const string DraftingName = "Drafting";
        private const string ValidatingName = "Validating";
        private const string AcceptingName = "Accepting";
        private const string PreparingName = "Preparing";
        private const string RunningName = "Running";
        private const string ReportingName = "Reporting";
        private const string FinishedName = "Finished";

        private const string ValidateFailedName = "Validate Failed";
        private const string AcceptFailedName = "Accept Failed";

        public MarketingPlanService(IMarketingPlanRepository _marketingPlanRepository, IUnitOfWork _unitOfWork,
            IStaffRepository _staffRepository)
        {
            this._marketingPlanRepository = _marketingPlanRepository;
            this._staffRepository = _staffRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateNewPlan(MarketingPlan marketingPlan, bool isFinished, string budgetB64, string taskAssignB64, string eventB64, string licenseB64)
        {
            _marketingPlanRepository.Add(marketingPlan);
            marketingPlan.CreateStaffID = marketingPlan.ModifiedStaffID;
            marketingPlan.LastModifiedDate = DateTime.Today.Date;
            marketingPlan.CreatedDate = marketingPlan.LastModifiedDate;


            if (isFinished)
            {
                marketingPlan.Stage = ValidatingName;
            }
            else
            {
                marketingPlan.Stage = DraftingName;
            }

            _unitOfWork.Commit();


            string fileName;

            if (budgetB64 != null)
            {
                //todo here
                fileName = GetFileName(marketingPlan.ID,true,false, false, false);
                marketingPlan.BudgetFileSrc = WriteMarketingFiles(fileName, budgetB64);

            }
            if (taskAssignB64 != null)
            {
                //todo
                fileName = GetFileName(marketingPlan.ID, false, false, true, false);
                marketingPlan.TaskAssignSrc = WriteMarketingFiles(fileName, taskAssignB64);
            }
            if (eventB64 != null)
            {
                //todo
                fileName = GetFileName(marketingPlan.ID, false, true, false, false);
                marketingPlan.EventScheduleFileSrc = WriteMarketingFiles(fileName, eventB64);

            }
            if (licenseB64 != null)
            {
                //todo
                fileName = GetFileName(marketingPlan.ID, false, false, false, true);
                marketingPlan.LicenseFileSrc = WriteMarketingFiles(fileName, licenseB64);
            }

            _unitOfWork.Commit();
            return marketingPlan.ID;
        }
        //write and return filePath
        private string WriteMarketingFiles(string fileName, string fileContentB64)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/MarketingPlanFiles");
            string filePath = Path.Combine(rootPath, fileName);
            File.WriteAllBytes(filePath, Convert.FromBase64String(fileContentB64));
            return filePath;
        }

        private string GetFileName(int planID, bool isBudget, bool isEvent, bool isTask, bool isLicense)
        {
            DateTime timeNow = DateTime.Now;
            string fileName = planID + "_" + timeNow.Year + timeNow.Month + timeNow.Day + timeNow.Hour + timeNow.Minute
                + timeNow.Second;
            if (isBudget)
            {
                fileName += "_budget";
            }
            else if (isEvent)
            {
                fileName += "_event";
            }else if (isTask)
            {
                fileName += "_task";
            }
            else
            {
                fileName += "_license";
            }

            fileName += ".pdf";
            return fileName;
        }

        public IEnumerable<MarketingPlan> GetMarketingPlans()
        {
            BackgroundMoveStage();
            return _marketingPlanRepository.GetAll();
        }

        public MarketingPlan GetMarketingPlan(int id)
        {
            BackgroundMoveStage(id);
            return _marketingPlanRepository.GetById(id);
        }

        public bool UpdatePlan(MarketingPlan marketingPlan, bool isFinished)
        {
            MarketingPlan plan = _marketingPlanRepository.GetById(marketingPlan.ID);
            if (plan != null)
            {
                Staff foundStaff = _staffRepository.GetById(marketingPlan.ModifiedStaffID.Value);
                if (foundStaff != null)
                {
                    if (plan.Stage == DraftingName)
                    {
                        plan.ModifiedStaffID = marketingPlan.ModifiedStaffID;
                        plan.LastModifiedDate = DateTime.Today.Date;
                        plan.Title = marketingPlan.Title;
                        plan.Budget = marketingPlan.Budget;
                        plan.Description = marketingPlan.Description;
                        plan.StartDate = marketingPlan.StartDate;
                        plan.EndDate = marketingPlan.EndDate;

                        if (isFinished)
                        {
                            MoveToNextStage(plan);
                        }

                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }
            return false;
        }

        private void MoveToNextStage(MarketingPlan plan)
        {
            plan.Status = null;
            if (plan.Stage == DraftingName)
            {
                plan.Stage = ValidatingName;
                plan.AcceptStaffID = null;
                plan.ValidateStaffID = null;
                plan.ValidateNotes = null;
                plan.AcceptNotes = null;
            }
            else if (plan.Stage == ValidatingName)
            {
                plan.Stage = AcceptingName;
                plan.ValidateStaffID = plan.ModifiedStaffID;
            }
            else if (plan.Stage == AcceptingName)
            {
                plan.Stage = PreparingName;
                plan.AcceptStaffID = plan.ModifiedStaffID;
            }
            else if (plan.Stage == PreparingName)
            {
                plan.Stage = RunningName;
            }
            else if (plan.Stage == RunningName)
            {
                plan.Stage = ReportingName;
            }
            else if (plan.Stage == ReportingName)
            {
                plan.Stage = FinishedName;
            }
        }

        private void BackgroundMoveStage(int? planId = null)
        {
            var planList = _marketingPlanRepository.GetAll();
            if (planId.HasValue)
            {
                var plan = planList.Where(c => c.ID == planId.Value).SingleOrDefault();
                if (plan != null)
                {
                    if (plan.Stage == PreparingName)
                    {
                        if (DateTime.Compare(plan.StartDate, DateTime.Today.Date) <= 0)
                        {
                            MoveToNextStage(plan);
                        }
                    }
                    if (plan.Stage == RunningName)
                    {
                        if (DateTime.Compare(plan.EndDate, DateTime.Today.Date) < 0)
                        {
                            MoveToNextStage(plan);
                        }
                    }
                }
            }
            else
            {
                foreach (var planItem in planList)
                {
                    if (planItem.Stage == PreparingName)
                    {
                        if (DateTime.Compare(planItem.StartDate, DateTime.Today.Date) <= 0)
                        {
                            MoveToNextStage(planItem);
                        }
                    }
                    if (planItem.Stage == RunningName)
                    {
                        if (DateTime.Compare(planItem.EndDate, DateTime.Today.Date) < 0)
                        {
                            MoveToNextStage(planItem);
                        }
                    }
                }
            }
            _unitOfWork.Commit();
        }
        public bool ValidatePlan(MarketingPlan marketingPlan, bool validate)
        {
            MarketingPlan plan = _marketingPlanRepository.GetById(marketingPlan.ID);
            if (plan != null)
            {
                Staff foundStaff = _staffRepository.GetById(marketingPlan.ModifiedStaffID.Value);
                if (foundStaff != null)
                {
                    if (plan.Stage == ValidatingName)
                    {
                        plan.ModifiedStaffID = marketingPlan.ModifiedStaffID;
                        plan.ValidateNotes = marketingPlan.ValidateNotes;
                        if (validate)
                        {
                            MoveToNextStage(plan);
                        }
                        else
                        {
                            plan.Stage = DraftingName;
                            plan.Status = ValidateFailedName;
                        }

                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AcceptPlan(MarketingPlan marketingPlan, bool accept)
        {
            MarketingPlan plan = _marketingPlanRepository.GetById(marketingPlan.ID);
            if (plan != null)
            {
                Staff foundStaff = _staffRepository.GetById(marketingPlan.ModifiedStaffID.Value);
                if (foundStaff != null)
                {
                    if (plan.Stage == AcceptingName)
                    {
                        plan.ModifiedStaffID = marketingPlan.ModifiedStaffID;
                        plan.AcceptNotes = marketingPlan.AcceptNotes;
                        if (accept)
                        {
                            MoveToNextStage(plan);
                        }
                        else
                        {
                            plan.Stage = DraftingName;
                            plan.Status = AcceptFailedName;
                        }

                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }

            return false;
        }
    }


}
