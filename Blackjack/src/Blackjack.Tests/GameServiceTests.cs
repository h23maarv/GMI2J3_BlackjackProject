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
    public class GameServiceTests
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
    }
}
