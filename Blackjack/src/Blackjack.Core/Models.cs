namespace Blackjack.Core.Models
{

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public class Card
    {
        public Suit Suit { get; }
        public int Rank { get; }  // 1 = Ess, 11 = Knekt, 12 = Dam, 13 = Kung

        public Card(Suit suit, int rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }

    public enum GameResult
    {
        PlayerWin,
        DealerWin,
        Push
    }
}