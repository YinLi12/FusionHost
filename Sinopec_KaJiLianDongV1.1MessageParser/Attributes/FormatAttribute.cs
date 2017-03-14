using System;

namespace MessageParser
{
    [AttributeUsage(AttributeTargets.Property)]
    class FormatAttribute : AttributeBase
    {
        private FormatAttribute() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fixedLength">the field length is a constant fixed value, like 1, 2.</param>
        /// <param name="encodingType"></param>
        /// <param name="index"></param>
        public FormatAttribute(int fixedLength, EncodingType encodingType, int index)
        {
            base.FixedLengthOrCount = fixedLength;
            base.EncodingType = encodingType;
            base.Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lengthLink">the field is a variable length and determined by another field with numerical value, and the 
        /// numerical value directly reflect the original field length, like field 'IamTheLengthLink' has the value 8, then the 
        /// linking field length is 8.</param>
        /// <param name="encodingType"></param>
        /// <param name="index"></param>
        public FormatAttribute(string lengthLink, EncodingType encodingType, int index)
        {
            base.LengthOrCountLink = lengthLink;
            base.EncodingType = encodingType;
            base.Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lengthLink">the field is a variable length and determined by another field with numerical value, and the 
        /// numerical value is NOT directly reflect the original field length, have to use the argument 'lengthLinkExpression' together
        /// to detect the length, like field 'IamTheLengthLink' has the value 8, and the 'lengthLinkExpression' contains "8:12;", then the 
        /// original field length is actually 12.</param>
        /// <param name="lengthLinkExpression">like "3:8;A:9", means field 'lengthLink' has value 3, then original field length is 8, if has 
        /// value A, then original field length is 9</param>
        /// <param name="encodingType"></param>
        /// <param name="index"></param>
        public FormatAttribute(string lengthLink, string lengthLinkExpression, EncodingType encodingType, int index)
        {
            base.LengthOrCountLink = lengthLink;
            base.LengthOrCountLinkExpression = lengthLinkExpression;
            base.EncodingType = encodingType;
            base.Index = index;
        }

        public FormatAttribute(int fixedLength, string operationExpression, EncodingType encodingType, int index)
        {
            base.FixedLengthOrCount = fixedLength;
            base.LengthOrCountLink = "";
            base.LengthOrCountLinkExpression = operationExpression;
            base.EncodingType = encodingType;
            base.Index = index;
        }

        //public FormatAttribute(string switchedBy, byte[] determinedBy, int index)
        //{
        //    this.LengthLink = lengthLink;
        //    base.EncodingType = encodingType;
        //    base.Index = index;
        //}

        /// <summary>
        /// it's a variable length, and no LengthLink
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="index"></param>
        //public FormatAttribute(EncodingType encodingType, int index)
        //{
        //    base.EncodingType = encodingType;
        //    base.Index = index;
        //}





    }

    [Flags]
    public enum EncodingType
    {
        BIN = 0,
        /// <summary>
        /// BCD encoded in bytes, like 0x64, after BCD decoded, it's 64 in decimal base.
        /// output int value.
        /// </summary>
        BCD = 1,

        /// <summary>
        /// ASCII encoded in bytes, like 0x3233, after ASCII decoded, it's a string of 23.
        /// If fixed length specified, then pad right with ' '.
        /// </summary>
        ASCII = 2,

        /// <summary>
        /// BCD encoded in bytes, but the output is a string, like 0x02 56 01 28 64 32 16 08 04 02 01 will turn to 0256012864321608040201.
        /// typically this kind of case should choose ASCII, but somehow...
        /// </summary>
        BcdString = 4,

        /* following are some 'special' encoding type, most likely the protocol maker
            made some unconsistent choice, we have to support it.
         */

        /// <summary>
        /// ASCII encoded in bytes,like 0x3233 output is a int 23.
        /// </summary>
        ASCIIInt = 8,

        /// <summary>
        /// ASCII encoded in bytes,like 0x3233 output is a long 23.
        /// </summary>
        ASCIILong = 16,
    }
}
