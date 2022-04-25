using CrawExpenseReport.Base;
using CrawExpenseReport.Screen;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace CrawExpenseReport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow? _main;
        public static MainWindow WindowInstance
        {
            get
            {
                if (_main == null)
                {
                    _main = new MainWindow();
                }
                return _main;
            }
        }
        private static SettingWindow? _setting;
        public static SettingWindow SettingWindow
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new SettingWindow();
                }
                return _setting;
            }
        }
        private static BrowserWindow? _web;
        public static BrowserWindow Web
        {
            get
            {
                if (_web == null)
                {
                    _web = new BrowserWindow();
                }

                return _web;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (CheckRunning())
            {
                return;
            }

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            FBaseFunc.Ins.InitializeSystem();
            WindowInstance.Show();
            FBaseFunc.Ins.ViewAfterInitializeSystem();
        }
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            WindowInstance.IHaveToCloseThis(this);
        }

        public static void Close()
        {
        }

        private static bool CheckRunning()
        {
            string mutexName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            try
            {
                using Mutex? mutex = new(true, mutexName, out bool isCreateNew);
                if (isCreateNew)
                {
                    var process = System.Diagnostics.Process.GetProcesses().Select(x => x.ProcessName).ToList().FindAll(x => x == mutexName);
                    if (process.Count > 1)
                    {
                        MessageBox.Show("이미 실행중인 거 같은데요", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Shutdown();
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("이미 실행중인 거 같은데요", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
                return true;
            }
            return false;
        }
    }
}
