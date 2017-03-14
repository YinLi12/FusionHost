using MessageParser;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MessageParser
{
    public abstract class ParserBase : IMessageParser<byte[], MessageTemplateBase>
    {
        private IMessageTemplateLookup templateLookup;
        /// <summary>
        /// ongoing processing raw data array.
        /// </summary>
        private byte[] processingRawData = null;

        private int positiveIndexAccu = 0;

        /// <summary>
        /// Fired on when bytes incoming and will start to convert to a object, but not started yet.
        /// </summary>
        public event EventHandler<ParsingEventArg<byte[], MessageTemplateBase>> Deserializing;

        /// <summary>
        /// Fired on when bytes incoming and started to convert to a object, and finished already.
        /// </summary>
        public event EventHandler<ParsingEventArg<byte[], MessageTemplateBase>> Deserialized;

        public event EventHandler<ParsingEventArg<MessageTemplateBase, byte[]>> Serializing;

        public event EventHandler<ParsingEventArg<MessageTemplateBase, byte[]>> Serialized;

        /// <summary>
        /// Fired on a Format Attibute marked field will starting to convert to bytes, but not started yet.
        /// string in ParsingEventArg is the field name, byte[] should always null here since not started.
        /// </summary>
        public event EventHandler<ParsingEventArg<string, byte[]>> FieldSerializing;

        /// <summary>
        /// Fired on a Format Attibute marked field started to convert to bytes, and finished already.
        /// string in ParsingEventArg is the field name, byte[] should have values for this already serialized field.
        /// </summary>
        public event EventHandler<ParsingEventArg<string, byte[]>> FieldSerialized;

        protected ParserBase(IMessageTemplateLookup templateLookup)
        {
            this.templateLookup = templateLookup;
        }

        /// <summary>
        /// Deserialize a byte[] into a Message entity, the parsing template is loaded from `MessageCodeLookup.GetMessageEntityByFullMessageRawBytes`
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public virtual MessageTemplateBase Deserialize(byte[] rawData)
        {
            return this.Deserialize(rawData, null);
        }

        /// <summary>
        /// Deserialize a byte[] into a Message entity, the parsing template is specified from `template` parameter.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public virtual MessageTemplateBase Deserialize(byte[] rawData, MessageTemplateBase template)
        {
            //ybs add
            if (rawData.Length < 5)
            {
                return default(MessageTemplateBase);
            }

            var debug = rawData.ToHexLogString();

            var safeEvent = this.Deserializing;
            safeEvent?.Invoke(this, new ParsingEventArg<byte[], MessageTemplateBase>() { From = rawData, To = default(MessageTemplateBase) });

            if (template == null) template = this.templateLookup.GetMessageTemplateByRawBytes(rawData);
            this.processingRawData = rawData;
            this.ConvertToObject(template, this.processingRawData);
            if (this.processingRawData.Length > 0)
            {
                throw new FormatException("Unexpected Element Deserialize error occured(additional tail existed)");
            }

            #region if message level custom validation defined? then execute it

            Exception customValidationException;
            if ((customValidationException = template.Validate()) != null)
            {
                throw customValidationException;
            }

            #endregion

            safeEvent = this.Deserialized;
            safeEvent?.Invoke(this, new ParsingEventArg<byte[], MessageTemplateBase>() { From = rawData, To = template });

            return template;
        }

        public virtual byte[] Serialize(MessageTemplateBase message)
        {

            #region if message level custom validation defined? then execute it

            Exception customValidationException;
            if ((customValidationException = message.Validate()) != null)
            {
                throw customValidationException;
            }

            #endregion

            var safeEvent = this.Serializing;
            safeEvent?.Invoke(this, new ParsingEventArg<MessageTemplateBase, byte[]>() { From = message, To = null });

            this.processingRawData = new byte[0];
            this.positiveIndexAccu = 0;
            var content = this.ConvertToBytes(message);

            safeEvent = this.Serialized;
            safeEvent?.Invoke(this, new ParsingEventArg<MessageTemplateBase, byte[]>() { From = message, To = content });

            return content;
        }

        private void ConvertToObject(object elementInstance, byte[] rawData)
        {
            if (rawData.Length == 0)
            {
                return;
            }
            else
            {
                //ybs addd code
                string debug = rawData.Select(s => s.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n);//be convenient to see log

                // Only processing Format related Attribute marked properties, and order by its Index
                var targetPropertyList = elementInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                            .Where(p => p.GetCustomAttributes(typeof(AttributeBase), true).Length > 0)
                                            .OrderBy(info => ((AttributeBase)(info.GetCustomAttributes(typeof(AttributeBase), true)[0])).Index).ToList();
                foreach (var propertyInfo in targetPropertyList)
                {
                    // normal plain Property
                    if (propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true).Length == 0)
                    {
                        int currentElementLength = -1;
                        var format =
                           (FormatAttribute)propertyInfo.GetCustomAttributes(typeof(FormatAttribute), true)[0];
                        if (!string.IsNullOrEmpty(format.LengthOrCountLink))
                        {
                            currentElementLength = int.Parse(
                                    elementInstance.GetType().GetProperty(format.LengthOrCountLink).GetValue(elementInstance, null).ToString());
                            if (!string.IsNullOrEmpty(format.LengthOrCountLinkExpression))
                            {
                                if (!format.LengthOrCountLinkExpression.Contains(';') &&
                                    (format.LengthOrCountLinkExpression.Contains('+') || format.LengthOrCountLinkExpression.Contains('-')))
                                {
                                    var pureOperatorAndNumber = format.LengthOrCountLinkExpression.Trim().Replace(" ", "");
                                    if (pureOperatorAndNumber.Contains('+'))
                                    {
                                        currentElementLength += int.Parse(pureOperatorAndNumber.Substring(1));
                                    }
                                    else if (pureOperatorAndNumber.Contains('-'))
                                    {
                                        currentElementLength -= int.Parse(pureOperatorAndNumber.Substring(1));
                                    }
                                }
                                else
                                {
                                    var pair = format.LengthOrCountLinkExpression.Split(';');
                                    bool found = false;
                                    foreach (string p in pair)
                                    {
                                        var mappings = p.Split(':');
                                        if (currentElementLength == int.Parse(mappings[0]))
                                        {
                                            currentElementLength = int.Parse(mappings[1]);
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    { throw new ArgumentException("Deserializing LengthOrCountLinkExpression: " + format.LengthOrCountLinkExpression + " didn't found match values in link field: " + format.LengthOrCountLink); }
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            currentElementLength = format.FixedLengthOrCount;
                        }

                        // check if the remaining raw data bytes are enough to fill the current property
                        if (processingRawData.Count() < currentElementLength)
                        {
                            throw new ArgumentException(
                                string.Format("ConvertToObject Error: The remaining raw data bytes are insufficient to fill the current property '{0}'",
                                propertyInfo.Name));
                        }

                        this.FillPropertyData(format.EncodingType, propertyInfo, elementInstance, processingRawData.Take(currentElementLength).ToArray());

                        #region validate the range for Range Attribute marked field.

                        // Validate the range, once failed, throw a exception
                        var rangeValidator = propertyInfo.GetCustomAttributes(typeof(RangeAttribute), true).Length > 0
                            ? propertyInfo.GetCustomAttributes(typeof(RangeAttribute), true)[0] : null;
                        if (rangeValidator != null
                            && !((RangeAttribute)rangeValidator).IsValid(propertyInfo.GetValue(elementInstance, null)))
                        {
                            throw new ArgumentException(((RangeAttribute)rangeValidator).ErrorMessage);
                        }

                        #endregion

                        this.processingRawData = this.processingRawData.Skip(currentElementLength).ToArray();
                    }
                    // the IList Property
                    else
                    {
                        bool isCascadedList = false;
                        var listFormat =
                            (EnumerableFormatAttribute)propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true)[0];
                        int? listCount = null;
                        if (!string.IsNullOrEmpty(listFormat.LengthOrCountLink) && listFormat.LengthOrCountLink.ToLower() == "%cascade")
                        {
                            // never know the end of the data, it's a flow, so set a big value.
                            listCount = 9999;
                            isCascadedList = true;
                        }
                        else if (!string.IsNullOrEmpty(listFormat.LengthOrCountLink))
                        {
                            var linkPropertyValue = elementInstance.GetType().GetProperty(listFormat.LengthOrCountLink).GetValue(elementInstance, null);
                            if (linkPropertyValue is Enum)
                            {
                                listCount = (int)linkPropertyValue;
                            }
                            else
                            {
                                listCount = int.Parse(linkPropertyValue.ToString());
                            }
                            if (!string.IsNullOrEmpty(listFormat.LengthOrCountLinkExpression))
                            {
                                if (!listFormat.LengthOrCountLinkExpression.Contains(';') &&
                                    (listFormat.LengthOrCountLinkExpression.Contains('+') || listFormat.LengthOrCountLinkExpression.Contains('-')))
                                {
                                    var pureOperatorAndNumber = listFormat.LengthOrCountLinkExpression.Trim().Replace(" ", "");
                                    if (pureOperatorAndNumber.Contains('+'))
                                    {
                                        listCount += int.Parse(pureOperatorAndNumber.Substring(1));
                                    }
                                    else if (pureOperatorAndNumber.Contains('-'))
                                    {
                                        listCount -= int.Parse(pureOperatorAndNumber.Substring(1));
                                    }
                                }
                                else
                                {
                                    var pair = listFormat.LengthOrCountLinkExpression.Split(';');
                                    bool found = false;
                                    foreach (string p in pair)
                                    {
                                        var mappings = p.Split(':');
                                        if (listCount == int.Parse(mappings[0]))
                                        {
                                            listCount = int.Parse(mappings[1]);
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        throw new ArgumentException("Deserializing LengthOrCountLinkExpression: " +
                                                                    listFormat.LengthOrCountLinkExpression +
                                                                    " didn't found match values in link field: " +
                                                                    listFormat.LengthOrCountLink);
                                    }
                                }
                            }
                        }
                        else if (listFormat.FixedLengthOrCount > 0)
                        {
                            listCount = listFormat.FixedLengthOrCount;
                        }
                        else
                        {
                            throw new FormatException("Can't resolve the count number for IList property: " + propertyInfo.Name);
                        }

                        IList list = null;
                        // since it's CascadedList, then probably there's no data anymore, it's a empty list, no need to do any parsing.
                        if (this.processingRawData.Count() == 0 && isCascadedList)
                        {

                        }
                        else
                        {
                            list = (IList)Activator.CreateInstance(propertyInfo.PropertyType);
                            var genericArg = list.GetType().GetGenericArguments()[0];

                            for (int i = 0; i < listCount; i++)
                            {
                                if (genericArg.IsPrimitive)
                                {
                                    var subPrimitiveInstance = Activator.CreateInstance(genericArg);
                                    // assume primitive always take 1 byte, since we only use int and byte.  need refine future.
                                    subPrimitiveInstance = this.processingRawData.Take(listFormat.ElementLength).ToArray().ToInt32();
                                    if (genericArg.IsAssignableFrom(typeof(Byte)))
                                    {
                                        list.Add(Convert.ToByte(subPrimitiveInstance));
                                    }
                                    else if (genericArg.IsAssignableFrom(typeof(int)))
                                    {
                                        list.Add(subPrimitiveInstance);
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Now Only support primitive types of 'byte' and 'int'");
                                    }

                                    this.processingRawData = this.processingRawData.Skip(listFormat.ElementLength).ToArray();
                                }
                                else
                                {
                                    var subElementInstance = Activator.CreateInstance(genericArg);
                                    list.Add(subElementInstance);
                                    this.ConvertToObject(subElementInstance, this.processingRawData);
                                }

                                // no data need to be processing, break it since we might set a big listCount earlier for cascade structure.
                                // or you'll see a big List<T>
                                if (this.processingRawData.Length == 0)
                                {
                                    break;
                                }
                            }
                        }

                        propertyInfo.SetValue(elementInstance, list, null);
                    }
                }
            }
        }

        /// <summary>
        /// Set the value to a property in a specific object based on the encoding type associated with the property.
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="elementInstance"></param>
        /// <param name="data"></param>
        private void FillPropertyData(EncodingType encodingType, PropertyInfo propertyInfo, object elementInstance, byte[] data)
        {
            switch (encodingType)
            {
                case EncodingType.ASCII:
                    propertyInfo.SetValue(elementInstance, Encoding.ASCII.GetString(data), null);
                    break;
                case EncodingType.ASCIIInt:
                    propertyInfo.SetValue(elementInstance, int.Parse(Encoding.ASCII.GetString(data)), null);
                    break;
                case EncodingType.ASCIILong:
                    propertyInfo.SetValue(elementInstance, long.Parse(Encoding.ASCII.GetString(data)), null);
                    break;
                case EncodingType.BCD:
                    var tempBytes = data;
                    //if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum") {
                    //    if (!Enum.IsDefined(propertyInfo.PropertyType, tempBytes.GetBCD()))
                    //        throw new ArgumentException("The given value is not a valid " + propertyInfo.PropertyType + " element.");
                    //}
                    propertyInfo.SetValue(elementInstance,
                                          tempBytes.Length == 1 ? tempBytes[0].GetBCD() : tempBytes.GetBCD(), null);
                    break;
                case EncodingType.BcdString:
                    tempBytes = data;
                    //if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum") {
                    //    if (!Enum.IsDefined(propertyInfo.PropertyType, tempBytes.GetBCD()))
                    //        throw new ArgumentException("The given value is not a valid " + propertyInfo.PropertyType + " element.");
                    //}
                    var fieldValue = tempBytes.Length == 1 ? tempBytes[0].GetBCD().ToString() : tempBytes.GetBCDString();
                    if (propertyInfo.PropertyType.IsAssignableFrom(typeof(long)))
                    {
                        propertyInfo.SetValue(elementInstance, long.Parse(fieldValue)
                                              , null);
                    }
                    else
                    {
                        propertyInfo.SetValue(elementInstance, fieldValue
                                              , null);
                    }
                    break;
                case EncodingType.BIN:
                    if (propertyInfo.PropertyType.IsAssignableFrom(typeof(Enum)))
                    {
                        var o = Enum.ToObject(propertyInfo.PropertyType, data.ToInt32());
                        propertyInfo.SetValue(elementInstance, o, null);
                    }
                    else if (propertyInfo.PropertyType == typeof(Byte))
                    {
                        propertyInfo.SetValue(elementInstance, (byte)(data.ToInt32()), null);
                    }
                    // in case the Property is a Nullable enum type.
                    else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (propertyInfo.PropertyType.GetGenericArguments()[0].IsEnum)
                        {
                            var o = Enum.ToObject(propertyInfo.PropertyType.GetGenericArguments()[0], data.ToInt32());
                            propertyInfo.SetValue(elementInstance, o, null);
                        }
                        else
                        {
                            var value = Convert.ChangeType(data.ToInt32(), propertyInfo.PropertyType);
                            propertyInfo.SetValue(elementInstance, value, null);
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(elementInstance, data.ToInt32(), null);
                    }
                    break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="propertyValue"></param>
        /// <param name="propertyValueFixedLength"></param>
        /// <param name="elementInstance"></param>
        /// <returns></returns>
        private byte[] SerializePropertyData(EncodingType encodingType, object propertyValue, int propertyValueFixedLength, object elementInstance)
        {
            string enumTypeConvertedValue;
            switch (encodingType)
            {
                case EncodingType.ASCII:
                    //if (propertyValue is Enum)
                    //{
                    //    var temp = ((int)propertyValue).ToString();
                    //    enumTypeConvertedValue = temp.ToString().PadLeft(propertyValueFixedLength, '0');
                    //}
                    //else
                    //{
                    enumTypeConvertedValue = propertyValue.ToString();
                    //}
                    if (enumTypeConvertedValue.Length > propertyValueFixedLength)
                        throw new ArgumentOutOfRangeException("ASCII encoded property with value: " + enumTypeConvertedValue
                            + " exceed its declared Fixedlength(" + propertyValueFixedLength + "), try shorten the value");
                    return Encoding.ASCII.GetBytes(enumTypeConvertedValue.PadRight(propertyValueFixedLength, ' '));
                    break;

                case EncodingType.ASCIIInt:
                    if (propertyValue is Enum)
                    {
                        var temp = ((int)propertyValue).ToString();
                        enumTypeConvertedValue = temp.ToString().PadLeft(propertyValueFixedLength, '0');
                    }
                    else
                    {
                        enumTypeConvertedValue = propertyValue.ToString().PadLeft(propertyValueFixedLength, '0'); ;
                    }

                    return Encoding.ASCII.GetBytes(enumTypeConvertedValue);
                    break;
                case EncodingType.ASCIILong:
                    if (propertyValue is Enum)
                    {
                        var temp = ((long)propertyValue).ToString();
                        enumTypeConvertedValue = temp.ToString().PadLeft(propertyValueFixedLength, '0');
                    }
                    else
                    {
                        enumTypeConvertedValue = propertyValue.ToString().PadLeft(propertyValueFixedLength, '0'); ;
                    }

                    return Encoding.ASCII.GetBytes(enumTypeConvertedValue);
                    break;
                case EncodingType.BCD:

                    if (propertyValue is Enum)
                    {
                        enumTypeConvertedValue = ((int)propertyValue).ToString();
                    }
                    else
                    {
                        enumTypeConvertedValue = propertyValue.ToString();
                    }

                    return long.Parse(enumTypeConvertedValue).GetBCDBytes(propertyValueFixedLength);
                    break;
                case EncodingType.BcdString:

                    if (propertyValue is Enum)
                    {
                        enumTypeConvertedValue = ((int)propertyValue).ToString();
                    }
                    else
                    {
                        enumTypeConvertedValue = propertyValue.ToString();
                    }

                    return long.Parse(enumTypeConvertedValue).GetBCDBytes(propertyValueFixedLength);
                    break;
                case EncodingType.BIN:
                    if (propertyValue is Enum)
                    {
                        enumTypeConvertedValue = ((int)propertyValue).ToString();
                    }
                    else
                    {
                        enumTypeConvertedValue = propertyValue.ToString();
                    }

                    return (int.Parse(enumTypeConvertedValue)).GetBinBytes(propertyValueFixedLength);

                    break;
            }

            return null;
        }

        private byte[] ConvertToBytes(object elementInstance)
        {
            // Only processing MessageAttribute marked properties, and order by its Index in DESC, like 10, 9, 8, 7.... -1, -2...-10.
            var targetPropertyList = elementInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(p => p.GetCustomAttributes(typeof(AttributeBase), true).Length > 0)
                                        .OrderByDescending(info => ((AttributeBase)(info.GetCustomAttributes(typeof(AttributeBase), true)[0])).Index);
            foreach (var propertyInfo in targetPropertyList)
            {
                // normal Format Property
                if (propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true).Length == 0
                    && propertyInfo.GetCustomAttributes(typeof(FormatAttribute), true).Length > 0)
                {

                    #region validate the Range Attibute marked property, if existed and failed to pass, throw exception

                    // Validate the range, once failed, throw a exception
                    var propertyRangeAttributes = propertyInfo.GetCustomAttributes(typeof(RangeAttribute), true);
                    if (propertyRangeAttributes.Length > 0)
                    {
                        var rangeAttribute = (RangeAttribute)propertyRangeAttributes[0];
                        if (rangeAttribute != null)
                        {
                            if (!rangeAttribute.IsValid(propertyInfo.GetValue(elementInstance, null)))
                            {
                                throw new ArgumentException(rangeAttribute.ErrorMessage);
                            }
                        }
                    }

                    #endregion

                    var format =
                        (FormatAttribute)propertyInfo.GetCustomAttributes(typeof(FormatAttribute), true)[0];
                    var propertyValue = propertyInfo.GetValue(elementInstance, null);
                    // if this property value is null, then indicate it's a optional field, then skip to parsing it.
                    if (propertyValue == null)
                    {
                        var safeEvent = this.FieldSerializing;
                        if (safeEvent != null) { safeEvent(this, new ParsingEventArg<string, byte[]>() { From = propertyInfo.Name, To = null }); }
                        continue;
                    }

                    var safe = this.FieldSerializing;
                    if (safe != null) { safe(this, new ParsingEventArg<string, byte[]>() { From = propertyInfo.Name, To = null }); }

                    #region special case brought in in project KaJiLianDong, add a new command of `PositiveIndexLenAccumulator`

                    if (format.LengthOrCountLinkExpression != null && format.LengthOrCountLinkExpression.Contains("%PositiveIndexLenAccumulator%"))
                    {
                        //sss
                        //this.positiveIndexAccu = this.processingRawData.Length;
                        propertyInfo.SetValue(elementInstance, this.positiveIndexAccu, null);
                        var accuBytes = this.SerializePropertyData(format.EncodingType,
                            this.positiveIndexAccu, format.FixedLengthOrCount > 0 ? format.FixedLengthOrCount : 0,
                                                                      elementInstance);
                        this.processingRawData = this.processingRawData.AppendToHeader(accuBytes);
                        continue;
                    }

                    #endregion

                    byte[] currentPropertyBytes = this.SerializePropertyData(format.EncodingType,
                        propertyValue, format.FixedLengthOrCount > 0 ? format.FixedLengthOrCount : 0,
                                                                          elementInstance);
                    safe = this.FieldSerialized;
                    if (safe != null) { safe(this, new ParsingEventArg<string, byte[]>() { From = propertyInfo.Name, To = currentPropertyBytes }); }

                    this.processingRawData = this.processingRawData.AppendToHeader(currentPropertyBytes);
                    if (format.Index >= 0) this.positiveIndexAccu += currentPropertyBytes.Length;

                    // try fill the LengthLink field.
                    if (!string.IsNullOrEmpty(format.LengthOrCountLink))
                    {
                        var lengthLinkProperty = targetPropertyList.FirstOrDefault(t => t.Name == format.LengthOrCountLink);
                        if (lengthLinkProperty != null
                            && string.IsNullOrEmpty(format.LengthOrCountLinkExpression)
                            // if it's not a LengthOrCountLinkExpression field, then the linking field is directly reflected the length, so it must euqal 0
                            // if not, means the users code set the length explicitly, accept this behavior to NOT overwirte it.
                            && int.Parse(lengthLinkProperty.GetValue(elementInstance, null).ToString()) == 0)
                        {
                            lengthLinkProperty.SetValue(elementInstance, currentPropertyBytes.Length, null);
                        }

                        else if (false && !string.IsNullOrEmpty(format.LengthOrCountLinkExpression))
                        {
                            var pair = format.LengthOrCountLinkExpression.Split(';');
                            bool found = false;
                            foreach (string p in pair)
                            {
                                var mappings = p.Split(':');
                                if (currentPropertyBytes.Length == int.Parse(mappings[1]))
                                {
                                    //lengthLinkProperty.SetValue(elementInstance, int.Parse(mappings[0], null));
                                    lengthLinkProperty.SetValue(elementInstance, int.Parse(mappings[0], null), null);
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            { throw new ArgumentException("Serializing LengthOrCountLinkExpression: " + format.LengthOrCountLinkExpression + " didn't found match values in link field: " + format.LengthOrCountLink); }
                        }
                    }
                }
                // the Enumerable Property
                else if (propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true).Length > 0)
                {
                    var enumerableFormat =
                        (EnumerableFormatAttribute)propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true)[0];
                    // for now, we only support List<T>
                    if (propertyInfo.PropertyType.IsAssignableFrom(typeof(IList)))
                    {
                        throw new ArgumentException("For now, we only support Enumerable of List<T>");
                    }

                    var list = (IList)propertyInfo.GetValue(elementInstance, null);
                    if (list != null)
                    {
                        if (enumerableFormat.FixedLengthOrCount > 0)
                        {
                            if (enumerableFormat.FixedLengthOrCount != list.Count)
                            {
                                throw new ArgumentException("The current count(" + list.Count + ") of EnumerableFormat property: "
                                    + propertyInfo.Name + " must match its declared FixedLengthOrCount(" + enumerableFormat.FixedLengthOrCount + ")");
                            }
                        }

                        if (list.GetType().GetGenericArguments()[0].IsPrimitive)
                        {
                            var safe = this.FieldSerializing;
                            if (safe != null) { safe(this, new ParsingEventArg<string, byte[]>() { From = propertyInfo.Name, To = null }); }

                            byte[] primitiveSerilaizedBytes = new byte[0];
                            for (int i = list.Count - 1; i >= 0; i--)
                            {
                                primitiveSerilaizedBytes = primitiveSerilaizedBytes.AppendToHeader(
                                      this.SerializePropertyData(
                                          enumerableFormat.EncodingType,
                                          list[i],
                                          1,
                                          elementInstance)
                                      );
                            }

                            this.processingRawData = this.processingRawData.AppendToHeader(primitiveSerilaizedBytes);
                            if (enumerableFormat.Index >= 0) this.positiveIndexAccu += primitiveSerilaizedBytes.Length;
                            safe = this.FieldSerialized;
                            if (safe != null) { safe(this, new ParsingEventArg<string, byte[]>() { From = propertyInfo.Name, To = primitiveSerilaizedBytes }); }

                        }
                        else
                        {
                            // reverse
                            for (int i = list.Count - 1; i >= 0; i--)
                            {
                                this.processingRawData.AppendToHeader(this.ConvertToBytes(list[i]));
                            }
                        }


                        if (!string.IsNullOrEmpty(enumerableFormat.LengthOrCountLink) && enumerableFormat.LengthOrCountLink.ToLower() != "%cascade")
                        {
                            var countLinkProperty = targetPropertyList.First(t => t.Name == enumerableFormat.LengthOrCountLink);
                            if (string.IsNullOrEmpty(enumerableFormat.LengthOrCountLinkExpression)
                                // if it's not a LengthOrCountLinkExpression field, then the linking field is directly reflected the length, so it must euqal 0
                                // if not, then means the user code set the length or count explicitly, still accept this behavior.
                                && int.Parse(countLinkProperty.GetValue(elementInstance, null).ToString()) == 0)
                            {
                                if (countLinkProperty.PropertyType.IsAssignableFrom(typeof(Byte)))
                                {
                                    countLinkProperty.SetValue(elementInstance, Convert.ToByte(list.Count), null);
                                }
                                else if (countLinkProperty.PropertyType.IsAssignableFrom(typeof(int)))
                                {
                                    countLinkProperty.SetValue(elementInstance, list.Count, null);
                                }
                                else
                                {
                                    throw new ArgumentException("Now Only support countLinkProperty with types of 'byte' and 'int'");
                                }
                            }
                            else if (false && !string.IsNullOrEmpty(enumerableFormat.LengthOrCountLinkExpression))
                            {
                                var pair = enumerableFormat.LengthOrCountLinkExpression.Split(';');
                                bool found = false;
                                foreach (string p in pair)
                                {
                                    var mappings = p.Split(':');
                                    if (list.Count == int.Parse(mappings[1]))
                                    {
                                        //countLinkProperty.SetValue(elementInstance, int.Parse(mappings[0], null));
                                        this.FillPropertyData(enumerableFormat.EncodingType, countLinkProperty, elementInstance, new byte[] { byte.Parse(mappings[0], null) });

                                        found = true;
                                        break;
                                    }
                                }

                                if (!found) { throw new ArgumentException("Serializing LengthOrCountLinkExpression: " + enumerableFormat.LengthOrCountLinkExpression + " didn't found match values in link field: " + enumerableFormat.LengthOrCountLink); }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(enumerableFormat.LengthOrCountLink) && enumerableFormat.LengthOrCountLink.ToLower() != "%cascade")
                            {
                                targetPropertyList.First(t => t.Name == enumerableFormat.LengthOrCountLink).
                                    SetValue(elementInstance, 0, null);
                            }
                        }
                    }
                }
            }

            return this.processingRawData;
        }

        // make sure the Indexes property of one element including the sub elements 
        // of its list structures are sequential and unique
        protected void CheckAttributeIndexes(object elementInstance)
        {
            if (elementInstance == null)
            {
                return;
            }

            // Only processing WayneAttribute marked properties, and order by its Index in DESC
            var targetPropertyList = elementInstance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttributes(typeof(AttributeBase), true).Length > 0)
                .OrderByDescending(
                    info =>
                        ((AttributeBase)(info.GetCustomAttributes(typeof(AttributeBase), true)[0]))
                            .Index);

            // make sure no duplicated Index defined.
            targetPropertyList.GroupBy(
                p => ((AttributeBase)(p.GetCustomAttributes(typeof(AttributeBase), true)[0])).Index)
                .ToList()
                .ForEach(g =>
                {
                    if (g.Count() > 1)
                    {
                        throw new ArgumentException(
                            string.Format("Duplicated Index: {0} defined, make sure the Index value unique", g.Key));
                    }
                });

            // make sure the Indexes are contiguously sequential and count from 0
            if (targetPropertyList.Max(
                p => ((AttributeBase)(p.GetCustomAttributes(typeof(AttributeBase), true)[0])).Index) !=
                (targetPropertyList.Count() - 1))
            {
                throw new ArgumentException(string.Format("Index must be in sequential and count from 0"));
            }

            // recursively check the sub element type of ListPropertyFormatAttribute
            foreach (var propertyInfo in targetPropertyList)
            {
                // the IList Property
                if (propertyInfo.GetCustomAttributes(typeof(EnumerableFormatAttribute), true).Length != 0)
                {
                    var subElementInstance = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments()[0]);
                    CheckAttributeIndexes(subElementInstance);
                }
            }
        }
    }
}
