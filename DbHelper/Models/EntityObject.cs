using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Utility
{
    [DataContract, Serializable]
    public abstract class EntityObject<T> : INotifyPropertyChanged where T : EntityObject<T>, new()
    {
        private bool deserializing;

        /// <summary>
        /// 属性的值改变时发生
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// 属性的值改变时发生
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        internal IList<string> ChangedProperties { get; set; }

        /// <summary>
        /// 未更改之前的值
        /// </summary>
        internal IList<EntityProperty> Properties { get; set; }


        [NonSerialized]
        private IList<ColumnMapping<T>> mappings;

        internal IList<ColumnMapping<T>> Mappings
        {
            get { return this.mappings; }
            private set { this.mappings = value; }
        }

        /// <summary>
        /// 获取或设置实体映射表的表名
        /// </summary>
        [DataMember]
        internal protected string TableName { get; set; }

        /// <summary>
        /// 子类重载此方法，实现表字段与对象属性的映射关系
        /// </summary>
        protected abstract void Mapping();

        /// <summary>
        /// 对象被序列化前的处理
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;

            this.Mappings = new List<ColumnMapping<T>>();
            this.Mapping();
        }

        public EntityObject()
        {
            this.ChangedProperties = new List<string>();
            this.Properties = new List<EntityProperty>();
            this.Mappings = new List<ColumnMapping<T>>();
            this.Mapping();
        }

        /// <summary>
        /// 属性的值改变时的处理
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="oldValue">旧值</param>
        protected virtual void OnPropertyChanging(string propertyName, object originalValue)
        {
            EntityProperty ep = this.Properties.FirstOrDefault(d => d.PropertyName == propertyName);

            if (ep == null)
            {
                ep = new EntityProperty();
                ep.PropertyName = propertyName;
                this.Properties.Add(ep);
            }

            ep.OriginalValue = originalValue;
        }

        /// <summary>
        /// 属性的值改变时的处理
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="oldValue">旧值</param>
        protected virtual void OnPropertyChanging(string propertyName, object originalValue, object currentValue)
        {
            if (object.Equals(originalValue, currentValue))
            {
                return;
            }

            EntityProperty ep = this.Properties.Where(s => s.PropertyName == propertyName).FirstOrDefault();

            if (ep == null)
            {
                ep = new EntityProperty();
                ep.PropertyName = propertyName;
                this.Properties.Add(ep);
            }

            ep.OriginalValue = originalValue;
            ep.CurrentValue = currentValue;
        }

        /// <summary>
        /// 属性的值改变后的处理
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName, object currentValue)
        {
            if (!this.ChangedProperties.Contains(propertyName))
            {
                this.ChangedProperties.Add(propertyName);
            }
        }

        /// <summary>
        /// 清空已更新属性列表
        /// </summary>
        public void Reset()
        {
            this.ChangedProperties.Clear();
            this.Properties.Clear();
        }

        /// <summary>
        /// 通过 Lambda 表达式映射字段与属性的关系
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected ColumnMapping<T> Map(Expression<Func<T, object>> expression)
        {
            string propertyName = expression.ToMember().Name;

            ColumnMapping<T> mapping = new ColumnMapping<T>()
            {
                Expression = expression,
                PropertyName = propertyName,
                ColumnName = propertyName.AsUnderScore()
            };

            this.Mappings.Add(mapping);

            return mapping;
        }
    }
}
