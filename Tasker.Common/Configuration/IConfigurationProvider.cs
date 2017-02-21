using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using NLog;

namespace Tasker.Common.Configuration
{
    public interface IConfigurationProvider
    {
        NameValueCollection AppSettings { get; }

        NameValueCollection ConnectionStrings { get; }

        void UpdateSettings(IDictionary<string, object> values);
    }

    public abstract class ConfigurationProvider : IConfigurationProvider
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected System.Configuration.Configuration Configuration;

        protected ConfigurationProvider(System.Configuration.Configuration configuration)
        {
            Configuration = configuration;
        }
        
        public virtual NameValueCollection AppSettings
        {
            get
            {
                NameValueCollection collection = new NameValueCollection();
                foreach (var key in Configuration.AppSettings.Settings.AllKeys)
                {
                    collection.Add(key, Configuration.AppSettings.Settings[key].Value);
                }

                return collection;
            }
        }

        public NameValueCollection ConnectionStrings
        {
            get
            {
                NameValueCollection connectionStrings = new NameValueCollection();
                foreach (var cstring in Configuration.ConnectionStrings.ConnectionStrings.OfType<ConnectionStringSettings>())
                {
                    connectionStrings.Add(cstring.Name, cstring.ConnectionString);
                }

                return connectionStrings;
            }
        }

        public void UpdateSettings(IDictionary<string, object> values)
        {
            try
            {
                var settings = Configuration.AppSettings.Settings;

                foreach (var key in values.Keys)
                {
                    if (settings[key] == null)
                    {
                        settings.Add(key, values[key].ToString());
                    }
                    else
                    {
                        settings[key].Value = values[key].ToString();
                    }
                }

                Configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(Configuration.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Logger.Fatal("Error writing app settings");
                throw;
            }
        }
    }
}
