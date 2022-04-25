using CrawExpenseReport.Base;
using MaterialDesignThemes.Wpf;
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

namespace CrawExpenseReport.Screen.Popup
{
    /// <summary>
    /// WindowMessageBoxDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowMessageBoxDialog : Window
    {
        public WindowMessageBoxDialog(string titleMsg = "", string contentsMsg = "", bool cancelVisible = false)
        {
            InitializeComponent();
            DataContext = new WindowMessageBoxDialogViewModel();
            SetData(titleMsg, contentsMsg, cancelVisible);
            this.Resources = App.WindowInstance.Resources;
        }

        public void SetData(string titleMsg = "", string contentsMsg = "", bool cancelVisible = false)
        {
            SetTitleMsg(titleMsg);
            SetContentsMsg(contentsMsg);
            SetCancelVisible(cancelVisible);
        }

        public void SetTitleMsg(string titleMsg)
        {
            ((WindowMessageBoxDialogViewModel)DataContext).TitleMsg = titleMsg;
        }
        public void SetContentsMsg(string contentsMsg)
        {
            ((WindowMessageBoxDialogViewModel)DataContext).ContentsMsg = contentsMsg;
        }
        public void AppendContentsMsg(string contentsMsg)
        {
            ((WindowMessageBoxDialogViewModel)DataContext).ContentsMsg += contentsMsg;
        }
        public void SetCancelVisible(bool cancelVisible)
        {
            ((WindowMessageBoxDialogViewModel)DataContext).IsCancelVisible = cancelVisible;
        }
    }
}
