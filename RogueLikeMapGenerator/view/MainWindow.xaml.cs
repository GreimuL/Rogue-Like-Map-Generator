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

namespace RogueLikeMapGenerator.view
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //로그 텍스트 스크롤 항상 밑으로 유지
            debugLogTextBox.TextChanged += (object sender, TextChangedEventArgs e) => { debugLogTextBox.ScrollToEnd(); };
        }
    }
}
