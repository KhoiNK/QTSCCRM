using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace APIProject.Helper
{
    public class PdfHelper
    {
        public PdfDocument CreateQuotePdf(string title, List<string> data, double tax, double discount)
        {
            //Create a pdf document.<br> 
            PdfDocument doc = new PdfDocument();
            //margin 
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(3.17f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;
            // Create new page 
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);
            float y = 10;
            //title 
            PdfBrush brush1 = PdfBrushes.Black;
            //PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Times New Roman", 16f, FontStyle.Bold));
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Times New Roman", 16f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Times New Roman", 9f), true);
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("Times New Roman", 14f, FontStyle.Bold), true);
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);
            page.Canvas.DrawString(title, font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);
            y = y + font1.MeasureString(title, format1).Height;
            y = y + 5;
            #region mock data
            //String[] data
            //= {
            //   "Name;Capital;Continent;Area;Population;Oright",
            //   "Argentina;Buenos Aires;South America;2777815;32300003;1",
            //   "Bolivia;La Paz;South America;1098575;7300000;1",
            //   "Brazil;Brasilia;South America;8511196;150400000;1",
            //   "Canada;Ottawa;North America;9976147;26500000;1",
            //   };
            #endregion
            String[][] dataSource
                = new String[data.Count][];
            for (int i = 0; i < data.Count; i++)
            {
                dataSource[i] = data[i].Split(';');
            }

            PdfTable table = new PdfTable();
            table.Style.CellPadding = 2;
            table.Style.HeaderSource = PdfHeaderSource.Rows;
            table.Style.HeaderRowCount = 1;
            table.Style.ShowHeader = true;
            table.Style.HeaderStyle.Font = font2;
            table.Style.DefaultStyle.Font = font2;
            table.Style.AlternateStyle.Font = font2;
            table.DataSource = dataSource;
            PdfLayoutResult result = table.Draw(page, new PointF(0, y));
            y = y + result.Bounds.Height + 5;
            PdfBrush brush2 = PdfBrushes.Gray;
            //PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Times New Roman", 9f));
            //PdfFont font3 = new PdfFont(PdfFontFamily.TimesRoman, 9f);
            page.Canvas.DrawString(String.Format("* Thuế: {0}%", tax.ToString()), font2, brush1, 5, y);
            y = y + font1.MeasureString(String.Format("* Thuế: {0}%", tax.ToString()), format1).Height;
            //y = y + 5;
            page.Canvas.DrawString(String.Format("* Giảm giá: {0}%", discount.ToString()), font2, brush1, 5, y);

            //Save pdf file. 
            //SaveFileHelper saveHelper = new SaveFileHelper();
            //string fileRoot = HttpContext.Current.Server.MapPath("~/Resources/CustomerAvatarFiles/");
            //doc.SaveToFile(fileRoot +"SimpleTable.pdf");
            //doc.Close();
            return doc;
            //System.Diagnostics.Process.Start("SimpleTable.pdf");
        }
    }
}