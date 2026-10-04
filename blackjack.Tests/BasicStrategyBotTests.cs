using blackjack.Strategies;
using static blackjack.Tests.Cards;

namespace blackjack.Tests
{
    public class BasicStrategyBotTests
    {
        private readonly BasicStrategyBot bot = new();

        [Theory]
        // Hard 17+: stand.
        [InlineData("10h 7c", "As", Move.Stand)]
        [InlineData("10h 9c", "10s", Move.Stand)]
        // Hard 13 to 16: stand against 2 to 6, otherwise hit.
        [InlineData("10h 3c", "2s", Move.Stand)]
        [InlineData("10h 6c", "6s", Move.Stand)]
        [InlineData("10h 6c", "7s", Move.Hit)]
        [InlineData("10h 3c", "As", Move.Hit)]
        [InlineData("5h 4c 7d", "Ks", Move.Hit)]
        // Hard 12: stand against 4 to 6, otherwise hit.
        [InlineData("10h 2c", "3s", Move.Hit)]
        [InlineData("10h 2c", "4s", Move.Stand)]
        [InlineData("10h 2c", "6s", Move.Stand)]
        [InlineData("10h 2c", "7s", Move.Hit)]
        // Hard 11 or less: hit.
        [InlineData("6h 5c", "6s", Move.Hit)]
        [InlineData("2h 3c", "4s", Move.Hit)]
        // Soft 19+: stand.
        [InlineData("Ah 8c", "10s", Move.Stand)]
        [InlineData("Ah 9c", "As", Move.Stand)]
        // Soft 18: stand against 2 to 8, hit against 9, 10 or ace.
        [InlineData("Ah 7c", "2s", Move.Stand)]
        [InlineData("Ah 7c", "8s", Move.Stand)]
        [InlineData("Ah 7c", "9s", Move.Hit)]
        [InlineData("Ah 7c", "Qs", Move.Hit)]
        [InlineData("Ah 7c", "As", Move.Hit)]
        // Soft 17 or less: hit.
        [InlineData("Ah 6c", "6s", Move.Hit)]
        [InlineData("Ah Ac", "5s", Move.Hit)]
        // An ace that has dropped to 1 makes the hand hard again.
        [InlineData("Ah 6c 10d", "10s", Move.Stand)]
        [InlineData("Ah 5c 10d", "10s", Move.Hit)]
        public void DecideMove_FollowsTheBook(string hand, string upcard, Move expected)
        {
            Assert.Equal(expected, bot.DecideMove(HandOf(hand), C(upcard)));
        }

        [Fact]
        public void DecideBet_BetsTheMinimum_OrSitsOutWhenBroke()
        {
            Assert.Equal(10m, bot.DecideBet(500m, 10m));
            Assert.Equal(10m, bot.DecideBet(10m, 10m));
            Assert.Equal(0m, bot.DecideBet(9m, 10m));
        }
    }
}
