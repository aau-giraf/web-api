using System;

namespace GirafRest.IntegrationTest.Order
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PriorityAttribute : Attribute
    {
        public int Priority { get; private set; }

        public PriorityAttribute(int priority) => Priority = priority;
    }
}