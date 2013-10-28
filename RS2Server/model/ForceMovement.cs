namespace RS2.Server.model
{
    internal class ForceMovement
    {
        private int speed1;
        private int speed2;
        private int direction;
        private int x1;
        private int y1;
        private int x2;
        private int y2;

        public ForceMovement(int x1, int y1, int x2, int y2, int speed1, int speed2, int dir)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.speed1 = speed1;
            this.speed2 = speed2;
            this.direction = dir;
        }

        public int getX1()
        {
            return x1;
        }

        public int getY1()
        {
            return y1;
        }

        public int getX2()
        {
            return x2;
        }

        public int getY2()
        {
            return y2;
        }

        public int getSpeed1()
        {
            return speed1;
        }

        public int getSpeed2()
        {
            return speed2;
        }

        public int getDirection()
        {
            return direction;
        }
    }
}