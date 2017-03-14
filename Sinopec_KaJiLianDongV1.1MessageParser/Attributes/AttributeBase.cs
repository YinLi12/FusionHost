using System;

namespace MessageParser
{
    public class AttributeBase : Attribute
    {
        /// <summary>
        /// The index value which mapping to the field appearence in raw bytes, based on 0
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// encoding type.
        /// </summary>
        public EncodingType EncodingType { get; set; }


        /// <summary>
        /// If a var length field, then set the linked property name
        /// </summary>
        public string LengthOrCountLink { get; protected set; }

        /// <summary>
        /// If a fixed length field, then set the length value. otherwise, leave it.
        /// </summary>
        public int FixedLengthOrCount { get; protected set; }

        /// <summary>
        /// a message typically have a field to input the total length.
        /// </summary>
        //public bool IsMessageLengthField { get; set; }

        /// <summary>
        /// a field was turned on or off by another field with specific value
        /// Tuple<Tuple<string, byte[]>, int> means field 'string' control the appearance, if field 'string' equals 'byte[]', then
        /// the current field length is 'int'
        /// </summary>
        //public Tuple<Tuple<string, byte[]>, int> SwitcherLink { get; set; }

        /// <summary>
        /// all hex based: 00:04; means value 00 in LengthLink field mapping to length current field length 4.
        /// 0102:05; means values of 2 bytes 01 and 02 in LengthLink field mapping to current field length 5.
        /// can setup a group of expression, like: 00:04;0102:05;03:0801;
        /// MUST END WITH-> ;
        /// </summary>
        public string LengthOrCountLinkExpression { get; protected set; }
    }
}
