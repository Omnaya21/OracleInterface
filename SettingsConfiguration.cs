using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OracleInterface
{
    public class SettingsConfiguration
    {
        #region Default Data
        private const int DEF_BUFF_SIZE = 2048;
        private const string DEF_LOCAL_GATE_IP = "192.168.2.2";
        private const string DEF_TARGET_IP = "192.168.2.10";
        private static readonly int[] DEF_PORTS = new int[] {
            3500,
            4500,
        };
        #endregion 

        // name of the .xml file
        public static string CONFIG_FNAME = "config.xml";

        public static ConfigData GetConfigData()
        {
            if (!File.Exists(CONFIG_FNAME)) // create config file with default values
            {
                using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ConfigData));
                    ConfigData defaultConfigData = new ConfigData();
                    xs.Serialize(fs, defaultConfigData);
                    return defaultConfigData;
                }
            }
            else // read configuration from file
            {
                    using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.OpenOrCreate))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(ConfigData));
                        ConfigData savedConfigData = (ConfigData)xs.Deserialize(fs);
                        return savedConfigData;
                    }
            }
        }

        public static bool SaveConfigData(ConfigData config)
        {
            if (!File.Exists(CONFIG_FNAME)) return false; // don't do anything if file doesn't exist

            using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.Truncate))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ConfigData));
                xs.Serialize(fs, config);
                return true;
            }
        }

        // this class holds configuration data
        public class ConfigData
        {
            public string IcaServer;
            public string IcaDatabase;
            public string IcaUser;
            public string IcaPassword;
            public string OrionServer;
            public string OrionDatabase;
            public string OrionUser;
            public string OrionPassword;
            public string QuantoDatabase;
            public int OraclePort;
            public string DefaultDatabase;
            public string ProjectNumber;
            public int ImportOptions;
            public int ExportOptions;

            public ConfigData()
            {
                IcaServer = "10.44.180.54";
                IcaDatabase = "TEST";
                IcaUser = "DISCO_IV";
                IcaPassword = "TIMBERLINE";
                OrionServer = "SP3\\SQLEXPRESS";
                OrionDatabase = "Orion";
                OrionUser = "sa";
                OrionPassword = "F115";
                QuantoDatabase = "C:\\Program Files\\Magic Motion\\Quanto\\Data\\Quanto.Mdb";
                OraclePort = 1521;
                DefaultDatabase = "Orion";  // La otra opcion por el momento es "Quanto"
                ProjectNumber = "";
                ImportOptions = (int)(ImportOption.ImportDepartaments | ImportOption.ImportPositions | ImportOption.UpdateKardex);
                ExportOptions = 0;
            }
        }
    }

    [Flags]
    public enum ImportOption
    {
        ImportDepartaments = 0x01,
        ImportPositions = 0x02,
        UpdateKardex = 0x04,
        UpdateCurrentEmployees = 0x08,
        ImportTechnitians = 0x10
    }

    [Flags]
    public enum ExportOption
    {

    }

    /*
    public sealed class SettingsConfiguration : IConfigurationSectionHandler
    {
        public ConnectionSettingElement Read()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConnectionSettingElement));
            var sectionNode = (XmlNode)ConfigurationManager.GetSection("settings");
            
            ConnectionSettingElement connectionSetting;
            if (sectionNode != null)
            {
                XmlNode connectionSettingNode = sectionNode.SelectSingleNode("connectionsetting");
                var reader = new XmlNodeReader(connectionSettingNode);
                connectionSetting = (ConnectionSettingElement)serializer.Deserialize(reader);                
            }
            else
            {
                return new ConnectionSettingElement
                {
                    IcaServer = "ICA",
                    IcaDatabase = "ICA",
                    IcaUser = "none",
                    IcaPassword = "undefined",
                    OrionServer = "",
                    OrionDatabase = "Orion",
                    OrionPassword = "000",
                    OrionUser = "sa",
                    QuantoDatabase = ""

                };

            }
            return connectionSetting;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }

    #region Mapping Classes
    [XmlRoot("connectionsetting")]
    public class ConnectionSettingElement
    {
        [XmlElement("icaserver")]
        public string IcaServer { get; set; }

        [XmlElement("icadatabase")]
        public string IcaDatabase { get; set; }

        [XmlElement("icauser")]
        public string IcaUser { get; set; }

        [XmlElement("icapassword")]
        public string IcaPassword { get; set; }

        [XmlElement("orionserver")]
        public string OrionServer { get; set; }

        [XmlElement("oriondatabase")]
        public string OrionDatabase { get; set; }

        [XmlElement("orionuser")]
        public string OrionUser { get; set; }

        [XmlElement("orionpassword")]
        public string OrionPassword { get; set; }

        [XmlElement("quantodatabase")]
        public string QuantoDatabase { get; set; }        
    }

    [XmlRoot("customSetting")]
    public class CustomSettingElement
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("port")]
        public int Port { get; set; }

        [XmlElement("usessl")]
        public bool UseSsl { get; set; }

        [XmlElement("description")]
        public Description Description { get; set; }

        [XmlElement("from")]
        public string From { get; set; }

        [XmlElement("to")]
        public To To { get; set; }
    }

    [XmlRoot("description")]
    public class Description
    {
        [XmlAttribute("companyname")]
        public string CompanyName { get; set; }
    }

    [XmlRoot("to")]
    public class To
    {
        [XmlElement("email")]
        public List<Email> Email { get; set; }
    }

    [XmlRoot("email")]
    public class Email
    {
        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    #endregion
    */
}
