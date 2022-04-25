using CrawExpenseReport.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CrawExpenseReport.Screen.Popup
{
    public class WindowListBoxDialogViewModel : INotifyPropertyChanged
    {
        private bool _isConfirmVisible;
        private int _selectedListOfIndex;
        private string _selectedListOfString;
        private ObservableCollection<string> _listOfString;
        private Window _window;
        public WindowListBoxDialogViewModel()
        {
            SelectedListOfString = null;
            SelectedListOfIndex = -1;
            ListOfString = new ObservableCollection<string>();
            LoadedCommand = new CommandImpl(LoadedEvent);
            ConfirmCommand = new CommandImpl(ConfirmEvent);
            CancelCommand = new CommandImpl(CancelEvent);
        }
        public bool IsConfirmVisible
        {
            get => _isConfirmVisible;
            set
            {
                _isConfirmVisible = value;
                OnPropertyChanged();
            }
        }
        public int SelectedListOfIndex
        {
            get => _selectedListOfIndex;
            set
            {
                _selectedListOfIndex = value;
                OnPropertyChanged();
            }
        }
        public string SelectedListOfString
        {
            get => _selectedListOfString;
            set
            {
                _selectedListOfString = value;
                if (value != null)
                {
                    IsConfirmVisible = true;
                }
                else
                {
                    IsConfirmVisible = false;
                }
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> ListOfString
        {
            get => _listOfString;
            set
            {
                _listOfString = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public void SetListOfString(IEnumerable<string> data)
        {
            ListOfString.Clear();
            foreach (var title in data)
            {
                ListOfString.Add(title);
            }
            SelectedListOfString = null;
        }
        private void LoadedEvent(object obj)
        {
            _window = obj as Window;
            if (_window == null)
            {
                return;
            }
            _window.PreviewKeyDown += Window_PreviewKeyDown;
        }
        private void ConfirmEvent(object obj)
        {
            _window.DialogResult = true;
            _window.Close();
        }
        private void CancelEvent(object obj)
        {
            _window.DialogResult = false;
            _window.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (ListOfString.Count >= SelectedListOfIndex)
                {
                    SelectedListOfIndex++;
                }
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                if (SelectedListOfIndex > -1)
                {
                    SelectedListOfIndex--;
                }
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                if (SelectedListOfIndex != -1)
                {
                    _window.DialogResult = true;
                    _window.Close();
                }
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                _window.DialogResult = false;
                _window.Close();
                e.Handled = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}