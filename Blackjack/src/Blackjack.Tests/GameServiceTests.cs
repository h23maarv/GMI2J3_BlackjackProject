using System.Linq;
using System.Collections.Generic;
using Blackjack.Core.Interfaces;
using Blackjack.Core.Models;
using Blackjack.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;
using System;

namespace Blackjack.Tests
{
    public class GameServiceTests
    {
        // Hjälpmetod för en riktig handservice (integration)
        private IHandService CreateRealHandService()
        {
            var randomMock = new Mock<IRandomProvider>();
            randomMock.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((min, max) => min);
            randomMock.Setup(r => r.Shuffle(It.IsAny<IList<Card>>())).Callback<IList<Card>>(l => { });
            return new HandService(randomMock.Object);
        }

        [Fact]
        public void DealInitialHands_GivesTwoCardsToPlayerAndDealer()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 5),
                new Card(Suit.Ruter, 7),
                new Card(Suit.Klöver, 2)
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);

            var handService = CreateRealHandService(); // Äkta handservice för riktig logik!
            var bettingServiceMock = new Mock<IBettingService>();

            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);

            // Act
            gameSvc.DealInitialHands();

            // Assert
            gameSvc.PlayerHand.Should().HaveCount(2);
            gameSvc.DealerHand.Should().HaveCount(2);
        }

        [Fact]
        public void PlayerHit_AddsCardToPlayerHand()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 5),
                new Card(Suit.Ruter, 7),
                new Card(Suit.Klöver, 2)
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Act
            gameSvc.PlayerHit();
            // Assert
            gameSvc.PlayerHand.Should().HaveCount(3);
        }

        [Fact]
        public void DealerPlay_StopsWhenDealerReaches17OrMore()
        {
            // Arrange
            var deck = new List<Card>
            {
                new Card(Suit.Hjärter, 10), // Player
                new Card(Suit.Spader, 6),   // Dealer
                new Card(Suit.Ruter, 7),    // Player
                new Card(Suit.Klöver, 2),   // Dealer
                new Card(Suit.Hjärter, 10), // Dealer nästa kort
            };
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Act
            gameSvc.DealerPlay();
            var dealerHandValue = handService.CalculateValue(gameSvc.DealerHand);
            // Assert: Dealer har nu t.ex. 6 + 2 + 10 = 18, så ska stanna på >= 17
            Assert.True(dealerHandValue >= 17);
        }

        [Fact]
        public void EvaluateOutcome_PlayerGetsBlackjack_ReturnsPlayerWin()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 1),  // Ess till Player
                new Card(Suit.Spader, 5),   // Dealer
                new Card(Suit.Ruter, 13),   // Kung till Player
                new Card(Suit.Klöver, 2)    // Dealer
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Act
            var result = gameSvc.EvaluateOutcome();
            // Assert
            result.Should().Be(GameResult.PlayerWin); // Beroende på implementation kan du ha GameResult.Blackjack eller liknande!
        }

        [Fact]
        public void EvaluateOutcome_PlayerBusts_DealerWins()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 10),
                new Card(Suit.Ruter, 10),
                new Card(Suit.Klöver, 2)
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Låtsas att spelaren tar ett tredje kort och går över 21
            gameSvc.PlayerHit();
            // Act
            var result = gameSvc.EvaluateOutcome();
            // Assert
            result.Should().Be(GameResult.DealerWin);
        }

        [Fact]
        public void EvaluateOutcome_DealerBusts_PlayerWins()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10), // Player
                new Card(Suit.Spader, 10),  // Dealer
                new Card(Suit.Ruter, 9),    // Player
                new Card(Suit.Klöver, 10),  // Dealer
                new Card(Suit.Hjärter, 5)   // Dealer tar 5, går bust
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            gameSvc.DealerPlay();
            // Act
            var result = gameSvc.EvaluateOutcome();
            // Assert
            result.Should().Be(GameResult.PlayerWin);
        }

        [Fact]
        public void EvaluateOutcome_Push_ReturnsPush()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 10),
                new Card(Suit.Ruter, 10),
                new Card(Suit.Klöver, 10)
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Act
            var result = gameSvc.EvaluateOutcome();
            // Assert
            result.Should().Be(GameResult.Push);
        }

        [Fact]
        public void Stand_ShouldTriggerDealerPlay_AndSetGameOver()
        {
            // Arrange
            var deck = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 5),
                new Card(Suit.Ruter, 7),
                new Card(Suit.Klöver, 2),
                new Card(Suit.Hjärter, 4),
                new Card(Suit.Spader, 3),
                new Card(Suit.Ruter, 6),
                new Card(Suit.Klöver, 9),
                new Card(Suit.Hjärter, 8),
                new Card(Suit.Spader, 10),
                new Card(Suit.Klöver, 7),
                new Card(Suit.Ruter, 5),
                new Card(Suit.Hjärter, 2)
            }.ToList();
            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.Setup(d => d.CreateDeck()).Returns(deck);
            deckServiceMock.Setup(d => d.Shuffle(deck)).Returns(deck);
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);
            gameSvc.DealInitialHands();
            // Act
            gameSvc.Stand();
            // Assert: DealerPlay har körts och game är slut
            gameSvc.DealerHand.Should().NotBeEmpty();
            gameSvc.IsGameOver.Should().BeTrue();
        }

        [Fact]
        public void NewGame_AfterGameOver_ShouldResetHandsAndDeck()
        {
            // Arrange
            var deck1 = new[]
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 5),
                new Card(Suit.Ruter, 7),
                new Card(Suit.Klöver, 2),
                new Card(Suit.Hjärter, 8),
                new Card(Suit.Spader, 3),
                new Card(Suit.Ruter, 6),
                new Card(Suit.Klöver, 9),
                new Card(Suit.Hjärter, 4),
                new Card(Suit.Spader, 2),
                new Card(Suit.Ruter, 11),
                new Card(Suit.Klöver, 12),
                new Card(Suit.Hjärter, 13)
            }.ToList();
            var deck2 = new[]
            {
                new Card(Suit.Hjärter, 2),
                new Card(Suit.Spader, 7),
                new Card(Suit.Ruter, 8),
                new Card(Suit.Klöver, 3),
                new Card(Suit.Hjärter, 4),
                new Card(Suit.Spader, 5),
                new Card(Suit.Ruter, 6),
                new Card(Suit.Klöver, 7),
                new Card(Suit.Hjärter, 8),
                new Card(Suit.Spader, 9)
            }.ToList();

            var deckServiceMock = new Mock<IDeckService>();
            deckServiceMock.SetupSequence(d => d.CreateDeck())
                .Returns(deck1)
                .Returns(deck2);
            deckServiceMock.Setup(d => d.Shuffle(It.IsAny<List<Card>>())).Returns<List<Card>>(d => d);

            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);

            gameSvc.DealInitialHands();
            gameSvc.Stand();

            // Starta ny omgång
            gameSvc.DealInitialHands();

            // Assert: Nya kort har delats ut, händerna återställda
            gameSvc.PlayerHand.Should().HaveCount(2);
            gameSvc.DealerHand.Should().HaveCount(2);
        }

        [Fact]
        public void PlayerCannotHitOrStandWithoutDeal()
        {
            // Arrange
            var deckServiceMock = new Mock<IDeckService>();
            var handService = CreateRealHandService();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handService,
                bettingServiceMock.Object);

            // Act & Assert
            // PlayerHit och Stand utan DealInitialHands ska inte ändra händerna
            gameSvc.Invoking(g => g.PlayerHit()).Should().Throw<InvalidOperationException>();
            gameSvc.Invoking(g => g.Stand()).Should().Throw<InvalidOperationException>();
            gameSvc.PlayerHand.Should().BeEmpty();
            gameSvc.DealerHand.Should().BeEmpty();
        }
    }
}
