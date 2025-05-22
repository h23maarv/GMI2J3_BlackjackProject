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
    }
}