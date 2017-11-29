using APIProject.Model.Models;
using APIProject.ViewModels;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        public bool SaveStaffImage(string fileName, string b64Content)
        {
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/StaffAvatarFiles");
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

        public CustomB64ImageFileViewModel GetCustomerAvatarBase64View(Customer customer)
        {
            try
            {
                string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/CustomerAvatarFiles");
                string fileName = Path.GetFileName(customer.AvatarSrc);
                string filePath = Path.Combine(fileRoot, fileName);

                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                var extension = Path.GetExtension(filePath).Replace(".", "");
                string firstConcat = "data:image/" + extension + ";base64,";
                return new CustomB64ImageFileViewModel
                {
                    Name = fileName,
                    Base64Content = firstConcat + base64ImageRepresentation
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public CustomB64ImageFileViewModel GetContactAvatarBase64View(Contact contact)
        {
            try
            {
                string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/ContactAvatarFiles");
                string filePath = Path.Combine(fileRoot, contact.AvatarSrc);
                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                var extension = Path.GetExtension(filePath).Replace(".","");
                string firstConcat = "data:image/" + extension + ";base64,";
                return new CustomB64ImageFileViewModel
                {
                    Name = contact.AvatarSrc,
                    Base64Content = firstConcat +base64ImageRepresentation
                };
            }catch(Exception e)
            {
                return null;
            }
        }
        public string SavePdfQuoteFile(PdfDocument doc, string fileName)
        {
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/QuoteFiles");
            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }
            string filePath = Path.Combine(fileRoot, fileName);
            doc.SaveToFile(filePath);
            doc.Close();
            return filePath;
        }
        public void SaveMarketingResult()
        {
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/MarketingResultFiles");
            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }
            var provider = new MultipartFormDataContent(fileRoot);
        }
    }
}