namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// NotificationTracker class for tracking notification status for a single controller.
    /// </summary>
    public class NotificationTracker
    {
        /// <summary>
        /// Gets or sets non roamable id.
        /// </summary>
        public string NonRoamableId { get; set; }

        /// <summary>
        /// Gets or sets current charge value.
        /// </summary>
        public byte CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user was notified about the charge level.
        /// </summary>
        public bool Notified { get; set; }

        /// <summary>
        /// Gets or sets notification timer.
        /// </summary>
        public Stopwatch NotifyTimer { get; set; }

        /// <summary>
        /// Checks if the notification timer is running and stops it.
        /// </summary>
        public void StopTimer()
        {
            if (this.NotifyTimer?.IsRunning == true)
            {
                this.NotifyTimer.Stop();
            }
        }

        /// <summary>
        /// Resets notification time to zero and start measuring again.
        /// </summary>
        public void RestartTimer()
        {
            this.NotifyTimer?.Restart();
        }

        /// <summary>
        /// Creates the timer if it doesn't exists, sets the time to zero and start measuring.
        /// </summary>
        public void StartTimer()
        {
            if (this.NotifyTimer == null)
            {
                this.NotifyTimer = new Stopwatch();
            }

            this.NotifyTimer.Restart();
        }

        /// <summary>
        /// Checks if the notification needs to be displayed again.
        /// </summary>
        /// <param name="notifyEvery">Renotifications interval time in minutes.</param>
        /// <returns><c>true</c>if the user needs to be renotified about the charge level; otherwise <c>false</c>.</returns>
        public bool NeedsRenotification(int notifyEvery)
        {
            if (notifyEvery != 0 && this.NotifyTimer != null && this.NotifyTimer.IsRunning)
            {
                return this.NotifyTimer.Elapsed.Minutes > notifyEvery;
            }

            return false;
        }

        /// <summary>
        /// Start measuring time if the timer is not running, otherwise does nothing.
        /// </summary>
        public void RunTimerIfNotRunning()
        {
            if (this.NotifyTimer.IsRunning == false)
            {
                this.StartTimer();
            }
        }

        /// <summary>
        /// Stops the timer associated with a controller if it has been disconnected for more than notifyEvery minutes.
        /// <param name="notifyEvery">Renotifications interval time in minutes.</param>
        /// </summary>
        public void StopTimerIfZombie(int notifyEvery)
        {
            if (this.NotifyTimer != null && this.NotifyTimer.IsRunning == true && this.NotifyTimer.Elapsed.Minutes > notifyEvery)
            {
                this.NotifyTimer.Stop();
                this.Notified = false;
            }
        }
    }
}
