using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface Basic
    {
        public string Name { get; set; }
        public string ToString();
        public bool Equals(object? obj);
    }
}
