namespace RS2.Server.model
{
    internal class AttackStyle
    {
        public enum CombatSkill { ACCURATE, DEFENSIVE, AGGRESSIVE, RANGE, CONTROLLED };

        public enum CombatStyle { STAB, SLASH, CRUSH, MAGIC, RANGE_ACCURATE, RANGE_RAPID, RANGE_DEFENSIVE };

        private CombatSkill skill;
        private CombatStyle style;
        private int slot;

        public AttackStyle()
        {
            setDefault();
        }

        public void setDefault()
        {
            this.skill = CombatSkill.ACCURATE;
            this.style = CombatStyle.CRUSH;
            this.slot = 0;
        }

        public void setSkill(CombatSkill skill)
        {
            this.skill = skill;
        }

        public void setStyle(CombatStyle style)
        {
            this.style = style;
        }

        public CombatSkill getSkill()
        {
            return skill;
        }

        public CombatStyle getStyle()
        {
            return style;
        }

        public int getSlot()
        {
            return slot;
        }

        public void setSlot(int slot)
        {
            this.slot = slot;
        }
    }
}