using BaseLib_Net6;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Flow
{
    public static class CopyFlow
    {
        public static bool OpenApproval(ref EdgeDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl(FBaseFunc.Ins.Cfg.CopySector.Url);
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
            }
            catch (Exception ex)
            {
                FBaseFunc.Ins.SetResultMethod(ex.Message);
                FBaseFunc.Ins.SetLog(ex.Message);

                return false;
            }

            return true;
        }

        public static bool CopyExpenseReport(EdgeDriver driver)
        {
            FBaseFunc.CopySectorTable copySector = FBaseFunc.Ins.Cfg.CopySector;
            try
            {
                if (copySector.Title.Length <= 0)
                {
                    FBaseFunc.Ins.SetResultMethod("어? 설정 파일이 바뀐 거 같은데요?\n왜 제목 섹터 파싱 패스가 없죠?\n이럴 수는 없는데....");
                    return false;
                }
                string title = "";
                switch (FBaseFunc.Ins.Cfg.CopySector.Type)
                {
                    case "임시":
                        {
                            title = FindElement(driver, copySector.Title, FBaseFunc.ElementType.ID).GetAttribute("data-value");
                            if (title.Length <= 0)
                            {
                                title = string.Format("임시저장함복사_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                            }
                        }
                        break;
                    case "완료":
                        {
                            title = string.Format("기안완료함복사_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                        }
                        break;
                    default: return false;
                }
                FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\t제목 : {0}", title));
                FBaseFunc.Ins.TempCopyTable.Title = title;

                if (copySector.CompanySector.CompanyCode.Length > 0)
                {
                    IWebElement company = FindElement(driver, copySector.CompanySector.CompanyCode, FBaseFunc.ElementType.CLASS);
                    string companyCode = company.GetAttribute("data-code");
                    if (companyCode != null || companyCode.Length > 0)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\t회사 : {0}", companyCode));
                        FBaseFunc.Ins.TempCopyTable.CompanyValue.CompanyCode = companyCode;
                    }
                    string companyName = company.GetAttribute("data-name");
                    if (companyCode != null || companyCode.Length > 0)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\t회사 : {0}", companyName));
                        FBaseFunc.Ins.TempCopyTable.CompanyValue.CompanyName = companyName;
                    }
                }

                if (copySector.WorkplaceSector.WorkplaceCode.Length > 0)
                {
                    IWebElement workplace = FindElement(driver, copySector.WorkplaceSector.WorkplaceCode, FBaseFunc.ElementType.CLASS);
                    string workplaceCode = workplace.GetAttribute("data-code");
                    if (workplaceCode != null || workplaceCode.Length > 0)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\t사업장 : {0}", workplaceCode));
                        FBaseFunc.Ins.TempCopyTable.WorkplaceValue.WorkplaceCode = workplaceCode;
                    }
                    string workplaceName = workplace.GetAttribute("data-name");
                    if (workplaceName != null || workplaceName.Length > 0)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\t사업장 : {0}", workplaceName));
                        FBaseFunc.Ins.TempCopyTable.WorkplaceValue.WorkplaceName = workplaceName;
                    }
                }

            }
            catch (Exception ex)
            {
                FBaseFunc.Ins.SetResultMethod(ex.Message);
                FBaseFunc.Ins.SetLog(ex.Message);
                return false;
            }

            return FBaseFunc.Ins.Cfg.CopySector.Type switch
            {
                "임시" => CopyTemporaryStoredDoc(driver, copySector),
                "완료" => CopyCompleateDoc(driver, copySector),
                _ => false,
            };
        }

        private static bool CopyTemporaryStoredDoc(EdgeDriver driver, FBaseFunc.CopySectorTable copySector)
        {
            try
            {
                if (copySector.Body.Length <= 0)
                {
                    FBaseFunc.Ins.SetResultMethod("Body XPath has no Value");
                    FBaseFunc.Ins.SetLog("Body XPath has no value");
                    return false;
                }

                ReadOnlyCollection<IWebElement> bodys = FindElement(driver, copySector.Body, FBaseFunc.ElementType.ID).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                int count = bodys.Count / 2;
                for (int i = 0; i < count; i++)
                {
                    if (FBaseFunc.Ins.IsTerminateOn)
                    {
                        FBaseFunc.Ins.SetLog("TerminateOn");
                        return false;
                    }

                    FBaseFunc.SlipTable buff = new FBaseFunc.SlipTable();

                    // 구분 찾기
                    if (copySector.SlipSector.IsEmptyGubun() == false)
                    {
                        ReadOnlyCollection<IWebElement> gubun = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Gubun)).FindElements(By.TagName("input"));
                        for (int j = 0; j < gubun.Count; j++)
                        {
                            if (gubun[j].GetAttribute("checked") != null)
                            {
                                buff.Gubun = gubun[j].GetAttribute("value");
                                break;
                            }
                        }
                    }

                    // 계정 과목
                    if (copySector.SlipSector.Account.AccountName.Length > 0)
                    {
                        buff.Account.SetData(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Account.AccountName)).FindElement(By.TagName("input")).GetAttribute("data-value"));
                    }

                    // 증빙일자
                    if (copySector.SlipSector.IsEmptyTaxDate() == false)
                    {
                        buff.SetTaxTime(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.TaxDate)).FindElement(By.TagName("input")).GetAttribute("data-value"));
                    }

                    // 증빙유형
                    if (copySector.SlipSector.IsEmptyType() == false)
                    {
                        IWebElement type = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Type)).FindElement(By.TagName("select"));
                        buff.Type = type.GetAttribute("data-selectaval");
                        if (buff.Type == null || buff.Type.Length <= 0)
                        {
                            buff.Type = type.FindElement(By.TagName("option")).GetAttribute("value");
                        }
                    }

                    // 거래처
                    if (copySector.SlipSector.Correspondent.CorrespondentName.Length > 0)
                    {
                        buff.Correspondent.SetData(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Correspondent.CorrespondentName)).FindElement(By.TagName("input")).GetAttribute("data-value"));
                    }

                    // 금액
                    if (copySector.SlipSector.IsEmptyPrice() == false)
                    {
                        buff.Price = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Price)).FindElement(By.TagName("input")).GetAttribute("data-value");
                    }

                    // 적요
                    if (copySector.SlipSector.IsEmptyBriefs() == false)
                    {
                        buff.Briefs = bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Briefs)).FindElement(By.TagName("input")).GetAttribute("data-value");
                    }

                    // 사용부서
                    if (copySector.SlipSector.Department.DepartmentName.Length > 0)
                    {
                        string departmentValue = string.Format("{0}/{1}", bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Department.DepartmentName)).GetAttribute("data-code"), bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Department.DepartmentName)).GetAttribute("data-name"));
                        buff.Department.SetData(departmentValue);
                    }

                    // 공급가액
                    if (copySector.SlipSector.IsEmptySupplyPrice() == false)
                    {
                        buff.SupplyPrice = bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.SupplyPrice)).FindElement(By.TagName("input")).GetAttribute("data-value");
                    }

                    FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\n테이블 : {0}", buff.GetString()));
                    FBaseFunc.Ins.TempCopyTable.SlipValues.Add(buff);
                }
            }
            catch (Exception ex)
            {
                FBaseFunc.Ins.SetResultMethod(ex.Message);
                FBaseFunc.Ins.SetLog(ex.Message);
                return false;
            }

            return true;
        }
        private static bool CopyCompleateDoc(EdgeDriver driver, FBaseFunc.CopySectorTable copySector)
        {
            try
            {
                if (copySector.Body.Length <= 0)
                {
                    FBaseFunc.Ins.SetResultMethod("Body XPath has no Value");
                    FBaseFunc.Ins.SetLog("Body XPath has no value");
                    return false;
                }

                ReadOnlyCollection<IWebElement> bodys = FindElement(driver, copySector.Body, FBaseFunc.ElementType.ID).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                int count = bodys.Count / 2;
                for (int i = 0; i < count; i++)
                {
                    if (FBaseFunc.Ins.IsTerminateOn)
                    {
                        FBaseFunc.Ins.SetLog("TerminateOn");
                        return false;
                    }

                    FBaseFunc.SlipTable buff = new FBaseFunc.SlipTable();

                    // 구분 찾기
                    if (copySector.SlipSector.IsEmptyGubun() == false)
                    {
                        ReadOnlyCollection<IWebElement> gubun = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Gubun)).FindElements(By.TagName("input"));
                        for (int j = 0; j < gubun.Count; j++)
                        {
                            if (gubun[j].GetAttribute("checked") != null)
                            {
                                buff.Gubun = gubun[j].GetAttribute("value");
                                break;
                            }
                        }
                    }

                    // 계정 과목
                    if (copySector.SlipSector.Account.AccountName.Length > 0)
                    {
                        buff.Account.SetData(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Account.AccountName)).Text);
                    }

                    // 증빙일자
                    if (copySector.SlipSector.IsEmptyTaxDate() == false)
                    {
                        buff.SetTaxTime(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.TaxDate)).Text);
                    }

                    // 증빙유형
                    if (copySector.SlipSector.IsEmptyType() == false)
                    {
                        buff.Type = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Type)).Text;
                    }

                    // 거래처
                    if (copySector.SlipSector.Correspondent.CorrespondentName.Length > 0)
                    {
                        buff.Correspondent.SetData(bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Correspondent.CorrespondentName)).Text);
                    }

                    // 금액
                    if (copySector.SlipSector.IsEmptyPrice() == false)
                    {
                        buff.Price = bodys[i * 2].FindElement(By.ClassName(copySector.SlipSector.Price)).Text;
                    }

                    // 적요
                    if (copySector.SlipSector.IsEmptyBriefs() == false)
                    {
                        buff.Briefs = bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Briefs)).Text;
                    }

                    // 사용부서
                    if (copySector.SlipSector.Department.DepartmentName.Length > 0)
                    {
                        string department = string.Format("{0}/{1}", bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Department.DepartmentName)).GetAttribute("data-code"), bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.Department.DepartmentName)).GetAttribute("data-name"));
                        buff.Department.SetData(department);
                    }

                    // 공급가액
                    if (copySector.SlipSector.IsEmptySupplyPrice() == false)
                    {
                        buff.SupplyPrice = bodys[i * 2 + 1].FindElement(By.ClassName(copySector.SlipSector.SupplyPrice)).Text;
                    }

                    FBaseFunc.Ins.SetResultMethod(string.Format("[Copy]\n테이블 : {0}", buff.GetString()));
                    FBaseFunc.Ins.TempCopyTable.SlipValues.Add(buff);
                }
            }
            catch (Exception ex)
            {
                FBaseFunc.Ins.SetResultMethod(ex.Message);
                FBaseFunc.Ins.SetLog(ex.Message);
                return false;
            }

            return true;
        }

        private static IWebElement FindElement(EdgeDriver driver, string data, FBaseFunc.ElementType type, int timeout = 0)
        {
            if (timeout == 0)
            {
                timeout = FBaseFunc.Ins.Cfg.Timeout;
            }

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(timeout));
            return type switch
            {
                FBaseFunc.ElementType.XPATH => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(data))),
                FBaseFunc.ElementType.ID => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(data))),
                FBaseFunc.ElementType.CLASS => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName(data))),
                _ => null,
            };
        }
        private static ReadOnlyCollection<IWebElement> FindElements(EdgeDriver driver, string data, FBaseFunc.ElementType type, int timeout = 0)
        {
            if (timeout == 0)
            {
                timeout = FBaseFunc.Ins.Cfg.Timeout;
            }

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(timeout));
            return type switch
            {
                FBaseFunc.ElementType.XPATH => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(data))),
                FBaseFunc.ElementType.ID => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(data))),
                FBaseFunc.ElementType.CLASS => wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName(data))),
                _ => null,
            };
        }
    }
}
