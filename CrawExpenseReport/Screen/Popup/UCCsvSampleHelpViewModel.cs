using CrawExpenseReport.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CrawExpenseReport.Screen.Popup
{
    public class UCCsvSampleHelpViewModel : INotifyPropertyChanged
    {
        private bool _isNewExcel;
        private bool _isOldExcel;
        public UCCsvSampleHelpViewModel()
        {
            IsNewExcel = true;
            IsOldExcel = false;
            VersionChange = new CommandImpl(VersionChangeEvent);
        }

        public bool IsNewExcel
        {
            get => _isNewExcel;
            set
            {
                _isNewExcel = value;
                OnPropertyChanged();
            }
        }
        public bool IsOldExcel
        {
            get => _isOldExcel;
            set
            {
                _isOldExcel = value;
                OnPropertyChanged();
            }
        }

        public ICommand VersionChange { get; }

        private void VersionChangeEvent(object obj)
        {
            IsNewExcel = !IsNewExcel;
            IsOldExcel = !IsOldExcel;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}