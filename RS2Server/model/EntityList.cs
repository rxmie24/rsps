using System.Collections.Generic;
using System.Linq;

namespace RS2.Server.model
{
    internal class EntityList<T> : ICollection<T> where T : Entity
    {
        private const int DEFAULT_CAPACITY = 1600, MIN_VALUE = 1;
        private T[] entities;
        private HashSet<int> indicies = new HashSet<int>();
        private int curIndex = MIN_VALUE;
        private int capacity;

        public EntityList()
            : this(DEFAULT_CAPACITY) { }

        public EntityList(int capacity)
        {
            this.entities = new T[capacity];
            this.capacity = capacity;
        }

        public EntityList(IEnumerable<T> collection)
        {
            foreach (T entity in collection)
                Add(entity);
        }

        public T this[int index]
        {
            get
            {
                return entities[index];
            }
            set
            {
                entities[index] = value;
            }
        }

        public void Add(T entity, int index)
        {
            if (entities[curIndex] != null)
            {
                increaseIndex();
                Add(entity, curIndex);
            }
            else
            {
                entities[curIndex] = entity;
                entity.setIndex(index);
                indicies.Add(curIndex);
                increaseIndex();
            }
        }

        public void Add(T entity)
        {
            Add(entity, curIndex);
        }

        public bool Remove(T entity)
        {
            entities[entity.getIndex()] = null;
            indicies.Remove(entity.getIndex());
            decreaseIndex();
            return true;
        }

        public T Remove(int index)
        {
            T temp = entities[index];
            entities[index] = null;
            indicies.Remove(index);
            decreaseIndex();
            return temp;
        }

        public void Clear()
        {
            foreach (T entity in entities)
                Remove(entity);
        }

        public bool Contains(T item)
        {
            return this.entities.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T entity in array)
                Add(entity, arrayIndex);
        }

        public int IndexOf(T entity)
        {
            foreach (int index in indicies)
            {
                if (entities[index].Equals(entity))
                {
                    return index;
                }
            }
            return -1;
        }

        public int Count
        {
            get { return indicies.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new EntityListEnumerator<T>(entities, indicies, this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }

        private void increaseIndex()
        {
            curIndex++;
            if (curIndex >= capacity)
            {
                curIndex = MIN_VALUE;
            }
        }

        private void decreaseIndex()
        {
            curIndex--;
            if (curIndex <= capacity)
                curIndex = MIN_VALUE;
        }
    }
}