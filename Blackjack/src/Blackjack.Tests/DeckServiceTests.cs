using System;
using System.Linq;
using System.Collections.Generic;
using Blackjack.Core.Interfaces;
using Blackjack.Core.Models;
using Blackjack.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blackjack.Tests;

public class DeckServiceTests
{
    [Fact]
    public void CreateDeck_ShouldReturn52Cards()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);

        // Act
        var deck = deckService.CreateDeck();

        // Assert
        deck.Should().HaveCount(52);
    }

    [Fact]
    public void CreateDeck_ShouldContainAllSuitsAndRanksExactlyOnce()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);

        // Act
        var deck = deckService.CreateDeck();

        // Assert
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            deck.Count(card => card.Suit == suit).Should().Be(13);

            for (int rank = 1; rank <= 13; rank++)
            {
                deck.Count(card => card.Suit == suit && card.Rank == rank).Should().Be(1);
            }
        }
    }

    [Fact]
    public void DrawCard_ShouldReduceDeckSizeByOne()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var deck = deckService.CreateDeck();
        var initialCount = deck.Count;

        // Act
        var drawnCard = deckService.DrawCard(deck);

        // Assert
        deck.Should().HaveCount(initialCount - 1);
    }

    [Fact]
    public void DrawCard_ShouldRemoveCardFromDeck()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var deck = deckService.CreateDeck();

        // Act
        var drawnCard = deckService.DrawCard(deck);

        // Assert
        deck.Should().NotContain(drawnCard);
    }

    [Fact]
    public void DrawCard_ShouldEmptyDeckAfterDrawingAllCards()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var deck = deckService.CreateDeck();
        var initialCount = deck.Count;

        // Act
        for (int i = 0; i < initialCount; i++)
        {
            deckService.DrawCard(deck);
        }

        // Assert
        deck.Should().BeEmpty();
    }

    [Fact]
    public void DrawCard_ShouldThrowWhenDeckIsEmpty()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var emptyDeck = new List<Card>();

        // Act & Assert
        var action = () => deckService.DrawCard(emptyDeck);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot draw from an empty deck");
    }

    [Fact]
    public void Shuffle_ShouldChangeCardOrder()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var deck = deckService.CreateDeck();
        var originalOrder = deck.ToList(); // Create a copy of original order
        
        // Act
        var shuffledDeck = deckService.Shuffle(deck);
        
        // Assert
        // Check that at least one card has moved position
        bool atLeastOneCardMoved = false;
        for (int i = 0; i < originalOrder.Count; i++)
        {
            if (!originalOrder[i].Equals(shuffledDeck[i]))
            {
                atLeastOneCardMoved = true;
                break;
            }
        }
        atLeastOneCardMoved.Should().BeTrue();
        mockRandomProvider.Verify(r => r.Next(It.IsAny<int>(), It.IsAny<int>()), Times.AtLeast(1));
    }

    [Fact]
    public void DifferentShuffles_ShouldProduceDifferentResults()
    {
        // Arrange
        var mockRandomProvider1 = new Mock<IRandomProvider>();
        var mockRandomProvider2 = new Mock<IRandomProvider>();

        // First shuffle swaps first and last cards
        mockRandomProvider1
            .SetupSequence(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(51);

        // Second shuffle swaps first and second cards
        mockRandomProvider2
            .SetupSequence(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1);

        var deckService1 = new DeckService(mockRandomProvider1.Object);
        var deckService2 = new DeckService(mockRandomProvider2.Object);

        var originalDeck = deckService1.CreateDeck();
        var copyDeck = originalDeck.ToList();

        // Act
        var shuffledDeck1 = deckService1.Shuffle(originalDeck);
        var shuffledDeck2 = deckService2.Shuffle(copyDeck);

        // Assert
        shuffledDeck1.Should().NotBeEquivalentTo(shuffledDeck2, options => options.WithStrictOrdering());
    }

    [Fact]
    public void ResetDeck_ShouldRestoreFullDeck()
    {
        // Arrange
        var mockRandomProvider = new Mock<IRandomProvider>();
        var deckService = new DeckService(mockRandomProvider.Object);
        var deck = deckService.CreateDeck();
        
        // Draw some cards
        for (int i = 0; i < 10; i++)
        {
            deckService.DrawCard(deck);
        }
        
        // Act - reset by creating a new deck
        var resetDeck = deckService.CreateDeck();
        
        // Assert
        resetDeck.Should().HaveCount(52);
        
        // Check all suits and ranks are present
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            resetDeck.Count(card => card.Suit == suit).Should().Be(13);
            for (int rank = 1; rank <= 13; rank++)
            {
                resetDeck.Count(card => card.Suit == suit && card.Rank == rank).Should().Be(1);
            }
        }
    }
}