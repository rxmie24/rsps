using System;

namespace RS2.Server.events
{
    internal class Event
    {
        /**
         * How many ms the event should wait between each cycle.
         */
        private long tick;

        /**
         * How long it was since the event was last run.
         */
        private long lastRun;

        /**
         * If the event has been stopped.
         */
        private bool stopped;

        /**
         * The action
         */
        private Action action;

        /**
	     * Create an event with the specified tick time.
	     * @param tick
	     */

        public Event(long tick)
        {
            this.tick = tick;
            this.lastRun = Environment.TickCount;
        }

        /**
         * Gets the current tick.
         * @return
         */

        public long getTick()
        {
            return tick;
        }

        /**
         * Sets the event tick.
         * @param tick
         */

        public void setTick(int tick)
        {
            this.tick = tick;
        }

        /**
         * Checks if the event is ready to execute.
         * @return
         */

        public bool isReady()
        {
            if (stopped)
            {
                return false;
            }
            return (Environment.TickCount - lastRun) > tick;
        }

        /**
         * Checks if the event has been stopped.
         * @return
         */

        public bool isStopped()
        {
            return stopped;
        }

        /**
         * Stops the current event.
         */

        public void stop()
        {
            stopped = true;
        }

        /**
         * Runs the event.
         */

        public void run()
        {
            this.lastRun = Environment.TickCount;
            runAction();
        }

        public virtual void runAction() //<- this detour is so it can be easily overwritten by other events.
        {
            action();
        }

        public void setAction(Action action)
        {
            this.action = action;
        }
    }
}