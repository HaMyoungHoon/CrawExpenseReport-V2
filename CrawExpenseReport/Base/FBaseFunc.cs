using BaseLib_Net6;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Text;
using CrawExpenseReport.Data;
using System.Collections.Generic;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using CrawExpenseReport.Base.Rest;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using CrawExpenseReport.Screen.Popup;
using System.Threading;

namespace CrawExpenseReport.Base
{
    public partial class FBaseFunc : FThread
    {
        public FBaseFunc()
        {
            _paletteHelper = new MaterialDesignExtensions.Themes.PaletteHelper();
            TempCopyTable = new CopyDataTable();
            CopyedTable = new List<CopyDataTable>();

            IsSettingOn = false;
            IsCopyRun = false;
            IsPasteRun = false;
            IsTerminateOn = false;

            SelectedList = -1;
            CurrentUrl = "";
            _ret = new Queue<string>();
            _testCmd = new Queue<string>();
        }

        public override bool ProcThread1()
        {
            IsCopyRun = true;
            GuardScreenSaver(true);
            Thread.Sleep(1000);
            bool ret = Flow.StartFlow.Login();
            if (ret)
            {
                SetResultMethod("로그인 완료");

                TempCopyTable.Clear();
//                ret = Flow.CopyFlow.OpenApproval();
//                if (ret)
//                {
//                    ret = Flow.CopyFlow.CopyExpenseReport();
//                }
                if (ret)
                {
//                    Cfg.SaveCopyListFile(TempCopyTable.Clone(), SelectedList);
                    SetResultMethod("복사 완료");

                    if (_copyEnd != null)
                    {
                        _copyEnd(true);
                    }
                }
                else
                {
                    SetResultMethod("복사 실패");
                }
            }
            else
            {
                SetResultMethod("로그인 실패");
            }
            GuardScreenSaver(false);

            IsCopyRun = false;
            return base.ProcThread1();
        }
        public override bool ProcThread2()
        {
            IsPasteRun = true;
            bool ret = Flow.StartFlow.Login();
            if (ret)
            {
                SetResultMethod("로그인 완료");
                ret = Flow.PasteFlow.OpenApproval();
                if (ret)
                {
                    SetLog("시작");
                    ret = Flow.PasteFlow.PasteExpenseReportDirectSpeedUp();
                    SetLog("끝");
                }

                if (ret)
                {
                    SetResultMethod("붙여넣기 완료");
                }
                else
                {
                    SetResultMethod("붙여넣기 실패");
                }

                if (_pasteEnd != null)
                {
                    _pasteEnd(true);
                }
            }
            else
            {
                SetResultMethod("로그인 실패");
            }

            IsPasteRun = false;
            return base.ProcThread2();
        }

        public void SetCallBack(BreakMethod breakMethod, ResultMethod resultMethod)
        {
            _breakMethod = breakMethod;
            _resultMethod = resultMethod;
        }
        public void SetCopyEndCallback(ResultEnd copyEnd)
        {
            if (_copyEnd == null)
            {
                _copyEnd = copyEnd;
            }
            else
            {
                _copyEnd += copyEnd;
            }
        }
        public void SetPasteEndCallback(ResultEnd pasteEnd)
        {
            _pasteEnd = pasteEnd;
        }
        public void SetResultMethod(string data)
        {
            if (_resultMethod != null && data != null && Cfg.IsSuccessLogHide == false)
            {
                _resultMethod(data);
            }
        }
        public void SetLog(string data)
        {
            if (_log != null && data != null)
            {
                _log.PRINT_F(data);
            }
        }

        public void InitializeSystem()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            SetResultMethod("Initialize");
            Cfg = new FBaseConfig();
            Cfg.LoadSettingFile();
            Sql = new dbSQL.FSqlite_Command(Cfg.GetDBPath());
            Sql.CreateDB();
            SetCopyedTable();
            Cfg.CompanyModelList = Sql.SelectCompanyModel();
            Cfg.WorkplaceModelList = Sql.SelectWorkplaceModel();
            Cfg.AccountModelList = Sql.SelectAccountModel();
            Cfg.CorrespondentModelList = Sql.SelectCorrespondentModel();
            Cfg.DepartmentModelList = Sql.SelectDepartmentModel();
            _log = new FPrintf(@"Log\Craw", "Flow");
            _paletteHelper.SetLightDark(Cfg.IsDarkTheme);
            _signInService = new SignInService();
            _icubeService = new ICubeService();
            InitTheme();
        }
        public void ViewAfterInitializeSystem()
        {
            SetResultMethod(PROGRAM_VERSION);
        }
        public void TerminateSystem()
        {
            IsTerminateOn = true;
            WaitThreadTerminate(eTHREAD.TH1);
            WaitThreadTerminate(eTHREAD.TH2);
            App.Web.CloseRequest();
        }
        public void GuardScreenSaver(bool data)
        {
            if (Cfg.IsGuardScreenSaver && data)
            {
                SetThreadExecutionState(ExecutionState.ES_ALL);
            }
            else
            {
                SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
            }
        }

        public void Setting()
        {
            if (CanSetting())
            {
                IsSettingOn = true;
                App.SettingWindow.Show();
            }
        }
        public void Copy()
        {
            if (CanCopy())
            {
                App.Web.Show();
                App.Web.NavigateURL(DefaultURL());
                Thread.Sleep(1000);
                CreateThread(eTHREAD.TH1);
            }
        }
        public void Paste()
        {
            if (CanPaste())
            {
                App.Web.Show();
                App.Web.NavigateURL(DefaultURL());
                Thread.Sleep(1000);
                CreateThread(eTHREAD.TH2);
            }
        }

        public void TestXPath()
        {
            App.Web.Show();
            App.Web.NavigateURL(DefaultURL());
        }

        public bool CanSetting()
        {
            if (IsSettingOn || IsCopyRun || IsPasteRun)
            {
                return false;
            }

            return true;
        }
        public bool CanLogin()
        {
            if (Cfg.LoginInfo.IsEmpty())
            {
                return false;
            }

            return true;
        }
        public bool CanCopy()
        {
            if (!IsSettingOn && Cfg.CopySector.Url.Length <= 0)
            {
                return false;
            }
            if (IsCopyRun || IsPasteRun)
            {
                return false;
            }

            return true;
        }
        public bool CanPaste()
        {
            if (!CanLogin())
            {
                return false;
            }
            if (DefaultURL().Length <= 0)
            {
                return false;
            }
            if (SelectedList == -1 || SelectedList >= CopyedTable.Count)
            {
                return false;
            }
            if (CopyedTable[SelectedList].IsEmpty())
            {
                return false;
            }

            if (IsSettingOn || IsCopyRun || IsPasteRun)
            {
                return false;
            }

            return true;
        }
        public void SetList(int selectedListOfCopyIndex)
        {
            if (selectedListOfCopyIndex == -1)
            {
                SelectedList = -1;
            }
            else if (CopyedTable.Count <= selectedListOfCopyIndex)
            {
                SelectedList = -1;
            }
            else if (CopyedTable.ElementAt(selectedListOfCopyIndex).Title == "")
            {
                SelectedList = -1;
            }
            else
            {
                SelectedList = selectedListOfCopyIndex;
            }
        }

        public string DefaultURL()
        {
            switch (Cfg.Company)
            {
                case "산업": return Cfg.Industrial;
                case "유화": return Cfg.Petrochem;
            }

            return Cfg.Industrial;
        }

        public void SetCopyedTable()
        {
            CopyedTable.Clear();

            var company = Sql.SelectCopyCompany();
            var slip = Sql.SelectCopySlipTable();
            for (int i = 0; i < 20; i++)
            {
                CopyDataTable copy = new CopyDataTable();
                copy = company.Find(x => x.CopyIndex == i);
                if (copy != null)
                {
                    copy.SlipValues = slip.FindAll(x => x.CopyIndex == i);
                }
                else
                {
                    copy = new CopyDataTable();
                }
                CopyedTable.Add(copy);
            }
        }

        #region Theme
        public void SetTheme(bool isDarkTheme)
        {
            Cfg.SetDarkTheme(isDarkTheme);
            InitTheme();
        }
        public void InitTheme()
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, Cfg.PrimaryForegroundColor);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, Cfg.PrimaryForegroundColor);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, Cfg.PrimaryForegroundColor);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, Cfg.PrimaryForegroundColor);

            theme.SetBaseTheme(Cfg.IsDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
            paletteHelper.ChangePrimaryColor(Cfg.PrimaryColor);
            paletteHelper.ChangeSecondaryColor(Cfg.SecondaryColor);

//            App.WindowInstance.DialogHost.DialogTheme = Cfg.IsDarkTheme ? MaterialDesignThemes.Wpf.BaseTheme.Dark : MaterialDesignThemes.Wpf.BaseTheme.Light;
//            App.SettingWindow.DialogHost.DialogTheme = Cfg.IsDarkTheme ? MaterialDesignThemes.Wpf.BaseTheme.Dark : MaterialDesignThemes.Wpf.BaseTheme.Light;
        }
        #endregion

        #region Page Login Setting
        public bool DBToCSV(out string err)
        {
            err = "";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "csv file (*.csv) | *.csv";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    if (File.Exists(dlg.FileName))
                    {
                        File.Delete(dlg.FileName);
                    }

                    File.Create(dlg.FileName).Dispose();
                    using (StreamWriter sw = new StreamWriter(dlg.FileName, true, Encoding.GetEncoding(949)))
                    {
                        sw.WriteLine("회사코드,회사명,,회사코드,사업장코드,사업장명,,회사코드,계정구분,계정코드,계정명,계정타입,,회사코드,거래처코드,거래처명,거래처타입,,회사코드,사업장코드,부서코드,부서명");
                        string company = ",,,";
                        string workplace = ",,,,";
                        string account = ",,,,,,";
                        string correspondent = ",,,,,";
                        string department = ",,,";

                        int count = Cfg.AccountModelList.Count > Cfg.CorrespondentModelList.Count ? Cfg.AccountModelList.Count : Cfg.CorrespondentModelList.Count;

                        for (int i = 0; i < count; i++)
                        {
                            company = ",,,";
                            workplace = ",,,,";
                            account = ",,,,,,";
                            correspondent = ",,,,,";
                            department = ",,,";

                            if (Cfg.CompanyModelList.Count > i)
                            {
                                var buff = Cfg.CompanyModelList[i];
                                if (buff.CompanyName == null)
                                {
                                    company = string.Format("{0},{1},,", buff.CompanyCode, buff.CompanyName);
                                }
                                else
                                {
                                    company = string.Format("{0},{1},,", buff.CompanyCode, buff.CompanyName.Replace(",", ""));
                                }
                            }
                            if (Cfg.WorkplaceModelList.Count > i)
                            {
                                var buff = Cfg.WorkplaceModelList[i];
                                if (buff.WorkplaceName == null)
                                {
                                    workplace = string.Format("{0},{1},{2},,", buff.CompanyCode, buff.WorkplaceCode, buff.WorkplaceName);
                                }
                                else
                                {
                                    workplace = string.Format("{0},{1},{2},,", buff.CompanyCode, buff.WorkplaceCode, buff.WorkplaceName.Replace(",", ""));
                                }
                            }
                            if (Cfg.AccountModelList.Count > i)
                            {
                                var buff = Cfg.AccountModelList[i];
                                if (buff.AccountName == null)
                                {
                                    account = string.Format("{0},{1},{2},{3},{4},,", buff.CompanyCode, buff.AccountDebit, buff.AccountCode, buff.AccountName, buff.AccountType);
                                }
                                else
                                {
                                    account = string.Format("{0},{1},{2},{3},{4},,", buff.CompanyCode, buff.AccountDebit, buff.AccountCode, buff.AccountName.Replace(",", ""), buff.AccountType);
                                }
                            }
                            if (Cfg.CorrespondentModelList.Count > i)
                            {
                                var buff = Cfg.CorrespondentModelList[i];
                                if (buff.CorrespondentName == null)
                                {
                                    correspondent = string.Format("{0},{1},{2},{3},,", buff.CompanyCode, buff.CorrespondentCode, buff.CorrespondentName, buff.CorrespondentType);
                                }
                                else
                                {
                                    correspondent = string.Format("{0},{1},{2},{3},,", buff.CompanyCode, buff.CorrespondentCode, buff.CorrespondentName.Replace(",", ""), buff.CorrespondentType);
                                }
                            }
                            if (Cfg.DepartmentModelList.Count > i)
                            {
                                var buff = Cfg.DepartmentModelList[i];
                                if (buff.DepartmentName == null)
                                {
                                    department = string.Format("{0},{1},{2}", buff.CompanyCode, buff.DepartmentCode, buff.DepartmentName);
                                }
                                else
                                {
                                    department = string.Format("{0},{1},{2}", buff.CompanyCode, buff.DepartmentCode, buff.DepartmentName.Replace(",", ""));
                                }
                            }

                            sw.WriteLine(string.Format("{0}{1}{2}{3}{4}", company, workplace, account, correspondent, department));
                        }
                    }
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return false;
                }
            }
            else
            {
                err = "취소 됨.";
            }

            return true;
        }
        public bool SyncList()
        {
            Sql.TruncateTable();
            Cfg.ClearModelList();

            bool ret = _signInService.SignIn(Cfg.OldLoginInfo.ID, Cfg.OldLoginInfo.PW, out string err, out Rest.Common.RestResult data);
            if (ret)
            {
                Token = data.Data.ToString();
            }
            else
            {
                SetResultMethod(err);
                SetLog(data.Msg);
                return ret;
            }

            ICubeService icubeService = new ICubeService();
            ret = icubeService.CompanyList(Cfg.Company, out err, out data);
            if (ret)
            {
                try
                {
                    List<CompanyModel>? buff = JsonSerializer.Deserialize<List<CompanyModel>>(data.List.ToString());
                    Cfg.CompanyModelList = buff ?? new();
                }
                catch (Exception ex)
                {
                    SetResultMethod(ex.Message);
                    SetLog(data.Msg);
                    SetLog(data.List.ToString() ?? "");
                }
            }
            else
            {
                SetResultMethod(err);
                SetLog(data.Msg);
                return ret;
            }

            foreach (CompanyModel companyItem in Cfg.CompanyModelList)
            {
                ret = icubeService.WorkplaceList(companyItem.CompanyCode, out err, out data);
                if (ret)
                {
                    try
                    {
                        List<WorkplaceModel>? buff = JsonSerializer.Deserialize<List<WorkplaceModel>>(data.List.ToString());
                        Cfg.WorkplaceModelList = Cfg.WorkplaceModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                ret = icubeService.AccountList(companyItem.CompanyCode, -1, out err, out data);
                if (ret)
                {
                    try
                    {
                        List<AccountModel>? buff = JsonSerializer.Deserialize<List<AccountModel>>(data.List.ToString());
                        Cfg.AccountModelList = Cfg.AccountModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                ret = icubeService.CorrespondentList(companyItem.CompanyCode, "A1", out err, out data);
                if (ret)
                {
                    try
                    {
                        List<CorrespondentModel>? buff = JsonSerializer.Deserialize<List<CorrespondentModel>>(data.List.ToString());
                        Cfg.CorrespondentModelList = Cfg.CorrespondentModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                List<WorkplaceModel> workPlaceData = Cfg.WorkplaceModelList.FindAll(x => x.CompanyCode == companyItem.CompanyCode);
                foreach (WorkplaceModel workPlace in workPlaceData)
                {
                    ret = icubeService.DepartmentList(companyItem.CompanyCode, workPlace.WorkplaceCode, out err, out data);
                    if (ret)
                    {
                        try
                        {
                            List<DepartmentModel>? buff = JsonSerializer.Deserialize<List<DepartmentModel>>(data.List.ToString());
                            Cfg.DepartmentModelList = Cfg.DepartmentModelList.Concat(buff.Clone()).ToList();
                        }
                        catch (Exception ex)
                        {
                            SetResultMethod(ex.Message);
                            SetLog(data.Msg);
                            SetLog(data.List.ToString() ?? "");
                        }
                    }
                    else
                    {
                        SetResultMethod(err);
                        SetLog(data.Msg);
                        return ret;
                    }
                }
            }

            Sql.InsertCompanyModel(Cfg.CompanyModelList);
            Sql.InsertWorkplaceModel(Cfg.WorkplaceModelList);
            Sql.InsertAccountModel(Cfg.AccountModelList);
            Sql.InsertCorrespondentModel(Cfg.CorrespondentModelList);
            Sql.InsertDepartmentModel(Cfg.DepartmentModelList);

            return true;
        }
        public bool SyncList_OLD()
        {
            Sql.TruncateTable();
            Cfg.ClearModelList();

            bool ret = _signInService.SignIn(Cfg.OldLoginInfo.ID, Cfg.OldLoginInfo.PW, out string err, out Rest.Common.RestResult data);
            if (ret)
            {
                Token = data.Data.ToString();
            }
            else
            {
                SetResultMethod(err);
                SetLog(data.Msg);
                return ret;
            }

            ICubeService icubeService = new ICubeService();
            ret = icubeService.CompanyList(Cfg.Company, out err, out data);
            if (ret)
            {
                try
                {
                    List<CompanyModel>? buff = JsonSerializer.Deserialize<List<CompanyModel>>(data.List.ToString());
                    Sql.InsertCompanyModel(buff);
                    Cfg.CompanyModelList = buff;
                }
                catch (Exception ex)
                {
                    SetResultMethod(ex.Message);
                    SetLog(data.Msg);
                    SetLog(data.List.ToString() ?? "");
                }
            }
            else
            {
                SetResultMethod(err);
                SetLog(data.Msg);
                return ret;
            }

            foreach (CompanyModel companyItem in Cfg.CompanyModelList)
            {
                ret = icubeService.WorkplaceList(companyItem.CompanyCode, out err, out data);
                if (ret)
                {
                    try
                    {
                        List<WorkplaceModel>? buff = JsonSerializer.Deserialize<List<WorkplaceModel>>(data.List.ToString());
                        Sql.InsertWorkplaceModel(buff);
                        Cfg.WorkplaceModelList = Cfg.WorkplaceModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                ret = icubeService.AccountList(companyItem.CompanyCode, -1, out err, out data);
                if (ret)
                {
                    try
                    {
                        List<AccountModel>? buff = JsonSerializer.Deserialize<List<AccountModel>>(data.List.ToString());
                        Sql.InsertAccountModel(buff);
                        Cfg.AccountModelList = Cfg.AccountModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                ret = icubeService.CorrespondentList(companyItem.CompanyCode, "A1", out err, out data);
                if (ret)
                {
                    try
                    {
                        List<CorrespondentModel>? buff = JsonSerializer.Deserialize<List<CorrespondentModel>>(data.List.ToString());
                        Sql.InsertCorrespondentModel(buff);
                        Cfg.CorrespondentModelList = Cfg.CorrespondentModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }
                ret = icubeService.CorrespondentList(companyItem.CompanyCode, "A2", out err, out data);
                if (ret)
                {
                    try
                    {
                        List<CorrespondentModel>? buff = JsonSerializer.Deserialize<List<CorrespondentModel>>(data.List.ToString());
                        Sql.InsertCorrespondentModel(buff);
                        Cfg.CorrespondentModelList = Cfg.CorrespondentModelList.Concat(buff.Clone()).ToList();
                    }
                    catch (Exception ex)
                    {
                        SetResultMethod(ex.Message);
                        SetLog(data.Msg);
                        SetLog(data.List.ToString() ?? "");
                    }
                }
                else
                {
                    SetResultMethod(err);
                    SetLog(data.Msg);
                    return ret;
                }

                List<WorkplaceModel> workPlaceData = Cfg.WorkplaceModelList.FindAll(x => x.CompanyCode == companyItem.CompanyCode);
                foreach (WorkplaceModel workPlace in workPlaceData)
                {
                    ret = icubeService.DepartmentList(companyItem.CompanyCode, workPlace.WorkplaceCode, out err, out data);
                    if (ret)
                    {
                        try
                        {
                            List<DepartmentModel>? buff = JsonSerializer.Deserialize<List<DepartmentModel>>(data.List.ToString());
                            Sql.InsertDepartmentModel(buff);
                            Cfg.DepartmentModelList = Cfg.DepartmentModelList.Concat(buff.Clone()).ToList();
                        }
                        catch (Exception ex)
                        {
                            SetResultMethod(ex.Message);
                            SetLog(data.Msg);
                            SetLog(data.List.ToString() ?? "");
                        }
                    }
                    else
                    {
                        SetResultMethod(err);
                        SetLog(data.Msg);
                        return ret;
                    }
                }
            }

            return true;
        }
        public List<CompanyModel> CallCompanyList()
        {
            return Cfg.CallCompanyList();
        }
        public List<WorkplaceModel> CallWorkplaceList(string companyName)
        {
            if (companyName == null)
            {
                return new List<WorkplaceModel>();
            }
            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }

            return Cfg.CallWorkplaceList(companyName);
        }
        public List<AccountModel> CallAccountList(string gubun, string companyName)
        {
            if (companyName == null)
            {
                return new List<AccountModel>();
            }
            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }

            return Cfg.CallAccountList(gubun, companyName);
        }
        public List<string> CallTypeList()
        {
            return Cfg.CallTypeList();
        }
        public List<CorrespondentModel> CallCorrespondentList(string companyName, string accountName)
        {
            if (companyName == null)
            {
                return new List<CorrespondentModel>();
            }
            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }
            if (accountName.Contains("/"))
            {
                int index = accountName.IndexOf("/");
                accountName = accountName.Substring(index + 1);
            }

            return Cfg.CallCorrespondentList(companyName, accountName);
        }
        public List<DepartmentModel> CallDepartmentList(string companyName, string workplaceName)
        {
            if (companyName == null)
            {
                return new List<DepartmentModel>();
            }
            if (workplaceName == null)
            {
                return new List<DepartmentModel>();
            }
            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }

            if (workplaceName.Contains("/"))
            {
                int index = workplaceName.IndexOf("/");
                workplaceName = workplaceName.Substring(index + 1);
            }

            return Cfg.CallDepartmentList(companyName, workplaceName);
        }
        public List<string> CallCompanyNameList()
        {
            return Cfg.CallCompanyNameList();
        }
        public List<string> CallWorkplaceNameList(string companyName)
        {
            if (companyName == null)
            {
                return new List<string>();
            }

            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }

            return Cfg.CallWorkplaceNameList(companyName);
        }
        public List<string> CallAccountNameList(string gubun, string companyName)
        {
            if (companyName == null)
            {
                return new List<string>();
            }

            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }

            return Cfg.CallAccountNameList(gubun, companyName);
        }
        public List<string> CallTypeNameList()
        {
            return Cfg.CallTypeNameList();
        }
        public List<string> CallCorrespondentNameList(string companyName, string accountName)
        {
            if (companyName == null)
            {
                return new List<string>();
            }
            if (accountName == null)
            {
                return new List<string>();
            }

            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }
            if (accountName.Contains("/"))
            {
                int index = accountName.IndexOf("/");
                accountName = accountName.Substring(index + 1);
            }

            return Cfg.CallCorrespondentNameList(companyName, accountName);
        }
        public List<string> CallDepartmentNameList(string companyName, string workplaceName)
        {
            if (companyName == null)
            {
                return new List<string>();
            }
            if (workplaceName == null)
            {
                return new List<string>();
            }

            if (companyName.Contains("/"))
            {
                int index = companyName.IndexOf("/");
                companyName = companyName.Substring(index + 1);
            }
            if (workplaceName.Contains("/"))
            {
                int index = workplaceName.IndexOf("/");
                workplaceName = workplaceName.Substring(index + 1);
            }

            return Cfg.CallDepartmentNameList(companyName, workplaceName);
        }
        #endregion

        #region Page Upload Setting
        public void GetSampleCSV(out string err)
        {
            err = "";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel file (*.xlsx) | *.xlsx";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    if (File.Exists(dlg.FileName))
                    {
                        File.Delete(dlg.FileName);
                    }

                    FSlipExcelFile temp = new FSlipExcelFile();
                    temp.CreateSampleFile(dlg.FileName, Cfg.Company == "산업", out err);
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }
            else
            {
                err = "취소 됨.";
            }
        }
        public CopyDataTable GetCSV(out string err)
        {
            err = "";
            CopyDataTable ret = new CopyDataTable();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel file (*.xlsx) | *.xlsx";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    FSlipExcelFile temp = new FSlipExcelFile();
                    var rows = temp.ReadFile(dlg.FileName, out err);
                    if (rows.Count <= 1)
                    {
                        err = "데이터가 존재하지 않음.";
                        return ret;
                    }

                    string[] column = rows[0].ToArray();
                    if (column.Count() != 12)
                    {
                        err = "섹션 개수가 올바르지 않음.";
                        return ret;
                    }

                    if (new CSVSectionTable(column).IsEmpty() == true)
                    {
                        err = "섹션 명이 올바르지 않음.";
                        return ret;
                    }

                    column = rows[1].ToArray();
                    if (column.Count() < 10)
                    {
                        err = "데이터 개수가 올바르지 않음.";
                        return ret;
                    }

                    ret.Title = column[0];
                    ret.CompanyValue.SetCSV(column);
                    if (ret.CompanyValue.IsEmpty())
                    {
                        err = "사명 정보가 올바르지 않음.";
                        return ret;
                    }
                    ret.WorkplaceValue.SetCSV(ret.CompanyValue.CompanyCode, column);
                    if (ret.WorkplaceValue.IsEmpty())
                    {
                        err = "사업장 정보가 올바르지 않음.";
                        return ret;
                    }

                    ret.SlipValues.Add(new SlipTable(column));

                    for (int i = 2; i < rows.Count; i++)
                    {
                        column = rows[i].ToArray();
                        ret.SlipValues.Add(new SlipTable(column));
                    }
                }
                catch (Exception ex)
                {
                    WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("파일 열기 실패", ex.Message);
                    messageBoxDialog.ShowDialog();
                }
            }

            return ret;
        }
        public bool ValidationData(ref CopyDataTable data, out string err)
        {
            err = "";
            CopyDataTable buff = data;

            #region 필수값들
            if (data.Title.Length <= 0)
            {
                err = "제목이 필요합니다.";
                return false;
            }

            if (data.CompanyValue.CompanyCode.Length <= 0 && data.CompanyValue.CompanyName.Length <= 0)
            {
                err = "회사가 필요합니다.";
                return false;
            }

            if (data.WorkplaceValue.WorkplaceCode.Length <= 0 || data.WorkplaceValue.WorkplaceName.Length <= 0)
            {
                err = "사업장이 필요합니다.";
                return false;
            }

            var companyList = CallCompanyList();
            if (data.CompanyValue.IsEmpty())
            {
                if (data.CompanyValue.CompanyCode.Length <= 0)
                {
                    var companyData = companyList.Find(x => x.CompanyName == buff.CompanyValue.CompanyName);
                    if (companyData == null)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("입력된 회사명 {0}에 해당하는 회사코드를 찾을 수 없습니다.\n", data.CompanyValue.CompanyName);
                        stb.AppendFormat("데이터베이스 동기화 작업을 하거나, 데이터를 다시 구성해주세요.\n");
                        stb.AppendFormat("이는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.");
                        err = stb.ToString();
                        return false;
                    }
                    data.CompanyValue.CompanyCode = companyData.CompanyCode;
                }
                else if (data.CompanyValue.CompanyName.Length <= 0)
                {
                    var companyData = companyList.Find(x => x.CompanyCode == buff.CompanyValue.CompanyCode);
                    if (companyData == null)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("입력된 회사코드 {0}에 해당하는 회사명을 찾을 수 없습니다.\n", data.CompanyValue.CompanyCode);
                        stb.AppendFormat("데이터베이스 동기화 작업을 하거나, 데이터를 다시 구성해주세요.\n");
                        stb.AppendFormat("이는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.");
                        err = stb.ToString();
                        return false;
                    }
                    data.CompanyValue.CompanyName = companyData.CompanyName;
                }
            }

            var workplaceList = CallWorkplaceList(data.CompanyValue.CompanyName);
            if (data.WorkplaceValue.IsEmpty())
            {
                if (data.WorkplaceValue.CompanyCode.Length <= 0)
                {
                    data.WorkplaceValue.CompanyCode = data.CompanyValue.CompanyCode;
                }
                if (data.WorkplaceValue.WorkplaceCode.Length <= 0)
                {
                    var workplaceData = workplaceList.Find(x => x.CompanyCode == buff.WorkplaceValue.CompanyCode);
                    if (workplaceData == null)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("입력된 회사코드 {0}에 해당하는 사업장코드를 찾을 수 없습니다.\n", data.WorkplaceValue.CompanyCode);
                        stb.AppendFormat("데이터베이스 동기화 작업을 하거나, 데이터를 다시 구성해주세요.\n");
                        stb.AppendFormat("이는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.");
                        err = stb.ToString();
                        return false;
                    }
                    data.WorkplaceValue.WorkplaceCode = workplaceData.WorkplaceCode;
                }
                if (data.WorkplaceValue.WorkplaceName.Length <= 0)
                {
                    var workplaceData = workplaceList.Find(x => x.CompanyCode == buff.WorkplaceValue.CompanyCode);
                    if (workplaceData == null)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("입력된 회사코드 {0}에 해당하는 사업장명을 찾을 수 없습니다.\n", data.WorkplaceValue.CompanyCode);
                        stb.AppendFormat("데이터베이스 동기화 작업을 하거나, 데이터를 다시 구성해주세요.\n");
                        stb.AppendFormat("이는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.");
                        err = stb.ToString();
                        return false;
                    }
                    data.WorkplaceValue.WorkplaceName = workplaceData.WorkplaceName;
                }
            }
            if (workplaceList.Find(x => x.CompanyCode == buff.WorkplaceValue.CompanyCode && 
                                        x.WorkplaceCode == buff.WorkplaceValue.WorkplaceCode && 
                                        x.WorkplaceName == buff.WorkplaceValue.WorkplaceName) == null)
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("회사코드 {0}\n사업장코드 {1}\n사업장명 {2}\n에 일치하는 데이터가 존재하지 않습니다.\n", data.WorkplaceValue.CompanyCode, data.WorkplaceValue.WorkplaceCode, data.WorkplaceValue.WorkplaceName);
                stb.AppendFormat("데이터베이스 동기화 작업을 하거나, 데이터를 다시 구성해주세요.\n");
                stb.AppendFormat("이는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.");
                err = stb.ToString();
                return false;
            }
            #endregion

            StringBuilder slipErr = new StringBuilder();
            var creditAccountList = CallAccountList("차변", data.CompanyValue.CompanyName);
            var deditAccountList = CallAccountList("대변", data.CompanyValue.CompanyName);
            var typeList = CallTypeList();

            var departmentList = CallDepartmentList(data.CompanyValue.CompanyName, data.WorkplaceValue.WorkplaceName);
            int count = 1;
            foreach (var slipItem in data.SlipValues)
            {
                slipItem.NullValueDelete();

                if (slipItem.IsEmptyGubun())
                {
                    slipErr.AppendFormat("{0}번째 구분 값이 없습니다.\n", count);
                    if (slipItem.Account.AccountName.Length > 0 || slipItem.Account.AccountCode.Length > 0)
                    {
                        slipErr.AppendFormat("{0}번째 구분 값이 없으므로 계정명을 공백 처리 합니다.\n", count);
                    }
                    if (slipItem.Correspondent.CorrespondentName.Length > 0 || slipItem.Correspondent.CorrespondentName.Length > 0)
                    {
                        slipErr.AppendFormat("{0}번째 구분 값이 없으므로 거래처명을 공백 처리 합니다.\n", count);
                    }
                }
                else
                {
                    #region 계정코드
                    AccountModel accountData = null;
                    if (slipItem.IsEmptyAccount() == false)
                    {
                        switch (slipItem.Gubun)
                        {
                            case "차변":  accountData = creditAccountList.Find(x => x.GetAccountName() == slipItem.Account.GetAccountName());  break;
                            case "대변":  accountData = deditAccountList.Find(x => x.GetAccountName() == slipItem.Account.GetAccountName());   break;
                        }

                        if (accountData == null)
                        {
                            if (slipItem.Account.AccountCode.Length > 0)
                            {
                                switch (slipItem.Gubun)
                                {
                                    case "차변": accountData = creditAccountList.Find(x => x.AccountCode == slipItem.Account.AccountCode); break;
                                    case "대변": accountData = deditAccountList.Find(x => x.AccountCode == slipItem.Account.AccountCode); break;
                                }
                            }
                            else if (slipItem.Account.AccountName.Length > 0)
                            {
                                switch (slipItem.Gubun)
                                {
                                    case "차변": accountData = creditAccountList.Find(x => x.AccountName == slipItem.Account.AccountName); break;
                                    case "대변": accountData = deditAccountList.Find(x => x.AccountName == slipItem.Account.AccountName); break;
                                }
                            }

                            if (accountData != null)
                            {
                                slipItem.Account = accountData;
                                slipErr.AppendFormat("{0}번째 계정과목의 코드명을 찾아, 계정과목명을 {1}로 변경하였습니다.\n", count, slipItem.Account.GetAccountName());
                            }
                            else
                            {
                                slipErr.AppendFormat("{0}번째 계정과목 {1}코드 또는 {2}명이 존재하지 않습니다.\n", count, slipItem.Account.AccountCode, slipItem.Account.AccountName);
                                slipErr.AppendFormat("{0}번째 계정에 문제가 있어 공백으로 변경합니다.\n", count);
                                slipItem.Account.AccountName = "";
                                slipItem.Account.AccountCode = "";
                            }
                        }
                        else
                        {
                            slipItem.Account = accountData;
                        }
                    }
                    else
                    {
                        slipErr.AppendFormat("{0}번째 계정 값이 공백이였습니다.\n", count);
                    }
                    #endregion

                    #region 거래처
                    if (slipItem.IsEmptyCorrespondent() == false)
                    {
                        if (accountData == null)
                        {
                            slipErr.AppendFormat("{0}번째 계정 데이터를 찾을 수 없어 거래처 내용을 공백으로 변경합니다.\n", count);
                            slipItem.Correspondent.CorrespondentCode = "";
                            slipItem.Correspondent.CorrespondentName = "";
                        }
                        else
                        {
                            var corresspondentList = CallCorrespondentList(data.CompanyValue.CompanyName, accountData.AccountName);
                            CorrespondentModel correspondentData = corresspondentList.Find(x => x.GetCorrespondentName() == slipItem.Correspondent.GetCorrespondentName());
                            if (correspondentData == null)
                            {
                                if (slipItem.Correspondent.CorrespondentCode.Length > 0)
                                {
                                    correspondentData = corresspondentList.Find(x => x.CorrespondentCode == slipItem.Correspondent.CorrespondentCode);
                                }
                                else if (slipItem.Correspondent.CorrespondentName.Length > 0)
                                {
                                    correspondentData = corresspondentList.Find(x => x.CorrespondentName == slipItem.Correspondent.CorrespondentName);
                                }

                                if (correspondentData != null)
                                {
                                    slipItem.Correspondent = correspondentData;
                                    slipErr.AppendFormat("{0}번째 거래처명의 코드명을 찾아, 거래처명을 {1}로 변경하였습니다.", count, slipItem.Account.GetAccountName());
                                }
                                else
                                {
                                    slipErr.AppendFormat("{0}번째 거래처 {1}코드 또는 {2}명이 존재하지 않습니다.\n", count, slipItem.Correspondent.CorrespondentCode, slipItem.Correspondent.CorrespondentName);
                                    slipErr.AppendFormat("{0}번째 거래처에 문제가 있어 공백으로 변경합니다.\n", count);
                                    slipItem.Correspondent.CorrespondentName = "";
                                    slipItem.Correspondent.CorrespondentCode = "";
                                }
                            }
                            else
                            {
                                slipItem.Correspondent = correspondentData;
                            }
                        }
                    }
                    else
                    {
                        if (accountData != null)
                        {
                            var corresspondentList = CallCorrespondentList(data.CompanyValue.CompanyName, accountData.AccountName);
                            if (corresspondentList.Count() > 0)
                            {
                                slipErr.AppendFormat("{0}번째 거래처 값이 공백이였습니다.\n", count);
                            }
                        }
                        else
                        {
                            slipErr.AppendFormat("{0}번째 거래처 값이 공백이였습니다.\n", count);
                        }
                    }
                    #endregion
                }

                #region 증빙유형
                if (slipItem.IsEmptyTaxDate() == true)
                {
                    if(slipItem.Account.AccountName.Contains("부가세"))
                    {
                        slipErr.AppendFormat("{0}번째 증빙일자 값이 필요한데 없어서 오늘 날짜로 바꿉니다.\n", count);
                        slipItem.TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                    }
                }
                #endregion

                #region 증빙유형
                if (slipItem.IsEmptyType() == false)
                {
                    string typeData = typeList.Find(x => x == slipItem.Type);
                    if (typeData == null)
                    {
                        slipErr.AppendFormat("{0}번째 증빙유형 {1}이 존재하지 않습니다.\n", count, slipItem.Type);
                        slipErr.AppendFormat("{0}번째 증빙유형에 문제가 있어 공백으로 변경합니다.\n", count);
                        slipItem.Type = "";
                    }
                }
                else
                {
                    slipErr.AppendFormat("{0}번째 증빙유형이 공백이였습니다.", count);
                }
                #endregion

                #region 금액
                if (slipItem.IsEmptyPrice() == false)
                {
                    if (slipItem.Price == null)
                    {
                        slipErr.AppendFormat("{0}번째 금액 {1}의 형식이 숫자가 아닙니다.\n", count, slipItem.Price);
                        slipErr.AppendFormat("{0}번째 금액 데이터에서 숫자 이외의 값을 모두 지웠습니다.\n", count);
                        slipItem.Price = "0";
                    }
                    else if (long.TryParse(slipItem.Price.Replace(",", ""), out long price) == false)
                    {
                        slipErr.AppendFormat("{0}번째 금액 {1}의 형식이 숫자가 아닙니다.\n", count, slipItem.Price);
                        slipErr.AppendFormat("{0}번째 금액 데이터에서 숫자 이외의 값을 모두 지웠습니다.\n", count);
                        slipItem.Price = Regex.Replace(slipItem.Price, @"\D", "");
                    }
                }
                else
                {
                    slipErr.AppendFormat("{0}번째 금액이 공백이였습니다.\n", count);
                }
                #endregion

                #region 부서
                if (slipItem.IsEmptyDepartment() == false)
                {
                    DepartmentModel departmentData = departmentList.Find(x => x.GetDepartmentName() == slipItem.Department.GetDepartmentName());
                    if (departmentData == null)
                    {
                        if (slipItem.Department.DepartmentCode.Length > 0)
                        {
                            departmentData = departmentList.Find(x => x.DepartmentCode == slipItem.Department.DepartmentCode);
                        }
                        else if (slipItem.Department.DepartmentName.Length > 0)
                        {
                            departmentData = departmentList.Find(x => x.DepartmentName == slipItem.Department.DepartmentName);
                        }

                        if (departmentData != null)
                        {
                            slipItem.Department = departmentData;
                            slipErr.AppendFormat("{0}번째 부서명의 코드명을 찾아, 거래처명을 {1}로 변경하였습니다.", count, slipItem.Department.GetDepartmentName());
                        }
                        else
                        {
                            slipErr.AppendFormat("{0}번째 부서명 {1}코드 또는 {2}명이 존재하지 않습니다.\n", count, slipItem.Department.DepartmentCode, slipItem.Department.DepartmentName);
                            slipErr.AppendFormat("{0}번째 부서명에 문제가 있어 공백으로 변경합니다.\n", count);
                            slipItem.Department.DepartmentCode = "";
                            slipItem.Department.DepartmentName = "";
                        }
                    }
                    else
                    {
                        slipItem.Department = departmentData;
                    }
                }
                else
                {
                    slipErr.AppendFormat("{0}번째 부서명이 공백이였습니다.\n", count);
                }
                #endregion

                #region 공급가액
                if (slipItem.IsEmptySupplyPrice() == false)
                {
                    if (slipItem.SupplyPrice == null)
                    {
                        slipErr.AppendFormat("{0}번째 공급가액 {1}의 형식이 숫자가 아닙니다.\n", count, slipItem.SupplyPrice);
                        slipErr.AppendFormat("{0}번째 공급가액 데이터에서 숫자 이외의 값을 모두 지웠습니다.\n", count);
                        slipItem.SupplyPrice = "0";
                    }
                    else if (long.TryParse(slipItem.SupplyPrice.Replace(",", ""), out long supplyPrice) == false)
                    {
                        slipErr.AppendFormat("{0}번째 공급가액 {1}의 형식이 숫자가 아닙니다.\n", count, slipItem.SupplyPrice);
                        slipErr.AppendFormat("{0}번째 공급가액 데이터에서 숫자 이외의 값을 모두 지웠습니다.\n", count);
                        slipItem.SupplyPrice = Regex.Replace(slipItem.SupplyPrice, @"\D", "");
                    }
                }
                #endregion

                count++;
            }
            if (slipErr.Length != 0)
            {
                slipErr.AppendFormat("이상의 문제점이 발견되었습니다.\n");
                slipErr.AppendFormat("이는 오래된 데이터들을 사용했거나 데이터베이스 동기화 문제 또는 더존 데이터베이스 변경으로 인한 오류일 수도 있습니다.\n");
                slipErr.AppendFormat("그래도 저장하시겠습니까?");
            }
            
            err = slipErr.ToString();
            SetResultMethod(err);
            return true;
        }
        public bool SaveCopyFileData(CopyDataTable data, int selectedListOfCopyIndex, out string err)
        {
            err = "";
            int selectedList = -1;
            if (selectedListOfCopyIndex == -1)
            {
                selectedList = -1;
            }
            else if (CopyedTable.Count() <= selectedListOfCopyIndex)
            {
                selectedList = -1;
            }
            else
            {
                selectedList = selectedListOfCopyIndex;
            }
            if (selectedList == -1)
            {
                err = "알 수 없는 오류";
                return false;
            }

            if (CopyedTable.ElementAt(selectedListOfCopyIndex).Title.Length > 0)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("경고경고", "이미 다른 데이터가 존재하는 것 같습니다.\n그래도 저장하시겠습니까?", true);
                if (messageBoxDialog.ShowDialog() == false)
                {
                    err = "취소 됨.";
                    return false;
                }
            }

            data.CopyIndex = selectedListOfCopyIndex;
            data.SlipValues.ForEach(x => x.CopyIndex = data.CopyIndex);

            Sql.InsertCopyCompany(data);
            Sql.InsertCopySlipTable(data.SlipValues);
            CopyedTable[selectedListOfCopyIndex] = data.Clone();

            if (_copyEnd != null)
            {
                _copyEnd(false);
            }

            return true;
        }
        #endregion

        public bool IsNavigateComp(string waitUrl)
        {
            long timeout = Cfg.Timeout;
            long elapsedTime = 0;
            while (elapsedTime < timeout * 1000)
            {
                if (CurrentUrl.ToLower().Contains(waitUrl))
                {
                    return true;
                }

                Thread.Sleep(10);
            }

            return false;
        }
        public bool IsLoading(string elementName, ElementType type)
        {
            Thread.Sleep(100);
            long timeout = Cfg.Timeout;
            long elapsedTime = 0;
            while (elapsedTime < timeout * 1000)
            {
                if (ExecuteScript(FindElementString(elementName, type), out bool err).ToLower() != "null")
                {
                    return false;
                }

                Thread.Sleep(10);
            }

            return true;
        }
        public void ContentLoading(string absoluteUrl)
        {
            CurrentUrl = absoluteUrl;
        }

        public string ExecuteScript(string cmd, out bool err)
        {
            Thread.Sleep(100);
            err = false;
            long timeout = Cfg.Timeout;
            App.Web.ExecuteScript(cmd);
            long elapsed = 0;
            while (_ret.Count <= 0)
            {
                if (elapsed > timeout * 1000)
                {
                    err = true;
                    return "timeout";
                }

                Thread.Sleep(100);
                elapsed += 100;
            }

            return _ret.Dequeue();
        }
        public void SetResult(string data)
        {
            _ret.Enqueue(data);
        }

        public static string FindElementString(string data, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;",
                ElementType.ID => $"document.getElementById('{data}');",
                ElementType.CLASS => $"document.getElementsByClassName('{data}');",
                _ => "",
            };
        }
        public static string FindElementString(int data, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;",
                ElementType.ID => $"document.getElementById('{data}');",
                ElementType.CLASS => $"document.getElementsByClassName('{data}');",
                _ => "",
            };
        }
        public static string ElementGetValue(string data, string value, int index, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.getAttribute('{value}');",
                ElementType.ID => $"document.getElementById('{data}').getAttribute('{value}');",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].getAttribute('{value}');",
                _ => "",
            };
        }
        public static string ElementSetValue(string data, string value, int index, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.value = '{value}';",
                ElementType.ID => $"document.getElementById('{data}').value = '{value}';",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].value = '{value}';",
                _ => "",
            };
        }
        public static string ElementSetValue(string data, int value, int index, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.value = '{value}';",
                ElementType.ID => $"document.getElementById('{data}').value = '{value}';",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].value = '{value}';",
                _ => "",
            };
        }
        public static string ElementSetAttribute(string data, string attr, string value, int index, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.setAttribute('{attr}', '{value}');",
                ElementType.ID => $"document.getElementById('{data}').setAttribute('{attr}', '{value}');",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].setAttribute('{attr}', '{value}');",
                _ => "",
            };
        }
        public static string ElementSetAttribute(string data, string attr, int value, int index, ElementType type)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.setAttribute('{attr}', '{value}');",
                ElementType.ID => $"document.getElementById('{data}').setAttribute('{attr}', '{value}');",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].setAttribute('{attr}', '{value}');",
                _ => "",
            };
        }
        public static string ElementClick(string data, int index = 0, ElementType type = ElementType.ID)
        {
            return type switch
            {
                ElementType.XPATH => $"document.evaluate('{data}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click();",
                ElementType.ID => $"document.getElementById('{data}').click();",
                ElementType.CLASS => $"document.getElementsByClassName('{data}')[{index}].click();",
                _ => "",
            };
        }
    }
}
