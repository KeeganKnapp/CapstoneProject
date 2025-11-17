namespace Unit;
using Xunit;
using Moq;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using CapstoneMaui.Core.Services.Abstractions;
using MudBlazor.Services; // Add this!

public class UnitTest1 : TestContext
{
    /*
    [Fact]
    public void Login_NavigatesToEmployeeDashboard_WhenNotManager()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(true);
        authServiceMock
            .Setup(x => x.IsUserManagerAsync(default))
            .ReturnsAsync(false);
        authServiceMock
            .Setup(x => x.IsUserLoggedInAsync(default))
            .ReturnsAsync(true);
        authServiceMock
            .Setup(x => x.GetAuthTokenAsync(default))
            .ReturnsAsync("employee-token");

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(authServiceMock.Object);
        ctx.Services.AddMudServices();

        var navMan = ctx.Services.GetRequiredService<NavigationManager>();
        ctx.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        var cut = ctx.RenderComponent<Login>();
        cut.FindAll("input")[0].Change("user@example.com");
        cut.FindAll("input")[1].Change("password123");
        cut.Find("button").Click();

        Assert.Contains("/employee-dashboard", navMan.Uri);
    }

    [Fact]
    public void Login_NavigatesToManagerDashboard_WhenManager()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(true);
        authServiceMock
            .Setup(x => x.IsUserManagerAsync(default))
            .ReturnsAsync(true);
        authServiceMock
            .Setup(x => x.IsUserLoggedInAsync(default))
            .ReturnsAsync(true);
        authServiceMock
            .Setup(x => x.GetAuthTokenAsync(default))
            .ReturnsAsync("manager-token");

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(authServiceMock.Object);
        ctx.Services.AddMudServices();

        var navMan = ctx.Services.GetRequiredService<NavigationManager>();
        ctx.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        var cut = ctx.RenderComponent<Login>();
        cut.FindAll("input")[0].Change("user@example.com");
        cut.FindAll("input")[1].Change("password123");
        cut.Find("button").Click();

        Assert.Contains("/manager-dashboard", navMan.Uri);
    }
    */
}