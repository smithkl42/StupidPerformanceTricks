using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class FixedQueue<T>
    {
        public FixedQueue(int size)
        {
            queue = new T[size];
        }

        private T[] queue;
        private int lastAddedPosition;
        private int lastReadPosition;

        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue[lastAddedPosition++ % queue.Length] = item;
            }
        }

        public T Dequeue()
        {
            lock (queue)
            {
                int position = lastReadPosition++ % queue.Length;
                T item = queue[position];
                queue[position] = default(T);
                return item;
            }
        }

        public T Peek()
        {
            lock (queue)
            {
                return queue[lastReadPosition % queue.Length];
            }
        }
    }
}
