using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Alanta.Client.Media
{
    /// <summary>
    /// Retrieves, stores and recycles reusable objects (like SocketAsyncEventArgs) to help prevent heap fragmentation.
    /// </summary>
    /// <typeparam name="T">The type of the object to be reused.</typeparam>
    public class ObjectPool<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes the ObjectPool class.
        /// </summary>
        /// <param name="newFunction">A function which will return an instance of the targeted class if no instance is currently available.</param>
        public ObjectPool(Func<T> newFunction)
        {
            stack = new Stack<T>();
            this.NewFunction = newFunction;
        }

        /// <summary>
        /// Initializes the ObjectPool class.
        /// </summary>
        /// <param name="newFunction">A function which will return an instance of the targeted class if no instance is currently available.</param>
        /// <param name="resetAction">A function which takes an instance of the targeted class as a parameter and resets it.</param>
        public ObjectPool(Func<T> newFunction, Action<T> resetAction)
            : this(newFunction)
        {
            this.ResetAction = resetAction;
        }
        #endregion

        #region Fields and Properties
        private Stack<T> stack;
        public Func<T> NewFunction { get; set; }
        public Action<T> ResetAction { get; set; }
        #endregion

        /// <summary>
        /// Retrieves the next available instance of the targeted class, or creates one if none is available.
        /// </summary>
        /// <returns>An instance of the targeted class.</returns>
        public T GetNext()
        {
            T obj = default(T);
            lock (stack)
            {
                if (stack.Count == 0)
                {
                    return NewFunction();
                }
                else
                {
                    obj = stack.Pop();
                }
            }
            if (ResetAction != null)
            {
                ResetAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// Pushes the instance of the targeted class back onto the stack, making it available for re-use.
        /// </summary>
        /// <param name="obj">The instance of the targeted class to be recycled.</param>
        public void Recycle(T obj)
        {
            if (obj != null)
            {
                lock (stack)
                {
                    stack.Push(obj);
                }
            }
        }
    }
}
