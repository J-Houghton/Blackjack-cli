using blackjack.Domain;
using blackjack.Strategies;

namespace blackjack.Cli
{
    // The human seat: reads bets and moves from single keys in the table's action bar, keeping
    // Console out of the engine. On autopilot the book plays instead, at the table minimum.
    internal sealed class ConsoleStrategy : IPlayerStrategy
    {
        private readonly TableScreen screen;
        private readonly KeyBindings keys;
        private readonly FastForward fastForward;
        private readonly SessionStats stats;
        private readonly BasicStrategyBot book = new();
        private long bet;

        public ConsoleStrategy(TableScreen screen, KeyBindings keys, FastForward fastForward, SessionStats stats)
        {
            this.screen = screen;
            this.keys = keys;
            this.fastForward = fastForward;
            this.stats = stats;
        }

        // Bets are whole chips, so a balance never has more decimals than the table shows
        // (3:2 and 6:5 on whole bets leave at most one). Returns 0 to cash out.
        public decimal DecideBet(decimal chips, decimal minBet)
        {
            var min = (long)decimal.Ceiling(minBet);
            var max = (long)decimal.Floor(Math.Min(chips, long.MaxValue));
            if (max < min)
            {
                return 0m;
            }

            if (fastForward.IsActive)
            {
                return book.DecideBet(chips, minBet);
            }

            bet = Math.Clamp(bet == 0 ? min : bet, min, max);
            var typed = "";
            var cashingOut = false;
            try
            {
                while (true)
                {
                    screen.PendingBet = typed.Length > 0 && long.TryParse(typed, out var shown) ? shown : bet;
                    if (cashingOut)
                    {
                        screen.SetBar(
                            $"[bold red]Cash out with {TableScreen.Money(chips)} chips?[/]  {screen.Key(KeyAction.CashOut)} again to confirm",
                            "[grey]any other key keeps you at the table[/]",
                            BarTone.Alert);
                    }
                    else
                    {
                        screen.SetBar(BetLine(typed, min, max), BetCommands(), BarTone.Input);
                    }

                    var key = screen.ReadKey();
                    if (cashingOut)
                    {
                        if (keys.Is(KeyAction.CashOut, key))
                        {
                            return 0m;
                        }

                        cashingOut = false;
                        continue;
                    }

                    if (screen.HandleToggle(key))
                    {
                        continue;
                    }

                    if (key.KeyChar is >= '0' and <= '9')
                    {
                        if (typed.Length < 9 && !(typed.Length == 0 && key.KeyChar == '0'))
                        {
                            typed += key.KeyChar;
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        typed = typed.Length > 0 ? typed[..^1] : typed;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        typed = "";
                    }
                    else if (keys.Is(KeyAction.BetDown, key))
                    {
                        typed = "";
                        bet = Math.Max(min, bet - min);
                    }
                    else if (keys.Is(KeyAction.BetUp, key))
                    {
                        typed = "";
                        bet = Math.Min(max, bet + min);
                    }
                    else if (key.Key == ConsoleKey.Enter || keys.Is(KeyAction.Deal, key))
                    {
                        if (typed.Length == 0)
                        {
                            return bet;
                        }

                        var value = long.Parse(typed);
                        if (value >= min && value <= max)
                        {
                            bet = value;
                            return bet;
                        }
                    }
                    else if (keys.Is(KeyAction.FastForward, key))
                    {
                        fastForward.Watch();
                        return book.DecideBet(chips, minBet);
                    }
                    else if (keys.Is(KeyAction.NextShoe, key))
                    {
                        fastForward.SkipShoe();
                        return book.DecideBet(chips, minBet);
                    }
                    else if (keys.Is(KeyAction.CashOut, key))
                    {
                        cashingOut = true;
                    }
                }
            }
            finally
            {
                screen.PendingBet = null;
                screen.ClearBar();
            }
        }

        public Move DecideMove(Hand hand, Card dealerUpcard)
        {
            var advice = book.DecideMove(hand, dealerUpcard);
            if (fastForward.IsActive)
            {
                return advice;
            }

            try
            {
                while (true)
                {
                    screen.SetBar(MoveLine(hand, dealerUpcard, advice), screen.Toggles(), BarTone.Input);
                    var key = screen.ReadKey();
                    if (screen.HandleToggle(key))
                    {
                        continue;
                    }

                    Move? move = keys.Is(KeyAction.Hit, key) ? Move.Hit : keys.Is(KeyAction.Stand, key) ? Move.Stand : null;
                    if (move is null)
                    {
                        continue;
                    }

                    stats.RecordDecision(move == advice);
                    if (move != advice)
                    {
                        screen.Log($"[yellow]The book says {Verb(advice)} on {Describe(hand)} against {Describe(dealerUpcard)}.[/]");
                    }

                    return move.Value;
                }
            }
            finally
            {
                screen.ClearBar();
            }
        }

        private string BetLine(string typed, long min, long max)
        {
            var title = "[bold yellow]PLACE YOUR BET[/]";
            if (typed.Length == 0)
            {
                return $"{title}   [bold]◀ {bet} ▶[/]   {screen.Key(KeyAction.BetDown)}/{screen.Key(KeyAction.BetUp)} -/+{min}"
                    + $" · type an amount · {screen.Key(KeyAction.Deal)} deal";
            }

            var value = long.Parse(typed);
            var problem = value < min ? $"  [red]the minimum is {min}[/]" : value > max ? $"  [red]you have {max}[/]" : "";
            return $"{title}   [bold]{typed}_[/]{problem}   [grey]Backspace · Esc[/] · {screen.Key(KeyAction.Deal)} deal";
        }

        private string BetCommands() =>
            $"[grey]{screen.Key(KeyAction.FastForward)} autopilot · {screen.Key(KeyAction.NextShoe)} next shoe · "
            + $"{screen.Key(KeyAction.CashOut)} cash out ·[/] {screen.Toggles()}";

        private string MoveLine(Hand hand, Card dealerUpcard, Move advice)
        {
            string Option(KeyAction action, Move move) =>
                screen.ShowBook && move == advice
                    ? $"[bold green]{screen.Key(action)} {Verb(move).ToUpperInvariant()}[/]"
                    : $"{screen.Key(action)} {Verb(move)}";

            var hint = screen.ShowBook ? $"   [green]book: {Verb(advice)}[/]" : "";
            return $"[bold yellow]YOUR MOVE[/]  {Describe(hand)} against {Describe(dealerUpcard)}"
                + $"     {Option(KeyAction.Hit, Move.Hit)}     {Option(KeyAction.Stand, Move.Stand)}{hint}";
        }

        private static string Verb(Move move) => move == Move.Hit ? "hit" : "stand";

        private static string Describe(Hand hand) => hand.IsSoft ? $"soft {hand.Value}" : $"hard {hand.Value}";

        private static string Describe(Card upcard) =>
            upcard.Rank == Rank.Ace ? "an ace" : upcard.PointValue == 8 ? "an 8" : $"a {upcard.PointValue}";
    }
}
