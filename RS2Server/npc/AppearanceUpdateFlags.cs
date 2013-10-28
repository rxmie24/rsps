namespace RS2.Server.npc
{
    internal class AppearanceUpdateFlags
    {
        private bool animationUpdateRequired;
        private bool entityFocusUpdateRequired;
        private bool forceTextUpdateRequired;
        private bool graphicsUpdateRequired;
        private bool hitUpdateRequired;
        private bool hit2UpdateRequired;
        private bool faceLocationUpdateRequired;
        private Npc npc;

        public AppearanceUpdateFlags(Npc npc)
        {
            this.npc = npc;
            animationUpdateRequired = false;
            entityFocusUpdateRequired = false;
            forceTextUpdateRequired = false;
            graphicsUpdateRequired = false;
            hitUpdateRequired = false;
            hit2UpdateRequired = false;
            faceLocationUpdateRequired = false;
        }

        public bool isUpdateRequired()
        {
            return animationUpdateRequired || entityFocusUpdateRequired || forceTextUpdateRequired ||
                   graphicsUpdateRequired || hitUpdateRequired || hit2UpdateRequired ||
                   faceLocationUpdateRequired || npc.getSprites().getPrimarySprite() != -1 || npc.getSprites().getSecondarySprite() != -1;
        }

        public bool isFaceLocationUpdateRequired()
        {
            return faceLocationUpdateRequired;
        }

        public void setFaceLocationUpdateRequired(bool faceLocationUpdateRequired)
        {
            this.faceLocationUpdateRequired = faceLocationUpdateRequired;
        }

        public bool isHitUpdateRequired()
        {
            return hitUpdateRequired;
        }

        public void setHitUpdateRequired(bool hitUpdateRequired)
        {
            this.hitUpdateRequired = hitUpdateRequired;
        }

        public bool isHit2UpdateRequired()
        {
            return hit2UpdateRequired;
        }

        public void setHit2UpdateRequired(bool hit2UpdateRequired)
        {
            this.hit2UpdateRequired = hit2UpdateRequired;
        }

        public void setAnimationUpdateRequired(bool animationUpdateRequired)
        {
            this.animationUpdateRequired = animationUpdateRequired;
        }

        public bool isGraphicsUpdateRequired()
        {
            return graphicsUpdateRequired;
        }

        public void setGraphicsUpdateRequired(bool graphicsUpdateRequired)
        {
            this.graphicsUpdateRequired = graphicsUpdateRequired;
        }

        public bool isForceTextUpdateRequired()
        {
            return forceTextUpdateRequired;
        }

        public void setForceTextUpdateRequired(bool forceTextUpdateRequired)
        {
            this.forceTextUpdateRequired = forceTextUpdateRequired;
        }

        public bool isAnimationUpdateRequired()
        {
            return animationUpdateRequired;
        }

        public void clear()
        {
            animationUpdateRequired = false;
            entityFocusUpdateRequired = false;
            forceTextUpdateRequired = false;
            graphicsUpdateRequired = false;
            hitUpdateRequired = false;
            hit2UpdateRequired = false;
            faceLocationUpdateRequired = false;
            npc.removeTemporaryAttribute("forceText");
        }

        public bool isEntityFocusUpdateRequired()
        {
            return entityFocusUpdateRequired;
        }

        public void setEntityFocusUpdateRequired(bool entityFocusUpdateRequired)
        {
            this.entityFocusUpdateRequired = entityFocusUpdateRequired;
        }
    }
}