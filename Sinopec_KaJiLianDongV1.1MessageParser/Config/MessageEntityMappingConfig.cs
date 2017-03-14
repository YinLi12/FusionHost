using System.Configuration;

namespace MessageParser.Config
{
    public class MessageEntityMappingConfig : ConfigurationSection
    {
        public static MessageEntityMappingConfig GetConfig()
        {
            return (MessageEntityMappingConfig)System.Configuration.ConfigurationManager.GetSection("messageEntityMappings");
        }

        [System.Configuration.ConfigurationProperty("Mappings")]
        public MappingCollection Mappings
        {
            get
            {
                object o = this["Mappings"];
                return o as MappingCollection;
            }
        }

    }
}
