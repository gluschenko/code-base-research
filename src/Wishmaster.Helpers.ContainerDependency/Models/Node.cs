using System;

namespace Wishmaster.Helpers.ContainerDependency.Models
{
    public class Node<T>
    {
        public Guid Uid { get; private set; }
        public T Value { get; private set; }

        public Node(T value)
        {
            Uid = Guid.NewGuid();
            Value = value;
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uid, Value);
        }
    }
}
