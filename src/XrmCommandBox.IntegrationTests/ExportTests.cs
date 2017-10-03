using System;
using XrmCommandBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XrmCommandBox.IntegrationTests
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod]
        public void ExportAccount()
        {

            var commandParameters = new[] { "export",
                                            "--connection", "integrationTests",
                                            "--entity", "account",
                                            "--recordNumber" };

            Program.Main(commandParameters);

        }
    }
}
