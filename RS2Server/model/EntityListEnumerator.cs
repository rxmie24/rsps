using System.Collections;
using System.Collections.Generic;

namespace RS2.Server.model
{
    internal class EntityListEnumerator<T> : IEnumerator<T> where T : Entity
    {
        private int[] indicies;
        private T[] entities;
        private EntityList<T> entityList;

        protected int curIndex; //current index
        protected T _current; //current enumerated object in the collection

        public EntityListEnumerator(T[] entities, HashSet<int> indicies, EntityList<T> entityList)
        {
            this.entities = entities;
            this.indicies = new int[indicies.Count]; //Quick fix for not using System.Linq -> ToArray(); lol shits slow.
            indicies.CopyTo(this.indicies);
            this.entityList = entityList;
            curIndex = -1;
        }

        public T Current
        {
            get { return _current; }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public bool MoveNext()
        {
            if (++curIndex >= indicies.Length) //make sure we are within the bounds of the collection
                return false;
            else //if we are, then set the current element to the next object in the collection
                _current = entities[indicies[curIndex]];
            return true;
        }

        public void Remove()
        {
            if (curIndex >= 1)
            {
                entityList.Remove(indicies[curIndex - 1]);
            }
        }

        // Reset the enumerator
        public void Reset()
        {
            _current = default(T); //reset current object
            curIndex = -1;
        }

        // Dispose method
        public void Dispose()
        {
            entityList = null;
            _current = default(T);
            curIndex = -1;
        }
    }
}