using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Utility
{
    [Serializable]
    internal class MethodMember : Member
    {
        private readonly MethodInfo member;

        private Member backingField;

        public override void SetValue(object target, object value)
        {
            throw new NotSupportedException("方法成员不能直接赋值");
        }

        public override object GetValue(object target)
        {
            return member.Invoke(target, null);
        }

        public override bool TryGetBackingField(out Member field)
        {
            if (backingField != null)
            {
                field = backingField;
                return true;
            }

            var name = Name;

            if (name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(3);

            var reflectedField = DeclaringType.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            reflectedField = reflectedField ?? DeclaringType.GetField("_" + name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            reflectedField = reflectedField ?? DeclaringType.GetField("m_" + name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (reflectedField == null)
            {
                field = null;
                return false;
            }

            field = backingField = new FieldMember(reflectedField);
            return true;
        }

        public MethodMember(MethodInfo member)
        {
            this.member = member;
        }

        public override string Name
        {
            get { return member.Name; }
        }

        public override Type PropertyType
        {
            get { return member.ReturnType; }
        }

        public override bool CanWrite
        {
            get { return false; }
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
            get { return false; }
        }

        public override bool IsMethod
        {
            get { return true; }
        }

        public override bool IsField
        {
            get { return false; }
        }

        public override bool IsProperty
        {
            get { return false; }
        }

        public override bool IsAutoProperty
        {
            get { return false; }
        }

        public override bool IsPrivate
        {
            get { return member.IsPrivate; }
        }

        public override bool IsProtected
        {
            get { return member.IsFamily || member.IsFamilyAndAssembly; }
        }

        public override bool IsPublic
        {
            get { return member.IsPublic; }
        }

        public override bool IsInternal
        {
            get { return member.IsAssembly || member.IsFamilyAndAssembly; }
        }

        public bool IsCompilerGenerated
        {
            get { return member.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any(); }
        }

        public override string ToString()
        {
            return "{Method: " + member.Name + "}";
        }
    }
}
