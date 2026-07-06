using System;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed { get; } = 100;
        public int FirePower { get; } = 30;
        public int BatteryCapacity { get; private set; } = 100;
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;
        public int CurrentAngle { get; private set; } = 0;


        public void MoveForward()
        {
            if (CurrentAngle == 0) { X += Speed; }
            else if (CurrentAngle == 90) { Y -= Speed; }
            else if (CurrentAngle == 180) { X -= Speed; }
            else if (CurrentAngle == 270) { Y += Speed; }
        }
        public void Fire()
        {
            if (BatteryCapacity >= 10)
            {
                BatteryCapacity -= 10;
            }
        }
        public void Rotate(int angle)
        {
            CurrentAngle = (CurrentAngle + angle) % 360;
            if (CurrentAngle < 0) { CurrentAngle += 360; }
        }
    }
}

