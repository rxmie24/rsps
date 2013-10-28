using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunescapeServer.definitions
{
    class BarrowsData
    {
        public BarrowsData() {
	    }
	
	    protected static int BARROWS_CHANCE = 5;
	
	    /**
	     * The array indexes of the relevant barrow brother/crypt.
	     */
	    protected static int VERAC = 0;
	    protected static int DHAROK = 1;
	    protected static int AHRIM = 2;
	    protected static int GUTHAN = 3;
	    protected static int KARIL = 4;
	    protected static int TORAG = 5;
	
	    /**
	     * The 'inArea' Coords of the mounds, x (SW), y (SW), x2 (NE), y2 (NE) (to form a square).
	     */
	    protected static int[][] MOUND_COORDS = {
		    new int[] {3553, 3294, 3558, 3300}, // Verac.
		    new int[] {3573, 3296, 3577, 3300}, // Dharok.
		    new int[] {3563, 3288, 3567, 3291}, // Ahrim.
		    new int[] {3576, 3280, 3579, 3284}, // Guthan.
		    new int[] {3564, 3274, 3567, 3277}, // Karil.
		    new int[] {3552, 3281, 3555, 3284}  // Torag.
	    };
	
	    /**
	     * The ID of the Barrow brother, to thier relevant cryptIndex.
	     */
	    protected static int[] BROTHER_ID = {
		    2030, 2026, 2025, 2027, 2028, 2029
	    };
	
	    /**
	     * Array of the barrow heads
	     * Purple
	     * Purple & green
	     */
	    protected static int[][] HEADS = {
		    new int[] {4761, 4763, 4765, 4767, 4769, 4771},
		    new int[] {4761, 4762, 4763, 4764, 4765, 4766, 4767, 4768, 4769, 4770, 4771}
	    };
	
	    /**
	     * The X and Y you must be stood on to use the crypt stairs, 
	     * also the coords you will be teleport to upon entering a crypt.
	     */
	    protected static int[][] STAIR_COORDS = {
		    new int[] {3578, 9706}, // Verac.
		    new int[] {3556, 9718}, // Dharok.
		    new int[] {3557, 9703}, // Ahrim.
		    new int[] {3534, 9704}, // Guthan.
		    new int[] {3546, 9684}, // Karil.
		    new int[] {3568, 9683}  // Torag.
	    };
	
	    /**
	     * The area the player must be stood in to open the coffin.
	     */
	    protected static int[][] COFFIN_AREA = {
		    new int[] {3572, 9704, 3575, 9708}, // Verac.
		    new int[] {3553, 9713, 3557, 9716}, // Dharok.
		    new int[] {3554, 9697, 3557, 9701}, // Ahrim.
		    new int[] {3537, 9702, 3541, 9705}, // Guthan.
		    new int[] {3549, 9681, 3552, 9685}, // Karil.
		    new int[] {3568, 9684, 3571, 9688}  // Torag.
	    };
	
	    /**
	     * Tunnel door Id's.
	     */
	    protected static int[] DOORS = {
		    6747, 6741, 6735, 6739, 6746, 6745, 6737, 6735,
		    6728, 6722, 6716, 6720, 6727, 6726, 6718, 6716,
		    6731, 6728, 6722, 6720, 6727, 6731, 6726, 6718,
		    6750, 6747, 6741, 6739, 6746, 6750, 6745, 6737,
		    6742, 6749, 6748, 6743, 6744, 6740, 6742, 6738,
		    6723, 6730, 6729, 6724, 6725, 6723, 6721, 6719,
		    6749, 6748, 6736, 6743, 6744, 6740, 6738, 6736,
		    6730, 6729, 6717, 6724, 6725, 6721, 6719, 6717,
	    };
	
	    /**
	     * Tunnel door locations (X&Y), their index is the relevant to the door id in DOORS[].
	     */
	    protected static int[][] DOOR_LOCATION = { 
		    new int[] {3569, 9684}, new int[] {3569, 9701}, new int[] {3569, 9718}, new int[] {3552, 9701}, 
            new int[] {3552, 9684}, new int[] {3535, 9684}, new int[] {3535, 9701}, new int[] {3535, 9718},
            new int[] {3568, 9684}, new int[] {3568, 9701}, new int[] {3568, 9718}, new int[] {3551, 9701}, 
            new int[] {3551, 9684}, new int[] {3534, 9684}, new int[] {3534, 9701}, new int[] {3534, 9718}, 
            new int[] {3569, 9671}, new int[] {3569, 9688}, new int[] {3569, 9705}, new int[] {3552, 9705}, 
		    new int[] {3552, 9688}, new int[] {3535, 9671}, new int[] {3535, 9688}, new int[] {3535, 9705}, 
            new int[] {3568, 9671}, new int[] {3568, 9688}, new int[] {3568, 9705}, new int[] {3551, 9705}, 
            new int[] {3551, 9688}, new int[] {3534, 9671}, new int[] {3534, 9688}, new int[] {3534, 9705}, 
            new int[] {3575, 9677}, new int[] {3558, 9677}, new int[] {3541, 9677}, new int[] {3541, 9694}, 
            new int[] {3558, 9694}, new int[] {3558, 9711}, new int[] {3575, 9711}, new int[] {3541, 9711}, 
		    new int[] {3575, 9678}, new int[] {3558, 9678}, new int[] {3541, 9678}, new int[] {3541, 9695}, 
            new int[] {3558, 9695}, new int[] {3575, 9712}, new int[] {3558, 9712}, new int[] {3541, 9712}, 
            new int[] {3562, 9678}, new int[] {3545, 9678}, new int[] {3528, 9678}, new int[] {3545, 9695}, 
            new int[] {3562, 9695}, new int[] {3562, 9712}, new int[] {3545, 9712}, new int[] {3528, 9712}, 
            new int[] {3562, 9677}, new int[] {3545, 9677}, new int[] {3528, 9677}, new int[] {3545, 9694}, 
		    new int[] {3562, 9694}, new int[] {3562, 9711}, new int[] {3545, 9711}, new int[] {3528, 9711}
	    };
	
	    /**
	     * The direction the door will face when opening
	     * Door id, face, distance from old doors
	     * (distance from doors = 1 = x+1, 2 = y+1, 3 = x-1, 4 = y-1)
	     */
	    protected static int[][] DOOR_OPEN_DIRECTION = { 
		    new int[] {6732, 2, 4}, new int[] {6732, 2, 4}, new int[] {6732, 2, 4}, new int[] {6732, 2, 4}, 
            new int[] {6732, 2, 4}, new int[] {6732, 2, 4}, new int[] {6732, 2, 4}, new int[] {6732, 2, 4},
		    new int[] {6713, 0, 4}, new int[] {6713, 0, 4}, new int[] {6713, 0, 4}, new int[] {6713, 0, 4}, 
            new int[] {6713, 0, 4}, new int[] {6713, 0, 4}, new int[] {6713, 0, 4}, new int[] {6713, 0, 4},
		    new int[] {6713, 2, 2}, new int[] {6713, 2, 2}, new int[] {6713, 2, 2}, new int[] {6713, 2, 2}, 
            new int[] {6713, 2, 2}, new int[] {6713, 2, 2}, new int[] {6713, 2, 2}, new int[] {6713, 2, 2},
		    new int[] {6732, 4, 2}, new int[] {6732, 4, 2}, new int[] {6732, 4, 2}, new int[] {6732, 4, 2}, 
            new int[] {6732, 4, 2}, new int[] {6732, 4, 2}, new int[] {6732, 4, 2}, new int[] {6732, 4, 2},
		    new int[] {6732, 3, 3}, new int[] {6732, 3, 3}, new int[] {6732, 3, 3}, new int[] {6732, 3, 3}, 
            new int[] {6732, 3, 3}, new int[] {6732, 3, 3}, new int[] {6732, 3, 3}, new int[] {6732, 3, 3},
		    new int[] {6713, 1, 3}, new int[] {6713, 1, 3}, new int[] {6713, 1, 3}, new int[] {6713, 1, 3}, 
            new int[] {6713, 1, 3}, new int[] {6713, 1, 3}, new int[] {6713, 1, 3}, new int[] {6713, 1, 3},
		    new int[] {6732, 1, 1}, new int[] {6732, 1, 1}, new int[] {6732, 1, 1}, new int[] {6732, 1, 1}, 
            new int[] {6732, 1, 1}, new int[] {6732, 1, 1}, new int[] {6732, 1, 1}, new int[] {6732, 1, 1},
		    new int[] {6713, 3, 1}, new int[] {6713, 3, 1}, new int[] {6713, 3, 1}, new int[] {6713, 3, 1}, 
            new int[] {6713, 3, 1}, new int[] {6713, 3, 1}, new int[] {6713, 3, 1}, new int[] {6713, 3, 1}
	    };
	
	    /**
	     * An array of coordinates for the 'mini tunnels' inbetween doors.
	     * X, Y, X2, Y2
	     */
	    protected static int[][] DB = {
		    new int[] {3532, 9665, 3570, 9671},
		    new int[] {3575, 9676, 3581, 9714},
		    new int[] {3534, 9718, 3570, 9723},
		    new int[] {3523, 9675, 3528, 9712},
		    new int[] {3541, 9711, 3545, 9712},
		    new int[] {3558, 9711, 3562, 9712},
		    new int[] {3568, 9701, 3569, 9705},
		    new int[] {3551, 9701, 3552, 9705},
		    new int[] {3534, 9701, 3535, 9705},
		    new int[] {3541, 9694, 3545, 9695},
		    new int[] {3558, 9694, 3562, 9695},
		    new int[] {3568, 9684, 3569, 9688},
		    new int[] {3551, 9684, 3552, 9688},
		    new int[] {3534, 9684, 3535, 9688},
		    new int[] {3541, 9677, 3545, 9678},
		    new int[] {3558, 9677, 3562, 9678}
	    };
	
	    /**
	     * Barrow rewards.
	     */
	    protected static int[] BARROW_REWARDS = {
		    4757, 4759, 4753, 4755, // Verac's
		    4736, 4738, 4734, 4732, // Karil's
		    4745, 4747, 4749, 4751, // Torag's
		    4708, 4710, 4712, 4714, // Ahrim's
		    4716, 4718, 4720, 4722, // Dharok's
		    4724, 4726, 4728, 4730, // Guthan's
	    };
	
	    /**
	     * Other rewards.
	     */
	    protected static int[] OTHER_REWARDS = {
		    4740, // Bolt rack.
		    995, // Money.
		    560, // Death runes.
		    565, // Blood runes.
		    562, // Chaos runes.
		    558, // Mind runes.
	    };
	
	    protected static int[][] REWARD_KILLCOUNT = {
		    new int[] {6,	15, 	25, 	38, 	75, 	143, 	373, 	563, 	838, 	1734, 	2843, 	3948, 	4733, 	5938, 	6883,	8232, 	9639},
		    new int[] {6, 	7, 		22, 	59, 	121, 	283, 	694, 	1038, 	1774, 	2533, 	3746, 	4837, 	5661, 	6880, 	7390, 	8403, 	9840},
		    new int[] {6, 	8, 		19, 	43, 	92, 	186, 	228, 	473, 	771, 	990, 	1484, 	1945, 	2566, 	3849, 	5002, 	5982, 	6760, 	7389, 	8923},
		    new int[] {6, 	20,		45,		74,		135,	201,	273,	483,	893,	1027,	1877,	2043,	2837,	3766,	4650,	5847,	7299,	8034,	8774,	9531},
		    new int[] {6, 	15,		29,		78,		129,	198,	287,	407,	694,	883,	1287,	2084,	2776,	3581,	4299,	5400,	6839,	8394,	9984},
		    new int[] {6,	17,		39,		100,	199,	401,	674,	886,	1083,	1572,	2037,	3684,	4763,	6847,	7049,	8164,	8918,	9927}
	    };
	
	    protected static int[][] REWARD_AMOUNT = {
		    new int[] {98, 	144, 	179, 	202, 	232, 	293, 	390, 	421, 	489, 	529, 	590, 	683, 	772, 	805, 	993, 	1482,	1823}, // Amounts
		    new int[] {1779,	1934, 	2844, 	4554, 	6948, 	8771, 	9028, 	11837, 	14839, 	19837,	24827, 	30485, 	35774, 	46384, 	58344, 	78374, 	108334},
		    new int[] {86,	111,	163,	187,	231,	300,	379,	402,	502,	592,	607,	699,	782,	901,	983,	1386,	1746,	2049,	2673},
		    new int[] {61,	103,	172,	200,	233,	304,	355,	442,	511,	573,	599,	661,	701,	780,	892,	990,	1254,	1532,	1763,	1994},
		    new int[] {160,	242,	277,	304,	398,	465,	503,	606,	698,	799,	872,	945,	1023,	1382,	1491,	1687,	1983,	2455,	2873},
		    new int[] {283,	473,	701,	892,	1033,	1983,	2387,	2763,	3884,	4118,	4479,	4766,	5520,	6948,	7389,	7689,	8376,	8911,	10938}
	    };
	
	    protected static int[] MINIMUM_AMOUNT = {
		    39, // Bolt rack.
		    1000, // Money.
		    51, // Death runes.
		    38, // Blood runes.
		    84, // Chaos runes.
		    139, // Mind runes.
	    };
    }
}
