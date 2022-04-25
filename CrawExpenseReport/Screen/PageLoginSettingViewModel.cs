using CrawExpenseReport.Data;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CrawExpenseReport.Base;
using Microsoft.Win32;
using System.Windows;
using CrawExpenseReport.Screen.Popup;

namespace CrawExpenseReport.Screen
{
    public class PageLoginSettingViewModel : INotifyPropertyChanged
    {
        private int _timeout;
        private int _delayTime;
        private int _retryCount;
        private string _expenseReportName;
        private string _defIndustiral;
        private string _defPetrochem;
        private List<string> _companyList;
        private string _selectedCompanyItem;
        private string _id;
        private string _pw;
        private string _oldID;
        private string _oldPW;
        private string _resultText;

        private bool _isDarkTheme;
        private bool _isStartOnSync;
        private bool _isSuccessLogHide;
        private bool _isGuardScreenSaver;
        private bool _isSyncListEnable;

        public PageLoginSettingViewModel()
        {
            _timeout = 0;
            _delayTime = 0;
            _retryCount = 0;
            _expenseReportName = "";
            _defIndustiral = "";
            _defPetrochem = "";
            _companyList = new();
            _selectedCompanyItem = "";
            _id = "";
            _pw = "";
            _oldID = "";
            _oldPW = "";
            _resultText = "";

            _isDarkTheme = false;
            _isStartOnSync = false;
            _isSuccessLogHide = false;
            _isGuardScreenSaver = false;
            _isSyncListEnable = false;
            IsSyncListEnable = false;

            CompanyList = new();
            CompanyList.Add("산업");
            CompanyList.Add("유화");

            SaveTimeout = new CommandImpl(SaveTimeoutEvent);
            SaveDelayTime = new CommandImpl(SaveDelayTimeEvent);
            SaveRetryCount = new CommandImpl(SaveRetryCountEvent);
            SaveExpenseReport = new CommandImpl(SaveExpenseReportEvent);
            SaveDefIndustiral = new CommandImpl(SaveDefIndustiralEvent);
            SaveDefPetrochem = new CommandImpl(SaveDefPetrochemEvent);
            SaveCompany = new CommandImpl(SaveCompanyEvent);
            SaveID = new CommandImpl(SaveIDEvent);
            SavePW = new CommandImpl(SavePWEvent);
            SaveOldID = new CommandImpl(SaveOldIDEvent);
            SaveOldPW = new CommandImpl(SaveOldPWEvent);

            SyncList = new CommandImpl(SyncListEvenet);
            DBToCSV  = new CommandImpl(DBToCSVEvent);
        }
        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 0)
                {
                    _timeout = 10;
                }
                else
                {
                    _timeout = value;
                }

                OnPropertyChanged();
            }
        }
        public int DelayTime
        {
            get => _delayTime;
            set
            {
                if (value < 50)
                {
                    _delayTime = 50;
                }
                else
                {
                    _delayTime = value;
                }

                OnPropertyChanged();
            }
        }
        public int RetryCount
        {
            get => _retryCount;
            set
            {
                if (value < 1)
                {
                    _retryCount = 1;
                }
                else
                {
                    _retryCount = value;
                }

                OnPropertyChanged();
            }
        }
        public string ExpenseReportName
        {
            get => _expenseReportName;
            set
            {
                if (value != null && value.Length > 0)
                {
                    _expenseReportName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DefIndustiral
        {
            get => _defIndustiral;
            set
            {
                _defIndustiral = value;
                OnPropertyChanged();
            }
        }
        public string DefPetrochem
        {
            get => _defPetrochem;
            set
            {
                _defPetrochem = value;
                OnPropertyChanged();
            }
        }
        public List<string> CompanyList
        {
            get => _companyList;
            set
            {
                _companyList = value;
                OnPropertyChanged();
            }
        }
        public string SelectedCompanyItem
        {
            get => _selectedCompanyItem;
            set
            {
                _selectedCompanyItem = value;
                OnPropertyChanged();
            }
        }
        public string ID
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string PW
        {
            get => _pw;
            set
            {
                _pw = value;
                OnPropertyChanged();
            }
        }
        public string OldID
        {
            get => _oldID;
            set
            {
                _oldID = value;
                OnPropertyChanged();
            }
        }
        public string OldPW
        {
            get => _oldPW;
            set
            {
                _oldPW = value;
                OnPropertyChanged();
            }
        }
        public string ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged();
                    FBaseFunc.Ins.SetTheme(value);
                }
            }
        }
        public bool IsStartOnSync
        {
            get => _isStartOnSync;
            set
            {
                if (_isStartOnSync != value)
                {
                    _isStartOnSync = value;
                    OnPropertyChanged();
                    FBaseFunc.Ins.Cfg.SetStartOnSync(value);
                }
            }
        }
        public bool IsSuccessLogHide
        {
            get => _isSuccessLogHide;
            set
            {
                if (_isSuccessLogHide != value)
                {
                    _isSuccessLogHide = value;
                    OnPropertyChanged();
                    FBaseFunc.Ins.Cfg.SetIsSuccessLogHide(value);
                }
            }
        }
        public bool IsGuardScreenSaver
        {
            get => _isGuardScreenSaver;
            set
            {
                if (_isGuardScreenSaver != value)
                {
                    _isGuardScreenSaver = value;
                    OnPropertyChanged();
                    FBaseFunc.Ins.Cfg.SetIsGuardScreenSaver(value);
                }
            }
        }
        public bool IsSyncListEnable
        {
            get => _isSyncListEnable;
            set
            {
                _isSyncListEnable = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTimeout { get; }
        public ICommand SaveDelayTime { get; }
        public ICommand SaveRetryCount { get; }
        public ICommand SaveExpenseReport { get; }
        public ICommand SaveDefIndustiral { get; }
        public ICommand SaveDefPetrochem { get; }
        public ICommand SaveCompany { get; }
        public ICommand SaveID { get; }
        public ICommand SavePW { get; }
        public ICommand SaveOldID { get; }
        public ICommand SaveOldPW { get; }
        public ICommand SyncList { get; }
        public ICommand DBToCSV { get; }

        private void SaveTimeoutEvent(object? obj)
        {
            if (Timeout > 0)
            {
                if (Timeout < 1)
                {
                    ResultText = string.Format("타임아웃은 최소 1초로 해주세요");
                    return;
                }

                FBaseFunc.Ins.Cfg.SetTimeout(Timeout);
                ResultText = string.Format("타임아웃 : {0} 저장", Timeout);
            }
            else
            {
                ResultText = "타임아웃값이 이상합니다.";
            }
        }
        private void SaveDelayTimeEvent(object? obj)
        {
            if (DelayTime > 0)
            {
                if (DelayTime < 50)
                {
                    ResultText = string.Format("딜레이는 최소 50 밀리초로 해주세요");
                    return;
                }

                FBaseFunc.Ins.Cfg.SetDelayTime(DelayTime);
                ResultText = string.Format("딜레이 : {0} 저장", DelayTime);
            }
            else
            {
                ResultText = "딜레이 값이 이상합니다.";
            }
        }
        private void SaveRetryCountEvent(object? obj)
        {
            if (RetryCount > 0)
            {
                if (RetryCount < 50)
                {
                    ResultText = string.Format("리트라이 카운트는 최소 1초로 해주세요.");
                    return;
                }

                FBaseFunc.Ins.Cfg.SetRetryCount(RetryCount);
                ResultText = string.Format("리트라이 카운트 : {0} 저장", RetryCount);
            }
            else
            {
                ResultText = "리트라이 카운트 값이 이상합니다.";
            }
        }
        private void SaveExpenseReportEvent(object? obj)
        {
            if (ExpenseReportName != null && ExpenseReportName.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetExpenseReportName(ExpenseReportName);
                ResultText = string.Format("지품 이름 : {0} 저장", ExpenseReportName);
            }
            else
            {
                ResultText = "이름 값이 이상합니다.";
            }
        }
        private void SaveDefIndustiralEvent(object? obj)
        {
            if (DefIndustiral.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetDefIndustiral(DefIndustiral);
                ResultText = string.Format("산업 주소 : {0} 저장", DefIndustiral);
            }
            else
            {
                ResultText = "산업 주소가 공백입니다.";
            }
        }
        private void SaveDefPetrochemEvent(object? obj)
        {
            if (DefPetrochem.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetDefPetrochem(DefPetrochem);
                ResultText = string.Format("유화 주소 : {0} 저장", DefPetrochem);
            }
            else
            {
                ResultText = "유화 주소가 공백입니다.";
            }
        }
        private void SaveCompanyEvent(object? obj)
        {
            if (SelectedCompanyItem.Contains("산업") || SelectedCompanyItem.Contains("유화"))
            {
                FBaseFunc.Ins.Cfg.SetCompany(SelectedCompanyItem);
                ResultText = string.Format("사용할 그룹웨어 : {0} 저장", SelectedCompanyItem);
            }
            else
            {
                ResultText = "어 뭔가 이상한대요?";
            }
        }
        private void SaveIDEvent(object? obj)
        {
            if (ID.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetID(ID);
                ResultText = string.Format("ID : {0} 저장", ID);
            }
            else
            {
                ResultText = "ID가 공백입니다.";
            }
        }
        private void SavePWEvent(object? obj)
        {
            if (PW.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetPW(PW);
                ResultText = string.Format("PW : {0} 저장", PW);
                PW = "";
            }
            else
            {
                ResultText = "PW가 공백입니다.";
            }
        }
        private void SaveOldIDEvent(object? obj)
        {
            if (OldID.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetOldID(OldID);
                ResultText = string.Format("구 그룹웨어 ID : {0} 저장", OldID);
                IsSyncListEnable = !FBaseFunc.Ins.Cfg.OldLoginInfo.IsEmpty();
            }
            else
            {
                ResultText = "구 그룹웨어 ID가 공백입니다.";
            }
        }
        private void SaveOldPWEvent(object? obj)
        {
            if (OldPW.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetOldPW(OldPW);
                ResultText = string.Format("구 그룹웨어 PW : {0} 저장", OldPW);
                OldPW = "";
                IsSyncListEnable = !FBaseFunc.Ins.Cfg.OldLoginInfo.IsEmpty();
            }
            else
            {
                ResultText = "구 그룹웨어 PW가 공백입니다.";
            }
        }
        private void SyncListEvenet(object? obj)
        {
            WindowMessageBoxDialog messageBoxDialog = new("경고경고", "서버로부터 유효성검사 목록을 다시 받습니다.\n계속 하시겠습니까?", true);
            if (messageBoxDialog.ShowDialog() == false)
            {
                return;
            }

            bool ret = FBaseFunc.Ins.SyncList();
            if (!ret)
            {
                ResultText = "동기화 실패";
                return;
            }
            else
            {
                App.SettingWindow.Reload();
                ResultText = "동기화 끝";
            }
        }
        private void DBToCSVEvent(object? obj)
        {
            bool ret = FBaseFunc.Ins.DBToCSV(out string err);
            if (!ret)
            {
                ResultText = err;
            }
            else
            {
                ResultText = "파일 만들기 성공";
            }
        }

        public void Reload()
        {
            Timeout = FBaseFunc.Ins.Cfg.Timeout;
            DelayTime = FBaseFunc.Ins.Cfg.DelayTime;
            RetryCount = FBaseFunc.Ins.Cfg.RetryCount;
            ExpenseReportName = FBaseFunc.Ins.Cfg.ExpenseReportName;
            DefIndustiral = FBaseFunc.Ins.Cfg.Industrial;
            DefPetrochem = FBaseFunc.Ins.Cfg.Petrochem;
            ID = FBaseFunc.Ins.Cfg.LoginInfo.ID;
            PW = "";
            OldID = FBaseFunc.Ins.Cfg.OldLoginInfo.ID;
            OldPW = "";
            SelectedCompanyItem = FBaseFunc.Ins.Cfg.Company;

            ResultText = "";

            IsDarkTheme = FBaseFunc.Ins.Cfg.IsDarkTheme;
            IsStartOnSync = FBaseFunc.Ins.Cfg.IsStartOnSync;
            IsSuccessLogHide = FBaseFunc.Ins.Cfg.IsSuccessLogHide;
            IsGuardScreenSaver = FBaseFunc.Ins.Cfg.IsGuardScreenSaver;
            IsSyncListEnable = !FBaseFunc.Ins.Cfg.OldLoginInfo.IsEmpty();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}