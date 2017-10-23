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
        IEnumerable<Activity> GetAllActivities();
        List<string> GetActivityTypeNames();
        List<string> GetActivityMethodNames();
        List<string> GetActivityStatusNames();
        IEnumerable<Activity> GetByOpprtunity(int opportunityID);
        IEnumerable<Activity> GetByCustomer(int customerID);
        bool SaveChangeActivity(Activity activity);
        bool CompleteActivity(Activity activity);
        bool CancelActivity(Activity activity);
        bool CheckIsOppActivity(int activityID);
    }
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IUnitOfWork _unitOfWork;

        

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


        private void CheckAndChangeOverdue(Activity activity)
        {
            if(activity.Status == ActivityStatus.Open)
            {
                if(DateTime.Compare(DateTime.Now, activity.TodoTime.Value) >= 0)
                {
                    activity.Status = ActivityStatus.Overdue;
                    _unitOfWork.Commit();
                }
            }
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
            if (activity.OpportunityID.HasValue)
            {
                var foundOpp = _opportunityRepository.GetById(activity.OpportunityID.Value);
                if(foundOpp == null)
                {
                    return 0;
                }
                else if(foundOpp.ContactID != activity.ContactID || foundOpp.CreateStaffID != activity.CreateStaffID)
                {
                    return 0;
                }
                else
                {
                    var lastOppActivity = _activityRepository.GetLastOppActivty(foundOpp.ID);
                    if (lastOppActivity.Status == ActivityStatus.Open || lastOppActivity.Status == ActivityStatus.Overdue)
                    {
                        return 0;
                    }
                    else
                    {
                        activity.OfOpportunityStage = foundOpp.StageName;
                    }
                }

            }
            activity.CustomerID = foundContact.Customer.ID;
            
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

            CheckAndChangeOverdue(activity);

            return activity.ID;
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

        public bool SaveChangeActivity(Activity activity)
        {
            var foundActivity = _activityRepository.GetById(activity.ID);
            if(foundActivity != null)
            {
                if (foundActivity.CreateStaffID == activity.CreateStaffID)
                {
                    if (foundActivity.Status == ActivityStatus.Open ||
                        foundActivity.Status == ActivityStatus.Overdue)
                    {
                        if (DateTime.Compare(DateTime.Now, activity.TodoTime.Value) <= 0)
                        {
                            foundActivity.Title = activity.Title;
                            foundActivity.Description = activity.Description;
                            foundActivity.Method = activity.Method;
                            foundActivity.TodoTime = activity.TodoTime;
                            foundActivity.Status = ActivityStatus.Open;
                            _unitOfWork.Commit();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CompleteActivity(Activity activity)
        {
            var foundActivity = _activityRepository.GetById(activity.ID);
            if (foundActivity != null)
            {
                if (foundActivity.CreateStaffID == activity.CreateStaffID)
                {
                    if (foundActivity.Status == ActivityStatus.Open || foundActivity.Status == ActivityStatus.Overdue)
                    {
                        foundActivity.Title = activity.Title;
                        foundActivity.Description = activity.Description;
                        foundActivity.Status = ActivityStatus.Completed;
                        foundActivity.CompletedDate = DateTime.Today.Date;
                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CancelActivity(Activity activity)
        {
            var foundActivity = _activityRepository.GetById(activity.ID);
            if (foundActivity != null)
            {
                if (foundActivity.CreateStaffID == activity.CreateStaffID)
                {
                    if (foundActivity.Status == ActivityStatus.Open || foundActivity.Status == ActivityStatus.Overdue)
                    {
                        foundActivity.Status = ActivityStatus.Canceled;
                        _unitOfWork.Commit();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckIsOppActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID);
            return foundActivity.OpportunityID.HasValue;
        }
    }
}
