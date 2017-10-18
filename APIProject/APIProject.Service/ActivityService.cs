using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
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

            TypeNames = new List<string>
            {
                FromCustomerName,
                ToCustomerName,
            };

            MethodNames = new List<string>
            {
                "Email",
                "Gọi điện",
                "Gặp trực tiếp",
            };
        }

        public int CreateNewActivity(Activity activity)
        {
            var foundStaff = _staffRepository.GetById(activity.ModifiedStaffID.Value);
            if (foundStaff == null)
            {
                return 0;
            }
            var foundCustomer = _customerRepository.GetById(activity.CustomerID.Value);
            if (foundCustomer == null)
            {
                return 0;
            }
            if (foundCustomer.Contacts.Where(c => c.ID == activity.ContactID.Value).SingleOrDefault() == null)
            {
                return 0;
            }
            if (activity.OpportunityID.HasValue)
            {
                if(_opportunityRepository.GetById(activity.OpportunityID.Value) == null)
                {
                    return 0;
                }
            }
            if (!TypeNames.Contains(activity.Type))
            {
                return 0;
            }
            if (!MethodNames.Contains(activity.Method))
            {
                return 0;
            }
            var dateTimeNow = DateTime.Today.Date;
            if(activity.Type == ToCustomerName)
            {
                activity.Status = OpenStatusName;
            }
            else
            {
                activity.Status = FinishedStatusName;
                activity.CompletedDate = dateTimeNow;

            }
            activity.CreateStaffID = activity.ModifiedStaffID;
            activity.CreatedDate = dateTimeNow;

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
            return MethodNames;
        }

        public List<string> GetActivityTypeNames()
        {
            return TypeNames;
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

    }
}
