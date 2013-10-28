namespace RS2.Server.model
{
    /**
     * Holds data for a single animation request.
     */

    internal class Animation
    {
        private int id, delay;

        public Animation(int id)
        {
            this.id = id;
            delay = 0;
        }

        public Animation(int id, int delay)
        {
            this.id = id;
            this.delay = delay;
        }

        public int getId()
        {
            return id;
        }

        public int getDelay()
        {
            return delay;
        }
    }
}