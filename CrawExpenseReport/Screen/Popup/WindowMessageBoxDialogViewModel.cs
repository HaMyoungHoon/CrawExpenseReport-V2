using CrawExpenseReport.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CrawExpenseReport.Screen.Popup
{
    public class WindowMessageBoxDialogViewModel : INotifyPropertyChanged
    {
        private bool _isCancelVisible;
        private string _titleMsg;
        private string _contentsMsg;
        public WindowMessageBoxDialogViewModel()
        {
            IsCancelVisible = false;
            TitleMsg = "";
            ContentsMsg = "";
            LoadedCommand = new CommandImpl(LoadedEvent);
            ConfirmCommand = new CommandImpl(ConfirmEvent);
            CancelCommand = new CommandImpl(CancelEvent);
        }
        public bool IsCancelVisible
        {
            get => _isCancelVisible;
            set
            {
                _isCancelVisible = value;
                OnPropertyChanged();
            }
        }
        public string TitleMsg
        {
            get => _titleMsg;
            set
            {
                _titleMsg = value;
                OnPropertyChanged();
            }
        }
        public string ContentsMsg
        {
            get => _contentsMsg;
            set
            {
                _contentsMsg = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        private void LoadedEvent(object obj)
        { 
        }
        private void ConfirmEvent(object obj)
        {
            var window = obj as Window;
            window.DialogResult = true;
            window.Close();
        }
        private void CancelEvent(object obj)
        {
            var window = obj as Window;
            window.DialogResult = false;
            window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}