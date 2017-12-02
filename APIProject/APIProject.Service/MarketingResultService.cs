using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IMarketingResultService
    {
        IEnumerable<MarketingResult> GetMarketingResults();
        //bool CreateResults(List<MarketingResult> list, bool isFinished, int staffID);
        IEnumerable<MarketingResult> GetAll();
        MarketingResult Get(int planResultID);
        Dictionary<string, double> GetRatings();
        IEnumerable<MarketingResult> GetByPlan(int planID);
        Dictionary<string, int> GetLeadRates(int monthRange);
        Dictionary<string, int> GetSourceRates(int monthRange);
        MarketingResult Add(MarketingResult marketingResult);
        void UpdateLeadGenerated(MarketingResult foundResult, Customer customer, Contact contact);
        void UpdateSimilar(MarketingResult marketingResult);
        void Update(MarketingResult marketingResult);
        void SaveChanges();
    }

    public class MarketingResultService : IMarketingResultService
    {
        private readonly IMarketingResultRepository _marketingResultRepository;
        private readonly IMarketingPlanRepository _marketingPlanRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUnitOfWork _unitOfWork;

        private const string RunningName = "Running";
        private const string ReportingName = "Reporting";
        private const string FinishedName = "Finished";


        public MarketingResultService(IMarketingResultRepository _marketingResultRepository, IMarketingPlanRepository _marketingPlanRepository,
            ICustomerRepository _customerRepository, IContactRepository _contactRepository, IStaffRepository _staffRepository,
            IUnitOfWork _unitOfWork)
        {
            this._marketingResultRepository = _marketingResultRepository;
            this._marketingPlanRepository = _marketingPlanRepository;
            this._customerRepository = _customerRepository;
            this._contactRepository = _contactRepository;
            this._staffRepository = _staffRepository;
            this._unitOfWork = _unitOfWork;
        }


        public Dictionary<string, int> GetLeadRates(int monthRange)
        {
            var response = new Dictionary<string, int>();
            var startTime = DateTime.Now.AddMonths(-(monthRange - 1));
            var entities = _marketingResultRepository.GetAll().Where(c => c.IsDelete == false);
            for (int i = 1; i <= monthRange; i++)
            {
                response.Add(startTime.Month + "/" + startTime.Year,
                    entities.Where(c => c.Status == MarketingResultStatus.BecameNewLead
                    && c.CreatedDate.Value.Month == startTime.Month).Count());
                startTime = startTime.AddMonths(1);
            }
            return response;
        }
        public Dictionary<string, int> GetSourceRates(int monthRange)
        {
            var response = new Dictionary<string, int>();
            var labelList = new List<string>
            {
                MarketingResultSource.IsFromInvitation,
                MarketingResultSource.IsFromMedia,
                MarketingResultSource.IsFromFriend,
                MarketingResultSource.IsFromWebsite,
                MarketingResultSource.IsFromOthers
            };
            var entities = GetAll();
            response.Add(MarketingResultSource.IsFromInvitation,
                entities.Where(c => c.IsFromInvitation).Count());
            response.Add(MarketingResultSource.IsFromMedia,
               entities.Where(c => c.IsFromMedia).Count());
            response.Add(MarketingResultSource.IsFromFriend,
               entities.Where(c => c.IsFromFriend).Count());
            response.Add(MarketingResultSource.IsFromWebsite,
               entities.Where(c => c.IsFromWebsite).Count());
            response.Add(MarketingResultSource.IsFromOthers,
               entities.Where(c => c.IsFromOthers).Count());
            return response;
        }

        public MarketingResult Add(MarketingResult marketingResult)
        {
            var planEntity = _marketingPlanRepository.GetById(marketingResult.MarketingPlanID);
            VerifyCanAdd(planEntity);
            var entity = new MarketingResult
            {
                MarketingPlanID = marketingResult.MarketingPlanID,
                CustomerAddress = marketingResult.CustomerAddress,
                CustomerName = marketingResult.CustomerName,
                ContactName = marketingResult.ContactName,
                Email = marketingResult.Email,
                Phone = marketingResult.Phone,
                Notes = marketingResult.Notes,
                FacilityRate = marketingResult.FacilityRate,
                ArrangingRate = marketingResult.ArrangingRate,
                ServicingRate = marketingResult.ServicingRate,
                IndicatorRate = marketingResult.IndicatorRate,
                OthersRate = marketingResult.OthersRate,
                IsFromMedia = marketingResult.IsFromMedia,
                IsFromInvitation = marketingResult.IsFromInvitation,
                IsFromWebsite = marketingResult.IsFromWebsite,
                IsFromFriend = marketingResult.IsFromFriend,
                IsFromOthers = marketingResult.IsFromOthers,
                IsWantMore = marketingResult.IsWantMore,
                CreatedDate = DateTime.Now,
                Status = MarketingResultStatus.New
            };
            _marketingResultRepository.Add(entity);
            return entity;
        }
        public IEnumerable<MarketingResult> GetAll()
        {
            return _marketingResultRepository.GetAll().Where(c => c.IsDelete == false);
        }

        public MarketingResult Get(int planResultID)
        {
            var entity = _marketingResultRepository.GetById(planResultID);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception("Không tìm thấy khảo sát");
            }
        }
        public Dictionary<string, double> GetRatings()
        {
            var entities = GetAll();
            var response = new Dictionary<string, double>
            {
                {
                    MarketingRatingName.FacilityRate,
                    entities.Select(c=>c.FacilityRate).Average()
                },
                {
                    MarketingRatingName.ArrangingRate,
                    entities.Select(c=>c.ArrangingRate).Average()
                },
                {
                    MarketingRatingName.ServicingRate,
                    entities.Select(c=>c.ServicingRate).Average()
                },
                {
                    MarketingRatingName.IndicatorRate,
                    entities.Select(c=>c.IndicatorRate).Average()
                },
                {
                    MarketingRatingName.OthersRate,
                    entities.Select(c=>c.OthersRate).Average()
                },
            };
            return response;
        }
        public IEnumerable<MarketingResult> GetByPlan(int planID)
        {
            var entities = _marketingResultRepository.GetAll().Where(c => c.MarketingPlanID == planID);
            return entities;
        }
        public void UpdateLeadGenerated(MarketingResult foundResult, Customer customer, Contact contact)
        {
            var entity = Get(foundResult.ID);
            entity.UpdatedDate = DateTime.Now;
            if (customer != null)
            {
                entity.CustomerID = customer.ID;
                entity.Status = MarketingResultStatus.BecameNewLead;
            }
            else
            {
                entity.Status = MarketingResultStatus.BecameNewContact;
            }
            _marketingResultRepository.Update(entity);
        }
        public void UpdateSimilar(MarketingResult marketingResult)
        {
            var entity = Get(marketingResult.ID);
            entity.Status = MarketingResultStatus.HasSimilar;
            entity.UpdatedDate = DateTime.Now;
            _marketingResultRepository.Update(entity);
        }

        public void Update(MarketingResult marketingResult)
        {
            var entity = _marketingResultRepository.GetById(marketingResult.ID);
            entity = marketingResult;
            _marketingResultRepository.Update(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        #region private verify
        private void VerifyCanAdd(MarketingPlan marketingPlan)
        {
            if (marketingPlan.Status == MarketingStatus.Finished)
            {
                throw new Exception("Sự kiện đã kết thúc");
            }
        }

        #endregion
        //internal insert results and generate lead

        private void SendMessageToResults(List<MarketingResult> resultList)
        {
            string planTitle = resultList.First().MarketingPlan.Title;
            string senderEmail = "qtsccrmemailsender@gmail.com";
            string mailSubject = "QTSC sự kiện " + planTitle;


            SmtpClient smtpobj = new SmtpClient("smtp.gmail.com", 25)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("qtsccrmemailsender@gmail.com", "kenejnzwmzwboknd")
            };

            foreach (MarketingResult item in resultList)
            {
                string mailBody = $"Cám ơn anh/chị {item.ContactName} đã tham gia sự kiện {planTitle}";
                MailMessage message = new MailMessage(senderEmail, item.Email, mailSubject, mailBody);
                smtpobj.Send(message);
                item.Status = "Đã gửi";
            }
        }

        public IEnumerable<MarketingResult> GetMarketingResults()
        {
            return _marketingResultRepository.GetAll();
        }
    }


}
