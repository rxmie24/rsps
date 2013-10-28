using RS2.Server.definitions;
using System;

namespace RS2.Server.model
{
    public class Item : ICloneable
    {
        private int m_itemId; //<- fix for XML Serializater not loading itemDefinitions.

        public int itemId
        {
            get
            {
                return m_itemId;
            }
            set
            {
                m_itemId = value;
                this.itemDefinition = ItemData.forId(value);
            }
        }

        public int itemAmount;
        private ItemData.Item itemDefinition;

        public Item() //for serialization.
        {
        }

        public Item(int id, int amount)
        {
            this.itemId = id;
            this.itemAmount = amount;
            this.itemDefinition = ItemData.forId(id);
        }

        public ItemData.Item getDefinition()
        {
            return itemDefinition;
        }

        public int getItemId()
        {
            return itemId;
        }

        public void setItemId(int itemId)
        {
            this.itemId = itemId;
            this.itemDefinition = ItemData.forId(itemId);
        }

        public int getItemAmount()
        {
            return itemAmount;
        }

        public void setItemAmount(int amount)
        {
            this.itemAmount = amount;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}