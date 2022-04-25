using CrawExpenseReport.Base;
using CrawExpenseReport.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CrawExpenseReport.Screen.Popup
{
    public class WindowCopyBoxDialogViewModel : INotifyPropertyChanged
    {
        private bool _isConfirmVisible;
        private List<string> _typeList;
        private string _selectedTypeItem;
        private string _url;
        private ObservableCollection<string> _titleList;
        private string _resultText;
        public WindowCopyBoxDialogViewModel()
        {
            SelectedTypeItem = FBaseFunc.Ins.Cfg.CopySector.Type;
            Url = FBaseFunc.Ins.Cfg.CopySector.Url;
            TypeList = new List<string>();
            TypeList.Add("임시");
            TypeList.Add("완료");
            SaveType = new CommandImpl(SaveTypeEvent);
            SaveUrl = new CommandImpl(SaveUrlEvent);
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
        public List<string> TypeList
        {
            get => _typeList;
            set
            {
                _typeList = value;
                OnPropertyChanged();
            }
        }
        public string SelectedTypeItem
        {
            get => _selectedTypeItem;
            set
            {
                _selectedTypeItem = value;
                OnPropertyChanged();
            }
        }
        public string Url
        {
            get => _url;
            set
            {
                _url = value;

                if (value != null && value.Length > 0)
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
        public ObservableCollection<string> TitleList
        {
            get => _titleList;
            set
            {
                _titleList = value;
                OnPropertyChanged();
            }
        }
        public string ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveType { get; }
        public ICommand SaveUrl { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        private void SaveTypeEvent(object obj)
        {
            if (SelectedTypeItem.Contains("임시") || SelectedTypeItem.Contains("완료"))
            {
                FBaseFunc.Ins.Cfg.SetCopyType(SelectedTypeItem);
                ResultText = string.Format("사용할 문서함 : {0} 저장", SelectedTypeItem);
            }
            else
            {
                ResultText = "어 뭔가 이상한대요?";
            }
        }
        private void SaveUrlEvent(object obj)
        {
            if (Url.Length > 0)
            {
                FBaseFunc.Ins.Cfg.SetCopyUrl(Url);
                ResultText = string.Format("복사될 주소 : {0} 저장", Url);
            }
            else
            {
                ResultText = "주소가 공백입니다.";
            }
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