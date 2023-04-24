using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jouet.Utilities
{
    public interface IPooledObject<out T>
    {
        public void Clear();
    }
}
