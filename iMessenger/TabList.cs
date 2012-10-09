using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMessenger
{
    public class TabList<T>
    {
        private T[] mas;
        private int capacity;   
        private int count;
        public int Current { get; set; } 

        public T this[int index]
        {
            get
            {
                if (index <= mas.Length)
                    return mas[index];
                else
                    throw new ArgumentOutOfRangeException();
            }
            set
            {
                if (index <= mas.Length)
                    mas[index] = (T)value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int Count
        {
            get { return count; }
        }

        public int Capacity                 
        {
            get { return capacity; }
        }

        public TabList()
        {
            capacity = 12;
            mas = new T[capacity];
        }

        public void Add(T elem)             
        {
            if (count < capacity)
                mas[count++] = elem;
            else 
            {
                T[] mas2 = new T[capacity * 2];
                mas.CopyTo(mas2, 0);
                mas = mas2;
            }
            capacity *= 2;
        }

        public void Delete(int index)       
        {
            if (index < count)
            {
                for (int i = index; index < count; index++)
                    mas[i] = mas[i + 1];
                count--;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T value in mas)
                if(value != null)
                    yield return value;
        }

    }
}
