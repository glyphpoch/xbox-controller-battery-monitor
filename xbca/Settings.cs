using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca
{
    [Serializable()]
    public class Settings
    {
        // http://stackoverflow.com/questions/453161/best-practice-to-save-application-settings-in-a-windows-forms-application

        public Settings()
        {
            StartMinimized = true;
            Beep = true;
            Level = 1;
            WinStart = false;
            NotifyEvery = 0;
            CloseTray = false;
        }
        //
        // Minimize to system tray if True.
        //
        public bool StartMinimized { get; set; }
        //
        // Beep with notification if True.
        //
        public bool Beep { get; set; }
        //
        // If lower or equal than Level then display notifications.
        //
        public int Level { get; set; }
        //
        // Create an registry entry for running the application on Windows startup.
        //
        public bool WinStart { get; set; }

        public int NotifyEvery { get; set; }

        public bool CloseTray { get; set; }
    }
}
