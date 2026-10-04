using blackjack.Domain;

namespace blackjack.Strategies
{
    public interface IPlayerStrategy
    {
        // The stake for the next round, or 0 to sit it out.
        decimal DecideBet(decimal chips, decimal minBet);

        Move DecideMove(Hand hand, Card dealerUpcard);
    }
}
