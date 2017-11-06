using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
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
        MarketingPlan Get(int marketingID);
        MarketingPlan Add(MarketingPlan marketingPlan);
        void Finish(MarketingPlan marketingPlan);
        void UpdateInfo(MarketingPlan marketingPlan);
        void SaveChanges();
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


            

            _unitOfWork.Commit();



            

            _unitOfWork.Commit();
            return marketingPlan.ID;
        }
        //write and return filePath
        private string WriteMarketingFiles(string fileName, string fileContentB64)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/MarketingPlanFiles");
            string filePath = Path.Combine(rootPath, fileName);
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(fileContentB64));
            }
            catch (FormatException)
            {
                return null;
            }
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
            }
            else if (isTask)
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


        private void MoveToNextStage(MarketingPlan plan)
        {
            plan.Status = null;
            
        }

        private void BackgroundMoveStage(int? planId = null)
        {
            var planList = _marketingPlanRepository.GetAll();
            if (planId.HasValue)
            {
                var plan = planList.Where(c => c.ID == planId.Value).SingleOrDefault();
                if (plan != null)
                {
                    
                }
            }
            else
            {
                foreach (var planItem in planList)
                {
                    
                }
            }
            _unitOfWork.Commit();
        }

        public MarketingPlan Get(int marketingID)
        {
            var entity = _marketingPlanRepository.GetById(marketingID);
            if(entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception("Không tìm thấy chiến dịch");
            }
        }

        public MarketingPlan Add(MarketingPlan marketingPlan)
        {
            marketingPlan.CreatedDate = DateTime.Now;
            marketingPlan.Status = MarketingStatus.Drafting;
            _marketingPlanRepository.Add(marketingPlan);
            return marketingPlan;
        }

        public void Finish(MarketingPlan marketingPlan)
        {
            var entity = _marketingPlanRepository.GetById(marketingPlan.ID);
            VerifyCanFinish(entity);
            entity.UpdatedDate = DateTime.Now;
            entity.Status = MarketingStatus.Finished;
            _marketingPlanRepository.Update(entity);
        }


        public void UpdateInfo(MarketingPlan marketingPlan)
        {
            var entity = _marketingPlanRepository.GetById(marketingPlan.ID);
            VerifyCanUpdateInfo(entity);
            entity.UpdatedDate = DateTime.Now;
            entity.Title = marketingPlan.Title;
            entity.Description = marketingPlan.Description;
            _marketingPlanRepository.Update(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        #region private verify
        private void VerifyCanUpdateInfo(MarketingPlan plan)
        {
            if(plan.Status!= MarketingStatus.Drafting)
            {
                throw new Exception(CustomError.MarketingPlanStatusRequired
                    + MarketingStatus.Drafting);
            }
        }
        private void VerifyCanFinish(MarketingPlan plan)
        {
            if (plan.Status != MarketingStatus.Reporting)
            {
                throw new Exception(CustomError.MarketingPlanStatusRequired
                    + MarketingStatus.Reporting);
            }
        }
#endregion

    }


}
