using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private List<Controller> m_Items = new List<Controller>();
        private Random m_Rnd = new Random();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private Settings m_Settings = new Settings();
        private WindowState m_storedWindowState = WindowState.Normal;
        xdata Xd = new xdata();

        public MainWindow()
        {
            InitializeComponent();

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            //m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "XBCA";
            m_notifyIcon.Text = "XBCA";
            m_notifyIcon.Visible = true;
            Stream iconStream = Application.GetResourceStream(new Uri("chemistry.ico", UriKind.Relative)).Stream;
            m_notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

            // Thread events
            xdata.TestEvent += ReceiveData;

            InitSettings();
        }

        #region ////////- Event handlers -\\\\\\\\\
        private void controller_data_Loaded(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
            {
                m_Items.Add(new Controller((i + 1).ToString(), "", ""));
            }

            var grid = sender as DataGrid;
            grid.ItemsSource = m_Items;
        }

        private void btn_debuggrid_Click(object sender, RoutedEventArgs e)
        {
            m_Items[m_Rnd.Next(0, XInputConstants.XUSER_MAX_COUNT)].Charge = m_Rnd.Next().ToString();

            datagrid_controller.ItemsSource = null;
            datagrid_controller.ItemsSource = m_Items;
        }

        private void btn_debugStart_Click(object sender, RoutedEventArgs e)
        {
            AddToStartup();
        }

        private void btn_debugtray_Click(object sender, RoutedEventArgs e)
        {
            //m_notifyIcon.BalloonTipText

            //this.Hide();
            Thread.Sleep(15000);

            //System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            //m_notifyIcon.BalloonTipTitle = "XBCA";
            m_notifyIcon.ShowBalloonTip(2000);
        }

        private void btn_debugrun_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("debug run");

            Xd.Start(m_Settings);
        }

        private void btn_debugbattery_Click(object sender, RoutedEventArgs e)
        {

        }

        //
        // By clicking the notification bubble we restore the application back from system tray.
        //
        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
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

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Xd.Stop();

#if DEBUG
            Console.WriteLine("onClosing");
#endif

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
#if DEBUG
            Console.WriteLine("onClosed");
#endif

            m_notifyIcon.Dispose();
            m_notifyIcon = null;

            base.OnClosed(e);
        }

        private void ReceiveData(bool display, string type, string value)
        {
            //MessageBox.Show(msg);
            //Dispatcher.BeginInvoke((Action)delegate ()
            //{
            //    tb.Text = msg;
            //});

            //System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();

            if(display == true)
            {
                m_notifyIcon.BalloonTipText = "Controller type: " + type + " Battery status: " + value;
                //m_notifyIcon.BalloonTipTitle = "XBCA";
                m_notifyIcon.ShowBalloonTip(2000);
            }


            Console.WriteLine("data recieved from thread");

        }
        #endregion

        #region ////////- Methods -\\\\\\\\
        //
        // Reads or creates the settings file.
        //
        private void InitSettings(bool debug = false)
        {
            if(File.Exists("settings.cfg") || debug)
            {
                string line = "";
                string[] split_line;
                System.IO.StreamReader file = new System.IO.StreamReader(@"settings.cfg");
                while ((line = file.ReadLine()) != null)
                {
                    line = Regex.Replace(line, @"\s+", "");
                    split_line = line.Split('=');
                    
                    if(split_line.Length >= 2)
                    {
                        if (split_line[0] == Settings.MinimizeStr)
                        {
                            if (split_line[1] == "true")
                            {
                                m_Settings.Minimize = true;
                            }
                        }
                        else if(split_line[0] == Settings.BeepStr)
                        {
                            if (split_line[1] == "true")
                            {
                                m_Settings.Beep = true;
                            }
                        }
                        else if(split_line[0] == Settings.LevelStr)
                        {
                            int level;
                            bool result = int.TryParse(split_line[1], out level);
                            if (result == true)
                            {
                                m_Settings.Level = level;
                            }
                        }
                    }
                }

                file.Close();
            }
            else
            {
                string[] lines = { Settings.MinimizeStr + "=" + m_Settings.Minimize.ToString(),
                                   Settings.BeepStr + "=" + m_Settings.Beep.ToString(),
                                   Settings.LevelStr + "=" + m_Settings.Level.ToString() };

                File.WriteAllLines(@"settings.cfg", lines);
            }
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

            WindowState = WindowState.Normal;
        }

        #endregion
    }
}
