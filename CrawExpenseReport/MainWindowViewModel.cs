using CrawExpenseReport.Base;
using CrawExpenseReport.Data;
using CrawExpenseReport.Screen.Popup;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrawExpenseReport
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isPasteEnable;
        private bool _isSettingEnable;
        private string _retText;
        private ObservableCollection<TitleListItem> _listOfCopyData;
        private int _selectedListOfCopyIndex;
        private TitleListItem _selectedListOfCopyData;
        private readonly DispatcherTimer _timer;

        public MainWindowViewModel()
        {
            _isPasteEnable = false;
            _isSettingEnable = false;
            _retText = "로그 창\n그룹웨어가 업데이트 되거나, 지출품의서 양식이 변경될 경우\n해당 프로그램이 제대로 동작하지 않을 수 있습니다.";
            _listOfCopyData = new();
            ListOfCopyData.Clear();
            IEnumerable<string> listTitle = FBaseFunc.Ins.CopyedTable.Select(x => x.Title);
            for (int i = 0; i < listTitle.Count(); i++)
            {
                ListOfCopyData.Add(new()
                {
                    Index = i,
                    Title = listTitle.ElementAt(i),
                });
            }
            _selectedListOfCopyData = new();
            _selectedListOfCopyIndex = -1;

            FBaseFunc.Ins.SetCallBack(BreakMethod, ResultMethod);
            FBaseFunc.Ins.SetCopyEndCallback(CopyEndCallback);
            FBaseFunc.Ins.SetPasteEndCallback(PasteEndCallback);

            PasteCommand = new CommandImpl(PasteEvent);
            SettingCommand = new CommandImpl(SettingEvent);
            CloseCommand = new CommandImpl(CloseEvent);

            _timer = new();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.IsEnabled = true;
        }

        public bool IsPasteEnable
        {
            get => _isPasteEnable;
            set
            {
                _isPasteEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsSettingEnable
        {
            get => _isSettingEnable;
            set
            {
                _isSettingEnable = value;
                OnPropertyChanged();
            }
        }
        public string RetText
        {
            get => _retText;
            set
            {
                _retText = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TitleListItem> ListOfCopyData
        {
            get => _listOfCopyData;
            set
            {
                _listOfCopyData = value;
                OnPropertyChanged();
            }
        }
        public int SelectedListOfCopyIndex
        {
            get => _selectedListOfCopyIndex;
            set
            {
                _selectedListOfCopyIndex = value;
                FBaseFunc.Ins.SetList(value);
                OnPropertyChanged();
            }
        }
        public TitleListItem SelectedListOfCopyData
        {
            get => _selectedListOfCopyData;
            set
            {
                if (value == null)
                {
                    return;
                }
                _selectedListOfCopyData = value;
                OnPropertyChanged();
            }
        }

        public ICommand PasteCommand { get; }
        public ICommand SettingCommand { get; }
        public ICommand CloseCommand { get; }

        public void BreakMethod()
        {
            RetText = string.Format("{0}\n중단됨. 재시작 하려면 Start를 눌러주세요.", RetText);
            IsPasteEnable = true;
        }
        public void ResultMethod(string data)
        {
            if (Regex.Matches(RetText, "\n").Count > 200)
            {
                RetText = "";
            }
            RetText = string.Format("{0}\n{1}", RetText, data);
        }
        public void CopyEndCallback(bool isThreadCall)
        {
            App.WindowInstance.Dispatcher.Invoke((Action)(() =>
            {
                ListOfCopyData.Clear();
                IEnumerable<string> listTitle = FBaseFunc.Ins.CopyedTable.Select(x => x.Title);
                for (int i = 0; i < listTitle.Count(); i++)
                {
                    ListOfCopyData.Add(new()
                    {
                        Index = i,
                        Title = listTitle.ElementAt(i),
                    });
                }

                SelectedListOfCopyData = new();
            }));
            FBaseFunc.Ins.SetList(SelectedListOfCopyIndex);
        }
        public static void PasteEndCallback(bool isThreadCall)
        {

        }

        private void PasteEvent(object? obj)
        {
//            FBaseFunc.Ins.TestXPath();
            FBaseFunc.Ins.Paste();
        }
        private void SettingEvent(object? obj)
        {
            FBaseFunc.Ins.Setting();
        }
        public void CloseEvent(object? obj)
        {
            WindowMessageBoxDialog messageBoxDialog = new("경고경고", "종료하면 모든 작업이 중지됩니다.\n정말 종료하시겠습니까?", true);
            if (messageBoxDialog.ShowDialog() == true)
            {
                FBaseFunc.Ins.TerminateSystem();

                _timer.Stop();

                if (obj is Window window)
                {
                    App.WindowInstance.DestroyWPF();
                    window.Close();
                    App.SettingWindow.DestroyWPF();
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            IsSettingEnable = FBaseFunc.Ins.CanSetting();
            IsPasteEnable = FBaseFunc.Ins.CanPaste();
        }
        public void SetMessageHook(Window mother)
        {
            WindowInteropHelper helper = new(mother);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HookingFunc);
        }
        public static IntPtr HookingFunc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x10)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }
        public void IHaveToCloseThis(object sender, MainWindow mother)
        {
            MainWindow? parent = FindParent<MainWindow>((Button)sender);
            if (parent == null)
            {
                parent = mother;
            }
            if (PresentationSource.FromVisual(parent as Visual) is HwndSource hwndSource)
            {
                hwndSource.RemoveHook(new(HookingFunc));
            }
            Window.GetWindow(mother).Close();
        }
        private static T? FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null)
            {
                return null;
            }

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }
        public void DestroyWPF(Visual mother)
        {
            if (PresentationSource.FromVisual(mother) is HwndSource hwndSource)
            {
                hwndSource.RemoveHook(new(HookingFunc));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }

    public class TitleListItem : INotifyPropertyChanged
    {
        private int _index;
        private string _title;
        public TitleListItem()
        {
            _title = "";
            _index = -1;
        }
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}