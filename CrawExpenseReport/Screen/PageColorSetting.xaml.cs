﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrawExpenseReport.Screen
{
    /// <summary>
    /// PageColorSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageColorSetting : Page
    {
        public PageColorSetting()
        {
            InitializeComponent();
            DataContext = new PageColorSettingViewModel();
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

    }
}