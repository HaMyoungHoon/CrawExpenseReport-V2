using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using CrawExpenseReport.Data;
using CrawExpenseReport.Screen.Popup;
using CrawExpenseReport.Base;
using MaterialDesignExtensions.Model;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;

namespace CrawExpenseReport.Screen
{
    public class PageMakeViewModel : INotifyPropertyChanged
    {
        private bool _isDownloadEnable;
        private bool _isLoadDataEnable;
        private bool _isLoadFileEnable;
        private bool _isCopyDataEnable;
        private bool _isSaveEnable;
        private bool _isTitleEnable;
        private bool _isCompanyEnable;
        private bool _isWorkplaceEnable;
        private bool _isAddRowEnable;
        private bool _isDeleteRowEnable;
        private bool _isGridEnable;
        private long _creditTotal;
        private long _debitTotal;
        private string _title;
        private string _selectCompany;
        private IAutocompleteSource _companySource;
        private string _selectWorkplace;
        private IAutocompleteSource _workplaceSource;
        private ObservableCollection<SlipItem> _slips;
        private Page _page;
        private DataGrid _dgSheet;
        private int _sheetFontSize;
        private bool _isCtrlKey;

        private string _priviousPropertyValue;
        private DispatcherTimer _timer;
        public PageMakeViewModel()
        {
            Slips = new ObservableCollection<SlipItem>();
            IsDownloadEnable = true;
            IsLoadDataEnable = true;
            IsLoadFileEnable = true;
            IsCopyDataEnable = true;
            IsSaveEnable = false;
            IsTitleEnable = true;
            IsCompanyEnable = true;
            IsWorkplaceEnable = true;
            IsAddRowEnable = true;
            IsDeleteRowEnable = true;
            IsGridEnable = true;
            Title = "";
            SheetFontSize = FBaseFunc.Ins.Cfg.SheetFontSize;

            CompanySource = new OperatingAutocompleteSource();
            WorkplaceSource = new OperatingAutocompleteSource();

            LoadedCommand = new CommandImpl(LoadedEvent);
            KeyDownCommand = new CommandImpl(KeyDownEvent);
            SampleFileDownload = new CommandImpl(SampleFileDownloadEvent);
            LoadData = new CommandImpl(LoadDataEvent);
            LoadFile = new CommandImpl(LoadFileEvent);
            CopyData = new CommandImpl(CopyDataEvent);
            SaveFile = new CommandImpl(SaveFileEvent);
            AddRow = new CommandImpl(AddRowEvent);
            DeleteRow = new CommandImpl(DeleteRowEvent);
            CalcDebit = new CommandImpl(CalcDebitEvent);
            _isCtrlKey = false;

            _priviousPropertyValue = "";

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.IsEnabled = true;

            FBaseFunc.Ins.SetCopyEndCallback(CopyEndCallBack);
        }
        public bool IsDownloadEnable
        {
            get => _isDownloadEnable;
            set
            {
                _isDownloadEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoadDataEnable
        {
            get => _isLoadDataEnable;
            set
            {
                _isLoadDataEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoadFileEnable
        {
            get => _isLoadFileEnable;
            set
            {
                _isLoadFileEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsCopyDataEnable
        {
            get => _isCopyDataEnable;
            set
            {
                _isCopyDataEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsSaveEnable
        {
            get => _isSaveEnable;
            set
            {
                _isSaveEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsTitleEnable
        {
            get => _isTitleEnable;
            set
            {
                _isTitleEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsCompanyEnable
        {
            get => _isCompanyEnable;
            set
            {
                _isCompanyEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsWorkplaceEnable
        {
            get => _isWorkplaceEnable;
            set
            {
                _isWorkplaceEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsAddRowEnable
        {
            get => _isAddRowEnable;
            set
            {
                _isAddRowEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsDeleteRowEnable
        {
            get => _isDeleteRowEnable;
            set
            {
                _isDeleteRowEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsGridEnable
        {
            get => _isGridEnable;
            set
            {
                _isGridEnable = value;
                OnPropertyChanged();
            }
        }
        public long CreditTotal
        {
            get => _creditTotal;
            set
            {
                _creditTotal = value;
                OnPropertyChanged();
            }
        }
        public long DebitTotal
        {
            get => _debitTotal;
            set
            {
                _debitTotal = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value.Replace(@"'", "");
                OnPropertyChanged();
            }
        }
        public string SelectCompany
        {
            get => _selectCompany;
            set
            {
                if (_selectCompany != value)
                {
                    _selectCompany = value;
                    SelectWorkplace = null;
                    ((OperatingAutocompleteSource)WorkplaceSource).SetItems(FBaseFunc.Ins.CallWorkplaceNameList(value));
                    OnPropertyChanged();
                }
            }
        }
        public IAutocompleteSource CompanySource
        {
            get => _companySource;
            set
            {
                _companySource = value;
                OnPropertyChanged();
            }
        }
        public string SelectWorkplace
        {
            get => _selectWorkplace;
            set
            {
                if (_selectWorkplace != value)
                {
                    _selectWorkplace = value;
                    _dgSheet.Focus();
                    try
                    {
                        while (_slips.Count > 0)
                        {
                            _slips.RemoveAt(0);
                        }
                    }
                    catch
                    {

                    }
                    OnPropertyChanged();
                }
            }
        }
        public IAutocompleteSource WorkplaceSource
        {
            get => _workplaceSource;
            set
            {
                _workplaceSource = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SlipItem> Slips
        {
            get => _slips;
            set
            {
                _slips = value;
            }
        }
        public int SheetFontSize
        {
            get => _sheetFontSize;
            set
            {
                if (value >= 1)
                {
                    _sheetFontSize = value;
                    FBaseFunc.Ins.Cfg.SetSheetFontSize(value);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadedCommand { get; }
        public ICommand KeyDownCommand { get; }
        public ICommand SampleFileDownload { get; }
        public ICommand LoadData { get; }
        public ICommand LoadFile { get; }
        public ICommand CopyData { get; }
        public ICommand SaveFile { get; }
        public ICommand AddRow { get; }
        public ICommand DeleteRow { get; }
        public ICommand CalcDebit { get; }

        private bool? SetSlip(ref SlipItem newPropertyValue)
        {
            SlipItem slip = new SlipItem(SelectCompany, SelectWorkplace);
            slip.Gubun = newPropertyValue.Gubun;
            object propertyValue = newPropertyValue.Gubun;      SetGubun(ref propertyValue, ref slip);
            propertyValue = newPropertyValue.Account;           SetAccount(ref propertyValue, ref slip);
            slip.TaxDate = newPropertyValue.TaxDate;
            propertyValue = newPropertyValue.Type;              SetType(ref propertyValue, ref slip);
            propertyValue = newPropertyValue.Correspondent;     SetCorrespondent(ref propertyValue, ref slip);
            slip.Briefs = newPropertyValue.Briefs;
            propertyValue = newPropertyValue.Price;             SetPrice(ref propertyValue, ref slip);
            propertyValue = newPropertyValue.Department;        SetDepartment(ref propertyValue, ref slip);
            propertyValue = newPropertyValue.SupplyPrice;       SetSupplyPrice(ref propertyValue, ref slip);

            newPropertyValue = slip;
            return null;
        }
        private bool? SetGubun(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                slip.Gubun = "";
                return true;
            }
            string propertyValue = newProperyValue.ToString();
            if (slip.Gubun == propertyValue)
            {
                return null;
            }

            if (propertyValue == "차변" || propertyValue == "대변")
            {
                return null;
            }
            else
            {
                newProperyValue = slip.Gubun;
                return true;
            }
        }
        private bool? SetAccount(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                _priviousPropertyValue = "";
                slip.Account = "";
                return true;
            }
            string propertyValue = newProperyValue.ToString();
            if (slip.Account == propertyValue || propertyValue == "")
            {
                return null;
            }
            if (_priviousPropertyValue == propertyValue)
            {
                return null;
            }
            _priviousPropertyValue = propertyValue;

            var accountNameList = FBaseFunc.Ins.CallAccountNameList(slip.Gubun, SelectCompany).FindAll(x => x.Contains(propertyValue));
            if (accountNameList == null || accountNameList.Count == 0)
            {
                return true;
            }

            if (accountNameList.Count == 1)
            {
                slip.Account = accountNameList[0];
                return true;
            }

            WindowListBoxDialog windowListBox = new WindowListBoxDialog(accountNameList);
            if (windowListBox.ShowDialog() == true)
            {
                slip.Account = windowListBox.GetSelectString();
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool? SetType(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                _priviousPropertyValue = "";
                slip.Type = "";
                return true;
            }
            string propertyValue = newProperyValue.ToString();
            if (slip.Type == propertyValue || propertyValue == "")
            {
                return null;
            }
            if (_priviousPropertyValue == propertyValue)
            {
                return null;
            }
            _priviousPropertyValue = propertyValue;

            var typelist = FBaseFunc.Ins.CallTypeNameList().FindAll(x => x.Contains(propertyValue));
            if (typelist == null || typelist.Count == 0)
            {
                return true;
            }

            if (typelist.Count == 1)
            {
                slip.Type = typelist[0];
                return true;
            }

            WindowListBoxDialog windowListBox = new WindowListBoxDialog(typelist);
            if (windowListBox.ShowDialog() == true)
            {
                slip.Type = windowListBox.GetSelectString();
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool? SetCorrespondent(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                _priviousPropertyValue = "";
                slip.Correspondent = "";
                return true;
            }
            string propertyValue = newProperyValue.ToString();
            if (slip.Correspondent == propertyValue || propertyValue == "")
            {
                return null;
            }
            if (_priviousPropertyValue == propertyValue)
            {
                return null;
            }
            _priviousPropertyValue = propertyValue;

            var correspondentList = FBaseFunc.Ins.CallCorrespondentNameList(SelectCompany, slip.Account).FindAll(x => x.Contains(propertyValue));
            if (correspondentList == null || correspondentList.Count == 0)
            {
                return true;
            }

            if (correspondentList.Count == 1)
            {
                slip.Correspondent = correspondentList[0];
                return true;
            }

            WindowListBoxDialog windowListBox = new WindowListBoxDialog(correspondentList);
            if (windowListBox.ShowDialog() == true)
            {
                slip.Correspondent = windowListBox.GetSelectString();
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool? SetPrice(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                slip.Price = 0;
                return true;
            }

            string propertyValue = newProperyValue.ToString();
            if (slip.Price.ToString() == propertyValue)
            {
                return null;
            }
            if (long.TryParse(propertyValue.Replace(",", ""), out long result) == false)
            {
                newProperyValue = slip.Price;
                return true;
            }
            else
            {
                slip.Price = result;
            }

            return null;
        }
        private bool? SetDepartment(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                _priviousPropertyValue = "";
                slip.Department = "";
                return true;
            }

            string propertyValue = newProperyValue.ToString();
            if (slip.Department == propertyValue || propertyValue == "")
            {
                return null;
            }
            if (_priviousPropertyValue == propertyValue)
            {
                return null;
            }
            _priviousPropertyValue = propertyValue;

            var departmentList = FBaseFunc.Ins.CallDepartmentNameList(SelectCompany, SelectWorkplace).FindAll(x => x.Contains(propertyValue));
            if (departmentList == null || departmentList.Count == 0)
            {
                return true;
            }

            if (departmentList.Count == 1)
            {
                slip.Department = departmentList[0];
                return true;
            }

            WindowListBoxDialog windowListBox = new WindowListBoxDialog(departmentList);
            if (windowListBox.ShowDialog() == true)
            {
                slip.Department = windowListBox.GetSelectString();
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool? SetSupplyPrice(ref object newProperyValue, ref SlipItem slip)
        {
            if (newProperyValue == null)
            {
                slip.SupplyPrice = 0;
                return true;
            }

            string propertyValue = newProperyValue.ToString();
            if (slip.SupplyPrice.ToString() == propertyValue)
            {
                return null;
            }
            if (!slip.Account.Contains("부가세") && !slip.Account.Contains("13500") && !slip.Account.Contains("25500"))
            {
                slip.SupplyPrice = 0;
                return true;
            }
            if (long.TryParse(propertyValue.Replace(",", ""), out long result) == false)
            {
                newProperyValue = slip.SupplyPrice;
                return true;
            }
            else
            {
                slip.SupplyPrice = result;
            }

            return null;
        }

        private void LoadedEvent(object obj)
        {
            _page = obj as Page;
            if (_page == null)
            {
                return;
            }

            var dgSheet = (_page as PageMake)?.dgSheet;
            if (dgSheet == null)
            {
                return;
            }
            _page.PreviewKeyDown += Page_PreviewKeyDown;
            _dgSheet = dgSheet;
            _dgSheet.KeyDown += _dgSheet_KeyDown;
            _dgSheet.KeyUp += _dgSheet_KeyUp;
            _dgSheet.PreviewMouseWheel += _dgSheet_PreviewMouseWheel;
            _dgSheet.Focus();
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Slips.Add(new SlipItem(SelectCompany, SelectWorkplace));
                e.Handled = true;
            }
            if (e.Key == Key.W && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Slips.Count > 0)
                {
                    Slips.Remove(Slips.Last());
                }
                if (Slips.Count == 0)
                {
                    _dgSheet.Focus();
                }
                e.Handled = true;
            }
        }
        private void _dgSheet_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                _isCtrlKey = false;
                e.Handled = true;
            }
        }
        private void _dgSheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ClipboardPaste();
                _isCtrlKey = false;
                e.Handled = true;
            }
            if (e.Key == Key.LeftCtrl)
            {
                _isCtrlKey = true;
                e.Handled = true;
            }
            if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Slips.Add(new SlipItem(SelectCompany, SelectWorkplace));
                e.Handled = true;
            }
            if (e.Key == Key.W && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Slips.Count > 0)
                {
                    Slips.Remove(Slips.Last());
                }
                if (Slips.Count == 0)
                {
                    _dgSheet.Focus();
                }
                e.Handled = true;
            }
        }
        private void _dgSheet_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isCtrlKey == true)
            {
                e.Handled = true;
                if (e.Delta < 0)
                {
                    SheetFontSize--;
                }
                else
                {
                    SheetFontSize++;
                }
            }
        }

        private void KeyDownEvent(object obj)
        {

        }
        private void SampleFileDownloadEvent(object obj)
        {
            FBaseFunc.Ins.GetSampleCSV(out string err);
            if (err.Length > 0)
            {
                WindowMessageBoxDialog dlg = new WindowMessageBoxDialog("경고경고", err);
                dlg.ShowDialog();
            }
        }
        private void LoadDataEvent(object obj)
        {
            try
            {
                WindowListBoxDialog windowListBox = new WindowListBoxDialog(FBaseFunc.Ins.CopyedTable.Select(x => x.Title));
                if (windowListBox.ShowDialog() == true)
                {
                    int selectIndex = windowListBox.GetSelectIndex();
                    if (windowListBox.GetSelectString().Length <= 0 || selectIndex == -1 || FBaseFunc.Ins.CopyedTable.Count <= selectIndex)
                    {
                        Title = "";
                        SelectCompany = null;
                        SelectWorkplace = null;
                        return;
                    }
                    var title = FBaseFunc.Ins.CopyedTable.ElementAt(selectIndex).Title;
                    if (title.Length <= 0)
                    {
                        Title = "";
                        SelectCompany = null;
                        SelectWorkplace = null;
                        return;
                    }

                    var list = FBaseFunc.Ins.CopyedTable[selectIndex].Clone();
                    if (list == null)
                    {
                        return;
                    }
                    SetAll(list);
                }
            }
            catch (Exception ex)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("불러오기 실패", ex.Message, false);
                messageBoxDialog.ShowDialog();
            }
        }
        private void LoadFileEvent(object obj)
        {
            FBaseFunc.CopyDataTable data = FBaseFunc.Ins.GetCSV(out string err);
            if (err.Length <= 0 && data.IsEmpty() == false)
            {
                SetAll(data);
            }

            if (err.Length > 0)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("파일 불러오기 실패", err, false);
                messageBoxDialog.ShowDialog();
            }
        }
        private void CopyDataEvent(object obj)
        {
            WindowCopyBoxDialog copyBoxDialog = new WindowCopyBoxDialog();
            if (copyBoxDialog.ShowDialog() == true)
            {
                if (FBaseFunc.Ins.CanLogin())
                {
                    FBaseFunc.Ins.Copy();
                }
                else
                {
                    WindowMessageBoxDialog dlg = new WindowMessageBoxDialog("로그인 정보를 넣어주세요.", "로그인 정보를 넣어주세요.");
                    dlg.ShowDialog();
                }
            }
        }
        private void SaveFileEvent(object obj)
        {
            if (Title.Length <= 0 || SelectCompany.Length <= 0 || SelectWorkplace.Length <= 0)
            {
                IsSaveEnable = false;
                return;
            }

            FBaseFunc.CopyDataTable copyedTable = SetCopyDataTable();

            if (FBaseFunc.Ins.ValidationData(ref copyedTable, out string err) == false)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("유효성검사 실패", err, false);
                messageBoxDialog.ShowDialog();
                return;
            }

            if (err.Length > 0)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("유효성검사 오류", err, true);
                if (messageBoxDialog.ShowDialog() == false)
                {
                    return;
                }

                SetAll(copyedTable);
            }

            try
            {
                WindowListBoxDialog windowListBox = new WindowListBoxDialog(FBaseFunc.Ins.CopyedTable.Select(x => x.Title));
                if (windowListBox.ShowDialog() == true)
                {
                    if (FBaseFunc.Ins.SaveCopyFileData(copyedTable, windowListBox.GetSelectIndex(), out err) == false)
                    {
                        WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("저장실패", err, false);
                        messageBoxDialog.ShowDialog();
                    }
                    else
                    {
                        WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("저장완료!", "데이터가 저장된 것 같습니다.\n필요하다면 다시 불러서 확인해주세요.", false);
                        messageBoxDialog.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowMessageBoxDialog messageBoxDialog = new WindowMessageBoxDialog("저장실패", ex.Message, false);
                messageBoxDialog.ShowDialog();
            }
        }
        private void AddRowEvent(object obj)
        {
            var checkedSlip = Slips.LastOrDefault(x => x.IsChecked == true);
            if (checkedSlip != null)
            {
                Slips.Insert(Slips.IndexOf(checkedSlip), new SlipItem(SelectCompany, SelectWorkplace));
            }
            else
            {
                Slips.Add(new SlipItem(SelectCompany, SelectWorkplace));
            }
        }
        private void DeleteRowEvent(object obj)
        {
            if (Slips.Count > 0)
            {
                var checkedSlips = Slips.Where(x => x.IsChecked == true).ToList();
                if (checkedSlips.Count > 0)
                {
                    foreach(var checkedSlip in checkedSlips)
                    {
                        Slips.Remove(checkedSlip);
                    }
                }
                else
                {
                    Slips.Remove(Slips.Last());
                }
            }
        }
        private void CalcDebitEvent(object obj)
        {
            CalcDebitMethod();
        }

        private void CopyEndCallBack(bool isThreadCall)
        {
            if (isThreadCall)
            {
                App.SettingWindow.Dispatcher.Invoke((Action)(() =>
                {
                    SetAll(FBaseFunc.Ins.TempCopyTable.Clone());
                }));
            }
        }
        public void Reload()
        {
            ClearAll();
            ((OperatingAutocompleteSource)CompanySource).SetItems(FBaseFunc.Ins.CallCompanyNameList());
        }
        private void ClearAll()
        {
            Title = "";
            SelectCompany = null;
            SelectWorkplace = null;
            CreditTotal = 0;
            DebitTotal = 0;
            ((OperatingAutocompleteSource)CompanySource).Clear();
            ((OperatingAutocompleteSource)WorkplaceSource).Clear();
        }
        private void CalcDebitMethod()
        {
            long credit = 0;
            long debit = 0;
            foreach (long price in Slips.Where(x => x.Gubun == "차변").Select(x => x.Price).ToList())
            {
                credit += price;
            }

            foreach (long price in Slips.Where(x => x.Gubun == "대변").Select(x => x.Price).ToList())
            {
                debit += price;
            }

            CreditTotal = credit;
            DebitTotal = debit;
        }
        private void SetAll(FBaseFunc.CopyDataTable data)
        {
            Title = data.Title;
            SelectCompany = data.CompanyValue.GetCompanyName();
            SelectWorkplace = null;
            SelectWorkplace = data.WorkplaceValue.GetWorkplaceName();

            for (int i = 0; i < data.SlipValues.Count; i++)
            {
                SlipItem slip = new SlipItem(SelectCompany, SelectWorkplace, data.SlipValues[i]);
                SetSlip(ref slip);
                if (data.SlipValues.Count > _slips.Count)
                {
                    _slips.Add(slip);
                }
                else
                {
                    _slips[i] = slip;
                }
            }
        }
        private FBaseFunc.CopyDataTable SetCopyDataTable()
        {
            FBaseFunc.CopyDataTable copyedTable = new FBaseFunc.CopyDataTable();
            copyedTable.Title = Title;

            Regex regex = new Regex("[^0-9]");
            int index = SelectCompany.IndexOf("/");
            if (index == -1)
            {
                if (regex.IsMatch(SelectCompany))
                {
                    copyedTable.CompanyValue.CompanyCode = SelectCompany;
                    copyedTable.WorkplaceValue.CompanyCode = SelectCompany;
                }
                else
                {
                    copyedTable.CompanyValue.CompanyName = SelectCompany;
                }
            }
            else
            {
                copyedTable.CompanyValue.CompanyCode = SelectCompany.Substring(0, index);
                copyedTable.CompanyValue.CompanyName = SelectCompany.Substring(index + 1);
                copyedTable.WorkplaceValue.CompanyCode = copyedTable.CompanyValue.CompanyCode;
            }

            index = SelectWorkplace.IndexOf("/");
            if (index == -1)
            {
                if (regex.IsMatch(SelectWorkplace))
                {
                    copyedTable.WorkplaceValue.WorkplaceCode = SelectWorkplace;
                }
                else
                {
                    copyedTable.WorkplaceValue.WorkplaceName = SelectWorkplace;
                }
            }
            else
            {
                copyedTable.WorkplaceValue.WorkplaceCode = SelectWorkplace.Substring(0, index);
                copyedTable.WorkplaceValue.WorkplaceName = SelectWorkplace.Substring(index + 1);
            }

            foreach (SlipItem slip in _slips)
            {
                var buff = new FBaseFunc.SlipTable()
                {
                    Gubun = slip.Gubun,
                    Type = slip.Type,
                    Briefs = slip.Briefs,
                    Price = slip.Price.ToString(),
                    SupplyPrice = slip.SupplyPrice.ToString(),
                };
                buff.SetTaxTime(slip.TaxDate);
                buff.Account.CompanyCode = copyedTable.CompanyValue.CompanyCode;
                buff.Account.SetData(slip.Account);
                buff.Correspondent.CompanyCode = copyedTable.CompanyValue.CompanyCode;
                buff.Correspondent.SetData(slip.Correspondent);
                buff.Department.CompanyCode = copyedTable.CompanyValue.CompanyCode;
                buff.Department.WorkplaceCode = copyedTable.WorkplaceValue.WorkplaceCode;
                buff.Department.SetData(slip.Department);
                copyedTable.SlipValues.Add(buff);
            }

            return copyedTable;
        }
        public void ClipboardPaste()
        {
            string clipboard = Clipboard.GetData(DataFormats.Text) as string;
            if (clipboard == null)
            {
                return;
            }

            List<string[]> rowsData = ClipboardHelper.ParseClipboardData(clipboard);
            if (_dgSheet.CurrentColumn == null)
            {
                return;
            }
            int startCol = _dgSheet.CurrentColumn.DisplayIndex;
            if (startCol == -1)
            {
                return;
            }
            int startRow = 0;
            if (_dgSheet.SelectedCells.Count > 0)
            {
                if (Slips.Count > 0)
                {
                    startRow = Slips.IndexOf(_dgSheet.SelectedCells[0].Item as SlipItem);
                }
            }
            foreach (string[] colData in rowsData)
            {
                if (colData.Length <= 0)
                {
                    continue;
                }

                if (startRow >= Slips.Count)
                {
                    Slips.Add(new SlipItem(SelectCompany, SelectWorkplace));
                }

                Slips[startRow].SetData(startCol, colData);
                OnPropertyChanged("Slips");

                startRow++;
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            IsDownloadEnable = FBaseFunc.Ins.CanCopy();
            IsLoadDataEnable = FBaseFunc.Ins.CanCopy();
            IsLoadFileEnable = FBaseFunc.Ins.CanCopy();
            IsCopyDataEnable = FBaseFunc.Ins.CanCopy();
            IsSaveEnable = FBaseFunc.Ins.CanCopy() && Title.Length > 0;
            IsTitleEnable = FBaseFunc.Ins.CanCopy();
            IsCompanyEnable = FBaseFunc.Ins.CanCopy();
            IsWorkplaceEnable = FBaseFunc.Ins.CanCopy();
            IsAddRowEnable = FBaseFunc.Ins.CanCopy();
            IsDeleteRowEnable = FBaseFunc.Ins.CanCopy();
            IsGridEnable = FBaseFunc.Ins.CanCopy();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class OperatingAutocompleteSource : IAutocompleteSource, INotifyPropertyChanged
        {
            private ObservableCollection<string> _items;
            public OperatingAutocompleteSource()
            {
                Items = new ObservableCollection<string>();
            }
            public ObservableCollection<string> Items
            {
                get => _items;
                set
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }

            public IEnumerable Search(string searchTerm)
            {
                searchTerm = searchTerm ?? string.Empty;
                searchTerm = searchTerm.ToLower();

                return Items.Where(x => x.ToLower().Contains(searchTerm));
            }
            public OperatingAutocompleteSource SetItems(List<string> data)
            {
                Items.Clear();
                foreach (var item in data)
                {
                    Items.Add(item);
                }
                return this;
            }
            public void Clear()
            {
                _items.Clear();
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class SlipItem : INotifyPropertyChanged
        {
            private bool _isChecked;
            private string _gubun;
            private string _account;
            private DateTime _taxDate;
            private string _type;
            private string _correspondent;
            private long _price;
            private string _briefs;
            private string _department;
            private long _supplyPrice;
            public SlipItem(string selectCompany, string selectWorkplace)
            {
                IsChecked = false;
                SelectCompany = selectCompany;
                SelectWorkplace = selectWorkplace;
                Gubun = "차변";
                Account = "";
                TaxDate = DateTime.Now;
                Type = "";
                Correspondent = "";
                Price = 0;
                Briefs = "";
                Department = "";
                SupplyPrice = 0;
            }
            public SlipItem(string selectCompany, string selectWorkplace, FBaseFunc.SlipTable data)
            {
                IsChecked = false;
                SelectCompany = selectCompany;
                SelectWorkplace = selectWorkplace;
                Gubun = data.Gubun;
                if (data.Account.GetAccountName().Equals("/"))
                {
                    Account = "";
                }
                else
                {
                    Account = data.Account.GetAccountName();
                }

                if (data.Correspondent.GetCorrespondentName().Equals("/"))
                {
                    Correspondent = "";
                }
                else
                {
                    Correspondent = data.Correspondent.GetCorrespondentName();
                }
                TaxDate = data.GetTaxTimeDateType();
                Type = data.Type;

                if (data.Price == null)
                {
                    Price = 0;
                }
                else
                {
                    long.TryParse(data.Price.Replace(",", ""), out long price);
                    Price = price;
                }
                Briefs = data.Briefs;

                if (data.Department.GetDepartmentName().Equals("/"))
                {
                    Department = "";
                }
                else
                {
                    Department = data.Department.GetDepartmentName();
                }

                if (data.SupplyPrice == null)
                {
                    SupplyPrice = 0;
                }
                else
                {
                    long.TryParse(data.SupplyPrice.Replace(",", ""), out long supplyPrice);
                    SupplyPrice = supplyPrice;
                }
            }
            public string SelectCompany { get; set; }
            public string SelectWorkplace { get; set; }

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
            public string Gubun
            { 
                get => _gubun;
                set
                {
                    SetGubun(value);
                    OnPropertyChanged();
                } 
            }
            public string Account
            {
                get => _account;
                set
                {
                    SetAccount(value);
                    OnPropertyChanged();
                }
            }
            public DateTime TaxDate
            {
                get => _taxDate;
                set
                {
                    _taxDate = value;
                    OnPropertyChanged();
                }
            }
            public string Type
            {
                get => _type;
                set
                {
                    SetType(value);
                    OnPropertyChanged();
                }
            }
            public string Correspondent
            {
                get => _correspondent;
                set
                {
                    SetCorrespondent(value);
                    OnPropertyChanged();
                }
            }
            public long Price
            {
                get => _price;
                set
                {
                    SetPrice(value);
                    OnPropertyChanged();
                }
            }
            public string Briefs
            {
                get => _briefs;
                set
                {
                    _briefs = value;
                    OnPropertyChanged();
                }
            }
            public string Department
            {
                get => _department;
                set
                {
                    SetDepartment(value);
                    OnPropertyChanged();
                }
            }
            public long SupplyPrice
            {
                get => _supplyPrice;
                set
                {
                    SetSupplyPrice(value);
                    OnPropertyChanged();
                }
            }

            public void SetData(int startIndex, string[] data)
            {
                int dataCount = data.Count();
                switch (startIndex)
                {
                    case 1:
                        {
                            if (dataCount >= 1) Gubun           = data[0];
                            if (dataCount >= 2) Account         = data[1];
                            if (dataCount >= 3)
                            {
                                if (DateTime.TryParse(data[2], out DateTime ret))
                                {
                                    TaxDate = ret;
                                }
                                else
                                {
                                    TaxDate = DateTime.Now;
                                }
                            }
                            if (dataCount >= 4) Type            = data[3];
                            if (dataCount >= 5) Correspondent   = data[4];
                            if (dataCount >= 6)
                            {
                                if (data[5] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[5].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 7) Briefs          = data[6];
                            if (dataCount >= 8) Department      = data[7];
                            if (dataCount >= 9)
                            {
                                if (data[8] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[8].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            if (dataCount >= 1) Account         = data[0];
                            if (dataCount >= 2)
                            {
                                if (DateTime.TryParse(data[1], out DateTime ret))
                                {
                                    TaxDate = ret;
                                }
                                else
                                {
                                    TaxDate = DateTime.Now;
                                }
                            }
                            if (dataCount >= 3) Type            = data[2];
                            if (dataCount >= 4) Correspondent   = data[3];
                            if (dataCount >= 5)
                            {
                                if (data[4] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[4].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 6) Briefs          = data[5];
                            if (dataCount >= 7) Department      = data[6];
                            if (dataCount >= 8)
                            {
                                if (data[7] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[7].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            if (dataCount >= 1)
                            {
                                if (DateTime.TryParse(data[0], out DateTime ret)) 
                                {
                                    TaxDate = ret;
                                }
                                else
                                {
                                    TaxDate = DateTime.Now;
                                }
                            }
                            if (dataCount >= 2) Type = data[1];
                            if (dataCount >= 3) Correspondent = data[2];
                            if (dataCount >= 4)
                            {
                                if (data[3] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[3].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 5) Briefs = data[4];
                            if (dataCount >= 6) Department = data[5];
                            if (dataCount >= 7)
                            {
                                if (data[6] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[6].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 4:
                        {
                            if (dataCount >= 1) Type = data[0];
                            if (dataCount >= 2) Correspondent = data[1];
                            if (dataCount >= 3)
                            {
                                if (data[2] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[2].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 4) Briefs = data[3];
                            if (dataCount >= 5) Department = data[4];
                            if (dataCount >= 6)
                            {
                                if (data[5] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[5].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 5:
                        {
                            if (dataCount >= 1) Correspondent = data[0];
                            if (dataCount >= 2)
                            {
                                if (data[1] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[1].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 3) Briefs = data[2];
                            if (dataCount >= 4) Department = data[3];
                            if (dataCount >= 5)
                            {
                                if (data[4] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[4].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 6:
                        {
                            if (dataCount >= 1)
                            {
                                if (data[0] == null)
                                {
                                    Price = 0;
                                }
                                else
                                {
                                    long.TryParse(data[0].Replace(",", ""), out long ret);
                                    Price = ret;
                                }
                            }
                            if (dataCount >= 2) Briefs = data[1];
                            if (dataCount >= 3) Department = data[2];
                            if (dataCount >= 4)
                            {
                                if (data[3] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[3].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 7:
                        {
                            if (dataCount >= 1) Briefs = data[0];
                            if (dataCount >= 2) Department = data[1];
                            if (dataCount >= 3)
                            {
                                if (data[2] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[2].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 8:
                        {
                            if (dataCount >= 1) Department = data[0];
                            if (dataCount >= 2)
                            {
                                if (data[1] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[1].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                    case 9:
                        {
                            if (dataCount >= 1)
                            {
                                if (data[0] == null)
                                {
                                    SupplyPrice = 0;
                                }
                                else
                                {
                                    long.TryParse(data[0].Replace(",", ""), out long ret);
                                    SupplyPrice = ret;
                                }
                            }
                        }
                        break;
                }
            }
            private void SetGubun(string data)
            {
                if (data == null)
                {
                    _gubun = "";
                    Account = "";
                    return;
                }
                if (_gubun == data)
                {
                    return;
                }

                if (data == "차변" || data == "대변")
                {
                    _gubun = data;
                    Account = "";
                    return;
                }
            }
            private void SetAccount(string data)
            {
                if (data == null || data == "")
                {
                    _account = "";
                    Correspondent = "";
                    return;
                }
                if (_account == data)
                {
                    return;
                }

                var accountNameList = FBaseFunc.Ins.CallAccountNameList(Gubun, SelectCompany).FindAll(x => x.Contains(data));
                if (accountNameList == null || accountNameList.Count == 0)
                {
                    return;
                }

                if (accountNameList.Count == 1)
                {
                    _account = accountNameList[0];
                    Correspondent = "";
                    return;
                }

                WindowListBoxDialog windowListBox = new WindowListBoxDialog(accountNameList);
                if (windowListBox.ShowDialog() == true)
                {
                    _account = windowListBox.GetSelectString();
                    Correspondent = "";
                    return;
                }
            }
            private void SetType(string data)
            {
                if (data == null || data == "")
                {
                    _type = "";
                    return;
                }
                if (_type == data)
                {
                    return;
                }
                var typelist = FBaseFunc.Ins.CallTypeNameList().FindAll(x => x.Contains(data));
                if (typelist == null || typelist.Count == 0)
                {
                    return;
                }

                if (typelist.Count == 1)
                {
                    _type = typelist[0];
                    return;
                }

                WindowListBoxDialog windowListBox = new WindowListBoxDialog(typelist);
                if (windowListBox.ShowDialog() == true)
                {
                    _type = windowListBox.GetSelectString();
                    return;
                }
            }
            private void SetCorrespondent(string data)
            {
                if (data == null || data == "")
                {
                    _correspondent = "";
                    return;
                }
                char likeSpace = (char)160;
                char space = (char)32;
                data = data.Replace(likeSpace, space);
                if (Correspondent == data)
                {
                    return;
                }
                var correspondentList = FBaseFunc.Ins.CallCorrespondentNameList(SelectCompany, Account).FindAll(x => x.Contains(data));
                if (correspondentList == null || correspondentList.Count == 0)
                {
                    return;
                }

                if (correspondentList.Count == 1)
                {
                    _correspondent = correspondentList[0];
                    return;
                }

                WindowListBoxDialog windowListBox = new WindowListBoxDialog(correspondentList);
                if (windowListBox.ShowDialog() == true)
                {
                    _correspondent = windowListBox.GetSelectString();
                    return;
                }
            }
            private void SetPrice(long data)
            {
                if (_price == data)
                {
                    return;
                }
                _price = data;
            }
            private void SetDepartment(string data)
            {
                if (data == null || data == "")
                {
                    _department = "";
                    return;
                }

                if (_department == data)
                {
                    return;
                }

                var departmentList = FBaseFunc.Ins.CallDepartmentNameList(SelectCompany, SelectWorkplace).FindAll(x => x.Contains(data));
                if (departmentList == null || departmentList.Count == 0)
                {
                    return;
                }

                if (departmentList.Count == 1)
                {
                    _department = departmentList[0];
                    return;
                }

                WindowListBoxDialog windowListBox = new WindowListBoxDialog(departmentList);
                if (windowListBox.ShowDialog() == true)
                {
                    _department = windowListBox.GetSelectString();
                    return;
                }
            }
            private void SetSupplyPrice(long data)
            {
                if (_supplyPrice == data)
                {
                    return;
                }
                if (Account == null)
                {
                    _supplyPrice = 0;
                    return;
                }
                if (!Account.Contains("부가세") && !Account.Contains("13500") && !Account.Contains("25500"))
                {
                    _supplyPrice = 0;
                    return;
                }
                _supplyPrice = data;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}