using CrawExpenseReport.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Screen
{
    public class BrowserWindowViewModel : INotifyPropertyChanged
    {
        private string _url;
        public BrowserWindowViewModel()
        {
            _url = "";
//            _url = FBaseFunc.Ins.DefaultURL();
        }
        public string URL
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public void NavigateURL(string url)
        {
            URL = url;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
