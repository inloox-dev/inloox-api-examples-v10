using Default;
using InLoox.ODataClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InLooxApiTests
{
    [TestClass]
    public class TestBase
    {
        private Uri EndPoint { get; set; }

        public Container Context { get; private set; }
        private IConfigurationRoot Configuration { get; set; }

        public TestContext TestContext { get; set; }
        public TestSettings Settings { get; set; }

        [TestInitialize]
        public void Load()
        {
            if (Context != null)
                return;

            Configuration = GetIConfigurationRoot(TestContext.DeploymentDirectory);

            var settings = new TestSettings();
            Configuration.Bind(settings);
            Settings = settings;

            EndPoint = new Uri(settings.EndPoint);
            Context = GetContext();
        }

        public Container GetContext()
        {
            return ODataBasics.GetInLooxContext(EndPoint, Settings.InLooxAccessToken);
        }

        private IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}

