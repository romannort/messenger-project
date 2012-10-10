using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMessenger
{
    public class TabList<TItem> : IEnumerable
    {
        private IList<TItem> source;
        //private int capacity;   
        //private int count;
        public TItem Current { get; set; }

        public TItem this[int index]
        {
            get
            {
                if (index <= source.Count - 1)
                    return source.ElementAt(index);
                else
                    throw new ArgumentOutOfRangeException();
            }
            set
            {
                if (index <= source.Count - 1)
                    source.Insert(index, value);
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int Count
        {
            get { return  source.Count; }
        }

        //public int Capacity                 
        //{
        //    get { return capacity; }
        //}

        public TabList()
        {
            //capacity = 12;
            source = new List<TItem>();
        }
        
        public void Add(TItem newElement)             
        {
            //if (count < capacity)
            //    source[count++] = elem;
            //else 
            //{
            //    TItem[] mas2 = new TItem[capacity * 2];
            //    source.CopyTo(mas2, 0);
            //    source = mas2;
            //}
            //capacity *= 2;
            source.Add(newElement);
        }

        public void Delete(int index)       
        {
            //if (index < count)
            //{
            //    for (int i = index; index < count; index++)
            //        source[i] = source[i + 1];
            //    count--;
            //}
            
        }

        // Явная реализация интерфейса
        public IEnumerator GetEnumerator()
        {
            //foreach (TItem value in source)
            //    if(value != null)
            //        yield return value;
            return source.GetEnumerator();
        }
    }
}
