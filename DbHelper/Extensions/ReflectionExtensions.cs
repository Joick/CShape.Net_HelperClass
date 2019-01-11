using System;
using System.Linq.Expressions;

namespace Utility
{
    public static class ReflectionExtensions
    {
        public static Member ToMember<M, R>(this Expression<Func<M, R>> propertyExpression)
        {
            return ReflectionHelper.GetMember(propertyExpression);
        }

        public static bool MemberEqual<M, R>(this Expression<Func<M, R>> origin, Expression<Func<M, R>> target)
        {
            return origin.ToMember() == target.ToMember();
        }
    }
}
