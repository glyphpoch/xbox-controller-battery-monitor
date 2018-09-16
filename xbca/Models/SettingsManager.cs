using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace xbca.Models
{
    public class SettingsManager
    {
        private static string m_PathToSettings = null;

        /// <summary>
        /// Static constructor. Constructs the default path where the settings will be stored.
        /// </summary>
        static SettingsManager()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsRelative = "XBCA\\settings.xml";

            m_PathToSettings = Path.Combine(appdata, settingsRelative);
        }

        /// <summary>
        /// Load the config stored in users AppData folder or default settings.
        /// </summary>
        /// <returns>The config object - with stored settings or initialized to default.</returns>
        public static Settings LoadConfig(ref bool areDefaultSettings)
        {
            try
            {
                if(File.Exists(m_PathToSettings))
                {
                    using (StreamReader srReader = File.OpenText(m_PathToSettings))
                    {
                        var tType = typeof(Settings);
                        var xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);

                        Settings config = (Settings)xsSerializer.Deserialize(srReader);

                        // Config.Beep = false;

                        areDefaultSettings = false;
                        return config;
                    }
                }
            }
            catch
            {
                
            }

            areDefaultSettings = true;
            return new Settings();
        }

        /// <summary>
        /// Tries to serialize the settings and saves them to user's AppData folder.
        /// </summary>
        /// <returns>True if the settings were successfully saved, False if the settings couldn't be saved.</returns>
        public static bool TrySaveConfig(Settings config)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_PathToSettings));

                using (StreamWriter swWriter = System.IO.File.CreateText(m_PathToSettings))
                {
                    var tType = typeof(Settings);
                    if (tType.IsSerializable)
                    {
                        var xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                        xsSerializer.Serialize(swWriter, config);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
