namespace blackjack.Domain
{
    public class Shoe
    {
        public const int CardsPerDeck = 52;

        private readonly IReadOnlyList<Card> template;
        private readonly Random? rng;
        private readonly int reshuffleAt;
        private Card[] cards = Array.Empty<Card>();
        private int next;

        // penetration is the fraction of the shoe dealt before the cut card comes out.
        public Shoe(int deckCount, Random rng, decimal penetration = 0.75m)
            : this(BuildDecks(deckCount), rng ?? throw new ArgumentNullException(nameof(rng)), penetration)
        {
        }

        private Shoe(IReadOnlyList<Card> template, Random? rng, decimal penetration)
        {
            if (penetration <= 0m || penetration > 1m)
            {
                throw new ArgumentOutOfRangeException(nameof(penetration), "Penetration must be above 0 and at most 1.");
            }

            this.template = template;
            this.rng = rng;
            reshuffleAt = (int)Math.Round(template.Count * (1m - penetration));
            Shuffle();
        }

        // Deals exactly these cards in this order, and Shuffle() puts them back in the same order.
        // Lets tests rig naturals and busts.
        public static Shoe Stacked(params Card[] order) => new(order.ToArray(), null, 1m);

        public int CardsRemaining => cards.Length - next;
        public bool NeedsReshuffle => CardsRemaining <= reshuffleAt;

        public void Shuffle()
        {
            cards = template.ToArray();
            next = 0;
            rng?.Shuffle(cards);
        }

        public Card Deal()
        {
            if (CardsRemaining == 0)
            {
                throw new InvalidOperationException("The shoe is empty; shuffle before dealing.");
            }

            return cards[next++];
        }

        private static List<Card> BuildDecks(int deckCount)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(deckCount, 1);

            var decks = new List<Card>(deckCount * CardsPerDeck);
            for (var d = 0; d < deckCount; d++)
            {
                foreach (var suit in Enum.GetValues<Suit>())
                {
                    foreach (var rank in Enum.GetValues<Rank>())
                    {
                        decks.Add(new Card(rank, suit));
                    }
                }
            }

            return decks;
        }
    }
}
