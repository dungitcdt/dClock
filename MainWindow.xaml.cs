using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace dClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        private DispatcherTimer timer;
        private ContextMenuStrip contextMenu;
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;

            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("&Settings", null, null, "Setting"));
            contextMenu.Items.Add(new ToolStripMenuItem("&About", null, null, "About"));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("&Exit", null, null, "Exit"));

            contextMenu.ItemClicked += ContextMenu_ItemClicked;
        }

        private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ContextMenuStrip).Close(ToolStripDropDownCloseReason.ItemClicked);

            switch (e.ClickedItem.Name)
            {
                case "Setting":
                    var settingsWindow = new SettingWindow(this);
                    settingsWindow.Show();
                    break;

                case "About":
                {
                    System.Windows.MessageBox.Show("dClock create by Le Tien Dung", "About", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                break;

                case "Exit":
                    System.Windows.Application.Current.Shutdown();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textTime.Text = DateTime.Now.ToString("HH:mm");
        }

        private void notifyIcon_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            // Activate the form.
            Activate();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(
                helper, 
                GWL_EX_STYLE, 
                (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);

            // Or specify a specific name in the current dir
            var ini = new IniFile("Config.ini");

            // Font size
            if (ini.KeyExists("Size", "Font"))
            {
                var sizeString = ini.Read("Size", "Font");
                if (int.TryParse(sizeString, out var size))
                    textTime.FontSize = size;
            }

            // Font family
            if (ini.KeyExists("FontFamily", "Font"))
            {
                var fontFamily = ini.Read("FontFamily", "Font");
                if (!string.IsNullOrEmpty(fontFamily))
                    textTime.FontFamily = new FontFamily(fontFamily);
            }

            textTime.Visibility = Visibility.Visible;

            timer.Start();
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.clock,
                Text = "dClock 1.0.0.0",
                ContextMenuStrip = contextMenu,
                Visible = true
            };
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Or specify a specific name in the current dir
            var ini = new IniFile("Config.ini");

            // Font size
            ini.Write("Size", textTime.FontSize.ToString(), "Font");

            // Font family
            ini.Write("FontFamily", textTime.FontFamily.Source, "Font");

            timer?.Stop();

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            if (contextMenu != null)
            {
                contextMenu.Dispose();
                contextMenu = null;
            }
        }
    }
}
