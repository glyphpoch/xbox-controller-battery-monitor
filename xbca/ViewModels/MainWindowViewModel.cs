using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Windows.Devices.Power;
using Windows.Gaming.Input;
using xbca.HelperClasses;
using xbca.Models;
using static xbca.Models.Constants;

namespace xbca.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Settings m_config;

        // TODO: move all controller collection related log to another subview.
        private bool m_run;

        private ObservableCollection<Controller> m_controllers;

        public ObservableCollection<Controller> Controllers { get; set; }

        public DelegateCommand QuitCommand { get; set; }

        public event EventHandler NotificationEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsQuitPressed
        {
            get
            {
                return m_run == false;
            }
        }

        public bool IsStartupChecked {
            get
            {
                return m_config.RunOnStartup;
            }
            set
            {
                m_config.RunOnStartup = value;

                if(m_config.RunOnStartup == true)
                {
                    AddToStartup();
                }
                else
                {
                    RemoveFromStartup();
                }
            }
        }

        public bool IsStartMinimizedChecked
        {
            get
            {
                return m_config.StartMinimized;
            }
            set
            {
                m_config.StartMinimized = value;
            }
        }

        public bool IsCloseToTrayChecked
        {
            get
            {
                return m_config.CloseToTray;
            }
            set
            {
                m_config.CloseToTray = value;
            }
        }

        // TODO: set these settings to correct values.
        public bool IsNotifyLowChecked
        {
            get
            {
                return m_config.Level == 1;
            }
            set
            {
                m_config.Level = 1;
                RaisePropertyChanged("IsNotifyLowChecked");
                RaisePropertyChanged("IsNotifyMediumChecked");
                RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        public bool IsNotifyMediumChecked
        {
            get
            {
                return m_config.Level == 2;
            }
            set
            {
                m_config.Level = 2;
                RaisePropertyChanged("IsNotifyLowChecked");
                RaisePropertyChanged("IsNotifyMediumChecked");
                RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        public bool IsNotifyHighChecked
        {
            get
            {
                return m_config.Level == 3;
            }
            set
            {
                m_config.Level = 3;
                RaisePropertyChanged("IsNotifyLowChecked");
                RaisePropertyChanged("IsNotifyMediumChecked");
                RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        public bool IsNotifyOnceChecked
        {
            get
            {
                return m_config.NotifyEvery == 0;
            }
            set
            {
                m_config.NotifyEvery = 0;
                RaisePropertyChanged("IsNotifyOnceChecked");
                RaisePropertyChanged("IsNotifyEveryHourChecked");
                RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        public bool IsNotifyEveryHourChecked
        {
            get
            {
                return m_config.NotifyEvery == 60;
            }
            set
            {
                m_config.NotifyEvery = 60;
                RaisePropertyChanged("IsNotifyOnceChecked");
                RaisePropertyChanged("IsNotifyEveryHourChecked");
                RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        public bool IsNotifyEvery4HoursChecked
        {
            get
            {
                return m_config.NotifyEvery == 240;
            }
            set
            {
                m_config.NotifyEvery = 240;
                RaisePropertyChanged("IsNotifyOnceChecked");
                RaisePropertyChanged("IsNotifyEveryHourChecked");
                RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        public string StatusBarText { get; set; }

        public MainWindowViewModel()
        {
            bool areDefaultSettings = false;
            m_config = SettingsManager.LoadConfig(ref areDefaultSettings);

            QuitCommand = new DelegateCommand(QuitMenuItemDoAction);

            m_controllers = new ObservableCollection<Controller>();

            for(int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
            {
                m_controllers.Add(new Controller());
            }

            if(areDefaultSettings)
            {
                StatusBarText = "Running with default settings! Ignore this if running the application for the first time.";
            }
            else
            {
                StatusBarText = "Program running normally.";
            }

            StartPollingAsync();
        }

        private void StartPollingAsync()
        {
            Task task = Task.Run(async () =>
            {
                await PollingTaskAsync();
            });
        }

        /// <summary>
        /// Command for shutting down the application (even if the close to tray is on).
        /// Try to find a more elegant way than App.Current.Shutdown.
        /// </summary>
        /// <param name="parameter"></param>
        private void QuitMenuItemDoAction(object parameter)
        {
            m_run = false;
            StoreConfig();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Adds a key, with assembly location, to registry which makes it run on Windows startup.
        /// TODO: there are other ways to add the application to startup, review.
        /// </summary>
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

        /// <summary>
        /// Remove the registry key that makes the app run on windows startup.
        /// </summary>
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

        /// <summary>
        /// Main polling task - polls for new data every 20 seconds, checks if it's still supposed to be running every 100ms.
        /// TODO: The for loop was used because this was previously run in another thread, review.
        /// </summary>
        /// <returns></returns>
        private async Task PollingTaskAsync()
        {
            m_run = true;

            while (m_run)
            {
                await Task.Run(new Action(PollOnce));
                
                for (int s = 0; s < 50 && (m_run); ++s) // was 200
                {
                    await Task.Delay(100);
                }
            }

            StatusBarText = "Program exiting.";
        }

        /// <summary>
        /// The method that polls the BatteryReport report class for information about connected controllers.
        /// Virtual, because it's possible to use other API's to get battery info (XInput for example).
        /// </summary>
        protected virtual void PollOnce()
        {
            try
            {
                bool gamepadDetected = false;

                // Loop through all gamepads. Set the status of non connected ones to -1 so we won't display them.
                for(int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                {
                    if(i < RawGameController.RawGameControllers.Count)
                    {
                        gamepadDetected = true;
                        var gameController = RawGameController.RawGameControllers[i];

                        var batteryReport = gameController.TryGetBatteryReport();

                        if (batteryReport != null)
                        {
                            m_controllers[i].Id = i;
                            m_controllers[i].Name = gameController.DisplayName; //  .GetHashCode().ToString();

                            if (batteryReport.RemainingCapacityInMilliwattHours != null && batteryReport.FullChargeCapacityInMilliwattHours != null)
                            {
                                m_controllers[i].BatteryType = (byte)BatteryTypes.BATTERY_TYPE_UNKNOWN;

                                int chargeLevel = ((int)batteryReport.RemainingCapacityInMilliwattHours * 100) / (int)batteryReport.FullChargeCapacityInMilliwattHours;

                                m_controllers[i].BatteryValue = ChargeLevelToValue(chargeLevel);

                                byte noteLevel = (byte)chargeLevel;

                                if (noteLevel == 0)
                                {
                                    noteLevel = 1;
                                }
                                m_controllers[i].Note = noteLevel;

                                m_controllers[i].Status = batteryReport.Status.ToString();
                            }

                            // Check if design capacity is null and conclude that battery type is NiMH or there is no battery.
                            // In that case we can't tell the charge level.
                        }
                        else
                        {
                            m_controllers[i].Id = -1;
                        }
                    }
                }

                // Reset notified status and timers.
                for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                {
                    if (m_controllers[i].NotifyTimer.IsRunning)
                    {
                        // If notify every is not 0 - means that the user wants to be notified every X minutes.
                        // TODO: Why do we stop it here?
                        if (m_config.NotifyEvery != 0)
                        {
                            m_controllers[i].NotifyTimer.Stop();
                        }
                        else
                        {
                            if (m_controllers[i].NotifyTimer.Elapsed.Minutes > m_config.NotifyEvery)
                            {
                                m_controllers[i].Notified = false;
                            }
                        }
                    }
                }

                if (gamepadDetected == true)
                {
                    //
                    // Check for all possible controllers. XUSER_MAX_COUNT is 4. Defined in Xinput.h and copied to Constants.cs.
                    //
                    for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                    {
                        if (m_controllers[i].BatteryType != (byte)BatteryTypes.BATTERY_TYPE_DISCONNECTED)
                        {
                            //
                            // Notify the user only once, unless otherwise specified.
                            // Values will be 0,1,2,3 - More in Constants.
                            //
                            if (m_controllers[i].Notified == false && m_controllers[i].BatteryValue <= m_config.Level)
                            {
                                //
                                // Raise an event which reports the status of the battery to the main window.
                                // Alternative Enum name: Enum.GetName(typeof(BatteryTypes), type[i])
                                //                           
                                NotifyBatteryLow(string.Format("ControllerID: {0}\nBattery status: {1}", 
                                    m_controllers[i].Id, Constants.GetEnumDescription((BatteryLevel)m_controllers[i].BatteryValue)));

                                m_controllers[i].Notified = true;

                                if (m_config.NotifyEvery != 0)
                                {
                                    m_controllers[i].NotifyTimer.Start();
                                }

                                m_controllers[i].LastLevel = m_controllers[i].BatteryValue;
                            }

                            if (m_controllers[i].Notified == true && (m_controllers[i].BatteryValue > m_config.Level || m_controllers[i].BatteryValue < m_controllers[i].LastLevel))
                            {
                                m_controllers[i].Notified = false;

                                if (m_controllers[i].NotifyTimer.IsRunning)
                                {
                                    m_controllers[i].NotifyTimer.Stop();
                                }
                            }
                        }
                        else
                        {
                            m_controllers[i].Notified = false;
                            m_controllers[i].LastLevel = 255;

                            if (m_controllers[i].NotifyTimer.IsRunning)
                            {
                                m_controllers[i].NotifyTimer.Stop();
                            }
                        }
                    }
                }
                else
                {
                    //
                    // If no m_controllers are connected just reset the notified array. It's initialized to false by default.
                    //
                    for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                    {
                        m_controllers[i].Id = -1;
                        m_controllers[i].Notified = false;
                        m_controllers[i].LastLevel = 255;

                        if (m_controllers[i].NotifyTimer.IsRunning)
                        {
                            m_controllers[i].NotifyTimer.Stop();
                        }
                    }
                }

                Controllers = new ObservableCollection<Controller>(m_controllers);
                RaisePropertyChanged("Controllers");
            }
            catch
            {

            }
        }

        private void PollOnceTest()
        {
            try
            {
                bool gamepadDetected = false;
                for(int i = 0; i < Gamepad.Gamepads.Count && i < Controllers.Count; ++i)
                {
                    var gamepad = Gamepad.Gamepads[i];
                    
                    var batteryReport = gamepad.TryGetBatteryReport();

                    if(batteryReport != null)
                    {
                        m_controllers[i].Id = i;
                        gamepadDetected = true;
                        m_controllers[i].BatteryType = (byte)BatteryTypes.BATTERY_TYPE_UNKNOWN;

                        m_controllers[i].Name = gamepad.GetHashCode().ToString();

                        var currentReading = gamepad.GetCurrentReading();

                        if (batteryReport.ChargeRateInMilliwatts == null)
                        {
                            // There's likely no battery present, or maybe alkaline batteries used. - Need to test this.

                            // Set the battery to full or use a separate status for cable.
                            m_controllers[i].Status = Windows.System.Power.BatteryStatus.NotPresent.ToString();
                            m_controllers[i].BatteryValue = (byte)BatteryLevel.BATTERY_LEVEL_FULL;
                        }
                        else
                        {
                            // Battery is charging (ChargeRate is positive) or discharging (ChargeRate is negative).
                            if (batteryReport.RemainingCapacityInMilliwattHours != null && batteryReport.FullChargeCapacityInMilliwattHours != null)
                            {
                                m_controllers[i].Status = batteryReport.Status.ToString();
                                m_controllers[i].BatteryValue = CalculateChargeValue((int)batteryReport.RemainingCapacityInMilliwattHours,
                                                                                      (int)batteryReport.FullChargeCapacityInMilliwattHours);
                            }
                        }
                    }
                    else
                    {
                        // Set to unknown or don't show the controller?
                        m_controllers[i].Id = -1;
                    }

                    gamepadDetected = true;
                }
                
                var notifiableControllers = m_controllers.Where(w => w.Id >= 0);

                foreach(var controller in notifiableControllers)
                {

                }
            }
            catch
            {

            }
        }

        private byte CalculateChargeValue(int remainingCapacityInMWh, int fullChargeCapacityInMWh)
        {
            int chargeLevel = (remainingCapacityInMWh * 100) / fullChargeCapacityInMWh;

            return ChargeLevelToValue(chargeLevel);
        }

        public void StoreConfig()
        {
            SettingsManager.TrySaveConfig(m_config);
        }

        protected byte ChargeLevelToValue(int chargeLevel)
        {
            if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_FULL)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_FULL;
            }
            else if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_MEDIUM)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_MEDIUM;
            }
            else if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_LOW)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_LOW;
            }
            else
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_EMPTY;
            }
        }

        private void NotifyBatteryLow(string info)
        {
            NotificationEvent?.Invoke(info, EventArgs.Empty);
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
