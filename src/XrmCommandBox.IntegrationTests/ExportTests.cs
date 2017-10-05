using System;
using XrmCommandBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmCommandBox.Data;

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

            // Check that a file named account.xml was created and its readable
            var ser = new DataTableSerializer();
            var dt = ser.Deserialize("account.xml");
        }
    }
}
