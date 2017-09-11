using System;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicsDataTools;

namespace DynamicsDataTools.Tests
{
    [TestClass]
    public class ExportToolTests
    {
        [TestMethod]
        public void Exports_An_Account()
        {
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var exportTool = new ExportTool(log,service);

        }
    }
}
