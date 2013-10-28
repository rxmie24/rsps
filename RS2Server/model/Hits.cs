namespace RS2.Server.model
{
    internal class Hits
    {
        public enum HitType
        {
            NO_DAMAGE = 0,			// blue
            NORMAL_DAMAGE = 1,		// red
            POISON_DAMAGE = 2,		// green
            DISEASE_DAMAGE = 3	    // orange
        }

        public class Hit
        {
            private HitType type;
            private int damage;

            public Hit(int damage, HitType type)
            {
                this.type = type;
                this.damage = damage;
            }

            public HitType getType()
            {
                return type;
            }

            public int getDamage()
            {
                return damage;
            }
        }

        public Hits()
        {
            hit1 = null;
            hit2 = null;
        }

        private Hit hit1;
        private Hit hit2;

        public void setHit1(Hit hit)
        {
            this.hit1 = hit;
        }

        public void setHit2(Hit hit)
        {
            this.hit2 = hit;
        }

        public int getHitDamage1()
        {
            if (hit1 == null)
            {
                return 0;
            }
            return hit1.getDamage();
        }

        public int getHitDamage2()
        {
            if (hit2 == null)
            {
                return 0;
            }
            return hit2.getDamage();
        }

        public HitType getHitType1()
        {
            if (hit1 == null)
            {
                return HitType.NO_DAMAGE;
            }
            return hit1.getType();
        }

        public HitType getHitType2()
        {
            if (hit2 == null)
            {
                return HitType.NO_DAMAGE;
            }
            return hit2.getType();
        }

        public void clear()
        {
            hit1 = null;
            hit2 = null;
        }
    }
}