using System;

namespace Utility
{
    [Serializable]
    public class EntityProperty
    {
        public string PropertyName { get; set; }

        public object OriginalValue { get; set; }

        public object CurrentValue { get; set; }
    }
}
