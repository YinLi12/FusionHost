using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace MessageParser.Config
{
    public class Mapping : ConfigurationElement
    {
        [ConfigurationProperty("code", IsRequired = true)]
        public string CodeRawString
        {
            get
            {
                return this["code"] as string;
            }
        }

        public byte[] Code
        {
            get
            {
                // it matters that starts with 0x or not, need take care differently
                if (this.CodeRawString.ToLower().StartsWith("0x"))
                {
                    return this.CodeRawString.Substring(2).ToBytes();
                }
                else
                {
                    return this.CodeRawString.ToBytes();
                }
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeRawString
        {
            get
            {
                return this["type"] as string;
            }
        }

        public Type Type
        {
            get { return Assembly.GetAssembly(typeof(MessageTemplateBase)).GetType(this.TypeRawString); }
        }

        [ConfigurationProperty("description", IsRequired = false)]
        public string Description
        {
            get
            {
                return this["description"] as string;
            }
        }
    }

    public class MappingCollection : ConfigurationElementCollection
    {
        //public Mapping this[int index]
        //{
        //    get
        //    {
        //        return base.BaseGet(index) as Mapping;
        //    }
        //    set
        //    {
        //        if (base.BaseGet(index) != null)
        //        {
        //            base.BaseRemoveAt(index);
        //        }
        //        this.BaseAdd(index, value);
        //    }
        //}

        //public new Mapping this[string responseString]
        //{
        //    get { return (Mapping)BaseGet(responseString); }
        //    set
        //    {
        //        if (BaseGet(responseString) != null)
        //        {
        //            BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
        //        }
        //        BaseAdd(value);
        //    }
        //}

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new Mapping();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((Mapping)element).CodeRawString.Select(s => s.ToString()).Aggregate((n, acc) => n + acc);
        }
    }
}
