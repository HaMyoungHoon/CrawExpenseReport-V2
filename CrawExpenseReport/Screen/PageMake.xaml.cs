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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrawExpenseReport.Screen
{
    /// <summary>
    /// PageMake.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageMake : Page
    {
        public PageMake()
        {
            InitializeComponent();
            DataContext = new PageMakeViewModel();
        }

        public void Reload()
        {
            ((PageMakeViewModel)DataContext).Reload();
        }
        private void DataGridTextColumn_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ((PageMakeViewModel)DataContext).ClipboardPaste();
                e.Handled = true;
            }
        }
    }
}
