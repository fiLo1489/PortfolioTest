﻿using ClosedXML.Excel;
using System;
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

        public void Write(string name, bool result)
        {
            worksheet.Cell(line, 1).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            worksheet.Cell(line, 2).Value = name;
            if (result)
            {
                worksheet.Cell(line, 3).Value = "OK";
                worksheet.Cell(line, 3).Style.Fill.BackgroundColor = XLColor.Green;
            }
            else
            {
                worksheet.Cell(line, 3).Value = "NOK";
                worksheet.Cell(line, 3).Style.Fill.BackgroundColor = XLColor.Red;
            }
            line++;
        }

        public void Close()
        {
            worksheet.Column(1).Style.NumberFormat.Format = "dd-MM-yyyy HH:mm:ss";
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Test.xlsx");
        }
    }
}