using System;

namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed { get; } = 50;
        public int FirePower { get; } = 100;
        public int BatteryCapacity { get; private set; } = 100; // емкость батареи ионной пушки
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;
        public int CurrentAngle { get; private set; } = 0; // текущий угол


        public void MoveForward()
        {
            if (CurrentAngle == 0) { X += Speed; }
            else if (CurrentAngle == 90) { Y -= Speed; }
            else if (CurrentAngle == 180) { X -= Speed; }
            else if (CurrentAngle == 270) { Y += Speed; }
        }
        public void Fire()
        {
            if (BatteryCapacity >= 25)
            {
                BatteryCapacity -= 25;
            }
        }
        public void Rotate(int angle)
        {
            ;
            CurrentAngle = (CurrentAngle + angle) % 360;
            if (CurrentAngle < 0) { CurrentAngle += 360; }
        }
    }
}

