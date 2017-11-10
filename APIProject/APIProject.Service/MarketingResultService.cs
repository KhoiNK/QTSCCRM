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
        MarketingResult Get(int planResultID);
        IEnumerable<MarketingResult> GetByPlan(int planID);
        MarketingResult Add(MarketingResult marketingResult);
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

        //public bool CreateResults(List<MarketingResult> list, bool isFinished, int staffID)
        //{
        //    int planID = list.First().MarketingPlanID;
        //    var foundPlan = _marketingPlanRepository.GetById(planID);
        //    var foundStaff = _staffRepository.GetById(staffID);

        //    //verify staff exist
        //    if (foundStaff == null)
        //    {
        //        return false;
        //    }

        //    //verify customer and contact
        //    foreach (var item in list)
        //    {
        //        //verify customer exist
        //        if (item.CustomerID.HasValue)
        //        {
        //            var foundCustomer = _customerRepository.GetById(item.CustomerID.Value);
        //            if (foundCustomer == null)
        //            {
        //                return false;
        //            }
        //        }
        //        //verify contact exist and in right customer
        //        if (item.ContactID.HasValue)
        //        {
        //            var foundContact = _contactRepository.GetById(item.ContactID.Value);
        //            if (foundContact == null)
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                if (item.CustomerID.HasValue)
        //                {
        //                    if (foundContact.CustomerID != item.CustomerID)
        //                    {
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    //verify plan exist
        //    if (foundPlan == null)
        //    {
        //        return false;
        //    }
        //    //verify stage
        //    //verify stage and is finished
        //    if (isFinished)
        //    {
        //    }


        //    //start adding result code here
        //    InsertResultsAndLeads(list);
        //    SendMessageToResults(list);
        //    _unitOfWork.Commit();

        //    //start sending email code here

        //    return true;
        //}

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
                CreatedDate = DateTime.Now
            };
            _marketingResultRepository.Add(entity);
            return entity;
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

        public IEnumerable<MarketingResult> GetByPlan(int planID)
        {
            var entities = _marketingResultRepository.GetAll().Where(c => c.MarketingPlanID == planID);
            return entities;
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
