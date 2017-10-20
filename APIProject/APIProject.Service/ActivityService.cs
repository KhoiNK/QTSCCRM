using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{

    public interface IActivityService
    {
        int CreateNewActivity(Activity activity);
        bool EditActivity(Activity activity);
        IEnumerable<Activity> GetAllActivities();
        List<string> GetActivityTypeNames();
        List<string> GetActivityMethodNames();
        List<string> GetActivityStatusNames();
        bool FinishActivity(Activity activity);
        IEnumerable<Activity> GetByOpprtunity(int opportunityID);
        IEnumerable<Activity> GetByCustomer(int customerID);
    }
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly string OpenStatusName = "Chưa thực hiện";
        private readonly string FinishedStatusName = "Hoàn thành";
        private readonly string FromCustomerName = "Từ khách hàng";
        private readonly string ToCustomerName = "Đến khách hàng";
        private List<string> TypeNames;
        private List<string> MethodNames;

        public ActivityService(IActivityRepository _activityRepository, IUnitOfWork _unitOfWork,
            IStaffRepository _staffRepository, ICustomerRepository _customerRepository,
            IContactRepository _contactRepository, IOpportunityRepository _opportunityRepository)
        {
            this._activityRepository = _activityRepository;
            this._staffRepository = _staffRepository;
            this._customerRepository = _customerRepository;
            this._contactRepository = _contactRepository;
            this._opportunityRepository = _opportunityRepository;
            this._unitOfWork = _unitOfWork;

            
        }

        public int CreateNewActivity(Activity activity)
        {
            var foundStaff = _staffRepository.GetById(activity.CreateStaffID.Value);
            if (foundStaff == null)
            {
                return 0;
            }

            var foundContact = _contactRepository.GetById(activity.ContactID.Value);
            if(foundContact == null)
            {
                return 0;
            }
            activity.CustomerID = foundContact.Customer.ID;
            
            //if (activity.OpportunityID.HasValue)
            //{
            //    var foundOpportunity = _opportunityRepository.GetById(activity.OpportunityID.Value);
            //    if (foundOpportunity == null)
            //    {
            //        return 0;
            //    }
            //    else
            //    {
            //        activity.OfOpportunityStage = foundOpportunity.StageName;
            //    }
            //}
            if (ActivityType.GetList().Contains(activity.Type))
            {
                if(activity.Type == ActivityType.FromCustomer)
                {
                    activity.Status = ActivityStatus.Recorded;
                }
                else
                {
                    activity.Status = ActivityStatus.Open;
                }
            }
            else
            {
                return 0;
            }
            if (!ActivityMethod.GetList().Contains(activity.Method))
            {
                return 0;
            }
            
            activity.CreatedDate = DateTime.Today.Date;

            _activityRepository.Add(activity);
            _unitOfWork.Commit();
            return activity.ID;
        }

        public bool EditActivity(Activity activity)
        {

            return true;
        }

        public bool FinishActivity(Activity activity)
        {
            var foundActivity = _activityRepository.GetById(activity.ID);
            if(foundActivity != null)
            {
                if (foundActivity.CreateStaffID == activity.ModifiedStaffID)
                {
                    if (foundActivity.Status != FinishedStatusName)
                    {
                        foundActivity.CompletedDate = DateTime.Today.Date;
                        foundActivity.Status = FinishedStatusName;
                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> GetActivityMethodNames()
        {
            return new List<string>
            {
                ActivityMethod.Email,
                ActivityMethod.Direct,
                ActivityMethod.Phone
            };
        }

        public List<string> GetActivityTypeNames()
        {
            return new List<string>
            {
                ActivityType.FromCustomer,
                ActivityType.ToCustomer
            };
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            return _activityRepository.GetAll();
        }

        public IEnumerable<Activity> GetByOpprtunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if(foundOpportunity != null)
            {
                var activities = foundOpportunity.Activities;
                if (activities.Any())
                {
                    return foundOpportunity.Activities;
                }
            }
            return null;
        }

        public IEnumerable<Activity> GetByCustomer(int customerID)
        {
            var foundCustomer = _customerRepository.GetById(customerID);
            if(foundCustomer != null)
            {
                var activities = foundCustomer.Activities;
                if (activities.Any())
                {
                    return activities;
                }
            }
            return null;
        }

        public List<string> GetActivityStatusNames()
        {
            return new List<string>
            {
                ActivityStatus.Open,
                ActivityStatus.Overdue,
                ActivityStatus.Completed,
                ActivityStatus.Canceled,
                ActivityStatus.Recorded
            };
        }
    }
}
