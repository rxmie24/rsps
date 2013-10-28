using RS2.Server.minigames.agilityarena;
using RS2.Server.model;

namespace RS2.Server.player.skills.agility
{
    internal class Agility : AgilityData
    {
        public Agility()
        {
        }

        public static bool doAgility(Player p, int gameObject, int x, int y)
        {
            for (int i = 0; i < GNOME_COURSE.Length; i++)
            {
                if (gameObject == (int)GNOME_COURSE[i][0])
                {
                    GnomeCourse.doCourse(p, x, y, GNOME_COURSE[i]);
                    return true;
                }
            }
            for (int i = 0; i < BARBARIAN_COURSE.Length; i++)
            {
                if (gameObject == (int)BARBARIAN_COURSE[i][0])
                {
                    BarbarianCourse.doCourse(p, x, y, BARBARIAN_COURSE[i]);
                    return true;
                }
            }
            for (int i = 0; i < WILDERNESS_COURSE.Length; i++)
            {
                if (gameObject == (int)WILDERNESS_COURSE[i][0])
                {
                    WildernessCourse.doCourse(p, x, y, WILDERNESS_COURSE[i]);
                    return true;
                }
            }
            for (int i = 0; i < APE_ATOLL_COURSE.Length; i++)
            {
                if (gameObject == (int)APE_ATOLL_COURSE[i][0])
                {
                    ApeAtollCourse.doCourse(p, x, y, APE_ATOLL_COURSE[i]);
                    return true;
                }
            }
            for (int i = 0; i < AGILITY_ARENA_PILLARS.Length; i++)
            {
                if (x == AGILITY_ARENA_PILLARS[i][1] && y == AGILITY_ARENA_PILLARS[i][2])
                {
                    if (gameObject == AGILITY_ARENA_PILLARS[i][0])
                    {
                        AgilityArena.tagPillar(p, i);
                        return true;
                    }
                }
            }
            if (Location.atAgilityArena(p.getLocation()))
            {
                for (int i = 0; i < AGILITY_ARENA_OBJECTS.Length; i++)
                {
                    if (x == (int)AGILITY_ARENA_OBJECTS[i][1] && y == (int)AGILITY_ARENA_OBJECTS[i][2])
                    {
                        if (gameObject == (int)AGILITY_ARENA_OBJECTS[i][0])
                        {
                            Obstacles.doObstacle(p, i);
                            return true;
                        }
                    }
                }
            }
            if (gameObject == 3205 && x == 2532 && y == 3545)
            {
                BarbarianCourse.useLadder(p);
                return true;
            }
            return false;
        }
    }
}