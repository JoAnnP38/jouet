using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jouet.Collections
{
    public interface IStack<T> : ICollection<T>
    {
        public ref T Peek();
        public ref T Pop();
        public void Push(T item);
        public bool TryPeek(out T item);
        public bool TryPop(out T item);
    }
}
