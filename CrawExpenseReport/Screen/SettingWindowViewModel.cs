using CrawExpenseReport.Base;
using CrawExpenseReport.Data;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrawExpenseReport.Screen
{
    public class SettingWindowViewModel : INotifyPropertyChanged
    {
        private bool _isSettingEnable;
        private bool _isColorEnable;
        private bool _isCloseEnable;
        private Page _pageMake;
        private Page _pageLogin;
        private Page _pageColor;
        private DispatcherTimer _timer;
        public SettingWindowViewModel()
        {
            IsSettingEnable = true;
            IsColorEnable = true;
            IsCloseEnable = true;
            PageMake = new PageMake();
            PageLogin = new PageLoginSetting();
            PageColor = new PageColorSetting();
            CloseCommand = new CommandImpl(CloseEvent);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.IsEnabled = true;
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
        public bool IsColorEnable
        {
            get => _isColorEnable;
            set
            {
                _isColorEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsCloseEnable
        {
            get => _isCloseEnable;
            set
            {
                _isCloseEnable = value;
                OnPropertyChanged();
            }
        }
        public Page PageMake
        {
            get => _pageMake;
            set
            {
                _pageMake = value;
                OnPropertyChanged();
            }
        }
        public Page PageLogin
        {
            get => _pageLogin;
            set
            {
                _pageLogin = value;
                OnPropertyChanged();
            }
        }
        public Page PageColor
        {
            get => _pageColor;
            set
            {
                _pageColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; }

        public void CloseEvent(object obj)
        {
            Window window = obj as Window;
            if (window != null)
            {
                FBaseFunc.Ins.IsSettingOn = false;
                window.Hide();
            }
        }

        public void Reload()
        {
            ((PageMake)PageMake).Reload();
            ((PageLoginSetting)PageLogin).Reload();
        }
        public void SetMessageHook(Window mother)
        {
            WindowInteropHelper helper = new WindowInteropHelper(mother);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HookingFunc);
        }
        public IntPtr HookingFunc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x10)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }
        public void IHaveToCloseThis(object sender, SettingWindow mother)
        {
            var parent = FindParent<SettingWindow>(sender as Button);
            if (parent == null)
            {
                parent = mother;
            }
            HwndSource hwndSource = PresentationSource.FromVisual(parent as Visual) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.RemoveHook(new HwndSourceHook(HookingFunc));
            }
            Window.GetWindow(mother).Close();
        }
        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
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
            HwndSource hwndSource = PresentationSource.FromVisual(mother) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.RemoveHook(new HwndSourceHook(HookingFunc));
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            IsSettingEnable = FBaseFunc.Ins.CanCopy();
            IsColorEnable = FBaseFunc.Ins.CanCopy();
            IsCloseEnable = FBaseFunc.Ins.CanCopy();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}