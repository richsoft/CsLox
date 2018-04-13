using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Collections
{
    /// <summary>
    /// Represents a stack that can be indexed
    /// </summary>
    class StackList<T> : List<T>
    {

        /// <summary>
        /// Push an item onto the top of the stack
        /// </summary>
        /// <param name=""></param>
        public void Push(T item)
        {
            this.Add(item);
        }

        /// <summary>
        /// Pop an item from the top of stack
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            int last = this.Count() - 1;
            T item = this[last];
            this.RemoveAt(last);
            return item;
        }

        /// <summary>
        /// Peek the item at the top of the stack
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return this[this.Count() - 1];
        }

        /// <summary>
        /// Check if the stack is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return !this.Any();
        }

    }
}
