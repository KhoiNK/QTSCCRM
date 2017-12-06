﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIProject.Model.Models;
using APIProject.Service.DotliquidFilters;
using APIProject.Service.Excel;
using DotLiquid;
using System.Web;

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
            Hash[] dataHashs = GetQuoteDataObjects(contact, staff, quoteItemMappings.ToList(), quote);
            var excelRenderer = new ExcelRenderer();
            string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/QuoteFiles");
            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }
            foreach (Hash dataHash in dataHashs)
            {
                Template.RegisterFilter(typeof(CustomFilters));
                var fileName = $"{_quoteTemplateFile.Name.Split('.').First()}_{dataHash["categoryGroup"]}_{Guid.NewGuid()}.{_fileExtension}";

                string filePath = Path.Combine(fileRoot, fileName);

                //var outputFile =
                //    new FileInfo(
                //        $"ExcelOutput/{_quoteTemplateFile.Name.Split('.').First()}_{dataHash["categoryGroup"]}.{_fileExtension}");
                var outputFile = new FileInfo(filePath);
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
            if (categories.Any(item => item.ID == specialCategoryId))
            {
                SalesCategory category = categories.Find(item => item.ID == 1);
                var itemMappings =
                    quoteItemMappings.Where(mapping => mapping.SalesItem.SalesCategory.ID == specialCategoryId)
                        .ToList();
                dataObjects.Add(GetQuoteDataObject(contact, staff, "dịch vụ thuê văn phòng", itemMappings, quote));
                categories = categories.Where(salesCategory => salesCategory.Name != category.Name).ToList();
            }

            if (categories.Count != 0)
            {
                var itemMappings =
                    quoteItemMappings.Where(mapping => mapping.SalesItem.SalesCategory.ID != specialCategoryId)
                        .ToList();
                foreach (SalesCategory category in categories)
                {
                    string cateName = category.Name.Split('-').First().Trim();
                    foreach (QuoteItemMapping itemMapping in itemMappings)
                    {
                        itemMapping.SalesItemName = cateName + " " + itemMapping.SalesItemName;
                    }
                }

                dataObjects.Add(GetQuoteDataObject(contact, staff, "dịch vụ viễn thông", itemMappings, quote));
            }

            return dataObjects.ToArray();
        }

        private Hash GetQuoteDataObject(Contact contact, Staff staff, string categoryGroup,
            IList<QuoteItemMapping> quoteItemMappings,
            Quote quote)
        {
            var salesItems = quoteItemMappings.Select(quoteItem => Hash.FromAnonymousObject(new
            {
                Name = quoteItem.SalesItemName,
                Price = quoteItem.Price,
                Unit = quoteItem.Unit
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