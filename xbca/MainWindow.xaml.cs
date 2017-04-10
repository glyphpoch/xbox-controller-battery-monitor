using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace xbca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //
        // Global variables
        //

        //
        // Lists for updating the GUI Datagrid.
        // Items holds states for all possible controllers, UpdateMe stores states for connected controllers only.
        // We display the UpdateMe list.
        //
        private List<Controller> m_Items = new List<Controller>();
        private List<Controller> m_UpdateMe = new List<Controller>();

        //
        // Random number generator. Used for debugging only.
        //
        private Random m_Rnd = new Random();

        //
        // Windows Forms Class used for displaying notifications and minimizing the class to system tray.
        //
        private System.Windows.Forms.NotifyIcon m_notifyIcon;

        //
        // User settings. More in Settings.cs.
        //
        private SettingsManager m_SettingsMng = new SettingsManager();

        //
        // Store WindowState so we can restore it after the the application is restored from system tray.
        // Without this the window would still be minimized after restoring.
        //
        private WindowState m_storedWindowState = WindowState.Normal;

        //
        // Create xdata object that runs the thread for polling battery information.
        //
        private xdata Xd = new xdata();

        //
        // When application is started by Windows the working directory is system32 so we need to get the correct location of the assembly.
        //
        private static string BaseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //
        // Use this for closing the app if the Close to Tray option is enabled.
        //
        private bool m_QuitPressed = false;

        //!
        //! TODO.
        //!
        public MainWindow()
        {
            InitializeComponent();

            // Moved to Window_Loaded event.
            //Init();
        }

#region ////////- Event handlers -\\\\\\\\\
        private void controller_data_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as DataGrid;
            grid.ItemsSource = m_UpdateMe;
        }

        //
        // By clicking the notification bubble we restore the application back from system tray.
        //
        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        //
        // Minimize to system tray when applicaiton is minimized.
        //
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

        //
        // Stop the polling thread when the application is closing.
        //
        protected override void OnClosing(CancelEventArgs e)
        {
#if DEBUG
            Console.WriteLine("onClosing");
#endif
            if(m_SettingsMng.Config.CloseTray == true && m_QuitPressed == false)
            {
                e.Cancel = true;

                this.Hide();
            }
            else
            {
                if (Xd.State() == 1)
                {
                    Xd.Stop();
                }

                m_SettingsMng.SaveConfig();
            }

            base.OnClosing(e);
        }

        //
        // Dispose the NotifyIcon object when the application is closed.
        //
        protected override void OnClosed(EventArgs e)
        {
#if DEBUG
            Console.WriteLine("onClosed");
#endif

            m_notifyIcon.Dispose();
            m_notifyIcon = null;

            base.OnClosed(e);
        }

        //
        // Thread will fire an event, if notification for battery status has to be displayed, that sends data to this handle.
        // This method then displays the notification.
        //
        private void ReceiveNotification(int device, string type, string value)
        {
            m_notifyIcon.BalloonTipText = "ControllerID: " + device.ToString() + " Battery type: " + type + " Battery status: " + value;
            m_notifyIcon.ShowBalloonTip(3000);

            if(m_SettingsMng.Config.Beep)
            {
                SystemSounds.Beep.Play();
            }              

            Console.WriteLine("data recieved from thread");
        }

        //
        // Periodically recieve data about controllers from the polling thread.
        //
        private void ReceiveData(byte[] type, byte[] value, byte[] note)
        {
            //
            // Update controller information.
            //
            Console.WriteLine(type.Length.ToString() + " " + value.Length.ToString());
            for(int i = 0; i < type.Length && i < value.Length; ++i)
            {
                m_Items[i].Type = Constants.GetEnumDescription((BatteryTypes)type[i]);
                m_Items[i].Charge = Constants.GetEnumDescription((BatteryLevel)value[i]);

                if (note[i] == 0)
                {
                    //
                    // This will ensure the controller that this controller is not displayed in the datagrid.
                    //
                    m_Items[i].ID = -1;
                }
                else if(type[i] == (byte)BatteryTypes.BATTERY_TYPE_DISCONNECTED
                    && value[i] == (byte)BatteryLevel.BATTERY_LEVEL_EMPTY)
                {
                    m_Items[i].ID = i;
                    m_Items[i].Type = Constants.GetEnumDescription((BatteryTypes)BatteryTypes.BATTERY_TYPE_UNKNOWN);
                    m_Items[i].Charge = "Initializing/Unknown";
                }
                else
                {
                    m_Items[i].ID = i;
                }         
            }

            //
            // Update the GUI.
            //
            UpdateDataGrid();
        }

        //
        // TODO: Ignoring error at the moment, add more error/exception types later.
        // This is to replace the dispatcherTimer polling.
        //
        private void ReceiveError(int error)
        {
            OnUIThread(() => tb_status.Text = "Something went wrong. Please restart the application.");
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            bool check = ((MenuItem)sender).IsChecked;
            ((MenuItem)sender).IsChecked = check;

            if (sender == menu_startup)
            {
                m_SettingsMng.Config.WinStart = check;
            }
            else if (sender == menu_startMinimized)
            {
                m_SettingsMng.Config.StartMinimized = check;
            }
            else if (sender == menu_closeToTray)
            {
                m_SettingsMng.Config.CloseTray = check;
            }
            else if (sender == menu_beep)
            {
                m_SettingsMng.Config.Beep = check;
            }
            else if (sender == menu_low)
            {
                m_SettingsMng.Config.Level = 1;
                menu_medium.IsChecked = false;
                menu_high.IsChecked = false;
            }
            else if (sender == menu_medium)
            {
                m_SettingsMng.Config.Level = 2;
                menu_low.IsChecked = false;
                menu_high.IsChecked = false;
            }
            else if (sender == menu_high)
            {
                m_SettingsMng.Config.Level = 3;
                menu_low.IsChecked = false;
                menu_medium.IsChecked = false;
            }
            else if (sender == menu_once)
            {
                m_SettingsMng.Config.NotifyEvery = 0;
                menu_onehour.IsChecked = false;
                menu_4hours.IsChecked = false;
            }
            else if (sender == menu_onehour)
            {
                m_SettingsMng.Config.NotifyEvery = 60;
                menu_once.IsChecked = false;
                menu_4hours.IsChecked = false;
            }
            else if (sender == menu_4hours)
            {
                m_SettingsMng.Config.NotifyEvery = 240;
                menu_once.IsChecked = false;
                menu_onehour.IsChecked = false;
            }
        
            if(Xd.State() == 1)
            {
                Xd.UpdateSettings(m_SettingsMng.Config);
                OnUIThread(() => tb_status.Text = "Thread running");
            }
        }

        private void menu_quit_Click(object sender, RoutedEventArgs e)
        {
            m_QuitPressed = true;
            Application.Current.Shutdown();
        }
        #endregion

        #region ////////- Methods -\\\\\\\\
        private void Init()
        {
            try
            {
                //
                // We'll be using one NotifyIcon object for all notifications, apperance in system tray, etc.
                //
                m_notifyIcon = new System.Windows.Forms.NotifyIcon();
                m_notifyIcon.BalloonTipText = "Test Balloon tip text";
                m_notifyIcon.BalloonTipTitle = "XBCA";
                m_notifyIcon.Text = "XBCA";
                m_notifyIcon.Visible = true;

                //
                // Load the icon for our application.
                //
                Stream iconStream = Application.GetResourceStream(new Uri("chemistry.ico", UriKind.Relative)).Stream;
                m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);

                //
                // Bind an event to handle what happens when a user clicks the notification or the icon in system tray.
                //
                m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

                //
                // Bind events for recieving data back from out polling thread in xdata.cs.
                //
                xdata.NotificationEvent += ReceiveNotification;
                xdata.DataEvent += ReceiveData;
                xdata.ErrorEvent += ReceiveError;

                //
                // Load settings from settings.cfg or create a new settings file.
                //
                bool ISResult = InitSettings();

                //
                // Initialize DataGrid items.
                //
                for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                {
                    m_Items.Add(new Controller(-1, (i + 1).ToString(), "Unknown", "Disconnected"));
                }

                //
                // Start the polling thread.
                // Only run if we're on Windows 8 or higher. Windows 7 does not support getting the battery information.
                //
                Version win8version = new Version(6, 2, 9200, 0);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    Environment.OSVersion.Version >= win8version)
                {
                    // its win8 or higher.
                    Xd.Start(m_SettingsMng.Config);
                    if (ISResult == true)
                    {
                        OnUIThread(() => tb_status.Text = "Thread running");
                    }
                    else
                    {
                        OnUIThread(() => tb_status.Text = "Running with default settings! Ignore this if running the application for the first time.");
                    }
                }
                else
                {
                    OnUIThread(() => tb_status.Text = "Windows 7 is not supported.");
                }           

                //
                // Delete or remove registry entry for running on Windows startup based on user set options.
                //
                if (m_SettingsMng.Config.WinStart == true)
                {
                    AddToStartup();
                }
                else
                {
                    RemoveFromStartup();
                }

                if(m_SettingsMng.Config.StartMinimized == true)
                {
                    MinimizeToTray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //
        // Reads or creates the settings file.
        //
        private bool InitSettings(bool debug = false)
        {
            bool result = m_SettingsMng.LoadConfig();

            menu_startup.IsChecked = m_SettingsMng.Config.WinStart;
            menu_closeToTray.IsChecked = m_SettingsMng.Config.CloseTray;
            menu_startMinimized.IsChecked = m_SettingsMng.Config.StartMinimized;
            menu_beep.IsChecked = m_SettingsMng.Config.Beep;

            if (m_SettingsMng.Config.Level == 1)
            {
                menu_low.IsChecked = true;
            }
            else if(m_SettingsMng.Config.Level == 2)
            {
                menu_medium.IsChecked = true;
            }
            else if (m_SettingsMng.Config.Level == 3)
            {
                menu_high.IsChecked = true;
            }
            else
            {
                m_SettingsMng.Config.Level = 1;
                menu_low.IsChecked = true;
            }

            /////
            if (m_SettingsMng.Config.NotifyEvery == 0)
            {
                menu_once.IsChecked = true;
            }
            else if (m_SettingsMng.Config.NotifyEvery == 60)
            {
                menu_onehour.IsChecked = true;
            }
            else if (m_SettingsMng.Config.NotifyEvery == 240)
            {
                menu_4hours.IsChecked = true;
            }
            else
            {
                menu_once.IsChecked = true;
                m_SettingsMng.Config.NotifyEvery = 0;
            }

            return result;
        }

        //
        // Adds a key with assembly location to registry which makes it run on Windows startup.
        //
        private void AddToStartup()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
            }
            catch
            {
                MessageBox.Show("Failed to add application to Windows startup");
            }
        }

        //
        // Remove the assembly key that makes the app run on windows startup.
        //
        private void RemoveFromStartup()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.DeleteValue(curAssembly.GetName().Name, false);
            }
            catch
            {
                MessageBox.Show("Failed to remove application from Windows startup");
            }
        }

        //
        // Minimizes the program to system tray.
        //
        private void MinimizeToTray()
        {
            this.Hide();            
        }

        //
        // Restores the program from system tray.
        //
        private void RestoreFromTray()
        {
            this.Show();

            WindowState = m_storedWindowState;
        }

        //
        // Beep twice with a small delay between the two.
        //
        private void DoBeep()
        {
            SystemSounds.Beep.Play();
            Thread.Sleep(500);
            SystemSounds.Beep.Play();
        }

        //
        // Update Datagrid if a new controller has been connected.
        //
        private void UpdateDataGrid()
        {
            m_UpdateMe.Clear();
            for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
            {
                if (m_Items[i].ID != -1)
                {
                    m_UpdateMe.Add(m_Items[i]);
                }
            }

            Console.WriteLine("updating datagrid");
            Dispatcher.Invoke(() => datagrid_controller.ItemsSource = null);
            Dispatcher.Invoke(() => datagrid_controller.ItemsSource = m_UpdateMe);
        }

        private void OnUIThread(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                // if you don't want to block the current thread while action is
                // executed, you can also call Dispatcher.BeginInvoke(action);
                Dispatcher.Invoke(action);
            }
        }

        #endregion
    }
}
