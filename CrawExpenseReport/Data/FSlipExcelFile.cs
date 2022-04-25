using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace CrawExpenseReport.Data
{
    public class FSlipExcelFile
    {
        public void CreateSampleFile(string path, bool isIndustrial, out string err)
        {
            List<List<string>> data = new List<List<string>>();
            data.Add(new List<string>()
            {
                "제목","회사","사업장","구분","계정","증빙일자","증빙유형","거래처","금액","적요","부서","공급가액"
            });

            if (isIndustrial)
            {
                data.Add(new List<string>()
                {
                    "csv테스트 파일","3099/(주)신성산업2021","2000/(주)신성산업_서울","차변","10100/현금","2021-08-23","개인카드","00101/극동제연공업(주)","1000","적요1","1010/경영지원실","0"
                });
                data.Add(new List<string>()
                {
                    "","","","대변","13500/부가세대급금","2021-08-23","기타","00102/동아특수화학(주)","100","적요 2","1021/재경팀","1000"
                });
            }
            else
            {
                data.Add(new List<string>()
                {
                    "csv테스트 파일","1000/(주)신성유화","1001/(주)신성유화.본점","차변","10100/현금","2021-08-23","개인카드","001001/현대자동차(주) 남양연구소","1234","적요1","1000/경영진","0"
                });
                data.Add(new List<string>()
                {
                    "","","","대변","13500/부가세대급금","2021-08-23","기타","001009/현대모비스(주) 울산1공장","1234","적요 2","1001/공통","4321"
                });
            }

            WriteFile(path, data, out err);
        }
        public List<List<string>> ReadFile(string path, out string err)
        {
            List<List<string>> data = new List<List<string>>();
            err = "";

            Application app = new Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            try
            {
                workbook = app.Workbooks.Open(path);
                worksheet = workbook.Worksheets.Item[1] as Worksheet;
                Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;
                for (int i = 1; i <= range.Rows.Count; i++)
                {
                    List<string> temp = new List<string>();
                    for (int j = 1; j <= range.Columns.Count; j++)
                    {
                        Microsoft.Office.Interop.Excel.Range buff = (range.Cells[i, j] as Microsoft.Office.Interop.Excel.Range);
                        if (buff.Value == null)
                        {
                            temp.Add("");
                        }
                        else
                        {
                            temp.Add(buff.Value.ToString());
                        }
                    }
                    data.Add(temp);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            finally
            {
                ReleaseObject(worksheet);
                if (workbook != null)
                {
                    workbook.Close();
                }
                ReleaseObject(workbook);
                app.Quit();
                ReleaseObject(app);
            }

            return data;
        }
        public void WriteFile(string path, List<List<string>> data, out string err)
        {
            err = "";
            Application app = new Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            try
            {
                workbook = app.Workbooks.Add();
                worksheet = workbook.Sheets.Add() as Worksheet;
                worksheet.Name = "샘플";
                int rowCount = 1;
                int colCount = 1;
                foreach (List<string> rows in data)
                {
                    foreach (string col in rows)
                    {
                        worksheet.Cells[rowCount, colCount] = col;
                        colCount++;
                    }

                    rowCount++;
                    colCount = 1;
                }
                worksheet.Columns.AutoFit();
                app.ActiveWorkbook.SaveAs(path, XlFileFormat.xlOpenXMLWorkbook);
                workbook.Close();
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            finally
            {
                app.Quit();
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(app);
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.FinalReleaseComObject(obj);
                    obj = null;
                }
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
