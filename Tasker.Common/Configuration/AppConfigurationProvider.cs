using System.Configuration;

namespace Tasker.Common.Configuration
{
    public class AppConfigurationProvider : ConfigurationProvider
    {
        public AppConfigurationProvider() : base(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None))
        {

        }

        public AppConfigurationProvider(System.Configuration.Configuration configuration) : base(configuration)
        {

        }

        public AppConfigurationProvider(string path) : base(ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = path }, ConfigurationUserLevel.None))
        {

        }
    }
}
