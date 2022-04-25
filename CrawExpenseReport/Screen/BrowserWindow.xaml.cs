using CrawExpenseReport.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CrawExpenseReport.Screen
{
    /// <summary>
    /// BrowserWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BrowserWindow : Window
    {
        private readonly Queue<string> _msg;
        private readonly Queue<string> _closeRequest;
        private readonly Queue<string> _ret;
        private readonly DispatcherTimer _timer;
        public BrowserWindow()
        {
            InitializeComponent();
            DataContext = new BrowserWindowViewModel();
            _msg = new();
            _closeRequest = new();
            _ret = new();
            webView.ContentLoading += WebView_ContentLoading;
            _timer = new();
            _timer.Interval = TimeSpan.FromMilliseconds(1);
            _timer.Tick += Timer_Tick;
            _timer.IsEnabled = true;
        }
        private void WebView_ContentLoading(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs e)
        {
            FBaseFunc.Ins.ContentLoading(webView.Source.AbsolutePath);
        }
        public void ExecuteScript(string cmd)
        {
            _msg.Enqueue(cmd);
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (_msg.Count > 0)
            {
                string msg = _msg.Dequeue();
                string ret = await webView.CoreWebView2.ExecuteScriptAsync(msg);
//                if (ret == "null" && !msg.Contains("click"))
//                {
//                    ret = await webView.CoreWebView2.ExecuteScriptAsync(msg);
//                }
                SetResult(ret);
            }
            if (_closeRequest.Count > 0)
            {
                _timer.Stop();
                Close();
            }
        }
        public void NavigateURL(string url)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ((BrowserWindowViewModel)DataContext).URL = url;
            }));
        }
        public void CloseRequest()
        {
            _closeRequest.Enqueue("true");
        }

        public static void SetResult(string data)
        {
            FBaseFunc.Ins.SetResult(data);
        }
        public bool IsGetResult()
        {
            if (_ret.Count > 0)
            {
                return true;
            }

            return false;
        }
        public string GetResult()
        {
            if (IsGetResult())
            {
                return _ret.Dequeue();
            }
            else
            {
                return "";
            }
        }
        public void ClearResult()
        {
            _ret.Clear();
        }
    }
}
