using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG._01_Shared.Enums;

using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.ValueObjects;
using NSubstitute;

namespace MTCG.Tests._02_Business_Logic_Layer.UnitTests;

[TestClass]
[TestCategory("CardRouteHandler Tests")]
public class CardRouteHandlerTest
{
    private CardRouteHandler _handler;
    private IDataContext _mockDataContext;
    
    [TestInitialize]
    public void Setup()
    {
        
        _mockDataContext = Substitute.For<IDataContext>();
        _handler = new CardRouteHandler();
    }
    
    [TestMethod]
    [Description("Creating a null card should return a failure.")]
    public void CreateCard_NullCard_ReturnsFailure()
    {
        // Arrange
        Card card = null;

        // Act
        var result = _handler.CreateCard(card);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for null card.");
        Assert.AreEqual("Card cannot be null.", result.ErrorMessage, "Unexpected error message for null card.");
    }

    [TestMethod]
    [Description("Creating a card with empty name should return a failure.")]
    public void CreateCard_EmptyName_ReturnsFailure()
    {
        // Arrange
        var card = new Card
        {
            CardName = "",
            Damage = 10,
            Id = "valid-id-1",
            ElementType = ElementType.NotDefined,
            CardType = CardType.NotDefined
        };

        // Act
        var result = _handler.CreateCard(card);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty card name.");
        Assert.AreEqual("Invalid card.", result.ErrorMessage, "Unexpected error message for empty card name.");
    }

    [TestMethod]
    [Description("Creating a card with negative damage should return a failure.")]
    public void CreateCard_NegativeDamage_ReturnsFailure()
    {
        // Arrange
        var card = new Card
        {
            CardName = "",
            Damage = -10,
            Id = "valid-id-2",
            ElementType = ElementType.NotDefined,
            CardType = CardType.NotDefined
        };
        
        // Act
        var result = _handler.CreateCard(card);
        
        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for negative damage.");
        Assert.AreEqual("Invalid card.", result.ErrorMessage, "Unexpected error message for negative damage.");
    }
    
    [TestMethod]
    [Description("Creating a card with empty ID should return a failure.")]
    public void CreateCard_EmptyId_ReturnsFailure()
    {
        // Arrange
        var card = new Card
        {
            CardName = "Ork",
            Damage = 30,
            Id = "",
            ElementType = ElementType.NotDefined,
            CardType = CardType.MonsterCard
        };

        // Act
        var result = _handler.CreateCard(card);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for empty ID.");
        Assert.AreEqual("Invalid card.", result.ErrorMessage, "Unexpected error message for empty ID.");
    }
    
    [TestMethod]
    [Description("Creating a card that already exists should return a failure.")]
    public void CreateCard_DuplicateCard_ReturnsFailure()
    {
        // Arrange
        var card = new Card
        {
            CardName = "Dragon",
            Damage = 50,
            Id = "duplicate-id",
            ElementType = ElementType.Fire,
            CardType = CardType.MonsterCard
        };

        // Setup the mock to return an existing card when GetById is called
        _mockDataContext.GetByStringId<Card>(card.Id).Returns(new Card
        {
            CardId = 1,
            CardName = "Dragon",
            Damage = 50,
            Id = "duplicate-id",
            ElementType = ElementType.Fire,
            CardType = CardType.MonsterCard
        });

        // Act
        var result = _handler.CreateCard(card);

        // Assert
        Assert.IsFalse(result.Success, "Expected Success to be false for duplicate card.");
        Assert.AreEqual("Card already exists.", result.ErrorMessage, "Unexpected error message for duplicate card.");
            
        // Verify that GetById was called once with the correct ID
        _mockDataContext.Received(1).GetByStringId<Card>(card.Id);
            
        // Verify that Add was never called since the card already exists
        _mockDataContext.DidNotReceive().Add(card);
    }
}