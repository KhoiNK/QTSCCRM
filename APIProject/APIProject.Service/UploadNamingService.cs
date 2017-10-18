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
        string GetContactAvatarNaming();
    }

    public class UploadNamingService : IUploadNamingService
    {
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
    }
}
