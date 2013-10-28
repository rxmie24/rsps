using RS2.Server.definitions;
using System;

namespace RS2.Server.model
{
    internal class Appearance
    {
        private bool invisible = false;
        private Appearance temporaryAppearance; // Used when designing character.
        private int walkAnimation;
        private bool asNpc = false; //player appears as npc. [staff command]
        private int npcId = -1; //player appears as npc. [staff command]
        private int gender = 0;
        private int[] look = new int[7];
        private int[] colour = new int[5];

        public Appearance()
        {
            look[1] = 10;
            look[2] = 18;
            look[3] = 26;
            look[4] = 33;
            look[5] = 36;
            look[6] = 42;
            walkAnimation = -1;
            for (int i = 0; i < 5; i++)
            {
                colour[i] = i * 3 + 2;
            }
        }

        public void setGender(int gender)
        {
            this.gender = gender;
        }

        public void setLook(int index, int look)
        {
            this.look[index] = look;
        }

        public void setColour(int index, int colour)
        {
            this.colour[index] = colour;
        }

        public bool isNpc()
        {
            return asNpc;
        }

        public void setNpcId(int npcId)
        {
            this.npcId = npcId;
            asNpc = npcId != -1;
        }

        public int getNpcId()
        {
            return npcId;
        }

        public int getGender()
        {
            return gender;
        }

        public int getLook(ItemData.EQUIP id)
        { //TODO move EQUIP to model folder -.-
            return look[Convert.ToInt32(id)];
        }

        public int getLook(int id)
        {
            return look[id];
        }

        public int getColour(int id)
        {
            return colour[id];
        }

        public int[] getColoursArray()
        {
            return colour;
        }

        public int[] getLookArray()
        {
            return look;
        }

        public void setTemporaryAppearance(Appearance temporaryAppearance)
        {
            this.temporaryAppearance = temporaryAppearance;
        }

        public Appearance getTemporaryAppearance()
        {
            return temporaryAppearance;
        }

        public void setColoursArray(int[] colours)
        {
            this.colour = colours;
        }

        public void setLookArray(int[] look)
        {
            this.look = look;
        }

        public void setWalkAnimation(int walkAnimation)
        {
            this.walkAnimation = walkAnimation;
        }

        public int getWalkAnimation()
        {
            return walkAnimation;
        }

        public void setInvisible(bool invisible)
        {
            this.invisible = invisible;
        }

        public bool isInvisible()
        {
            return invisible;
        }
    }
}