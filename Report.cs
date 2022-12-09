using ClosedXML.Excel;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SemestralnaPracaTest
{
    public class Report
    {
        private XLWorkbook workbook;
        private IXLWorksheet worksheet;
        private int line;

        public Report()
        {
            workbook = new XLWorkbook();
            worksheet = workbook.Worksheets.Add("Test");
            line = 1;
        }

        public void Write(string name, Exception exception, Dictionary<string, string> input)
        {
            worksheet.Cell(line, 1).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            worksheet.Cell(line, 2).Value = name;
            worksheet.Cell(line, 2).Style.Font.Bold = true;
            if (exception == null)
            {
                worksheet.Cell(line, 3).Value = "OK";
                worksheet.Cell(line, 3).Style.Fill.BackgroundColor = XLColor.Green;
            }
            else
            {
                worksheet.Cell(line, 3).Value = "NOK";
                worksheet.Cell(line, 3).Style.Fill.BackgroundColor = XLColor.Red;
                worksheet.Cell(line, 4).Value = exception.Message;
            }
            line++;

            if (input != null)
            {
                foreach (KeyValuePair<string, string> item in input)
                {
                    worksheet.Cell(line, 3).Value = item.Key;
                    worksheet.Cell(line, 4).Value = item.Value;
                    line++;
                }
            }
        }

        public void Save()
        {
            worksheet.Column(1).Style.NumberFormat.Format = "dd.MM.yyyy HH:mm:ss";
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Test.xlsx");
        }
    }
}