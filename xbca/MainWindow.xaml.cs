using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using xbca.ViewModels;

namespace xbca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Windows Forms Class used for displaying notifications and minimizing the class to system tray.
        /// </summary>
        private System.Windows.Forms.NotifyIcon m_notifyIcon;

        /// <summary>
        /// Store WindowState so we can restore it after the the application is restored from system tray.
        /// Without this the window would still be minimized after restoring.
        /// </summary>
        private WindowState m_storedWindowState = WindowState.Normal;

        // TODO: find a cleaner solution.
        private MainWindowViewModel m_mainWindowViewModel;

        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            m_mainWindowViewModel = new MainWindowViewModel();
            m_mainWindowViewModel.NotificationEvent += new EventHandler(HandleNotificationEvent);

            DataContext = m_mainWindowViewModel;

            InitializeComponent();

            // We'll be using one NotifyIcon object for all notifications, appearance in system tray, etc.
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "Test Balloon tip text";
            m_notifyIcon.BalloonTipTitle = "XBCA";
            m_notifyIcon.Text = "XBCA";
            m_notifyIcon.Visible = true;

            // Load the icon for our application.
            Stream iconStream = Application.GetResourceStream(new Uri("chemistry.ico", UriKind.Relative)).Stream;
            m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            // Bind an event to handle what happens when a user clicks the notification or the icon in system tray.
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        /// <summary>
        /// Restores the application from system tray when the icon is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        /// <summary>
        /// Handles the notification event sent from the viewmodel. Sets the baloontip text then display the notification for 3 seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNotificationEvent(object sender, EventArgs e)
        {
            m_notifyIcon.BalloonTipText = sender as string;
            m_notifyIcon.ShowBalloonTip(3000);
        }

        /// <summary>
        /// Minimizes the program to system tray.
        /// </summary>
        private void MinimizeToTray()
        {
            this.Hide();
        }

        /// <summary>
        /// Restores the program from system tray.
        /// </summary>
        private void RestoreFromTray()
        {
            this.Show();

            WindowState = m_storedWindowState;
        }

        /// <summary>
        /// OnStateChanged override - minimizes the application to system tray.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                MinimizeToTray();
            }
            else
            {
                m_storedWindowState = WindowState;
            }

            base.OnStateChanged(e);
        }

        /// <summary>
        /// OnClosing override - minimizes the application to system tray when close is clicked if the option is enabled.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (m_mainWindowViewModel.IsCloseToTrayChecked == true && m_mainWindowViewModel.IsQuitPressed == false)
            {
                e.Cancel = true;

                this.Hide();
            }
            else
            {
                m_mainWindowViewModel.StoreConfig();
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// OnClosed override - disposes the notify icon.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;

            base.OnClosed(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_mainWindowViewModel.IsStartMinimizedChecked == true)
            {
                MinimizeToTray();
            }
        }
    }
}
