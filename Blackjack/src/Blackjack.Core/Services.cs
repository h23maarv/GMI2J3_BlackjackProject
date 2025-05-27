using Blackjack.Core.Models;
using Blackjack.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Services
{
    public class SystemRandomProvider : IRandomProvider
    {
        private readonly Random _rnd = new Random();
        public int Next(int minValue, int maxValue) => _rnd.Next(minValue, maxValue);
        public void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rnd.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }

    public class DeckService : IDeckService
    {
        private readonly IRandomProvider _random;

        public DeckService(IRandomProvider random)
        {
            _random = random;
        }

        public IList<Card> DrawCard(IList<Card> deck)
        {
            if (deck.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck");

            var card = deck[0];
            deck.RemoveAt(0);
            return new List<Card> { card }; // Return a list containing the drawn card to match the interface signature
        }

        public IList<Card> CreateDeck()
        {
            var deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                for (int rank = 1; rank <= 13; rank++)
                    deck.Add(new Card(suit, rank));
            return deck;
        }

        public IList<Card> Shuffle(IList<Card> deck)
        {
            var copy = deck.ToList();
            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                (copy[i], copy[j]) = (copy[j], copy[i]);
            }
            return copy;
        }
    }

    public class HandService : IHandService
    {
        private readonly IRandomProvider _random;
        public HandService(IRandomProvider random)
        {
            _random = random;
        }
        public int GetRandomCardValue()
        {
            return _random.Next(1, 14);
        }

        public int CalculateValue(IEnumerable<Card> hand)
        {
            int total = hand.Sum(c => c.Rank > 10 ? 10 : c.Rank);
            int aceCount = hand.Count(c => c.Rank == 1);

            // Höj total med 10 för varje ess som kan bli 11 utan att busta
            while (aceCount > 0 && total + 10 <= 21)
            {
                total += 10;
                aceCount--;
            }

            return total;
        }
    }

    public class GameService : IGameService
    {
        private readonly IDeckService _deckService;
        private readonly IHandService _handService;
        private readonly IBettingService _bettingService;
        private IList<Card> _deck;

        public bool IsGameOver { get; private set; }
        public void Stand()
        {
            IsGameOver = true;
            DealerPlay();
        }

        // Nu matchar PlayerHand/DealerHand IList<Card>
        public IList<Card> PlayerHand { get; private set; }
        public IList<Card> DealerHand { get; private set; }

        public GameService(
            IDeckService deckService,
            IHandService handService,
            IBettingService bettingService)
        {
            _deckService = deckService;
            _handService = handService;
            _bettingService = bettingService;
            PlayerHand = new List<Card>();
            DealerHand = new List<Card>();
        }

        public void DealInitialHands()
        {
            _deck = _deckService.Shuffle(_deckService.CreateDeck());
            PlayerHand.Clear();
            DealerHand.Clear();
            PlayerHand.Add((Card)_deckService.DrawCard(_deck));
            PlayerHand.Add((Card)_deckService.DrawCard(_deck));
            DealerHand.Add((Card)_deckService.DrawCard(_deck));
            DealerHand.Add((Card)_deckService.DrawCard(_deck));
        }

        public void PlayerHit()
        {
            if (_deck == null || PlayerHand == null)
                throw new InvalidOperationException("Spelet är inte initierat.");
            if (_deck.Count == 0)
                throw new InvalidOperationException("No more cards in the deck.");

            PlayerHand.Add((Card)_deckService.DrawCard(_deck));
        }

        public void DealerPlay()
        {
            if (_deck == null || DealerHand == null || PlayerHand == null)
                throw new InvalidOperationException("Spelet är inte initierat.");

            while (_handService.CalculateValue(DealerHand) < 17 && _deck.Count>0)
                DealerHand.Add((Card)_deckService.DrawCard(_deck));
        }

        public GameResult EvaluateOutcome()
        {
            var playerTotal = _handService.CalculateValue(PlayerHand);
            var dealerTotal = _handService.CalculateValue(DealerHand);
            System.Diagnostics.Debug.WriteLine($"Player: {playerTotal}, Dealer: {dealerTotal}");

            if (playerTotal > 21)
                return GameResult.DealerWin;
            if (playerTotal == 21)
                return GameResult.PlayerWin;
            if (dealerTotal > 21)
                return GameResult.PlayerWin;
            if (dealerTotal == 21)
                return GameResult.DealerWin;

            if (playerTotal > dealerTotal)
                return GameResult.PlayerWin;
            if (dealerTotal > playerTotal)
                return GameResult.DealerWin;

            if (playerTotal > 21 || playerTotal == 21 || dealerTotal > 21 || dealerTotal == 21)
                IsGameOver = true;

            return GameResult.Push;
        }

    }

    public class BettingService : IBettingService
    {
        private decimal _betAmount;

        public void PlaceBet(decimal amount)
        {
            _betAmount = amount;
        }

        public decimal Payout(GameResult result)
        {
            return result switch
            {
                GameResult.PlayerWin => _betAmount * 2,
                GameResult.DealerWin => 0,
                GameResult.Push => _betAmount,
                _ => 0
            };
        }
    }
}
