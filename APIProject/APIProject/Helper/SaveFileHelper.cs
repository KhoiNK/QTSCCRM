using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace APIProject.Helper
{
    public class SaveFileHelper
    {
        public bool SaveCustomerImage(string fileName, string b64Content)
        {
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/CustomerAvatarFiles");
            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }
            string filePath = Path.Combine(fileRoot, fileName);
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(b64Content));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveContactImage(string fileName, string b64Content)
        {
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/ContactAvatarFiles");
            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }
            string filePath = Path.Combine(fileRoot, fileName);
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(b64Content));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}