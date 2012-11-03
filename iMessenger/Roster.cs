using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iMessenger
{
    public class Roster<TItem> : IEnumerable
    {
        private readonly IList<TItem> _source;
        public TItem Current { get; set; }

        public TItem this[int index]
        {
            get
            {
                if (index <= _source.Count - 1)
                {
                    return _source.ElementAt(index);
                }
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                if (index <= _source.Count - 1)
                {
                    _source.Insert(index, value);
                }
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int Count
        {
            get { return  _source.Count; }
        }

        public Roster()
        {
            _source = new List<TItem>();
        }
        
        public void Add(TItem newElement)             
        {
            _source.Add(newElement);
        }

        public void Delete(TItem oldItem)       
        {
            _source.Remove(oldItem);
        }

        public void Change(TItem oldItem, TItem newItem)
        {
            int index = IndexOf(oldItem);
            _source.Remove(oldItem);
            _source.Insert(index, newItem);
        }

        public int IndexOf(TItem item)
        {
            for (int i = 0; i < _source.Count; i++)
            { 
                if (item.Equals(_source[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public IEnumerator GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
