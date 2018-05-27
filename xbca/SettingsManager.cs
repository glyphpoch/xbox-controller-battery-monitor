using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace xbca
{
    public class SettingsManager
    {
        private Settings m_Settings = new Settings();

        private string m_PathToSettings = null;

        public SettingsManager()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsRelative = "XBCA\\settings.xml";

            m_PathToSettings = System.IO.Path.Combine(appdata, settingsRelative);
        }

        public Settings Config
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        public bool LoadConfig()
        {
            try
            {
                if(File.Exists(m_PathToSettings))
                {
                    System.IO.StreamReader srReader = System.IO.File.OpenText(m_PathToSettings);
                    Type tType = m_Settings.GetType();
                    System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);

                    object oData = xsSerializer.Deserialize(srReader);
                    m_Settings = (Settings)oData;
                    srReader.Close();

                    // Beeping is disabled because notifications make a noise anyway. This should be Windows settings based.
                    m_Settings.Beep = false;
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_PathToSettings));

                System.IO.StreamWriter swWriter = System.IO.File.CreateText(m_PathToSettings);
                Type tType = m_Settings.GetType();
                if (tType.IsSerializable)
                {
                    System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                    xsSerializer.Serialize(swWriter, m_Settings);
                    swWriter.Close();

#if DEBUG
                    Console.WriteLine("Settings file written.");
#endif
                }

                Console.WriteLine(tType.IsSerializable.ToString() + " " + m_PathToSettings);

                return true;
            }
            catch(Exception ex)
            {
#if DEBUG
                Console.WriteLine("Couldn't save config: " + ex.ToString());
#endif
                return false;
            }
        }

        public override string ToString()
        {
            string settings = m_Settings.WinStart.ToString() + " " + m_Settings.StartMinimized.ToString()
                + " " + m_Settings.Level.ToString() + " " + m_Settings.Beep.ToString() + " " + m_Settings.NotifyEvery.ToString()
                + " " + m_Settings.CloseTray.ToString();

            return settings;
            //return base.ToString();
        }
    }
}
