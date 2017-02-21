using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tasker.Common.Configuration;

namespace Tasker.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string email = Context.Parameters["email"] ?? "",
                userName = Context.Parameters["smtpusername"] ?? "",
                password = Context.Parameters["smtppassword"] ?? "";

            Debugger.Launch();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tasker.Service.exe.config");
            AppConfigurationProvider config = new AppConfigurationProvider(path);
            config.UpdateSettings(new Dictionary<string, object>
            {
                {"SmtpUsername", userName},
                {"SmtpPassword", password},
                {"EmailFromAddress", email},
            });
        }
    }
}
