using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace dClock
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private readonly MainWindow owner;

        public SettingWindow(MainWindow pOwner)
        {
            InitializeComponent();
            DataContext = this;

            owner = pOwner;

            FontSizes = new ObservableCollection<double>();

            for (var i = 8; i <= 200; i++)
                FontSizes.Add(i);
        }

        public ObservableCollection<double> FontSizes { get; }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (fontSelector.SelectedItem is FontFamily ff)
                owner.textTime.FontFamily = new FontFamily(ff.Source);

            if (fontSize.SelectedItem is double size)
                owner.textTime.FontSize = size;

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
