using System;
using System.Collections.Generic;
using System.Data;

namespace pitermarx.IoC
{
    public class Container
    {
        public readonly string Name;

        private readonly Dictionary<Type, Dictionary<string, Func<object>>> getters;
        private bool isSealed;

        internal Container(string instanceName)
        {
            this.Name = instanceName;
            this.getters = new Dictionary<Type, Dictionary<string, Func<object>>>();
        }

        public Container Register<T>(Func<T> getter, string getterName = "")
            where T : class 
        {
            if (isSealed)
            {
                throw new ConstraintException("The IoC instance is sealed");
            }

            var dic = this.getters.ContainsKey(typeof(T))
                ? this.getters[typeof(T)]
                : (this.getters[typeof(T)] = new Dictionary<string, Func<object>>());
            dic[getterName ?? string.Empty] = getter;
            return this;
        }

        public T Get<T>(string getterName = "")
        {
            return (T)this.getters[typeof(T)][getterName ?? string.Empty]();
        }

        public void Seal()
        {
            this.isSealed = true;
        }

        internal void Dispose()
        {
            this.Seal();
            this.getters.Clear();
        }
    }
}
