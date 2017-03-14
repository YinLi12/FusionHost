using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Xml.Serialization;

namespace SinopecPumpSim
{
    public class Configurations
    {
        public ConfigurationPumpSetting[] PumpSettings { get; set; }
        //public List<Card> Cards { get; set; }
        private readonly string _configurationFile = "Configuration.xml";
        private Configuration _configuration;

        public void Load()
        {
            var config = new XmlDocument();            

            try
            {
                config.Load(_configurationFile);
                var serializer = new XmlSerializer(typeof(Configuration));

                using (var reader = new StreamReader(config.InnerText))
                {
                    _configuration = (Configuration)serializer.Deserialize(reader);
                    PumpSettings = _configuration.Items;
                }
                OnConfigurationLoaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                OnConfigurationError?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Update()
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var writer = new StreamWriter(_configurationFile))
            {
                serializer.Serialize(writer, _configuration);
            }
        }

        public EventHandler OnConfigurationLoaded;
        public EventHandler OnConfigurationError;
    }
}
