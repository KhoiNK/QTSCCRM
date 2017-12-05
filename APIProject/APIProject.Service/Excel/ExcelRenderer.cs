using System;
using System.IO;
using DotLiquid;
using OfficeOpenXml;

namespace APIProject.Service.Excel
{
    public class ExcelRenderer
    {
        public void Render(FileInfo templateFile, FileInfo outputFile, Hash data)
        {
            using (var template = new ExcelPackage(templateFile))
            {
                using (var output = new ExcelPackage(outputFile, templateFile))
                {
                    for (int i = 1; i <= template.Workbook.Worksheets.Count; i++)
                    {
                        var templateSheet = template.Workbook.Worksheets[i];
                        var outputSheet = output.Workbook.Worksheets[i];
                        var parser = new ExcelTemplateParser(templateSheet,outputSheet, data);
                        parser.Render();
                    }
                    output.Save();
                }
            }
        }
    }
}