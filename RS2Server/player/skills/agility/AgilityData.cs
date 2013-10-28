namespace RS2.Server.player.skills.agility
{
    internal class AgilityData
    {
        public AgilityData()
        {
        }

        protected static int AGILITY_ARENA_PRICE = 50000;

        protected static object[][] GNOME_COURSE = {
		    //obstacle, x, y, x to be stood, y to be stood, render emote, emote, xp
		    new object[] {2295, 2474, 3435, 2474, 3436, 155, -1, 7.5}, // log
		    new object[] {2285, -1, -1, -1, -1, -1, 158, 7.5}, // net
		    new object[] {35970, 2473, 3422, 2473, 3423, -1, 158, 5.0}, // tree
		    new object[] {2312, 2478, 3420, 2477, 3420, 155, -1, 7.5}, // rope
		    new object[] {2314, 2486, 3419, 2485, 3419, -1, 158, 5.0}, // branch #2
		    new object[] {2286, -1, -1, -1, -1, -1, 158, 7.5}, // net #2
		    new object[] {4058, 2487, 3431, -1, -1, -1, 10580, 7.5}, // right obstacle pipe
		    new object[] {154, 2487, 3431, -1, -1, -1, 10580, 7.5} // left obstacle pipe
	    };

        protected static object[][] BARBARIAN_COURSE = {
		    //obstacle, x, y, x to be stood, y to be stood, render emote, emote, xp
		    new object[] {20210, 2552, 3559, -1, -1, -1, -1, 0.0}, // entrance tunnel
		    new object[] {2282, 2551, 3550, -1, -1, -1, -1, 22.0}, // swing
		    new object[] {2294, 2550, -1, -1, -1, 155, -1, 13.7}, // log
		    new object[] {20211, 2538, -1, -1, -1, -1, -1, 8.2}, // net
		    new object[] {2302, 2535, 3547, 2536, 3547, -1, -1, 22.0}, // balance plank
		    new object[] {1948, -1, -1, -1, -1, -1, -1, 13.7} // Crumbling wall
	    };

        protected static object[][] WILDERNESS_COURSE = {
		            //[obstacle],[x],[y],[x to be stood],[y to be stood],[render emote],[emote],[xp]
		    new object[] {2309, 2998, 3916, -1, -1, -1, -1, 0.0}, // entrance log
		    new object[] {2288, 3004, 3939, -1, -1, -1, -1, 12.5}, // tunnel
		    new object[] {2283, 3005, 3952, -1, -1, -1, -1, 20.0}, // swing
		    new object[] {37704, 3001, 3960, -1, -1, -1, -1, 20.0}, // stepping stone
		    new object[] {2297, 3001, -1, -1, -1, 155, -1, 20.0}, // log
		    new object[] {2328, -1, -1, -1, -1, 155, -1, 20.0} // rocks
	    };

        protected static object[][] APE_ATOLL_COURSE = {
		    //obstacle, x, y, x to be stood, y to be stood, render emote, emote, xp
	    };

        protected static int[][] AGILITY_ARENA_PILLARS = {
		    // id, x, y
		    new int[] {3608, 2805, 9579},
		    new int[] {3608, 2805, 9568},
		    new int[] {3608, 2805, 9557},
		    new int[] {3608, 2805, 9546},
		    new int[] {3608, 2794, 9546},
		    new int[] {3581, 2794, 9557},
		    new int[] {3581, 2794, 9568},
		    new int[] {3581, 2794, 9579},
		    new int[] {3608, 2794, 9590},
		    new int[] {3608, 2783, 9590},
		    new int[] {3608, 2783, 9579},
		    new int[] {3581, 2783, 9568},
		    new int[] {3581, 2783, 9557},
		    new int[] {3608, 2783, 9546},
		    new int[] {3608, 2772, 9546},
		    new int[] {3608, 2772, 9557},
		    new int[] {3581, 2772, 9568},
		    new int[] {3608, 2772, 9579},
		    new int[] {3608, 2772, 9590},
		    new int[] {3608, 2761, 9590},
		    new int[] {3608, 2761, 9579},
		    new int[] {3608, 2761, 9568},
		    new int[] {3608, 2761, 9557},
		    new int[] {3608, 2761, 9546}
	    };

        protected static object[][] AGILITY_ARENA_OBJECTS = {
		    // id, x, y, xp
		    new object[] {3572, 2802, 9591, 6.0}, // 3 planks, northern (east side)
		    new object[] {3572, 2802, 9590, 6.0}, // 3 planks, middle (east side)
		    new object[] {3572, 2802, 9589, 6.0}, // 3 planks, southern (east side)
		    new object[] {3572, 2797, 9591, 6.0}, // 3 planks, northern (west side)
		    new object[] {3572, 2797, 9590, 6.0}, // 3 planks, middle (west side)
		    new object[] {3571, 2797, 9589, 6.0}, // 3 planks, southern (west side)
		    new object[] {3583, 2792, 9592, 22.0}, // Handholds east of planks.
            new object[] {3583, 2785, 9592, 22.0} //Handholds west of planks.
	    };
    }
}