using OfficeOpenXml;

namespace APIProject.Service.Excel
{
    internal interface IExcelElement
    {
        int SetupSpace(ExcelWorksheet templateSheet, ExcelWorksheet outputSheet, int rowOffset);
        int ApplyValue(ExcelWorksheet templateSheet, ExcelWorksheet outputSheet, int rowOffset);
        void ApplyStyle(ExcelWorksheet templateSheet, ExcelWorksheet outputSheet, int rowOffset);
    }
}