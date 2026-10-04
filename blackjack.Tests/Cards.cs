using blackjack.Domain;

namespace blackjack.Tests
{
    // Short card names for tests: "As" is the ace of spades, "10h" the ten of hearts, "Kd", "7c".
    internal static class Cards
    {
        public static Card C(string name)
        {
            var rank = name[..^1].ToUpperInvariant() switch
            {
                "A" => Rank.Ace,
                "K" => Rank.King,
                "Q" => Rank.Queen,
                "J" => Rank.Jack,
                var n => (Rank)int.Parse(n)
            };

            var suit = char.ToLowerInvariant(name[^1]) switch
            {
                's' => Suit.Spades,
                'h' => Suit.Hearts,
                'd' => Suit.Diamonds,
                'c' => Suit.Clubs,
                var s => throw new ArgumentException($"Unknown suit '{s}'.")
            };

            return new Card(rank, suit);
        }

        public static Card[] Of(string names) =>
            names.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(C).ToArray();

        public static Hand HandOf(string names)
        {
            var hand = new Hand();
            foreach (var card in Of(names))
            {
                hand.Add(card);
            }

            return hand;
        }
    }
}
