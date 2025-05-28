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

        public Card DrawCard(IList<Card> deck)
        {
            if (deck.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck");

            var card = deck[0];
            deck.RemoveAt(0);
            return card;
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
            if (hand == null)
                return 0; // eller throw new ArgumentNullException(nameof(hand));

            int total = hand.Where(c => c != null).Sum(c => c.Rank > 10 ? 10 : c.Rank);
            int aceCount = hand.Count(c => c != null && c.Rank == 1);
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
            PlayerHand.Add(_deckService.DrawCard(_deck));
            PlayerHand.Add(_deckService.DrawCard(_deck));
            DealerHand.Add(_deckService.DrawCard(_deck));
            DealerHand.Add(_deckService.DrawCard(_deck));
        }

        public void PlayerHit()
        {
            if (_deck == null || _deck.Count <= PlayerHand.Count + DealerHand.Count)
                throw new InvalidOperationException("No more cards in the deck.");

            var card = _deck[PlayerHand.Count + DealerHand.Count];
            PlayerHand.Add(card);
        }

        public void DealerPlay()
        {
            while (_handService.CalculateValue(DealerHand) < 17)
            {
                int nextIndex = PlayerHand.Count + DealerHand.Count;
                if (_deck == null || nextIndex >= _deck.Count)
                    throw new InvalidOperationException("No more cards in the deck.");
                var card = _deck[nextIndex];
                if (card == null) throw new InvalidOperationException("Card is null.");
                DealerHand.Add(card);
            }
        }


        public GameResult EvaluateOutcome()
        {
            var playerTotal = _handService.CalculateValue(PlayerHand);
            var dealerTotal = _handService.CalculateValue(DealerHand);

            // Kolla blackjack på två första korten
            bool playerBlackjack = PlayerHand.Count == 2 && playerTotal == 21;
            bool dealerBlackjack = DealerHand.Count == 2 && dealerTotal == 21;

            if (playerBlackjack && dealerBlackjack)
                return GameResult.Push; // Oavgjort
            if (playerBlackjack)
                return GameResult.PlayerWin; // Eller skapa GameResult.Blackjack
            if (dealerBlackjack)
                return GameResult.DealerWin;

            if (playerTotal > 21)
                return GameResult.DealerWin;
            if (dealerTotal > 21)
                return GameResult.PlayerWin;
            if (playerTotal > dealerTotal)
                return GameResult.PlayerWin;
            if (dealerTotal > playerTotal)
                return GameResult.DealerWin;

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
