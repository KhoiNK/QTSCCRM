using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIProject.Model.Models;
using APIProject.Service.DotliquidFilters;
using APIProject.Service.Excel;
using DotLiquid;

namespace APIProject.Service
{
    public class ExcelService : IExcelService
    {
        private string _fileExtension = "xlsx";
        private static FileInfo _quoteTemplateFile;

        public ExcelService()
        {
            string root = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTemplate/";
            if (_quoteTemplateFile == null)
            {
                _quoteTemplateFile = new FileInfo(root + "Báo giá QTSC.xlsx");
            }
        }

        public FileInfo[] GenerateQuoteExcels(Contact contact, Staff staff,
            IEnumerable<QuoteItemMapping> quoteItemMappings, Quote quote)
        {
            List<FileInfo> results = new List<FileInfo>();
            object[] dataHashs = GetQuoteDataObjects(contact, staff, quoteItemMappings.ToList(), quote);
            var excelRenderer = new ExcelRenderer();
            foreach (Hash dataHash in dataHashs)
            {
                Template.RegisterFilter(typeof(CustomFilters));
                var outputFile = new FileInfo($"ExcelOutput/{_quoteTemplateFile.Name.Split('.').First()}_{dataHash["categoryGroup"]}.{_fileExtension}");
                excelRenderer.Render(_quoteTemplateFile, outputFile, dataHash);
                results.Add(outputFile);
            }

            return results.ToArray();
        }

        private Hash[] GetQuoteDataObjects(Contact contact, Staff staff, IList<QuoteItemMapping> quoteItemMappings,
            Quote quote)
        {
            int specialCategoryId = 1;
            List<Hash> dataObjects = new List<Hash>();
            List<SalesCategory> categories =
                quoteItemMappings.Select(mapping => mapping.SalesItem.SalesCategory).ToList();
            if (categories.Any(item => item.ID == specialCategoryId)) //Thuê văn phòng
            {
                SalesCategory category = categories.Find(item => item.ID == 1);
                var itemMappings =
                    quoteItemMappings.Where(mapping => mapping.SalesItem.SalesCategory.ID == specialCategoryId).ToList(); 
                dataObjects.Add(GetQuoteDataObject(contact,staff,category.Name,itemMappings,quote));
                categories = categories.Where(salesCategory => salesCategory.Name != category.Name).ToList();
            }

            if (categories.Count != 0)
            {
                string categoryGroup = "";
                foreach (SalesCategory category in categories)
                {
                    string cateName = category.Name.Split('-').First().Trim();
                    if (String.IsNullOrEmpty(categoryGroup))
                    {
                        categoryGroup += cateName;
                    }
                    else
                    {
                        categoryGroup += " " + cateName;
                    }
                }
                var itemMappings =
                    quoteItemMappings.Where(mapping => mapping.SalesItem.SalesCategory.ID != specialCategoryId).ToList(); 
                dataObjects.Add(GetQuoteDataObject(contact,staff,categoryGroup,itemMappings,quote));
            }

            return dataObjects.ToArray();
        }

        private Hash GetQuoteDataObject(Contact contact, Staff staff, string categoryGroup,
            IList<QuoteItemMapping> quoteItemMappings,
            Quote quote)
        {
            var salesItems = quoteItemMappings.Select(quoteItem => Hash.FromAnonymousObject(new
            {
                quoteItem.SalesItem.Name,
                quoteItem.SalesItem.Price,
                quoteItem.SalesItem.Unit
            }));
            object dataObject = new
            {
                customerName = contact.Customer.Name,
                contactName = contact.Name,
                categoryGroup = categoryGroup,
                salesItems = salesItems,
                tax = quote.Tax,
                discount = quote.Discount,
                staffName = staff.Name,
                staffEmail = staff.Email,
                staffPhonenumber = staff.Phone
            };
            return Hash.FromAnonymousObject(dataObject);
        }
    }

    public interface IExcelService
    {
        FileInfo[] GenerateQuoteExcels(Contact contact, Staff staff, IEnumerable<QuoteItemMapping> quoteItemMappings,
            Quote quote);
    }
}