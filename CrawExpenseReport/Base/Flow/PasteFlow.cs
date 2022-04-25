using BaseLib_Net6;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Flow
{
    public static class PasteFlow
    {
        public static bool OpenApproval()
        {
            try
            {
                string url = FBaseFunc.Ins.DefaultURL();
                if (url[url.Length - 1] != '/')
                {
                    url = url + "/";
                }

                App.Web.NavigateURL($"{url}{FBaseFunc.Ins.Cfg.ApprovalUrl}");
                FBaseFunc.Ins.IsLoading(FBaseFunc.Ins.Cfg.BtnNew, FBaseFunc.ElementType.XPATH);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BtnNew, 0, FBaseFunc.ElementType.XPATH), out bool err);
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                FBaseFunc.Ins.ExecuteScript("$('#searchInput').val('지출/수입'); $('#searchInput').trigger('keyup');", out err);
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BodyNew, 0, FBaseFunc.ElementType.XPATH), out err);
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BtnConfirm, 0, FBaseFunc.ElementType.XPATH), out err);
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

        public static bool PasteExpenseReportDirectSpeedUp()
        {
            if (FBaseFunc.Ins.SelectedList == -1)
            {
                FBaseFunc.Ins.SetResultMethod("선택된 리스트가 없습니다.");

                return false;
            }

            FBaseFunc.CopySectorTable copySector = FBaseFunc.Ins.Cfg.CopySector;
            FBaseFunc.CopyDataTable copyData = FBaseFunc.Ins.CopyedTable[FBaseFunc.Ins.SelectedList];
            FBaseFunc.PasteSectorTable pasteSector = FBaseFunc.Ins.Cfg.PasteSector;

            string errRet = FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetValue(copySector.Title, copyData.Title, 0, FBaseFunc.ElementType.ID), out bool err);
            if (err)
            {
                FBaseFunc.Ins.SetResultMethod("제목을 찾지 못했습니다.");
                FBaseFunc.Ins.SetResultMethod(errRet);
                FBaseFunc.Ins.SetLog(errRet);
            }

            // 회사
            if (copyData.CompanyValue.CompanyCode.Length > 0)
            {
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetAttribute(copySector.CompanySector.CompanyCode, "data-code", copyData.CompanyValue.CompanyCode, 0, FBaseFunc.ElementType.CLASS), out err);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetAttribute(copySector.CompanySector.CompanyCode, "data-name", copyData.CompanyValue.CompanyName, 0, FBaseFunc.ElementType.CLASS), out err);
                FBaseFunc.Ins.ExecuteScript($"document.getElementsByClassName{copySector.CompanySector.CompanyCode}[0].children[0].value = {copyData.CompanyValue.CompanyName}", out err);
                FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t회사 : {0}", copyData.CompanyValue.GetCompanyName()));
            }

            // 사업장
            if (copyData.WorkplaceValue.WorkplaceCode.Length > 0)
            {
                Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetAttribute(copySector.WorkplaceSector.WorkplaceCode, "data-code", copyData.WorkplaceValue.WorkplaceCode, 0, FBaseFunc.ElementType.CLASS), out err);
                FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetAttribute(copySector.WorkplaceSector.WorkplaceCode, "data-name", copyData.WorkplaceValue.WorkplaceName, 0, FBaseFunc.ElementType.CLASS), out err);
                FBaseFunc.Ins.ExecuteScript($"document.getElementsByClassName{copySector.WorkplaceSector.WorkplaceCode}[0].children[0].value = {copyData.WorkplaceValue.WorkplaceName}", out err);
                FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t사업장 : {0}", copyData.WorkplaceValue.GetWorkplaceName()));
            }

            int slipCount = copyData.SlipValues.Count;
            int retryCount = FBaseFunc.Ins.Cfg.RetryCount;
            // 행 추가
            for (int i = 0; i < slipCount - 1; i++)
            {
                errRet = FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BtnAddRow), out err);
                if (err)
                {
                    FBaseFunc.Ins.SetResultMethod(string.Format("{0}번째 행추가 실패", i + 1));
                    FBaseFunc.Ins.SetLog(string.Format("{0}번째 행추가 실패", i + 1));
                    FBaseFunc.Ins.SetResultMethod(errRet);
                    FBaseFunc.Ins.SetLog(errRet);
                }
            }

            //// 전표 테이블
            string copySectorBody = $"document.getElementById('{copySector.Body}').children[1]";
            err = !int.TryParse(FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.childElementCount;", out err), out int slipBodyCount);
            if (err)
            {
                FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t열찾기 실패"));
                return false;
            }

            for (int i = 0; i < slipCount; i++)
            {
                if (FBaseFunc.Ins.IsTerminateOn)
                {
                    FBaseFunc.Ins.SetLog("TerminateOn");
                    return false;
                }

                // 행 추가
                if (i >= slipBodyCount / 2)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        errRet = FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BtnAddRow), out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t행추가 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t행추가 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t열추가 {0}회 시도 실패", retryCount));
                        return false;
                    }
                    err = !int.TryParse(FBaseFunc.Ins.ExecuteScript($"document.getElementById('{copySector.Body}').children[1].childElementCount;", out err), out slipBodyCount);
                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t열찾기 실패"));
                        return false;
                    }
                }

                FBaseFunc.SlipTable slipValue = copyData.SlipValues[i];

                // 금액
                if (slipValue.Price.Length > 0 && copySector.SlipSector.IsEmptyPrice() == false)
                {
                    errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Price}')[0].children[0].value = '{slipValue.Price}';", out err);
                    FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t금액 : {0}", slipValue.Price));
                }

                // 구분 찾기
                if (slipValue.Gubun != "차변" && copySector.SlipSector.IsEmptyGubun() == false)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        Thread.Sleep(FBaseFunc.Ins.Cfg.DelayTime);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Gubun}')[0].getElementsByTagName('input')[1].click();", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t구분 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t구분 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t구분 선택 {0}회 시도 실패", retryCount));
                        return false;
                    }
                }

                // 계정 과목
                bool vattyn = false;
                bool trcdty = true;
                if (slipValue.Account.AccountCode.Length > 0 && copySector.SlipSector.Account.AccountName.Length > 0)
                {
                    if (slipValue.Account.AccountType.Contains("A") == false)
                    {
                        trcdty = false;
                    }

                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        if (slipValue.Account.AccountName.Contains("부가세"))
                        {
                            errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-vatyn', 'Y');", out err);
                            vattyn = true;
                        }
                        else
                        {
                            errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-vatyn', 'N');", out err);
                        }

                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-trcdty', '{slipValue.Account.AccountType}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-drcr', '{slipValue.Account.AccountDebit}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-code', '{slipValue.Account.AccountCode}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].setAttribute('data-name', '{slipValue.Account.AccountName}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Account.AccountName}')[0].children[0].value = '{slipValue.Account.GetAccountName()}';", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t계정 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t계정 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t계정 : {0}", slipValue.Account.GetAccountName()));
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t계정 선택 {0}회 시도 실패", retryCount));
                    }
                }

                // 증빙일자
                if (slipValue.TaxDate.Length > 0 && copySector.SlipSector.IsEmptyTaxDate() == false)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.TaxDate}')[0].children[0].value = '{slipValue.TaxDate}';", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t증빙일자 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t증빙일자 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t증빙일자 {0}회 시도 실패", retryCount));
                    }
                }

                // 증빙유형
                if (slipValue.Type.Contains("과세") == false && slipValue.Type.Length > 0 && copySector.SlipSector.IsEmptyType() == false)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Type}')[0].children[0].value = '{slipValue.Type}';", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Type}')[0].children[0].setAttribute('data-selectval', '{slipValue.Type}');", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t증빙유형 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t증빙유형 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t증빙유형 {0}회 시도 실패", retryCount));
                    }
                }

                // 거래처
                if (trcdty && slipValue.Correspondent.CorrespondentCode.Length > 0 && copySector.SlipSector.Correspondent.CorrespondentName.Length > 0)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Correspondent.CorrespondentName}')[0].setAttribute('data-code', '{slipValue.Correspondent.CorrespondentCode}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Correspondent.CorrespondentName}')[0].setAttribute('data-name', '{slipValue.Correspondent.CorrespondentName}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Correspondent.CorrespondentName}')[0].setAttribute('data-regnb', '{slipValue.Correspondent.CorrespondentName}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2}].getElementsByClassName('{copySector.SlipSector.Correspondent.CorrespondentName}')[0].children[0].value = '{slipValue.Correspondent.GetCorrespondentName()}';", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t거래처 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t거래처 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t거래처 선택 {0}회 시도 실패", retryCount));
                    }
                }

                // 적요
                if (slipValue.Briefs.Length > 0 && copySector.SlipSector.IsEmptyBriefs() == false)
                {
                    errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Briefs}')[0].children[0].value = '{slipValue.Briefs}';", out err);
                    FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t적요 : {0}", slipValue.Briefs));
                }

                // 사용부서
                if (slipValue.Department.DepartmentCode.Length > 0 && copySector.SlipSector.Department.DepartmentName.Length > 0)
                {
                    for (int retry = 0; retry < retryCount; retry++)
                    {
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Department.DepartmentName}')[0].setAttribute('data-code', '{slipValue.Department.DepartmentCode}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Department.DepartmentName}')[0].setAttribute('data-name', '{slipValue.Department.DepartmentName}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Department.DepartmentName}')[0].children[0].setAttribute('data-code', '{slipValue.Department.DepartmentCode}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Department.DepartmentName}')[0].children[0].setAttribute('data-div-cd', '{slipValue.Department.WorkplaceCode}');", out err);
                        errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.Department.DepartmentName}')[0].children[0].value = '{slipValue.Department.GetDepartmentName()}';", out err);
                        if (err)
                        {
                            FBaseFunc.Ins.SetLog(errRet);
                            FBaseFunc.Ins.SetLog(string.Format("[ERR]\t부서 선택 fail count {0}", retry + 1));
                            FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t부서 선택 fail count {0}", retry + 1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        FBaseFunc.Ins.SetResultMethod(string.Format("[ERR]\t부서 선택 {0}회 시도 실패", retryCount));
                    }
                }

                // 공급가액
                if (vattyn && slipValue.SupplyPrice.Length > 0 && slipValue.SupplyPrice != "0" && copySector.SlipSector.IsEmptySupplyPrice() == false)
                {
                    errRet = FBaseFunc.Ins.ExecuteScript($"{copySectorBody}.children[{i * 2 + 1}].getElementsByClassName('{copySector.SlipSector.SupplyPrice}')[0].children[0].value = '{slipValue.SupplyPrice}';", out err);
                    FBaseFunc.Ins.SetResultMethod(string.Format("[Paste]\t공급가액 : {0}", slipValue.SupplyPrice));
                }
            }

            return true;
        }
    }
}
