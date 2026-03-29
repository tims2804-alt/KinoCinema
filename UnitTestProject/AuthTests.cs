using KinoCinema.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AuthTests
{
    [TestMethod]
    public void AuthTestSuccess() // Позитивный тест
    {
        var page = new LoginPage();
        // Укажите логин и пароль, которые ТОЧНО есть в вашей БД
        bool result = page.Auth("admin", "admin");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AuthTestFail() // Негативные сценарии
    {
        var page = new LoginPage();
        Assert.IsFalse(page.Auth("", ""), "Пустые поля");
        Assert.IsFalse(page.Auth("non_existent_user", "wrong_password"), "Несуществующий юзер");
    }
}