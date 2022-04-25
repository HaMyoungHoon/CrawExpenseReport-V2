using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrawExpenseReport.Screen
{
    public class UCSlipViewModel : INotifyPropertyChanged
    {
        private string _gubun;
        private string _account;
        private string _type;
        private string _correspondent;
        private string _price;
        private string _briefs;
        private string _department;
        private string _supplyPrice;
        public UCSlipViewModel()
        {
            Gubun = "";
            Account = "";
            Type = "";
            Correspondent = "";
            Price = "";
            Briefs = "";
            Department = "";
            SupplyPrice = "";
        }

        public string Gubun
        {
            get => _gubun;
            set
            {
                _gubun = value;
                OnPropertyChanged();
            }
        }
        public string Account
        {
            get => _account;
            set
            {
                _account = value;
                OnPropertyChanged();
            }
        }
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        public string Correspondent
        {
            get => _correspondent;
            set
            {
                _correspondent = value;
                OnPropertyChanged();
            }
        }
        public string Price
        {
            get => _price;
            set
            {
                _price = value;
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
                _department = value;
                OnPropertyChanged();
            }
        }
        public string SupplyPrice
        {
            get => _supplyPrice;
            set
            {
                _supplyPrice = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}