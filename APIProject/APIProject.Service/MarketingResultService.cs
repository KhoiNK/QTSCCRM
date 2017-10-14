using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
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
        bool CreateResults(List<MarketingResult> list, bool isFinished, int staffID);
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

        public bool CreateResults(List<MarketingResult> list, bool isFinished, int staffID)
        {
            int planID = list.First().MarketingPlanID;
            var foundPlan = _marketingPlanRepository.GetById(planID);
            var foundStaff = _staffRepository.GetById(staffID);

            //verify staff exist
            if (foundStaff == null)
            {
                return false;
            }

            //verify customer and contact
            foreach (var item in list)
            {
                //verify customer exist
                if (item.CustomerID.HasValue)
                {
                    var foundCustomer = _customerRepository.GetById(item.CustomerID.Value);
                    if (foundCustomer == null)
                    {
                        return false;
                    }
                }
                //verify contact exist and in right customer
                if (item.ContactID.HasValue)
                {
                    var foundContact = _contactRepository.GetById(item.ContactID.Value);
                    if (foundContact == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (item.CustomerID.HasValue)
                        {
                            if (foundContact.CustomerID != item.CustomerID)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            //verify plan exist
            if (foundPlan == null)
            {
                return false;
            }
            //verify stage
            if (foundPlan.Stage != RunningName && foundPlan.Stage != ReportingName)
            {
                return false;
            }
            //verify stage and is finished
            if (isFinished)
            {
                if (foundPlan.Stage == RunningName)
                {
                    return false;
                }
            }


            //start adding result code here
            InsertResultsAndLeads(list);
            SendMessageToResults(list);
            foundPlan.ModifiedStaffID = staffID;
            foundPlan.LastModifiedDate = DateTime.Today.Date;
            if (isFinished)
            {
                foundPlan.Stage = FinishedName;
            }
            _unitOfWork.Commit();

            //start sending email code here

            return true;
        }

        //internal insert results and generate lead
        private void InsertResultsAndLeads(List<MarketingResult> resultList)
        {
            foreach (MarketingResult resultItem in resultList)
            {
                if (resultItem.CustomerID.HasValue)
                {
                    if (resultItem.ContactID.HasValue)
                    {
                        //do nothing
                    }
                    else
                    {
                        Contact _item = new Contact
                        {
                            Name = resultItem.ContactName,
                            Email = resultItem.Email,
                            Phone = resultItem.Phone,
                            CustomerID = resultItem.CustomerID
                        };
                        _contactRepository.Add(_item);
                        _unitOfWork.Commit();
                        resultItem.ContactID = _item.ID;
                    }
                }
                else
                {
                    if (resultItem.ContactID.HasValue)
                    {
                        //do nothing
                    }
                    else
                    {
                        resultItem.IsLeadGenerated = true;
                        //add new lead and it's contact
                        Customer _customer = new Customer
                        {
                            Name = resultItem.CustomerName,
                            Address = resultItem.CustomerAddress,
                            IsLead = true
                        };
                        _customerRepository.Add(_customer);
                        _unitOfWork.Commit();
                        resultItem.CustomerID = _customer.ID;

                        Contact _contact = new Contact
                        {
                            Name = resultItem.ContactName,
                            Email = resultItem.Email,
                            Phone = resultItem.Phone,
                            CustomerID = resultItem.CustomerID
                        };
                        _contactRepository.Add(_contact);
                        _unitOfWork.Commit();
                        resultItem.ContactID = _contact.ID;
                    }
                }
                resultItem.CreatedDate = DateTime.Today.Date;
                _marketingResultRepository.Add(resultItem);
            }
            _unitOfWork.Commit();
        }

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
                MailMessage message = new MailMessage(senderEmail,item.Email,mailSubject,mailBody);
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
