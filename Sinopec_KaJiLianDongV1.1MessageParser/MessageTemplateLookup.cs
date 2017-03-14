using MessageParser.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MessageParser
{
    /// <summary>
    /// Define the element code to its class object mapping relationship.
    /// </summary>
    public class MessageTemplateLookup : IMessageTemplateLookup
    {
        private static readonly MessageTemplateLookup defaultInstance = new MessageTemplateLookup();

        /// <summary>
        /// Gets the default singleton instance of type MessageTemplateLookup.
        /// </summary>
        public static MessageTemplateLookup Default => defaultInstance;

        /// <summary>
        /// resolve from the config
        /// </summary>
        private static readonly IEnumerable<Mapping> lookup = MessageEntityMappingConfig.GetConfig().Mappings.Cast<Mapping>();

        /// <summary>
        /// Create a message entity based on input whole message raw bytes which is: 2bytes Len + 1 byte AppId + 1 byte SSK 
        ///     + variable length message code + variable length message body
        /// </summary>
        /// <param name="bytes">whole message raw bytes</param>
        /// <returns>new created message entity</returns>
        public MessageTemplateBase GetMessageTemplateByRawBytes(byte[] bytes)
        {
            try
            {
                string debug = bytes.ToHexLogString();//be convenient to see log

                var type = GetMessageTemplateTypeByRawBytes(bytes);
                var targetInstance = (MessageTemplateBase)Activator.CreateInstance(type);
                return targetInstance;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("exception message = " + ex.Message);
            }

        }

        /// <summary>
        /// Create a message entity based on input whole message raw bytes which is: 2bytes Len + 1 byte AppId + 1 byte SSK 
        ///     + variable length message code + variable length message body
        /// </summary>
        /// <param name="bytes">whole message raw bytes</param>
        /// <returns>new created message entity</returns>
        private Type GetMessageTemplateTypeByRawBytes(byte[] bytes)
        {
            //ybs??
            string debug = bytes.Select(s => s.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n);//be convenient to see log
            //trace.TraceInformation("in the method GetMessageEntityTypeByRawBytes, debug=" + debug);

            // from protocol definition, the msg body started at 7th byte, and the 7th bytes is always the msg type code.
            var msgHandleCode = bytes.Skip(6).First();
            if (lookup.Any(a => a.Code.First() == msgHandleCode))
            {
                return Assembly.GetAssembly(typeof(MessageTemplateBase)).GetType(lookup.First(a => a.Code.First() == msgHandleCode).TypeRawString);
            }
            else
            {
                throw new ArgumentException("Can't find correlated message entity type for incoming raw message bytes: " + bytes.Select(s => s.ToString("X").PadLeft(2, '0')).Aggregate((n, acc) => n + " " + acc));
            }
        }
    }
}
