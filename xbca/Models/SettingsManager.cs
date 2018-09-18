namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the settings manager class.
    /// </summary>
    public class SettingsManager
    {
        private static readonly string PathToSettings = null;

        /// <summary>
        /// Initializes static members of the <see cref="SettingsManager"/> class.
        /// Constructs the default path where the settings will be stored.
        /// </summary>
        static SettingsManager()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsRelative = "XBCA\\settings.xml";

            PathToSettings = Path.Combine(appdata, settingsRelative);
        }

        /// <summary>
        /// Load the config stored in users AppData folder or default settings.
        /// </summary>
        /// <param name="areDefaultSettings"><c>true</c>if default settings are loaded; otherwise <c>false</c>.</param>
        /// <returns>The config object - with stored settings or initialized to default.</returns>
        public static Settings LoadConfig(ref bool areDefaultSettings)
        {
            try
            {
                if (File.Exists(PathToSettings))
                {
                    using (StreamReader srReader = File.OpenText(PathToSettings))
                    {
                        var tType = typeof(Settings);
                        var xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);

                        Settings config = (Settings)xsSerializer.Deserialize(srReader);

                        areDefaultSettings = false;
                        return config;
                    }
                }
            }
            catch
            {
                areDefaultSettings = true;
            }

            return new Settings();
        }

        /// <summary>
        /// Tries to serialize the settings and saves them to user's AppData folder.
        /// </summary>
        /// <param name="config">Settings object.</param>
        /// <returns>True if the settings were successfully saved, False if the settings couldn't be saved.</returns>
        public static bool TrySaveConfig(Settings config)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathToSettings));

                using (StreamWriter swWriter = System.IO.File.CreateText(PathToSettings))
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
