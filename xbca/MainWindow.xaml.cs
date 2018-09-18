namespace Xbca
{
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
    using Xbca.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Windows Forms Class used for displaying notifications and minimizing the class to system tray.
        /// </summary>
        private System.Windows.Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// Store WindowState so we can restore it after the the application is restored from system tray.
        /// Without this the window would still be minimized after restoring.
        /// </summary>
        private WindowState storedWindowState = WindowState.Normal;

        // TODO: find a cleaner solution.
        private MainWindowViewModel mainWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            this.mainWindowViewModel = new MainWindowViewModel();
            this.mainWindowViewModel.NotificationEvent += new EventHandler(this.HandleNotificationEvent);

            this.DataContext = this.mainWindowViewModel;

            this.InitializeComponent();

            // We'll be using one NotifyIcon object for all notifications, appearance in system tray, etc.
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "Test Balloon tip text";
            this.notifyIcon.BalloonTipTitle = "XBCA";
            this.notifyIcon.Text = "XBCA";
            this.notifyIcon.Visible = true;

            // Load the icon for our application.
            Stream iconStream = Application.GetResourceStream(new Uri("chemistry.ico", UriKind.Relative)).Stream;
            this.notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            // Bind an event to handle what happens when a user clicks the notification or the icon in system tray.
            this.notifyIcon.Click += new EventHandler(this.NotifyIcon_Click);
        }

        /// <summary>
        /// OnStateChanged override - minimizes the application to system tray.
        /// </summary>
        /// <param name="e">event arguments.</param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.MinimizeToTray();
            }
            else
            {
                this.storedWindowState = this.WindowState;
            }

            base.OnStateChanged(e);
        }

        /// <summary>
        /// OnClosing override - minimizes the application to system tray when close is clicked if the option for that is enabled.
        /// </summary>
        /// <param name="e">event arguments.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.mainWindowViewModel.IsCloseToTrayChecked == true && this.mainWindowViewModel.IsQuitPressed == false)
            {
                e.Cancel = true;

                this.Hide();
            }
            else
            {
                this.mainWindowViewModel.StoreConfig();
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// OnClosed override - disposes the notify icon.
        /// </summary>
        /// <param name="e">event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            this.notifyIcon.Dispose();
            this.notifyIcon = null;

            base.OnClosed(e);
        }

        /// <summary>
        /// Restores the application from system tray when the icon is clicked.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">event arguments.</param>
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.RestoreFromTray();
        }

        /// <summary>
        /// Handles the notification event sent from the viewmodel. Sets the baloontip text then display the notification for 3 seconds.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">event arguments.</param>
        private void HandleNotificationEvent(object sender, EventArgs e)
        {
            this.notifyIcon.BalloonTipText = sender as string;
            this.notifyIcon.ShowBalloonTip(3000);
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

            this.WindowState = this.storedWindowState;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.mainWindowViewModel.IsStartMinimizedChecked == true)
            {
                this.MinimizeToTray();
            }
        }
    }
}
