using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca.Models
{
    // http://stackoverflow.com/questions/453161/best-practice-to-save-application-settings-in-a-windows-forms-application
    [Serializable()]
    public class Settings
    {
        /// <summary>
        /// Default construtor. Initializes the settings to their default values.
        /// </summary>
        public Settings()
        {
            StartMinimized = true;
            Level = 1;
            RunOnStartup = false;
            NotifyEvery = 0;
            CloseToTray = false;
        }

        /// <summary>
        /// Minimize to system tray if True.
        /// </summary>
        public bool StartMinimized { get; set; }

        /// <summary>
        /// Beep with notification if True.
        /// Beeping is disabled because notifications make a sound anyway. This should be Windows settings based.
        /// </summary>
        public bool Beep
        {
            get => false;
        }
        
        /// <summary>
        /// If lower or equal than Level then display notifications.
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// Create an registry entry for running the application on Windows startup.
        /// </summary>
        public bool RunOnStartup { get; set; }

        /// <summary>
        /// Defines the notifications interval in minutes.
        /// </summary>
        public int NotifyEvery { get; set; }

        /// <summary>
        /// Closes to tray if true, otherwise exits the app.
        /// </summary>
        public bool CloseToTray { get; set; }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>Concatenation of all settings.</returns>
        public override string ToString()
        {
            string settings = RunOnStartup.ToString() + " " + StartMinimized.ToString()
                + " " + Level.ToString() + " " + Beep.ToString() + " " + NotifyEvery.ToString()
                + " " + CloseToTray.ToString();

            return settings;
        }
    }
}
