using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.Enums;
using NSubstitute;

namespace MTCG.Tests._02_Business_Logic_Layer.RouteHandlers
{
    [TestClass]
    [TestCategory("CardRouteHandler Tests")]
    public class CardRouteHandlerTest
    {
        private CardRouteHandler _handler;
        private ICardRepository _mockCardRepository;

        [TestInitialize]
        public void Setup()
        {
            // Mock the ICardRepository
            _mockCardRepository = Substitute.For<ICardRepository>();

            // Inject the mock into the CardRouteHandler
            _handler = new CardRouteHandler(_mockCardRepository);
        }

        [TestMethod]
        [Description("Creating a null card should return a failure.")]
        public void CreateCard_NullCard_ReturnsFailure()
        {
            // Act
            var result = _handler.CreateCard(null);

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
                CardName = "ValidName",
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

            // Set up the mock to return an existing card when GetCardByIdentifier is called
            _mockCardRepository.GetCardByIdentifier(card.Id).Returns(new Card
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
            _mockCardRepository.Received(1).GetCardByIdentifier(card.Id);
            _mockCardRepository.DidNotReceive().AddCard(Arg.Any<Card>());
        }

        [TestMethod]
        [Description("Creating a valid new card should return success.")]
        public void CreateCard_ValidCard_ReturnsSuccess()
        {
            // Arrange
            var card = new Card
            {
                CardName = "WaterGoblin",
                Damage = 10,
                Id = "unique-id-1",
                ElementType = ElementType.NotDefined,
                CardType = CardType.NotDefined
            };

            // Set up the mock to return null when GetCardByIdentifier is called, indicating the card doesn't exist
            _mockCardRepository.GetCardByIdentifier(card.Id).Returns((Card)null);

            // Act
            var result = _handler.CreateCard(card);

            // Assert
            Assert.IsTrue(result.Success, "Expected Success to be true for a valid new card.");
            Assert.IsNull(result.ErrorMessage, "Expected ErrorMessage to be null for a successful creation.");
        }

        [TestMethod]
        [Description("SpecifyCardElement with 'Water' in the name sets ElementType to Water.")]
        public void SpecifyCardElement_WaterInName_SetsWaterElement()
        {
            // Arrange
            var card = new Card
            {
                CardName = "WaterGoblin",
                Damage = 10,
                Id = "id-1"
            };

            // Act
            // Call the private method using reflection
            InvokePrivateMethod(_handler, "SpecifyCardElement", card);

            // Assert
            Assert.AreEqual(ElementType.Water, card.ElementType, "ElementType should be set to Water.");
        }

        [TestMethod]
        [Description("SpecifyCardElement with 'Fire' in the name sets ElementType to Fire.")]
        public void SpecifyCardElement_FireInName_SetsFireElement()
        {
            // Arrange
            var card = new Card
            {
                CardName = "FireSpell",
                Damage = 25,
                Id = "id-2"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardElement", card);

            // Assert
            Assert.AreEqual(ElementType.Fire, card.ElementType, "ElementType should be set to Fire.");
        }

        [TestMethod]
        [Description("SpecifyCardElement with 'Normal' in the name sets ElementType to Normal.")]
        public void SpecifyCardElement_NormalInName_SetsNormalElement()
        {
            // Arrange
            var card = new Card
            {
                CardName = "NormalSpell",
                Damage = 15,
                Id = "id-3"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardElement", card);

            // Assert
            Assert.AreEqual(ElementType.Normal, card.ElementType, "ElementType should be set to Normal.");
        }

        [TestMethod]
        [Description("SpecifyCardElement without specific keywords sets ElementType to NotDefined.")]
        public void SpecifyCardElement_UnknownName_SetsNotDefinedElement()
        {
            // Arrange
            var card = new Card
            {
                CardName = "MysteryCard",
                Damage = 5,
                Id = "id-4"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardElement", card);

            // Assert
            Assert.AreEqual(ElementType.NotDefined, card.ElementType, "ElementType should be set to NotDefined.");
        }

        [TestMethod]
        [Description("SpecifyCardType with 'Spell' in the name sets CardType to SpellCard.")]
        public void SpecifyCardType_SpellInName_SetsSpellCardType()
        {
            // Arrange
            var card = new Card
            {
                CardName = "FireSpell",
                Damage = 25,
                Id = "id-5"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardType", card);

            // Assert
            Assert.AreEqual(CardType.SpellCard, card.CardType, "CardType should be set to SpellCard.");
        }

        [TestMethod]
        [Description("SpecifyCardType with monster keywords in the name sets CardType to MonsterCard.")]
        public void SpecifyCardType_MonsterKeywordsInName_SetsMonsterCardType()
        {
            // Arrange
            var card = new Card
            {
                CardName = "Dragon",
                Damage = 50,
                Id = "id-6"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardType", card);

            // Assert
            Assert.AreEqual(CardType.MonsterCard, card.CardType, "CardType should be set to MonsterCard.");
        }

        [TestMethod]
        [Description("SpecifyCardType without specific keywords sets CardType to NotDefined.")]
        public void SpecifyCardType_UnknownName_SetsNotDefinedCardType()
        {
            // Arrange
            var card = new Card
            {
                CardName = "MysteryCard",
                Damage = 10,
                Id = "id-7"
            };

            // Act
            InvokePrivateMethod(_handler, "SpecifyCardType", card);

            // Assert
            Assert.AreEqual(CardType.NotDefined, card.CardType, "CardType should be set to NotDefined.");
        }

        // Helper method to invoke private methods using reflection
        private void InvokePrivateMethod(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null) method.Invoke(obj, parameters);
        }
    }
}
