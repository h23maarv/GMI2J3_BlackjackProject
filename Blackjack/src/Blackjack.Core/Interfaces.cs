using Blackjack.Core.Models;
using Blackjack.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Interfaces
{
    public interface IDeckService
    {
        IList<Card> CreateDeck();
        IList<Card> Shuffle(IList<Card> deck);
        Card DrawCard(IList<Card> deck);
    }

    public interface IRandomProvider
    {
        int Next(int minValue, int maxValue);
        void Shuffle<T>(IList<T> list);
    }

    public interface IHandService
    {
        int CalculateValue(IEnumerable<Card> hand);
    }

    public interface IGameService
    {
        void DealInitialHands();
        void PlayerHit();
        void DealerPlay();
        GameResult EvaluateOutcome();

        // Ändrat till IList<Card> för att matcha implementationen
        IList<Card> PlayerHand { get; }
        IList<Card> DealerHand { get; }
    }

    public interface IBettingService
    {
        void PlaceBet(decimal amount);
        decimal Payout(GameResult result);
    }
}
