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
    /// WindowListBoxDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowListBoxDialog : Window
    {
        public WindowListBoxDialog(IEnumerable<string> data = null)
        {
            InitializeComponent();
            DataContext = new WindowListBoxDialogViewModel();
            if (data != null)
            {
                SetList(data);
            }
        }

        public void SetList(IEnumerable<string> data)
        {
            ((WindowListBoxDialogViewModel)DataContext).SetListOfString(data);
        }
        public string GetSelectString()
        {
            return ((WindowListBoxDialogViewModel)DataContext).SelectedListOfString;
        }
        public int GetSelectIndex()
        {
            return ((WindowListBoxDialogViewModel)DataContext).SelectedListOfIndex;
        }
    }
}
