using System;

namespace MessageParser
{
    /// <summary>
    /// The message parsing is defined as 2 direction convert, 
    /// we say the 'forward' direction convert is from an unreadable format data to a readable static type object, like the bytes to object, this is called Deserialize.
    /// vice versa, the 'backward' direction convert is covert a readable staic type object to an unreadable format data, like covert the object to bytes, this is called Serialize. 
    /// </summary>
    /// <typeparam name="TRaw">Type for unreadable format data</typeparam>
    /// <typeparam name="TTemplate">Type for readable object</typeparam>
    public interface IMessageParser<TRaw, TTemplate> where TTemplate : MessageTemplateBase
    {
        event EventHandler<ParsingEventArg<TRaw, TTemplate>> Deserializing;
        event EventHandler<ParsingEventArg<TRaw, TTemplate>> Deserialized;
        event EventHandler<ParsingEventArg<TTemplate, TRaw>> Serializing;
        event EventHandler<ParsingEventArg<TTemplate, TRaw>> Serialized;
        event EventHandler<ParsingEventArg<string, TRaw>> FieldSerializing;
        event EventHandler<ParsingEventArg<string, TRaw>> FieldSerialized;
        TTemplate Deserialize(TRaw data);
        TRaw Serialize(TTemplate message);
    }
}
