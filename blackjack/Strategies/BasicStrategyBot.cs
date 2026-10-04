using blackjack.Domain;

namespace blackjack.Strategies
{
    // Basic strategy cut down to hit and stand. The full book also doubles and splits,
    // so this plays slightly below true basic strategy.
    public class BasicStrategyBot : IPlayerStrategy
    {
        public decimal DecideBet(decimal chips, decimal minBet) => chips >= minBet ? minBet : 0m;

        public Move DecideMove(Hand hand, Card dealerUpcard)
        {
            var total = hand.Value;
            var up = dealerUpcard.PointValue; // 2 to 11, ace is 11

            if (hand.IsSoft)
            {
                if (total >= 19)
                {
                    return Move.Stand;
                }

                if (total == 18)
                {
                    return up <= 8 ? Move.Stand : Move.Hit;
                }

                return Move.Hit;
            }

            if (total >= 17)
            {
                return Move.Stand;
            }

            if (total >= 13)
            {
                return up <= 6 ? Move.Stand : Move.Hit;
            }

            if (total == 12)
            {
                return up is >= 4 and <= 6 ? Move.Stand : Move.Hit;
            }

            return Move.Hit;
        }
    }
}
