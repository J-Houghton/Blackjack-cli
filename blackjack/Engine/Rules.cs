namespace blackjack.Engine
{
    public class Rules
    {
        public int DeckCount { get; set; } = 6;

        // Paid on top of the stake: 1.5 is 3:2, 1.2 is 6:5.
        public decimal BlackjackPayout { get; set; } = 1.5m;
        public bool DealerHitsSoft17 { get; set; }

        // Fraction of the shoe dealt before the cut card comes out.
        public decimal Penetration { get; set; } = 0.75m;
        public decimal MinimumBet { get; set; } = 10m;

        public void Validate()
        {
            if (DeckCount < 1)
            {
                throw new InvalidOperationException("A shoe needs at least one deck.");
            }

            if (BlackjackPayout <= 0m)
            {
                throw new InvalidOperationException("The blackjack payout must be positive.");
            }

            if (Penetration <= 0m || Penetration > 1m)
            {
                throw new InvalidOperationException("Penetration must be above 0 and at most 1.");
            }

            if (MinimumBet <= 0m)
            {
                throw new InvalidOperationException("The minimum bet must be positive.");
            }
        }
    }
}
