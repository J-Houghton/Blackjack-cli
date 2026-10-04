using static blackjack.Tests.Cards;

namespace blackjack.Tests
{
    public class HandTests
    {
        [Theory]
        [InlineData("10h 7c", 17, false)]
        [InlineData("As 6d", 17, true)]
        [InlineData("As 6d 10c", 17, false)]
        [InlineData("As Ad", 12, true)]
        [InlineData("As Ad 9c", 21, true)]
        [InlineData("As Ad Ah Ac", 14, true)]
        [InlineData("As Ad Ah Ac 10s", 14, false)]
        [InlineData("Ks Qh", 20, false)]
        [InlineData("Ks Qh 5c", 25, false)]
        public void Value_CountsAcesAsElevenUntilThatWouldBust(string cards, int value, bool soft)
        {
            var hand = HandOf(cards);

            Assert.Equal(value, hand.Value);
            Assert.Equal(soft, hand.IsSoft);
        }

        [Fact]
        public void Natural_IsTwentyOneInTwoCards()
        {
            Assert.True(HandOf("As Kd").IsNatural);
            Assert.True(HandOf("10c Ah").IsNatural);
            Assert.False(HandOf("7s 7d 7c").IsNatural);
            Assert.False(HandOf("As 9d").IsNatural);
        }

        [Fact]
        public void Bust_IsOverTwentyOne()
        {
            Assert.True(HandOf("Ks Qh 2c").IsBust);
            Assert.False(HandOf("Ks Qh Ac").IsBust);
        }

        [Fact]
        public void FaceCardsAreTenAndAcesEleven()
        {
            Assert.Equal(10, C("Js").PointValue);
            Assert.Equal(10, C("Qs").PointValue);
            Assert.Equal(10, C("Ks").PointValue);
            Assert.Equal(11, C("As").PointValue);
            Assert.Equal(2, C("2s").PointValue);
        }
    }
}
