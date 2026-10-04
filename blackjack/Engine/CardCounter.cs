using blackjack.Domain;

namespace blackjack.Engine
{
    // Hi-Lo count over the cards the table can see. Wire OnCardRevealed to Game.CardRevealed
    // and Reset to Game.Shuffled; the dealer's hole card only counts once it is turned over.
    public class CardCounter
    {
        public int RunningCount { get; private set; }

        // With nothing left to divide by, the running count is all there is.
        public decimal TrueCount(decimal decksRemaining) =>
            decksRemaining > 0m ? RunningCount / decksRemaining : RunningCount;

        public void OnCardRevealed(Card card) => RunningCount += HiLoValue(card);

        public void Reset() => RunningCount = 0;

        public static int HiLoValue(Card card) => card.PointValue switch
        {
            <= 6 => 1,
            >= 10 => -1,
            _ => 0
        };
    }
}
