using blackjack.Domain;
using static blackjack.Tests.Cards;

namespace blackjack.Tests
{
    public class ShoeTests
    {
        [Fact]
        public void NewShoe_HoldsEveryCardOncePerDeck()
        {
            var shoe = new Shoe(6, new Random(1));
            var dealt = Enumerable.Range(0, 6 * 52).Select(_ => shoe.Deal()).ToList();

            Assert.Equal(0, shoe.CardsRemaining);
            Assert.All(dealt.GroupBy(c => c), g => Assert.Equal(6, g.Count()));
            Assert.Equal(52, dealt.Distinct().Count());
        }

        [Fact]
        public void SameSeed_DealsSameOrder()
        {
            var a = new Shoe(2, new Random(42));
            var b = new Shoe(2, new Random(42));

            Assert.Equal(
                Enumerable.Range(0, 104).Select(_ => a.Deal()),
                Enumerable.Range(0, 104).Select(_ => b.Deal()));
        }

        [Fact]
        public void NeedsReshuffle_OnceThePenetrationIsDealt()
        {
            var shoe = new Shoe(1, new Random(1), penetration: 0.75m);

            for (var i = 0; i < 38; i++)
            {
                shoe.Deal();
            }

            Assert.False(shoe.NeedsReshuffle);
            shoe.Deal();
            Assert.True(shoe.NeedsReshuffle); // 39 of 52 dealt, 13 left
        }

        [Fact]
        public void Shuffle_RefillsTheShoe()
        {
            var shoe = new Shoe(1, new Random(1));
            for (var i = 0; i < 45; i++)
            {
                shoe.Deal();
            }

            shoe.Shuffle();

            Assert.Equal(52, shoe.CardsRemaining);
            Assert.False(shoe.NeedsReshuffle);
        }

        [Fact]
        public void Deal_FromEmptyShoe_Throws()
        {
            var shoe = Shoe.Stacked(C("As"));
            shoe.Deal();

            Assert.Throws<InvalidOperationException>(() => shoe.Deal());
        }

        [Fact]
        public void Stacked_DealsInOrder_AndShuffleRestoresIt()
        {
            var shoe = Shoe.Stacked(Of("As Kd 7c"));

            Assert.Equal(C("As"), shoe.Deal());
            Assert.Equal(C("Kd"), shoe.Deal());
            shoe.Shuffle();
            Assert.Equal(C("As"), shoe.Deal());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DeckCount_MustBePositive(int decks)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Shoe(decks, new Random()));
        }
    }
}
