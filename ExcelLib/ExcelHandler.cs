using System;
using Excel = Microsoft.Office.Interop.Excel;
namespace ExcelLib
{
    public class ExcelHandler
    {
        public static string ReadNxMCellsFromFirstWorksheet(string filename, string n, string m)
        {
            var excelApplication = new Excel.Application();

            excelApplication.Visible = false;
            var path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            //var path = "C:\\Users\\ivoro\\Desktop\\IS\\projeto\\Project\\";
            var excelWorkbook = excelApplication.Workbooks.Open(path + filename);
            var excelWorksheet = excelWorkbook.Worksheets.get_Item(1);

            Excel.Range myRange = excelWorksheet.Range(n, m);

            var content = myRange.Value;

            excelWorkbook.Close();
            excelApplication.Quit();

            ReleaseCOMObject(excelWorksheet);
            ReleaseCOMObject(excelWorkbook);
            ReleaseCOMObject(excelApplication);

            string res = "";

            foreach (var c in content)
            {
                if (c is char)
                {
                    return content;
                }
               
                res += c + ";";
            }
            return res;
        }

        public static DateTime ReadUpdatedGeolocationData(string filename)
        {
            var excelApplication = new Excel.Application();

            excelApplication.Visible = false;
            var path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            //var path = "C:\\Users\\ivoro\\Desktop\\IS\\projeto\\Project\\";
            var excelWorkbook = excelApplication.Workbooks.Open(path + filename);
            var excelWorksheet = excelWorkbook.Worksheets.get_Item(1);

            Excel.Range myRange = excelWorksheet.Range("B3", "B3");

            var content = myRange.Value;

            excelWorkbook.Close();
            excelApplication.Quit();

            ReleaseCOMObject(excelWorksheet);
            ReleaseCOMObject(excelWorkbook);
            ReleaseCOMObject(excelApplication);
            return content;
        }

        private static void ReleaseCOMObject(Object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
                System.Diagnostics.Debug.WriteLine("Error releasing COM object: " + e.Message);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
