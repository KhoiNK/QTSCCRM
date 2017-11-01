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
        IEnumerable<Activity> GetAllActivities();
        List<string> GetActivityTypeNames();
        List<string> GetActivityMethodNames();
        List<string> GetActivityStatusNames();
        IEnumerable<Activity> GetByCustomer(int customerID);
        Activity Get(int id);
        IEnumerable<Activity> GetByOpprtunity(int opportunityID);
        Activity Add(Activity activity);
        void UpdateInfo(Activity activity);
        void SetCancel(Activity activity);
        void SetComplete(Activity activity);
        void MapOpportunity(Activity activity, Opportunity opportunity);
        void SaveChanges();
        void VerifyCanCreateOpportunity(Activity activity);
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
            if (activity.Status == ActivityStatus.Open)
            {
                if (DateTime.Compare(DateTime.Now, activity.TodoTime.Value) >= 0)
                {
                    activity.Status = ActivityStatus.Overdue;
                    _unitOfWork.Commit();
                }
            }
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
            var entities = _activityRepository.GetAll()
                .Where(c => c.OpportunityID == opportunityID && c.IsDelete == false);
            return entities;
        }
        public IEnumerable<Activity> GetByCustomer(int customerID)
        {
            var entities = _activityRepository.GetAll()
                .Where(c => c.CustomerID == customerID && c.IsDelete == false);
            return entities;
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
        public Activity Get(int id)
        {
            var entity = _activityRepository.GetById(id);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.ActivityNotFound);
            }
        }
        public Activity Add(Activity activity)
        {
            var entity = new Activity
            {
                CreateStaffID = activity.CreateStaffID,
                ContactID = activity.ContactID,
                Title = activity.Title,
                Description = activity.Description,
                Type = activity.Type,
                Method = activity.Method,
                TodoTime = activity.TodoTime,
            };
            if (activity.Type == ActivityType.FromCustomer)
            {
                entity.Status = ActivityStatus.Recorded;
            }
            else
            {
                entity.Status = ActivityStatus.Open;
            }
            var customerID = _contactRepository.GetById(activity.ContactID.Value)
                .CustomerID;
            entity.CustomerID = customerID;
            _activityRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }
        public void UpdateInfo(Activity activity)
        {
            var entity = _activityRepository.GetById(activity.ID);
            VerifyCanUpdateInfo(entity);
            entity.Title = activity.Title;
            entity.Description = activity.Description;
            entity.Method = activity.Method;
            entity.TodoTime = activity.TodoTime;
            entity.Status = ActivityStatus.Open;
            _activityRepository.Update(entity);
        }
        public void SetCancel(Activity activity)
        {
            var entity = _activityRepository.GetById(activity.ID);
            VerifyCanSetCancel(entity);
            entity.Status = ActivityStatus.Canceled;
            entity.UpdatedDate = DateTime.Now;
            _activityRepository.Update(entity);
        }
        public void SetComplete(Activity activity)
        {
            var entity = _activityRepository.GetById(activity.ID);
            VerifyCanSetComplete(entity);
            entity.Status = ActivityStatus.Completed;
            entity.CompletedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;
            _activityRepository.Update(entity);
        }
        public void MapOpportunity(Activity activity, Opportunity opportunity)
        {
            var actEntity = _activityRepository.GetById(activity.ID);
            var OppEntity = _opportunityRepository.GetById(opportunity.ID);
            actEntity.OpportunityID = OppEntity.ID;
            actEntity.OfOpportunityStage = OppEntity.StageName;
            _activityRepository.Update(actEntity);
        }
        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
        public void VerifyCanCreateOpportunity(Activity activity)
        {
            var entity = _activityRepository.GetById(activity.ID);
            if (entity.OpportunityID.HasValue)
            {
                throw new Exception(CustomError.ActivityNotForCreateOpportunity);
            }
        }
        #region private verify
        private void VerifyCanUpdateInfo(Activity activity)
        {
            List<string> requiredStatus = new List<string>
            {
                ActivityStatus.Open,
                ActivityStatus.Overdue
            };
            if (!requiredStatus.Contains(activity.Status))
            {
                throw new Exception(CustomError.ActivityStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        private void VerifyCanSetCancel(Activity activity)
        {
            List<string> requiredStatus = new List<string>
            {
                ActivityStatus.Open,
                ActivityStatus.Overdue
            };
            if (!requiredStatus.Contains(activity.Status))
            {
                throw new Exception(CustomError.ActivityStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        private void VerifyCanSetComplete(Activity activity)
        {
            List<string> requiredStatus = new List<string>
            {
                ActivityStatus.Open,
                ActivityStatus.Overdue
            };
            if (!requiredStatus.Contains(activity.Status))
            {
                throw new Exception(CustomError.ActivityStatusRequired
                    + String.Join(", ", requiredStatus));
            }
        }
        #endregion
    }

}
