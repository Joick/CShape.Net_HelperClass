using System;
using System.Reflection;

namespace Utility
{
    [Serializable]
    internal class FieldMember : Member
    {
        private readonly FieldInfo member;

        public override void SetValue(object target, object value)
        {
            member.SetValue(target, value);
        }

        public override object GetValue(object target)
        {
            return member.GetValue(target);
        }

        public override bool TryGetBackingField(out Member backingField)
        {
            backingField = null;
            return false;
        }

        public FieldMember(FieldInfo member)
        {
            this.member = member;
        }

        public override string Name
        {
            get { return member.Name; }
        }
        public override Type PropertyType
        {
            get { return member.FieldType; }
        }
        public override bool CanWrite
        {
            get { return true; }
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
            get { return false; }
        }
        public override bool IsField
        {
            get { return true; }
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

        public override string ToString()
        {
            return "{Field: " + member.Name + "}";
        }
    }
}
