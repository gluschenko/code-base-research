using System;

namespace CodeBase.Domain
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageDescriptorAttribute : Attribute
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public PageLifetime Lifetime { get; set; }

        public PageDescriptorAttribute(PageLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public PageDescriptorAttribute(string title, PageLifetime lifetime) : this(lifetime)
        {
            Title = title;
        }

        public PageDescriptorAttribute(string title, int order, PageLifetime lifetime) : this(title, lifetime)
        {
            Order = order;
        }
    }

    public enum PageLifetime : byte
    {
        Transient = 1,
        Scoped = 2,
    }
}
