using RS2.Server.npc;
using RS2.Server.player;
using RS2.Server.util;

namespace RS2.Server.model
{
    internal class Follow
    {
        private Entity entity;
        private Entity follower;
        private int sameCoordWait;

        public Follow(Entity e)
        {
            this.entity = e;
        }

        public void followEntity()
        {
            if (follower == null || entity.isDead() || follower.isDead() || follower.isDestroyed())
            {
                follower = null;
                return;
            }

            if (follower.getLocation().withinDistance(entity.getLocation(), 1))
                return;

            if (follower is Player)
            {
                if (follower.getEntityFocus() == -1 || follower.getEntityFocus() != entity.getClientIndex())
                {
                    entity.setEntityFocus(follower.getClientIndex());
                }
                if (!follower.getLocation().withinDistance(entity.getLocation(), 15))
                {
                    setFollowing(null);
                    return;
                }
            }

            //TODO: Starting from here organize all this crap properly.

            bool sameCoords = false;
            int x = entity.getLocation().getX();
            int y = entity.getLocation().getY();
            int targetX = follower.getLocation().getX();
            int targetY = follower.getLocation().getY();
            int newX = x;
            int newY = y;
            if (x == targetX && y == targetY)
            {
                sameCoordWait++;
                if (sameCoordWait < 2)
                {
                    return;
                }
                if (sameCoordWait == 2)
                {
                    if (Misc.random(3) == 0)
                    {
                        newY--;
                    }
                    else if (Misc.random(3) == 1)
                    {
                        newY++;
                    }
                    else if (Misc.random(3) == 2)
                    {
                        newX--;
                    }
                    else if (Misc.random(3) == 3)
                    {
                        newX++;
                    }
                    sameCoords = true;
                    sameCoordWait = 0;
                }
            }

            if (!sameCoords)
            {
                if (targetX > x && targetY == y)
                {
                    newX++;
                }
                else
                    if (targetX < x && targetY == y)
                    {
                        newX--;
                    }
                    else
                        if (targetX == x && targetY > y)
                        {
                            newY++;
                        }
                        else
                            if (targetX == x && targetY < y)
                            {
                                newY--;
                            }
                            else
                                if (targetX > x && targetY > y)
                                {
                                    newX++;
                                    newY++;
                                }
                                else
                                    if (targetX < x && targetY < y)
                                    {
                                        newX--;
                                        newY--;
                                    }
                                    else
                                        if (targetX > x && targetY < y)
                                        {
                                            newX++;
                                            newY--;
                                        }
                                        else
                                            if (targetX < x && targetY > y)
                                            {
                                                newX--;
                                                newY++;
                                            }
            }
            //if (entity.getLocation().withinDistance(follower.getLocation(),  Combat.npcUsesRange((Npc)entity) ? 30 : Combat.getNPCSize(entity, follower))) {
            //	return;
            //}
            if (entity is Npc)
            {
                Location newLoc = new Location(newX, newY, entity.getLocation().getZ());
                if (!newLoc.inArea(((Npc)entity).getMinimumCoords().getX(), ((Npc)entity).getMinimumCoords().getY(), ((Npc)entity).getMaximumCoords().getX(), ((Npc)entity).getMaximumCoords().getY()))
                {
                    follower = null;
                    return;
                }
                if (!sameCoords && newX == targetX && newY == targetY)
                {
                    return;
                }
            }
            if (follower.getEntityFocus() == -1 || follower.getEntityFocus() != entity.getClientIndex())
            {
                entity.setEntityFocus(follower.getClientIndex());
            }
            /*
		    if (!entity.getLocation().withinDistance(follower.getLocation())) {
			    if (((Player)follower).getSummonedNPC() == npc) {
				    npc.setLastAnimation(new Animation(8298);
				    npc.setLastGraphics(new Graphics(NPCSizes.getNpcSize(npc.getId()) > 0 ? 1315 : 1314);
				    npc.setLocation(new Location(targetX, targetY, following.getLocation().getZ()));
				    npc.setEntityFocus(following.getClientIndex());
				    return;
			    }
		    }*/
            if (entity is Npc)
            { //npc following a player.
                ((Npc)entity).getSprites().setSprites(Misc.direction(x, y, newX, newY), -1);
                if (((Npc)entity).getSprites().getPrimarySprite() != -1)
                {
                    int sprite = ((Npc)entity).getSprites().getPrimarySprite() >> 1;
                    ((Npc)entity).getSprites().setSprites(sprite, -1);
                    ((Npc)entity).setLocation(new Location(newX, newY, ((Npc)entity).getLocation().getZ()));
                }
            }
            else
            { //player following a player
                int diffX = targetX - x;
                int diffY = targetY - y;

                //if(Math.Max(Math.Abs(diffX), Math.Abs(diffY)) <= 1) return; //1 step away or on top of player, no need to follow

                //Try to fix this up to make it look more natural. Because just using the else statement looks ugly.
                //TODO: Must add sprite direction detection for it to know which way the person is standing so it would be behind him.
                if (diffX == 0 && diffY == -2)
                { //go down
                    diffY = -1;
                }
                else if (diffX == 0 && diffY == 2)
                { //go up
                    diffY = 1;
                }
                else if (diffX == -2 && diffY == 0)
                { //go left
                    diffX = -1;
                }
                else if (diffX == 2 && diffY == 0)
                { //go right
                    diffX = 1;
                }
                else if ((diffX == 2 && diffY == -2) || (diffX == 2 && diffY == -1) || (diffX == 1 && diffY == -2))
                { //left down, down 1 left 2 or down 2 right 1
                    diffX = 1;
                    diffY = -1;
                }
                else if ((diffX == -2 && diffY == 2) || (diffX == -2 && diffY == 1) || (diffX == -1 && diffY == 2))
                { //left up, up 1 left 2 or up 2 left 1
                    diffX = -1;
                    diffY = 1;
                }
                else if ((diffX == 2 && diffY == 2) || (diffX == 2 && diffY == 1) || (diffX == 1 && diffY == 2))
                { //up right, up 1 right 2, up 2 right 1
                    diffX = 1;
                    diffY = 1;
                }
                else if ((diffX == -2 && diffY == -2) || (diffX == -1 && diffY == -2) || (diffX == -2 && diffY == -1))
                { //down left, down 2 left 1 or down 1 right 2
                    diffX = -1;
                    diffY = -1;
                }
                else
                {
                    if (diffX < 0)
                        diffX++;
                    else if (diffX > 0)
                        diffX--;
                    if (diffY < 0)
                        diffY++;
                    else if (diffY > 0)
                        diffY--;
                }

                ((Player)entity).getWalkingQueue().forceWalk(diffX, diffY);
            }
        }

        public Entity getFollowing()
        {
            return follower;
        }

        public void setFollowing(Entity following)
        {
            this.follower = following;
        }
    }
}