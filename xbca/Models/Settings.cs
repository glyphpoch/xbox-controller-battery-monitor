namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the settings class.
    /// http://stackoverflow.com/questions/453161/best-practice-to-save-application-settings-in-a-windows-forms-application.
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.StartMinimized = true;
            this.Level = 1;
            this.RunOnStartup = false;
            this.NotifyEvery = 0;
            this.CloseToTray = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application should minimize to system tray.
        /// </summary>
        public bool StartMinimized { get; set; }

        /// <summary>
        /// Gets a value indicating whether the notification should also make a beep sound.
        /// Beeping is disabled because notifications make a sound anyway. This should be Windows settings based.
        /// </summary>
        public bool Beep
        {
            get => false;
        }

        /// <summary>
        /// Gets or sets battery charge level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application should start on windows startup.
        /// </summary>
        public bool RunOnStartup { get; set; }

        /// <summary>
        /// Gets or sets the notification inteval in minutes.
        /// </summary>
        public int NotifyEvery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether closing the application minimizes it to system tray or exists it.
        /// </summary>
        public bool CloseToTray { get; set; }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>Concatenation of all settings.</returns>
        public override string ToString()
        {
            string settings = this.RunOnStartup.ToString() + " " + this.StartMinimized.ToString()
                + " " + this.Level.ToString() + " " + this.Beep.ToString() + " " + this.NotifyEvery.ToString()
                + " " + this.CloseToTray.ToString();

            return settings;
        }
    }
}
