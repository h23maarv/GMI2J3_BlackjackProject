using System.Collections.Generic;
using Blackjack.Core.Models;

namespace Blackjack.Web.Models
{
    public class BlackjackViewModel
    {
        public IEnumerable<Card> PlayerHand { get; set; } = new List<Card>();
        public IEnumerable<Card> DealerHand { get; set; } = new List<Card>();
        public bool IsGameOver { get; set; }
        public GameResult? Result { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Payout { get; set; }
        public decimal SessionVinst { get; set; }

        public int PlayerHandValue
        {
            get
            {
                if (PlayerHand == null)

                    return 0;

                return CalculateHandValue(PlayerHand);
            }
        }

        public int DealerHandValue
        {
            get
            {
                if (DealerHand == null)
                    return 0;
                return CalculateHandValue(DealerHand);
            }
        }

        private int CalculateHandValue(IEnumerable<Card> hand)
        {
            int value = 0;
            int aces = 0;

            foreach (var card in hand)
            {
                if (card.Rank >= 2 && card.Rank <= 10)
                    value += card.Rank;
                else if (card.Rank > 10)
                    value += 10;
                else if (card.Rank == 1)
                {
                    value += 11;
                    aces++;
                }
            }
            while (value > 21 && aces > 0)
            {
                value -= 10;
                aces--;
            }
            return value;
        }
    }
}