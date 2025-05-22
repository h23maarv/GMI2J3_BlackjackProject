using System.Linq;
using System.Collections.Generic;
using Blackjack.Core.Interfaces;
using Blackjack.Core.Models;
using Blackjack.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blackjack.Tests
{
    public class HandServiceTests
    {
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
            var handServiceMock = new Mock<IHandService>();
            var bettingServiceMock = new Mock<IBettingService>();
            var gameSvc = new GameService(
                deckServiceMock.Object,
                handServiceMock.Object,
                bettingServiceMock.Object);
            // Act
            gameSvc.DealInitialHands();
            // Assert
            gameSvc.PlayerHand.Should().HaveCount(2);
            gameSvc.DealerHand.Should().HaveCount(2);
        }

        [Fact]
        public void CalculateValueFromProvider_UsesCardProvider()
        {
            // Arrange
            var mockProvider = new Mock<IRandomProvider>();
            mockProvider.Setup(r => r.Next(1, 14)).Returns(7);
            var handService = new HandService(mockProvider.Object);

            // Act
            var value = handService.GetRandomCardValue();

            // Assert
            value.Should().Be(7);
            mockProvider.Verify(r => r.Next(1, 14), Times.Once);
        }

        [Fact]
        public void CalculateValue_CorrectlyCalculatesHandValue()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 1), // Ess
                new Card(Suit.Ruter, 5)
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(16); // 10 + 1 (ess som 11) + 5 = 16
        }

        [Fact]
        public void CalculateValue_HandlesMultipleAcesCorrectly()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1), // Ess
                new Card(Suit.Spader, 1), // Ess
                new Card(Suit.Ruter, 5)
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(17); // 1 (ess som 11) + 1 (ess som 1) + 5 = 17
        }

        [Fact]
        public void CalculateValue_HandlesBustedHand()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 10),
                new Card(Suit.Ruter, 5)
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(25); // 10 + 10 + 5 = 25 (bust)
        }

        [Fact]
        public void CalculateValue_HandlesBlackjack()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 1) // Ess
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(21); // 10 + 1 (ess som 11) = 21 (blackjack)
        }

        [Fact]
        public void CalculateValue_HandlesAllFaceCardsAsTen()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 10),
                new Card(Suit.Spader, 11), // Knekt
                new Card(Suit.Ruter, 12) // Dam
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(30); // 10 + 10 + 10 = 30 (bust)
        }

        [Fact]
        public void CalculateValue_HandlesEmptyHand()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>();
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(0); // Ingen kort, värde ska vara 0
        }

        [Fact]
        public void CalculateValue_HandlesSingleAceAsEleven()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1) // Ess
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(11); // Ess som 11
        }

        [Fact]
        public void CalculateValue_HandlesSingleAceAsOne()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1), // Ess
                new Card(Suit.Spader, 10) // Tio
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(21); // Ess som 11 + 10 = 21 (blackjack)
        }

        [Fact]
        public void CalculateValue_HandlesMultipleAcesAsOnes()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1), // Ess
                new Card(Suit.Spader, 1), // Ess
                new Card(Suit.Ruter, 10) // Tio
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(12); // Ess som 1 + Ess som 1 + 10 = 12
        }

        [Fact]
        public void CalculateValue_HandlesMultipleAcesAndFaceCards()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1), // Ess
                new Card(Suit.Spader, 10), // Knekt
                new Card(Suit.Ruter, 10) // Dam
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(21); // Ess som 11 + Knekt + Dam = 22 (bust)
        }

        [Fact]
        public void CalculateValue_HandlesMultipleAcesAndTens()
        {
            // Arrange
            var handService = new HandService(new Mock<IRandomProvider>().Object);
            var hand = new List<Card>
            {
                new Card(Suit.Hjärter, 1), // Ess
                new Card(Suit.Spader, 10), // Tio
                new Card(Suit.Ruter, 10) // Tio
            };
            // Act
            var value = handService.CalculateValue(hand);
            // Assert
            value.Should().Be(21); // Ess som 11 + Tio + Tio = 31 (bust)
        }
    }
}
