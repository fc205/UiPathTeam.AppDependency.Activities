using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace UiPathTeam.AppDependency.Activities
{
    //[Designer(typeof(AppDependencyCheckActivityDesigner))]
    [DisplayName("Check 3rd party application dependencies")]
    public class AppDependencyCheckActivity : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Path to the location of the appdependencies.json file.")]
        public InArgument<string> iPath { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var i_path = iPath.Get(context);

            JObject anAppDependency = JObject.Parse(System.IO.File.ReadAllText(i_path));

            AppDependencyContainer result = anAppDependency.ToObject<AppDependencyContainer>();

            foreach (AppDependency anApp in result.applicationDependencies)
            {
                // For each dependency

                // Check location exists
                if(!System.IO.File.Exists(anApp.Location))
                {
                    // If it doesn't, throw exception
                    throw new Exception("App location doesn't exist: "+ anApp.Name + " : " + anApp.Location);
                }

                string currentAppVersion = anApp.getCurrentAppVersion();
                bool versionValid = false;

                foreach(string validVersion in anApp.Versions)
                {
                    if(validVersion == currentAppVersion)
                    {
                        versionValid = true;
                    }
                }

                if(!versionValid)
                {
                    throw new Exception("App version is not valid: " + anApp.Name + " : " + currentAppVersion);
                }

                Console.WriteLine("App version is valid: " + anApp.Name + " : " + currentAppVersion);
            }
        }
    }

    public class AppDependency
    {
        public string Name;
        public string Location;
        public string[] Versions;

        public string getCurrentAppVersion()
        {
            string output = "";

            // Get App version
            var versionInfo = FileVersionInfo.GetVersionInfo(this.Location);
            output = versionInfo.ProductVersion; 

            return output;
        }
    }

    public class AppDependencyContainer
    {
        public AppDependency[] applicationDependencies;
    }
}
