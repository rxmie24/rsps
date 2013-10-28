using RS2.Server.model;

namespace RS2.Server.player
{
    internal class AppearanceUpdateFlags
    {
        private Player player;
        private Location lastRegion;
        private bool appearanceUpdateRequired;
        private bool forceMovementRequired;
        private bool chatTextUpdateRequired;
        private bool animationUpdateRequired;
        private bool graphicsUpdateRequired;
        private bool hitUpdateRequired;
        private bool hit2UpdateRequired;
        private bool entityFocusUpdateRequired;
        private bool forceTextUpdateRequired;
        private bool faceLocationUpdateRequired;
        private bool teleportUpdateRequired;
        private bool mapRegionChangeUpdateRequired;

        private bool clearable = false;

        public AppearanceUpdateFlags(Player player)
        {
            this.player = player;
            appearanceUpdateRequired = true;
            teleportUpdateRequired = true;
            forceMovementRequired = false;
            chatTextUpdateRequired = false;
            animationUpdateRequired = false;
            graphicsUpdateRequired = false;
            hitUpdateRequired = false;
            hit2UpdateRequired = false;
            entityFocusUpdateRequired = false;
            forceTextUpdateRequired = false;
            faceLocationUpdateRequired = false;
            mapRegionChangeUpdateRequired = false;
        }

        public bool isClearable()
        {
            return clearable;
        }

        public void setClearable(bool clearable)
        {
            this.clearable = clearable;
        }

        public Location getLastRegion()
        {
            return lastRegion;
        }

        public void setLastRegion(Location lastRegion)
        {
            this.lastRegion = lastRegion;
        }

        public bool isUpdateRequired()
        {
            return appearanceUpdateRequired || teleportUpdateRequired || chatTextUpdateRequired ||
                    animationUpdateRequired || graphicsUpdateRequired || hitUpdateRequired ||
                    hit2UpdateRequired || entityFocusUpdateRequired || forceTextUpdateRequired ||
                    faceLocationUpdateRequired || forceMovementRequired;
        }

        public bool hasMovementUpdate()
        {
            return player.getSprites().getPrimarySprite() != -1 ||
                   (player.getSprites().getPrimarySprite() != -1 && player.getSprites().getSecondarySprite() != -1);
        }

        public bool hasAnyUpdate()
        {
            return isUpdateRequired() || hasMovementUpdate();
        }

        public void clear()
        {
            appearanceUpdateRequired = false;
            teleportUpdateRequired = false; //not a isUpdateRequired because should do a update regardless if you are teleporting or not.
            mapRegionChangeUpdateRequired = false; //not a isUpdateRequired because mapRegion has to be accurately set when doing a update, or messes up).
            chatTextUpdateRequired = false;
            animationUpdateRequired = false;
            graphicsUpdateRequired = false;
            hitUpdateRequired = false;
            hit2UpdateRequired = false;
            entityFocusUpdateRequired = false;
            forceTextUpdateRequired = false;
            faceLocationUpdateRequired = false;
            forceMovementRequired = false;
            player.getSprites().setSprites(-1, -1);
            player.removeTemporaryAttribute("forceText");
        }

        public bool isAppearanceUpdateRequired()
        {
            return appearanceUpdateRequired;
        }

        public bool isGraphicsUpdateRequired()
        {
            return graphicsUpdateRequired;
        }

        public void setGraphicsUpdateRequired(bool b)
        {
            this.graphicsUpdateRequired = b;
        }

        public bool isTeleporting()
        {
            return this.teleportUpdateRequired;
        }

        public bool didMapRegionChange()
        {
            return mapRegionChangeUpdateRequired;
        }

        public void setDidMapRegionChange(bool didMapRegionChange)
        {
            this.mapRegionChangeUpdateRequired = didMapRegionChange;
        }

        public void setTeleporting(bool didTeleport)
        {
            this.teleportUpdateRequired = didTeleport;
        }

        public void setAppearanceUpdateRequired(bool b)
        {
            appearanceUpdateRequired = b;
        }

        public void setChatTextUpdateRequired(bool b)
        {
            chatTextUpdateRequired = b;
        }

        public bool isChatTextUpdateRequired()
        {
            return chatTextUpdateRequired;
        }

        public void setAnimationUpdateRequired(bool b)
        {
            this.animationUpdateRequired = b;
        }

        public bool isAnimationUpdateRequired()
        {
            return this.animationUpdateRequired;
        }

        public void setHitUpdateRequired(bool b)
        {
            this.hitUpdateRequired = b;
        }

        public bool isHitUpdateRequired()
        {
            return this.hitUpdateRequired;
        }

        public void setHit2UpdateRequired(bool b)
        {
            this.hit2UpdateRequired = b;
        }

        public bool isHit2UpdateRequired()
        {
            return this.hit2UpdateRequired;
        }

        public void setEntityFocusUpdateRequired(bool b)
        {
            this.entityFocusUpdateRequired = b;
        }

        public bool isEntityFocusUpdateRequired()
        {
            return entityFocusUpdateRequired;
        }

        public void setForceTextUpdateRequired(bool b)
        {
            this.forceTextUpdateRequired = b;
        }

        public bool isForceTextUpdateRequired()
        {
            return forceTextUpdateRequired;
        }

        public bool isFaceLocationUpdateRequired()
        {
            return faceLocationUpdateRequired;
        }

        public void setFaceLocationUpdateRequired(bool b)
        {
            this.faceLocationUpdateRequired = b;
        }

        public bool isForceMovementRequired()
        {
            return forceMovementRequired;
        }

        public void setForceMovementRequired(bool b)
        {
            this.forceMovementRequired = b;
        }
    }
}