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

namespace Gomoku_1312659_Ver4
{
    /// <summary>
    /// Interaction logic for StartOption.xaml
    /// </summary>
    public partial class StartOption : Window
    {
        int type = 0;

        public StartOption()
        {
            InitializeComponent();
        }

        private void rdoHvH_Checked(object sender, RoutedEventArgs e)
        {
            type = 1;
        }

        private void rdoHvC_Checked(object sender, RoutedEventArgs e)
        {
            type = 2;
        }

        private void rdoSvH_Checked(object sender, RoutedEventArgs e)
        {
            type = 3;
        }

        private void rdoSvC_Checked(object sender, RoutedEventArgs e)
        {
            type = 4;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (type != 0 && txtName.Text != "")
            {
                MainWindow mainWindow = new MainWindow(type, txtName.Text);
                mainWindow.Show();
            }
        }
    }
}
