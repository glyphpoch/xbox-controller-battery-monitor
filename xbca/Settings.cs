using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca
{
    class Settings
    {
        public Settings()
        {
            Minimize = true;
            Beep = true;
            Level = 2;
            WinStart = true;
        }
        //
        // Minimize to system tray if True.
        //
        public bool Minimize { get; set; }
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

        //
        // Settings constants.
        //
        public const string MinimizeStr = "minimize";
        public const string BeepStr = "beep";
        public const string LevelStr = "level";
        public const string WinStartStr = "startup";
    }
}
