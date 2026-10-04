using blackjack.Domain;

namespace blackjack.Cli
{
    // Cards as white tiles: three lines tall for the dealer and you, a one-line chip everywhere else.
    internal static class CardArt
    {
        public const int TileWidth = 5;
        public const int TileHeight = 3;

        // Rank in the top-left corner, suit in the middle, rank again bottom-right.
        public static string[] Tile(Card card)
        {
            var style = Style(card);
            return new[]
            {
                $"[{style}]{card.RankLabel.PadRight(TileWidth)}[/]",
                $"[{style}]  {card.SuitSymbol}  [/]",
                $"[{style}]{card.RankLabel.PadLeft(TileWidth)}[/]"
            };
        }

        public static string[] Back() => Enumerable.Repeat("[white on blue]▒▒▒▒▒[/]", TileHeight).ToArray();

        // Tiles side by side, as one block of markup lines.
        public static string Row(IEnumerable<string[]> tiles)
        {
            var list = tiles.ToList();
            return string.Join("\n", Enumerable.Range(0, TileHeight).Select(line => string.Join(" ", list.Select(t => t[line]))));
        }

        public static string Chip(Card card) => $"[{Style(card)}]{card}[/]";

        public const string HiddenChip = "[white on blue]▒▒[/]";

        // A chip with a little padding, for the event log.
        public static string LogChip(Card card) => $"[{Style(card)}] {card} [/]";

        private static string Style(Card card) => card.IsRed ? "red on white" : "black on white";
    }
}
