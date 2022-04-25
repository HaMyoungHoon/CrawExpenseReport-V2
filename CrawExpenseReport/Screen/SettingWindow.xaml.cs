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

namespace CrawExpenseReport.Screen
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            DataContext = new SettingWindowViewModel();
            this.IsVisibleChanged += SettingWindow_IsVisibleChanged;
        }

        private void SettingWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                Reload();
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((SettingWindowViewModel)DataContext).SetMessageHook(this);
        }

        public void Reload()
        {
            ((SettingWindowViewModel)DataContext).Reload();
        }
        public void IHaveToCloseThis(object sender)
        {
            ((SettingWindowViewModel)DataContext).IHaveToCloseThis(sender, this);
        }
        public void DestroyWPF()
        {
            ((SettingWindowViewModel)DataContext).DestroyWPF(this);
            this.Close();
        }
        public DialogHost DialogHost
        {
            get
            {
                return dlgHost;
            }
        }
    }
}
