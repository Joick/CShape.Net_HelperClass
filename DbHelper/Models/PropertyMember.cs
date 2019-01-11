using System;
using System.Reflection;

namespace Utility
{
    [Serializable]
    internal class PropertyMember : Member
    {
        readonly PropertyInfo member;
        readonly MethodMember getMethod;
        readonly MethodMember setMethod;
        Member backingField;

        public PropertyMember(PropertyInfo member)
        {
            this.member = member;
            getMethod = GetMember(member.GetGetMethod(true));
            setMethod = GetMember(member.GetSetMethod(true));
        }

        MethodMember GetMember(MethodInfo method)
        {
            if (method == null)
                return null;

            return (MethodMember)method.ToMember();
        }

        public override void SetValue(object target, object value)
        {
            if (member.PropertyType.Name == "Boolean" || member.PropertyType.IsGenericType && member.PropertyType.FullName.Contains("Boolean"))  // 解决 类型“System.UInt64”的对象无法转换为类型“System.Nullable`1[System.Boolean]”异常。
            {
                member.SetValue(target, Convert.ToBoolean(value), null);
            }
            else
            {
                member.SetValue(target, value, null);
            }
        }

        public override object GetValue(object target)
        {
            return member.GetValue(target, null);
        }

        public override bool TryGetBackingField(out Member field)
        {
            if (backingField != null)
            {
                field = backingField;
                return true;
            }

            var reflectedField = DeclaringType.GetField(Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            reflectedField = reflectedField ?? DeclaringType.GetField("_" + Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            reflectedField = reflectedField ?? DeclaringType.GetField("m_" + Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (reflectedField == null)
            {
                field = null;
                return false;
            }

            field = backingField = new FieldMember(reflectedField);
            return true;
        }

        public override string Name
        {
            get { return member.Name; }
        }

        public override Type PropertyType
        {
            get { return member.PropertyType; }
        }

        public override bool CanWrite
        {
            get
            {
                // override the default reflection value here. Private setters aren't
                // considered "settable" in the same sense that public ones are. We can
                // use this to control the access strategy later
                if (IsAutoProperty && (setMethod == null || setMethod.IsPrivate))
                    return false;

                return member.CanWrite;
            }
        }

        public override MemberInfo MemberInfo
        {
            get { return member; }
        }

        public override Type DeclaringType
        {
            get { return member.DeclaringType; }
        }

        public override bool HasIndexParameters
        {
            get { return member.GetIndexParameters().Length > 0; }
        }

        public override bool IsMethod
        {
            get { return false; }
        }

        public override bool IsField
        {
            get { return false; }
        }

        public override bool IsProperty
        {
            get { return true; }
        }

        public override bool IsAutoProperty
        {
            get
            {
                return (getMethod != null && getMethod.IsCompilerGenerated)
                    || (setMethod != null && setMethod.IsCompilerGenerated);
            }
        }

        public override bool IsPrivate
        {
            get { return getMethod.IsPrivate; }
        }

        public override bool IsProtected
        {
            get { return getMethod.IsProtected; }
        }

        public override bool IsPublic
        {
            get { return getMethod.IsPublic; }
        }

        public override bool IsInternal
        {
            get { return getMethod.IsInternal; }
        }

        public MethodMember Get
        {
            get { return getMethod; }
        }

        public MethodMember Set
        {
            get { return setMethod; }
        }

        public override string ToString()
        {
            return "{Property: " + member.Name + "}";
        }
    }
}
