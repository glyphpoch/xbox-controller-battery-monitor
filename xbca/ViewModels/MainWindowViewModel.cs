namespace Xbca.ViewModels
{
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
    using Xbca.HelperClasses;
    using Xbca.Models;
    using static Xbca.Models.Constants;

    /// <summary>
    /// Represents the main window view model class.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Settings config;

        // TODO: move all controller collection related log to another subview.
        private bool runPolling;

        private ObservableCollection<Controller> controllers;

        /// <summary>
        /// Dictionary which links non roamable id's to time tracking objects.
        /// </summary>
        private Dictionary<string, NotificationTracker> notifyTracking;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            bool areDefaultSettings = false;
            this.config = SettingsManager.LoadConfig(ref areDefaultSettings);

            this.QuitCommand = new DelegateCommand(this.QuitMenuItemDoAction);

            // TODO: this will be removed and replaced with something more dynamic.
            this.controllers = new ObservableCollection<Controller>();

            for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
            {
                this.controllers.Add(new Controller());
            }

            if (areDefaultSettings)
            {
                this.StatusBarText = "Running with default settings! Ignore this if running the application for the first time.";
            }
            else
            {
                this.StatusBarText = "Program running normally.";
            }

            this.StartPollingAsync();
        }

        /// <summary>
        /// Occurs when user needs to be notified that the battery charge level is low.
        /// </summary>
        public event EventHandler NotificationEvent;

        /// <summary>
        /// Occurs when a propery has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a collection of controllers.
        /// </summary>
        public ObservableCollection<Controller> Controllers { get; set; }

        /// <summary>
        /// Gets or sets a Quit command.
        /// </summary>
        public DelegateCommand QuitCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether the quit button was pressed.
        /// </summary>
        public bool IsQuitPressed
        {
            get
            {
                return this.runPolling == false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the run on startup options is checked.
        /// </summary>
        public bool IsStartupChecked
        {
            get
            {
                return this.config.RunOnStartup;
            }

            set
            {
                this.config.RunOnStartup = value;

                if (this.config.RunOnStartup == true)
                {
                    this.AddToStartup();
                }
                else
                {
                    this.RemoveFromStartup();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the start minimized option is checked.
        /// </summary>
        public bool IsStartMinimizedChecked
        {
            get
            {
                return this.config.StartMinimized;
            }

            set
            {
                this.config.StartMinimized = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the close to tray option is checked.
        /// </summary>
        public bool IsCloseToTrayChecked
        {
            get
            {
                return this.config.CloseToTray;
            }

            set
            {
                this.config.CloseToTray = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the notify when low option is checked.
        /// </summary>
        public bool IsNotifyLowChecked
        {
            get
            {
                return this.config.Level == 1;
            }

            set
            {
                this.config.Level = 1;
                this.RaisePropertyChanged("IsNotifyLowChecked");
                this.RaisePropertyChanged("IsNotifyMediumChecked");
                this.RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the notify when medium option is checked.
        /// </summary>
        public bool IsNotifyMediumChecked
        {
            get
            {
                return this.config.Level == 2;
            }

            set
            {
                this.config.Level = 2;
                this.RaisePropertyChanged("IsNotifyLowChecked");
                this.RaisePropertyChanged("IsNotifyMediumChecked");
                this.RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the notify when high option is checked.
        /// </summary>
        public bool IsNotifyHighChecked
        {
            get
            {
                return this.config.Level == 3;
            }

            set
            {
                this.config.Level = 3;
                this.RaisePropertyChanged("IsNotifyLowChecked");
                this.RaisePropertyChanged("IsNotifyMediumChecked");
                this.RaisePropertyChanged("IsNotifyHighChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether notify once option is checked.
        /// </summary>
        public bool IsNotifyOnceChecked
        {
            get
            {
                return this.config.NotifyEvery == 0;
            }

            set
            {
                this.config.NotifyEvery = 0;
                this.RaisePropertyChanged("IsNotifyOnceChecked");
                this.RaisePropertyChanged("IsNotifyEveryHourChecked");
                this.RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether notify every hour option is checked.
        /// </summary>
        public bool IsNotifyEveryHourChecked
        {
            get
            {
                return this.config.NotifyEvery == 60;
            }

            set
            {
                this.config.NotifyEvery = 60;
                this.RaisePropertyChanged("IsNotifyOnceChecked");
                this.RaisePropertyChanged("IsNotifyEveryHourChecked");
                this.RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether notify every 4 hours option is checked.
        /// </summary>
        public bool IsNotifyEvery4HoursChecked
        {
            get
            {
                return this.config.NotifyEvery == 240;
            }

            set
            {
                this.config.NotifyEvery = 240;
                this.RaisePropertyChanged("IsNotifyOnceChecked");
                this.RaisePropertyChanged("IsNotifyEveryHourChecked");
                this.RaisePropertyChanged("IsNotifyEvery4HoursChecked");
            }
        }

        /// <summary>
        /// Gets or sets the status bar text.
        /// </summary>
        public string StatusBarText { get; set; }

        /// <summary>
        /// Stores the setting to a file.
        /// </summary>
        public void StoreConfig()
        {
            SettingsManager.TrySaveConfig(this.config);
        }

        /// <summary>
        /// Polls the Windows 10 API for controller and battery data.
        /// </summary>
        protected virtual void PollOnceTest()
        {
            try
            {
                this.UpdateControllerData();

                this.UpdateNotificationTrackers();

                this.HandleNotifications();

                this.Controllers = new ObservableCollection<Controller>(this.controllers);
                this.RaisePropertyChanged("Controllers");
            }
            catch (Exception)
            {
                this.StatusBarText = "An exception occured while polling for controller data.";
            }
        }

        private void StartPollingAsync()
        {
            this.notifyTracking = new Dictionary<string, NotificationTracker>();

            Task task = Task.Run(async () =>
            {
                await this.PollingTaskAsync();
            });
        }

        /// <summary>
        /// Command for shutting down the application (even if the close to tray is on).
        /// Try to find a more elegant way than App.Current.Shutdown.
        /// </summary>
        /// <param name="parameter">menu item comamnd parameter.</param>
        private void QuitMenuItemDoAction(object parameter)
        {
            this.runPolling = false;
            this.StoreConfig();
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
        /// <returns>a Task.</returns>
        private async Task PollingTaskAsync()
        {
            this.runPolling = true;

            while (this.runPolling)
            {
                await Task.Run(new Action(this.PollOnceTest));

                // was 200
                for (int s = 0; s < 200 && this.runPolling; ++s)
                {
                    await Task.Delay(100);
                }
            }

            this.StatusBarText = "Program exiting.";
        }

        /// <summary>
        /// Checks which notifications need to be displayed and raise a notify event.
        /// </summary>
        private void HandleNotifications()
        {
            var trackersNeedingNotification = this.notifyTracking.Where(w =>
                    (w.Value.Notified == false || w.Value.NeedsRenotification(this.config.NotifyEvery))
                    && w.Value.CurrentValue <= this.config.Level).Select(w => w.Value);

            foreach (var tracker in trackersNeedingNotification)
            {
                // Display notifications.
                var controllerData = this.controllers.Where(w => w.UniqueId == tracker.NonRoamableId).FirstOrDefault();

                if (controllerData != null)
                {
                    this.NotifyBatteryLow(controllerData.GetNotificationString());
                    tracker.Notified = true;

                    if (this.config.NotifyEvery != 0)
                    {
                        tracker.RestartTimer();
                    }
                }
                else
                {
                    // The controller is not currently connected.
                    // No need to reset anything now, once it gets reconnected again and the battery level is still below the notification level it will either be notified again after a certain time.
                    // Might be a good idea to reset the Notified status here so the user gets notified again in case the controller gets disconnected in the meantime (to preserve original functionality).

                    // TODO: maybe stop timers after a certain time of the controller being disconnected.
                    tracker.Notified = false;
                    tracker.StopTimer();
                    //tracker.StopTimerIfZombie(this.config.NotifyEvery);
                }
            }
        }

        /// <summary>
        /// Adds new notification tracker if it doesn't exist for a unique id and start/stops it's timer - depending on whether the notify every option is set.
        /// </summary>
        private void UpdateNotificationTrackers()
        {
            var connectedControllers = this.controllers.Where(w => w.Id >= 0);

            foreach (var controller in connectedControllers)
            {
                if (!this.notifyTracking.ContainsKey(controller.UniqueId))
                {
                    this.notifyTracking.Add(controller.UniqueId, new NotificationTracker()
                    {
                        NonRoamableId = controller.UniqueId,
                        CurrentValue = controller.BatteryValue,
                        Notified = false,
                    });
                }

                this.notifyTracking[controller.UniqueId].CurrentValue = controller.BatteryValue;

                // Starts the renotification timer if the charge level is below threshold or stops it if it is above the threshold.
                if (this.config.NotifyEvery != 0)
                {
                    if (controller.BatteryValue <= this.config.Level)
                    {
                        this.notifyTracking[controller.UniqueId].RunTimerIfNotRunning();
                    }
                    else
                    {
                        this.notifyTracking[controller.UniqueId].StopTimer();
                        this.notifyTracking[controller.UniqueId].Notified = false;
                    }
                }
                else
                {
                    // Stop any running timers - since they shouldn't be running if NotifyEvery == 0.
                    this.notifyTracking[controller.UniqueId].StopTimer();
                }
            }
        }

        /// <summary>
        /// Updates controller data from RawGameController.
        /// </summary>
        private void UpdateControllerData()
        {
            for (int i = 0; i < RawGameController.RawGameControllers.Count && i < this.controllers.Count; ++i)
            {
                var gamepad = RawGameController.RawGameControllers[i];

                var batteryReport = gamepad.TryGetBatteryReport();

                if (batteryReport != null)
                {
                    this.controllers[i].Id = i;
                    this.controllers[i].BatteryType = (byte)BatteryTypes.BATTERY_TYPE_UNKNOWN;

                    this.controllers[i].Name = gamepad.DisplayName;
                    this.controllers[i].UniqueId = gamepad.NonRoamableId;

                    this.controllers[i].BatteryReportData = string.Format(
                        "RemainingCapacity: {0}, FullCapacity: {1}{2}DesignCapacity {3}, ChargeRateMw: {4}",
                        batteryReport.RemainingCapacityInMilliwattHours ?? -1,
                        batteryReport.FullChargeCapacityInMilliwattHours ?? -1,
                        Environment.NewLine,
                        batteryReport.DesignCapacityInMilliwattHours ?? -1,
                        batteryReport.ChargeRateInMilliwatts ?? -1);

                    this.controllers[i].Status = batteryReport.Status.ToString();

                    if (batteryReport.RemainingCapacityInMilliwattHours != null && batteryReport.FullChargeCapacityInMilliwattHours != null)
                    {
                        this.controllers[i].BatteryValue = this.CalculateChargeValue(
                            (int)batteryReport.RemainingCapacityInMilliwattHours,
                            (int)batteryReport.FullChargeCapacityInMilliwattHours);
                    }

                    if (batteryReport.ChargeRateInMilliwatts == null)
                    {
                        this.controllers[i].BatteryType = (byte)BatteryTypes.BATTERY_TYPE_NOBATTERY;

                        // TODO: Additional logic when for determining battery type when more data is available.
                    }
                }
                else
                {
                    // Set to unknown or don't show the controller?
                    this.controllers[i].Id = -1;
                }
            }
        }

        private byte CalculateChargeValue(int remainingCapacityInMWh, int fullChargeCapacityInMWh)
        {
            int chargeLevel = (remainingCapacityInMWh * 100) / fullChargeCapacityInMWh;

            return this.ChargeLevelToValue(chargeLevel);
        }

        /// <summary>
        /// Converts charge level in percentage to value.
        /// </summary>
        /// <param name="chargeLevel">Charge level in percentage.</param>
        /// <returns>The charge level value.</returns>
        private byte ChargeLevelToValue(int chargeLevel)
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
            this.NotificationEvent?.Invoke(info, EventArgs.Empty);
        }

        private void RaisePropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
