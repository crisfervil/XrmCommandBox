using System;

namespace XrmCommandBox
{
    public class HandlerAttribute : Attribute
    {
        public HandlerAttribute(Type handler)
        {
            HandlerType = handler;
        }

        public Type HandlerType { get; set; }
    }
}