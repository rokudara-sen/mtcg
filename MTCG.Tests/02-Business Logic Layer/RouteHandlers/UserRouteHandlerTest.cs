using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using NSubstitute;

namespace MTCG.Tests._02_Business_Logic_Layer.RouteHandlers;

[TestClass]
[TestSubject(typeof(UserRouteHandler))]
public class UserRouteHandlerTest
{
    private UserRouteHandler _handler;
    private IUserRepository _mockUserRepository;

    [TestInitialize]
    public void Setup()
    {
        // Mock the ICardRepository
        _mockUserRepository = Substitute.For<IUserRepository>();

        // Inject the mock into the CardRouteHandler
        _handler = new UserRouteHandler(_mockUserRepository);
    }

    [TestMethod]
    [Description("Registering a null user should return a failure.")]
    public void RegisterUser_NullUser_ReturnsFailure()
    {
        // Act
        var result = _handler.RegisterUser(null);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for null user credentials.");
        Assert.AreEqual("Invalid credentials.", result.ErrorMessage, "Unexpected error message for null user credentials.");
    }
    
    [TestMethod]
    [Description("Registering a user with empty name should return a failure.")]
    public void RegisterUser_EmptyName_ReturnsFailure()
    {
        // Arrange
        var userCredentials = new UserCredentials
        {
            Username = "",
            Password = "valid-password",
        };

        // Act
        var result = _handler.RegisterUser(userCredentials);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty username.");
        Assert.AreEqual("Username cannot be empty.", result.ErrorMessage, "Unexpected error message for empty username.");
    }
    
    [TestMethod]
    [Description("Registering a user with empty password should return a failure.")]
    public void RegisterUser_EmptyPassword_ReturnsFailure()
    {
        // Arrange
        var userCredentials = new UserCredentials
        {
            Username = "valid-username",
            Password = "",
        };

        // Act
        var result = _handler.RegisterUser(userCredentials);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty password.");
        Assert.AreEqual("Password cannot be empty.", result.ErrorMessage, "Unexpected error message for empty password.");
    }
    
    [TestMethod]
    [Description("Registering a user that already exists should return a failure.")]
    public void RegisterUser_DuplicateUser_ReturnsFailure()
    {
        // Arrange
        var userCredentials = new UserCredentials()
        {
            Username = "duplicate-username",
            Password = "valid-password",
        };

        // Set up the mock to return an existing user when GetUserByUsername is called
        _mockUserRepository.GetUserByUsername(userCredentials.Username).Returns(new User("duplicate-username", "other-password"));

        // Act
        var result = _handler.RegisterUser(userCredentials);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for duplicate username.");
        Assert.AreEqual("Username is already taken.", result.ErrorMessage, "Unexpected error message for duplicate username.");
        _mockUserRepository.Received(1).GetUserByUsername(userCredentials.Username);
        _mockUserRepository.DidNotReceive().AddUser(Arg.Any<User>());
    }
    
    [TestMethod]
    [Description("Registering a valid new user should return success.")]
    public void RegisterUser_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var userCredentials = new UserCredentials()
        {
            Username = "duplicate-username",
            Password = "valid-password",
        };

        // Set up the mock to return null when GetCardByIdentifier is called, indicating the card doesn't exist
        _mockUserRepository.GetUserByUsername(userCredentials.Username).Returns((User)null);

        // Act
        var result = _handler.RegisterUser(userCredentials);

        // Assert
        Assert.IsTrue(result.Success, "Expected Success to be true for a valid new user.");
        Assert.IsNull(result.ErrorMessage, "Expected ErrorMessage to be null for a successful registration.");
    }
    
    [TestMethod]
    [Description("Login with null user credentials should return a failure.")]
    public void LoginUser_NullCredentials_ReturnsFailure()
    {
        // Act
        var result = _handler.LoginUser(null);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for null user credentials.");
        Assert.AreEqual("Invalid credentials.", result.ErrorMessage, "Unexpected error message for null user credentials.");
    }
    
    [TestMethod]
    [Description("Login a user with empty name should return a failure.")]
    public void LoginUser_EmptyName_ReturnsFailure()
    {
        // Arrange
        var userCredentials = new UserCredentials
        {
            Username = "",
            Password = "valid-password",
        };

        // Act
        var result = _handler.LoginUser(userCredentials);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty username.");
        Assert.AreEqual("Username cannot be empty.", result.ErrorMessage, "Unexpected error message for empty username.");
    }
    
    [TestMethod]
    [Description("Login a user with empty password should return a failure.")]
    public void LoginUser_EmptyPassword_ReturnsFailure()
    {
        // Arrange
        var userCredentials = new UserCredentials
        {
            Username = "valid-username",
            Password = "",
        };

        // Act
        var result = _handler.LoginUser(userCredentials);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty password.");
        Assert.AreEqual("Password cannot be empty.", result.ErrorMessage, "Unexpected error message for empty password.");
    }
}