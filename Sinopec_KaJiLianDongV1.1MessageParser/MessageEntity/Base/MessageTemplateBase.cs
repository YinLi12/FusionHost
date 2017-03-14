using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public abstract class MessageTemplateBase
    {
        public MessageTemplateBase() { }
        /// <summary>
        /// Validate the element object against on some high level validation rules.
        /// </summary>
        /// <returns>If an exception returned by overriden element class, the validation will be indicated as fail, the exception will be re-throw out</returns>
        public virtual Exception Validate() { return null; }

        /// <summary>
        /// Simply Xml-serialize the object to get the log string.
        /// </summary>
        /// <returns>xml format string</returns>
        public virtual string ToLogString()
        {
            var unformattedLogs = new List<string>();
            try
            {
                this.ToLogStringHelper(this, unformattedLogs, "   ");
            }
            catch (Exception ex)
            {
                unformattedLogs.Add("Exception occured when generating the LogString for current message, exception detail: " + ex.ToString());
            }

            return unformattedLogs.Aggregate((p, n) => p + System.Environment.NewLine + n) + System.Environment.NewLine + "------------";
        }

        private List<string> ToLogStringHelper(object elementInstance, List<string> propertyNameValueString, string prefix)
        {
            int originalCount = propertyNameValueString.Count;
            // Only processing WayneAttribute marked properties, and order by its Index in DESC
            var targetPropertyList = elementInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(p => p.GetCustomAttributes(typeof(AttributeBase), true).Length > 0)
                                        .OrderBy(info => ((AttributeBase)(info.GetCustomAttributes(typeof(AttributeBase), true)[0])).Index);
            var plainPropertyLogStrAccumulator = string.Empty;
            foreach (var propertyInfo in targetPropertyList)
            {
                // normal plain Property
                if (propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true).Length == 0)
                {
                    plainPropertyLogStrAccumulator += propertyInfo.Name + ": " +
                                                      propertyInfo.GetValue(elementInstance, null).ToString() + ", ";

                }
                // the IList Property
                else
                {
                    var listFormat =
                        (EnumerableFormatAttribute)propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true)[0];
                    var list = (IList)propertyInfo.GetValue(this, null);
                    var genericArg = list.GetType().GetGenericArguments()[0];

                    if (list != null)
                    {
                        propertyNameValueString.Add(prefix + "*(List)" + propertyInfo.Name + "-->");
                        string primitivePropertyStr = string.Empty;
                        for (int i = 0; i <= list.Count - 1; i++)
                        {
                            if (genericArg.IsPrimitive)
                            {
                                primitivePropertyStr += "0x" + int.Parse(list[i].ToString()).ToString("X").PadLeft(2, '0') + " ";

                            }
                            else
                            {
                                this.ToLogStringHelper(list[i], propertyNameValueString, prefix + "   [" + i + "]");
                            }
                        }

                        propertyNameValueString.Add(primitivePropertyStr);
                        propertyNameValueString.Add(prefix + "*(List)" + propertyInfo.Name + "<--");
                    }
                }
            }

            if (!string.IsNullOrEmpty(plainPropertyLogStrAccumulator))
            {
                // Add prefix and remove the last ", "
                plainPropertyLogStrAccumulator = prefix + plainPropertyLogStrAccumulator.Remove(plainPropertyLogStrAccumulator.Length - 2);
                propertyNameValueString.Insert(originalCount, plainPropertyLogStrAccumulator);
            }

            return propertyNameValueString;
        }
    }
}
