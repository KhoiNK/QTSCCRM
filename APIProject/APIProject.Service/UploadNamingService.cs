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
    public interface IUploadNamingService
    {
        string GetCustomerAvatarNaming();
        void ConcatCustomerAvatar(Customer customer);
        string GetContactAvatarNaming();
        void ConcatContactAvatar(Contact contact);
        string GetStaffAvatarNaming();
        void ConcatStaffAvatar(Staff staff);

    }

    public class UploadNamingService : IUploadNamingService
    {
        private readonly IAppConfigRepository _appConfigRepository;

        public UploadNamingService(IAppConfigRepository _appConfigRepository)
        {
            this._appConfigRepository = _appConfigRepository;
        }

        public string GetContactAvatarNaming()
        {
            DateTime date = DateTime.Now;
            return Guid.NewGuid().ToString() + "_" + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second;
        }

        public string GetCustomerAvatarNaming()
        {
            DateTime date = DateTime.Now;
            return Guid.NewGuid().ToString() + "_" + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second;
        }

        public void ConcatCustomerAvatar(Customer customer)
        {
            if (customer.AvatarSrc != null)
            {
                customer.AvatarSrc = _appConfigRepository.GetHost() + "/"
                    + FileDirectory.CustomerAvatarFolder + "/"
                    + customer.AvatarSrc;
            }
        }

        public void ConcatContactAvatar(Contact contact)
        {
            if (contact.AvatarSrc != null)
            {
                contact.AvatarSrc = _appConfigRepository.GetHost() + "/"
                    + FileDirectory.ContactAvatarFolder + "/"
                    + contact.AvatarSrc;
            }
        }

        public string GetStaffAvatarNaming()
        {
            DateTime date = DateTime.Now;
            return Guid.NewGuid().ToString() + "_" + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second;
        }
        public void ConcatStaffAvatar(Staff staff)
        {
            staff.AvatarSrc = _appConfigRepository.GetHost() + "/"
                    + FileDirectory.StaffAvatarFolder + "/"
                    + staff.AvatarSrc;
        }

    }
}
