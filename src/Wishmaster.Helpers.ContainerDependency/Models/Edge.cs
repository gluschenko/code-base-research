using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wishmaster.Helpers.ContainerDependency.Models
{
    public class Edge<T>
    {
        public Guid From { get; private set; }
        public Guid To { get; private set; }
        public T Value { get; private set; }

        public Edge(T value, Guid from, Guid to)
        {
            From = from;
            To = to;
            Value = value;
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Value);
        }
    }
}
