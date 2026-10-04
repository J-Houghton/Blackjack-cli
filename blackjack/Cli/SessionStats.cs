using blackjack.Domain;
using blackjack.Engine;

namespace blackjack.Cli
{
    // The human's record for the cash-out summary. Hands the autopilot plays are kept apart.
    internal sealed class SessionStats
    {
        public SessionStats(decimal startingChips)
        {
            StartingChips = startingChips;
        }

        public decimal StartingChips { get; }
        public int Rounds { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Pushes { get; private set; }
        public int Blackjacks { get; private set; }
        public int AutopilotHands { get; private set; }
        public int Decisions { get; private set; }
        public int BookDecisions { get; private set; }

        public void Record(Hand hand)
        {
            Rounds++;
            switch (hand.Outcome)
            {
                case Outcome.Blackjack:
                    Blackjacks++;
                    Wins++;
                    break;
                case Outcome.Win:
                    Wins++;
                    break;
                case Outcome.Push:
                    Pushes++;
                    break;
                case Outcome.Lose:
                    Losses++;
                    break;
            }
        }

        public void RecordAutopilot() => AutopilotHands++;

        public void RecordDecision(bool followedBook)
        {
            Decisions++;
            if (followedBook)
            {
                BookDecisions++;
            }
        }
    }
}
