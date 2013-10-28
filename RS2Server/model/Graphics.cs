namespace RS2.Server.model
{
    /**
     * Represents still graphics.
     */

    internal class Graphics
    {
        private int id, delay, height;

        public Graphics(int id)
        {
            this.id = id;
            delay = 0;
            this.height = 0;
        }

        public Graphics(int id, int delay)
        {
            this.id = id;
            this.delay = delay;
            height = 0;
        }

        public Graphics(int id, int delay, int height)
        {
            this.id = id;
            this.delay = delay;
            this.height = height;
        }

        public int getId()
        {
            return id;
        }

        public int getDelay()
        {
            return delay;
        }

        public int getHeight()
        {
            return height;
        }
    }
}