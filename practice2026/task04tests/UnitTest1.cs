using Xunit;
using task04;


namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }
        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(30, fighter.FirePower);
        }
        [Fact]
        public void Cruiser_ShouldBePowerfulThanFighter()
        {
            var cruiser = new Cruiser();
            var fighter = new Fighter();
            Assert.True(cruiser.FirePower > fighter.FirePower);
        }
        [Fact]
        public void Fighter_BatteryCapacity_ShouldDecreaseAfterShots()
        {
            var fighter = new Fighter();
            fighter.Fire();
            fighter.Fire();
            Assert.Equal(80, fighter.BatteryCapacity);
        }
        [Fact]
        public void Cruiser_BatteryCapacity_ShouldDecreaseAfterShots()
        {
            var cruiser = new Cruiser();
            cruiser.Fire();
            cruiser.Fire();
            Assert.Equal(50, cruiser.BatteryCapacity);
        }
        [Fact]
        public void Fighter_ShouldMoveAndRotateCorrectly()
        {
            var fighter = new Fighter();
            fighter.Rotate(90);
            fighter.MoveForward();
            Assert.Equal(0, fighter.X);
            Assert.Equal(-100, fighter.Y);
            fighter.Rotate(-180);
            fighter.MoveForward();
            Assert.Equal(0, fighter.X);
            Assert.Equal(0, fighter.Y);
        }
        [Fact]
        public void Cruiser_ShouldMoveAndRotateCorrectly()
        {
            var cruiser = new Cruiser();
            cruiser.Rotate(-90);
            cruiser.MoveForward();

            Assert.Equal(0, cruiser.X);
            Assert.Equal(50, cruiser.Y);
        }
    }

}

