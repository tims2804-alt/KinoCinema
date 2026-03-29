using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class AuthPage
    {
        [TestMethod]
        public void AuthTest()
        {

            var page = new AuthPage();
            Assert.IsTrue(page.Login())

        }
    }
}
