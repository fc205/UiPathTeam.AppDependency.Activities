using System;
using System.Text;
using System.IO;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace UiPathTeam.AppDependency.Activities
{
    //[Designer(typeof(AppDependencyCheckActivityDesigner))]
    [DisplayName("Check 3rd party application dependencies")]
    public class AppDependencyCheckActivity : CodeActivity
    {
        [RequiredArgument]
        [Category("Input")]
        [Description("Path to the location of the appdependencies.json file.")]
        public InArgument<string> InputPath { get; set; }

        [Category("Input")]
        [Description("Raises an exception when the application doesn't exist or when the version is wrong, otherwise displays a message in the console.")]
        public bool RaiseExceptions { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var i_path = InputPath.Get(context);

            string json = File.ReadAllText(i_path);

            if(json.Length == 0)
            {
                Console.WriteLine("Empty json. Exiting...");
                return;
            }

            AppDependencyContainer deserialisedAppDependency = new AppDependencyContainer();

            try
            {

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(deserialisedAppDependency.GetType());
                deserialisedAppDependency = ser.ReadObject(ms) as AppDependencyContainer;
                ms.Close();
            }
            catch(Exception e)
            {
                throw new Exception("Malformed Json input file", e);
            }

            foreach (AppDependency anApp in deserialisedAppDependency.applicationDependencies)
            {
                // For each dependency

                // Check location exists
                if(!System.IO.File.Exists(anApp.Location))
                {
                    // If it doesn't, throw exception
                    string message = "Application location doesn't exist: " + anApp.Name + " : " + anApp.Location;
                    Console.WriteLine(message);

                    if (RaiseExceptions)
                    {
                        throw new Exception(message);
                    }
                    else
                    {
                        continue;
                    }
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
                    string message = "Application version is NOT valid: " + anApp.Name + " : " + currentAppVersion;
                    Console.WriteLine(message);
                    if (RaiseExceptions)
                    {
                        throw new Exception(message);
                    }
                }

                Console.WriteLine("Application version is valid: " + anApp.Name + " : " + currentAppVersion);
            }
        }
    }

    [DataContract]
    internal class AppDependency
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string Location;
        [DataMember]
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

    [DataContract]
    internal class AppDependencyContainer
    {
        [DataMember]
        public AppDependency[] applicationDependencies;
    }
}
