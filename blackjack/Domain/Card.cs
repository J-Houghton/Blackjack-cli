namespace blackjack.Domain
{
    public sealed record Card(Rank Rank, Suit Suit)
    {
        // Aces count 11 here; Hand drops them to 1 when the total would bust.
        public int PointValue => Rank switch
        {
            Rank.Ace => 11,
            >= Rank.Ten => 10,
            _ => (int)Rank
        };

        public string RankLabel => Rank switch
        {
            Rank.Ace => "A",
            Rank.King => "K",
            Rank.Queen => "Q",
            Rank.Jack => "J",
            _ => ((int)Rank).ToString()
        };

        public char SuitSymbol => Suit switch
        {
            Suit.Clubs => '♣',
            Suit.Diamonds => '♦',
            Suit.Hearts => '♥',
            _ => '♠'
        };

        public bool IsRed => Suit is Suit.Hearts or Suit.Diamonds;

        public override string ToString() => $"{RankLabel}{SuitSymbol}";
    }
}
