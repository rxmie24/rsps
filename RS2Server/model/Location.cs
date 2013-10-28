using System;

namespace RS2.Server.model
{
    public class Location : ICloneable
    {
        public int x, y, z;

        public Location()
        {
        } //used for xml serialization

        public Location(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public int getZ()
        {
            return z;
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public void setY(int y)
        {
            this.y = y;
        }

        public void setZ(int z)
        {
            this.z = z;
        }

        public int getLocalX()
        {
            return x - (8 * (getRegionX() - 6));
        }

        public int getLocalY()
        {
            return y - (8 * (getRegionY() - 6));
        }

        public int getLocalX(Location loc)
        {
            return x - (8 * (loc.getRegionX() - 6));
        }

        public int getLocalY(Location loc)
        {
            return y - (8 * (loc.getRegionY() - 6));
        }

        public int getRegionX()
        {
            return x >> 3;
        }

        public int getRegionY()
        {
            return y >> 3;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return z << 30 | x << 15 | y;
        }

        public override bool Equals(Object other)
        {
            if (!(other is Location))
                return false;
            Location loc = (Location)other;
            return loc.x == x && loc.y == y && loc.z == z;
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "," + z + "]";
        }

        public bool inArea(int a, int b, int c, int d)
        {
            return x >= a && y >= b && x <= c && y <= d;
        }

        public bool withinDistance(Location other, int dist)
        {
            if (other.z != z)
                return false;
            int deltaX = other.x - x, deltaY = other.y - y;
            return (deltaX <= (dist - 1) && deltaX >= -dist && deltaY <= (dist - 1) && deltaY >= -dist);
        }

        public bool withinDistance(Location other)
        {
            if (other.z != z)
                return false;
            int deltaX = other.x - x, deltaY = other.y - y;
            return deltaX <= 14 && deltaX >= -15 && deltaY <= 14 && deltaY >= -15;
        }

        public bool withinInteractionDistance(Location l)
        {
            return withinDistance(l, 3);
        }

        public int distanceToPoint(Location l)
        {
            return (int)Math.Sqrt(Math.Pow(x - l.getX(), 2) + Math.Pow(y - l.getY(), 2));
        }

        public static bool inWilderness(Location l)
        {
            return l.inArea(2945, 3524, 3391, 3975);
        }

        public int wildernessLevel()
        {
            int y = getY();
            if (!inWilderness(this))
                return -1;
            if (y > 3523 && y < 4000)
                return (((int)(Math.Ceiling((double)(y) - 3520D) / 8D) + 1));
            return -1;
        }

        public static bool atDuelArena(Location l)
        {
            return l.inArea(3318, 3247, 3327, 3247) ||
            l.inArea(3324, 3247, 3328, 3264) ||
            l.inArea(3327, 3262, 3342, 3270) ||
            l.inArea(3342, 3262, 3387, 3280) ||
            l.inArea(3387, 3262, 3394, 3271) ||
            l.inArea(3313, 3224, 3325, 3247) ||
            l.inArea(3326, 3200, 3398, 3267); // Entire arena
        }

        public static bool atBarrows(Location l)
        {
            return l.inArea(3521, 9663, 3582, 9727);
        }

        public static bool inMultiCombat(Location l)
        {
            return atGodwars(l) || inTzHaar(l) || inFightPits(l) || inFightCave(l);
        }

        public static bool inFightCave(Location l)
        {
            return l.getX() >= 19000;
        }

        public static bool atGodwars(Location l)
        {
            return l.inArea(2820, 5245, 2964, 5380);
        }

        public static bool atAgilityArena(Location l)
        {
            return l.inArea(2757, 9542, 2809, 9594);
        }

        public static bool inFightPits(Location l)
        {
            return l.inArea(2376, 5127, 2422, 5168);
        }

        public static bool inFightPitsWaitingArea(Location l)
        {
            return l.inArea(2394, 5169, 2404, 5175);
        }

        public static bool inTzHaar(Location l)
        {
            return l.inArea(2370, 5120, 2541, 5185);
        }

        public static bool onWaterbirthIsle(Location l)
        {
            return l.inArea(2494, 3710, 2564, 3786);
        }
    }
}