using System;

namespace XrmCommandBox
{
    public class HandlerAttribute : Attribute
    {
        public Type HandlerType { get; set; }

        public HandlerAttribute(Type handler)
        {
            this.HandlerType = handler;
        }
    }
}