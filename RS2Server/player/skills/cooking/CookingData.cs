namespace RS2.Server.player.skills.cooking
{
    internal class CookingData
    {
        public CookingData()
        {
        }

        protected static int COOKING_GAUNTLETS = 775;
        protected static int[] MEAT_RAW = { 2132, 2134, 2136, 317, 2138, 3226, 321, 327, 3142, 345, 353, 335, 341, 349, 331, 359, 377, 363, 371, 7944, 383, 395, 389 };
        protected static int[] MEAT_COOKED = { 2142, 2142, 2142, 315, 2140, 3228, 319, 325, 3144, 347, 355, 333, 339, 351, 329, 361, 379, 365, 373, 7946, 385, 397, 391 };
        protected static int[] MEAT_BURNT = { 2146, 2146, 2146, 7954, 2144, 7222, 323, 369, 3148, 357, 357, 343, 343, 343, 343, 367, 381, 367, 375, 7948, 387, 399, 393 };
        protected static int[] MEAT_LEVEL = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 10, 15, 18, 20, 25, 30, 40, 43, 45, 62, 80, 82, 91 };
        protected static double[] MEAT_XP = { 30, 30, 30, 30, 30, 30, 30, 40, 80, 50, 60, 70, 75, 80, 90, 100, 120, 130, 140, 150, 210, 211.3, 216.3 };
    }
}