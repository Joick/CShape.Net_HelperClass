using System;
using System.Linq.Expressions;

namespace Utility
{
    public interface IAccessor
    {
        string FieldName { get; }

        Type PropertyType { get; }
        Member InnerMember { get; }
        void SetValue(object target, object value);
        object GetValue(object target);

        IAccessor GetChildAccessor<T>(Expression<Func<T, object>> expression);

        string Name { get; }
    }
}
