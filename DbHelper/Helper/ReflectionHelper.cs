using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Utility
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取对象的成员信息
        /// </summary>
        /// <typeparam name="M">对象类型</typeparam>
        /// <typeparam name="R">成员对象</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns>返回成员信息</returns>
        public static Member GetMember<M, R>(Expression<Func<M, R>> expression)
        {
            return GetMember(expression.Body);
        }

        /// <summary>
        /// 获取对象的成员信息
        /// </summary>
        /// <typeparam name="M">对象类型</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns>返回成员信息</returns>
        public static Member GetMember<M>(Expression<Func<M, object>> expression)
        {
            return GetMember(expression.Body);
        }

        /// <summary>
        /// 获取对象的指定成员的访问器
        /// </summary>
        /// <typeparam name="M">对象类型</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns>返回访问器对象</returns>
        public static IAccessor GetAccessor<M>(Expression<Func<M, object>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression.Body);

            return GetAccessor(memberExpression);
        }

        /// <summary>
        /// 获取对象的指定成员的访问器
        /// </summary>
        /// <typeparam name="M">对象类型</typeparam>
        /// <typeparam name="R">成员对象</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns>返回访问器对象</returns>
        public static IAccessor GetAccessor<M, R>(Expression<Func<M, R>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression.Body);

            return GetAccessor(memberExpression);
        }

        private static bool IsIndexedPropertyAccess(Expression expression)
        {
            return IsMethodExpression(expression) && expression.ToString().Contains("get_Item");
        }

        private static bool IsMethodExpression(Expression expression)
        {
            return expression is MethodCallExpression || (expression is UnaryExpression && IsMethodExpression((expression as UnaryExpression).Operand));
        }

        private static Member GetMember(Expression expression)
        {
            if (IsIndexedPropertyAccess(expression))
                return GetDynamicComponentProperty(expression).ToMember();
            if (IsMethodExpression(expression))
                return ((MethodCallExpression)expression).Method.ToMember();

            var memberExpression = GetMemberExpression(expression);

            return memberExpression.Member.ToMember();
        }

        private static PropertyInfo GetDynamicComponentProperty(Expression expression)
        {
            Type desiredConversionType = null;
            MethodCallExpression methodCallExpression = null;
            var nextOperand = expression;

            while (nextOperand != null)
            {
                if (nextOperand.NodeType == ExpressionType.Call)
                {
                    methodCallExpression = nextOperand as MethodCallExpression;
                    desiredConversionType = desiredConversionType ?? methodCallExpression.Method.ReturnType;
                    break;
                }

                if (nextOperand.NodeType != ExpressionType.Convert)
                    throw new ArgumentException("不支持的 Lambda 表达式", "expression");

                var unaryExpression = (UnaryExpression)nextOperand;
                desiredConversionType = unaryExpression.Type;
                nextOperand = unaryExpression.Operand;
            }

            var constExpression = methodCallExpression.Arguments[0] as ConstantExpression;

            return new DummyPropertyInfo((string)constExpression.Value, desiredConversionType);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            return GetMemberExpression(expression, true);
        }

        private static MemberExpression GetMemberExpression(Expression expression, bool enforceCheck)
        {
            MemberExpression memberExpression = null;

            if (expression.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression as MemberExpression;
            }

            if (enforceCheck && memberExpression == null)
            {
                throw new ArgumentException("非成员访问", "expression");
            }

            return memberExpression;
        }

        private static IAccessor GetAccessor(MemberExpression memberExpression)
        {
            var members = new List<Member>();

            while (memberExpression != null)
            {
                members.Add(memberExpression.Member.ToMember());
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            if (members.Count == 1)
            {
                return new SingleMember(members[0]);
            }

            members.Reverse();

            return new PropertyChain(members.ToArray());
        }
    }
}
