using KinoCinema.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class RegTests
{
    [TestMethod]
    public void RegTestSuccess()
    {
        var page = new RegisterPage();
        string uniqueLogin = "User" + DateTime.Now.Ticks; // Чтобы логин не повторялся
        bool result = page.RegisterUser(uniqueLogin, "12345", "12345");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RegTestFail()
    {
        var page = new RegisterPage();
        // Пароли не совпадают
        Assert.IsFalse(page.RegisterUser("NewUser", "123", "321"));
        // Пустой логин
        Assert.IsFalse(page.RegisterUser("", "123", "123"));
    }
}