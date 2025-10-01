//class referencing CapstoneMaui.Core.Components using Xunit and Moq and Bunit to test navigation based on user roles
using Xunit;
using Moq;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using CapstoneMaui.Core.Services.Abstractions;
using CapstoneMaui.Core.Components.Pages;
using Microsoft.AspNetCore.Components;


namespace Capstone.Tests.Unit
{
    public class Navigation : TestContext
    {
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

            Services.AddSingleton(authServiceMock.Object);

            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = RenderComponent<Login>();
            // Simulate user input and login button click
            cut.Find("input[label='Email']").Change("user@example.com");
            cut.Find("input[label='Password']").Change("password123");
            cut.Find("button").Click();

            // Assert
            Assert.Contains("/employeedashboard", navMan.Uri);
        }
    }
}