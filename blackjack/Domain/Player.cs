using blackjack.Strategies;

namespace blackjack.Domain
{
    public class Player
    {
        public Player(string name, decimal chips, IPlayerStrategy strategy)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegative(chips);
            ArgumentNullException.ThrowIfNull(strategy);

            Name = name;
            Chips = chips;
            Strategy = strategy;
        }

        public string Name { get; }
        public decimal Chips { get; private set; }
        public List<Hand> Hands { get; } = new();
        public IPlayerStrategy Strategy { get; }

        public void PlaceBet(decimal amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            if (amount > Chips)
            {
                throw new InvalidOperationException("Not enough chips to place the bet.");
            }

            Chips -= amount;
            Hands.Add(new Hand { Bet = amount });
        }

        public void Receive(decimal amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            Chips += amount;
        }
    }
}
