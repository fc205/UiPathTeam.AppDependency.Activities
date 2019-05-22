using System;
using System.Activities;
using System.Activities.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiPathTeam.AppDependency.Activities.Test
{
    [TestClass]
    public class Test_AppDependencyCheckActivity
    {
        public const string Test_DummyPath = "test.json";

        [TestMethod]
        public void CheckSimpleDependencies()
        {
            string aTestCheckDependencies = @"{
""applicationDependencies"": [
    { ""Name"": ""Adobe Acrobat Reader"", ""Location"": ""C:\\Program Files (x86)\\Adobe\\Acrobat Reader DC\\Reader\\AcroRd32.exe"", ""Versions"": [""19.10.20069.311970"", ""19.10.20069.49826"", ""19.12.20034.328841""]},
    { ""Name"": ""Internet Explorer"", ""Location"": ""C:\\Program Files (x86)\\Internet Explorer\\iexplore.exe"", ""Versions"": [""123456"",""11.00.17134.1""]},
    { ""Name"": ""MyApp"", ""Location"": ""C:\\somewhere"", ""Versions"": [""12.34.56""]},
  ]
}";
            System.IO.File.WriteAllText(Test_DummyPath, aTestCheckDependencies);

            var checkAppDependencyActivity = new AppDependencyCheckActivity
            {
                InputPath = Test_DummyPath,
                RaiseExceptions = false
            };

            var output = WorkflowInvoker.Invoke(checkAppDependencyActivity);
        }

    }
}
