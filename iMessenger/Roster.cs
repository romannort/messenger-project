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

        /// <summary>
        /// Gets item by index
        /// </summary>
        /// <param name="index"> Index of element </param>
        /// <returns> Item of list by index </returns>
        public TItem this[int index]
        {
            get
            {
                if (index <= _source.Count - 1)
                {
                    return _source.ElementAt(index);
                }
                else
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

        /// <summary>
        /// Gets count of items
        /// </summary>
        public int Count
        {
            get { return  _source.Count; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Roster()
        {
            _source = new List<TItem>();
        }
        
        /// <summary>
        /// Adds new item in list
        /// </summary>
        /// <param name="newElement"> Element to adding </param>
        public void Add(TItem newElement)             
        {
            _source.Add(newElement);
        }
        
        /// <summary>
        /// Deletes item from list
        /// </summary>
        /// <param name="oldItem"></param>
        public void Delete(TItem oldItem)       
        {
            _source.Remove(oldItem);
        }

        /// <summary>
        /// Change old item to the new 
        /// </summary>
        /// <param name="oldItem"> Old item </param>
        /// <param name="newItem"> New item </param>
        public void Change(TItem oldItem, TItem newItem)
        {
            int index = IndexOf(oldItem);
            _source.Remove(oldItem);
            _source.Insert(index, newItem);
        }

        /// <summary>
        /// Gets index of item
        /// </summary>
        /// <param name="item"> Item from list </param>
        /// <returns> Index of item </returns>
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

        /// <summary>
        /// Default enumerator
        /// </summary>
        /// <returns> Enumerator </returns>
        public IEnumerator GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
