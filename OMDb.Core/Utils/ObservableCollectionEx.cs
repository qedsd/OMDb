using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public ObservableCollectionEx() : base()
        {

        }
        public ObservableCollectionEx(IEnumerable<T> collection) : base(collection)
        {

        }

        public ObservableCollectionEx(List<T> list) : base(list)
        {

        }


        public void ForEach(Action<T> action)
        {
            for (int index = 0; index < this.Count; ++index)
                action(this[index]);
        }

        public int IndexOf(Predicate<T> match)
        {
            var startIndex = 0;
            for (int index = startIndex; index < this.Count; ++index)
            {
                if (match(this[index]))
                {
                    return index;
                }
            }
            return -1;

        }

        public void Shift()
        {
            if (this.Count == 0)
            {
                return;
            }
            this.Remove(this.First());
        }

        public void Unshift(T item)
        {
            this.Insert(0, item);
        }

        public void Pop()
        {
            if (this.Count == 0)
            {
                return;
            }
            this.Remove(this.Last());
        }

        public void Push(T item)
        {
            this.Add(item);
        }
    }
}