using System;

namespace MessageParser
{
    /// <summary>
    /// only for ListT structure, not for primitive array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    class EnumerableFormatAttribute : AttributeBase
    {
        private EnumerableFormatAttribute()
        {
            // default set to 1 since the most possible used primitive type is byte. 
            this.ElementLength = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fixedCount">The current Enumerable field has a constant count. like 2,3</param>
        /// <param name="index"></param>
        public EnumerableFormatAttribute(int fixedCount,  int index)
            : this()
        {
            base.FixedLengthOrCount = fixedCount;
            base.Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countLink">the current Enumerable field is variable count and determined by another field with numerical value, and the 
        /// numerical value directly reflect the original field count, like field 'IamTheCountLink' has the value 8, then the 
        /// linking field count is 8.</param>
        /// <param name="index"></param>
        public EnumerableFormatAttribute(string countLink, int index)
            : this()
        {
            base.LengthOrCountLink = countLink;
            base.Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countLink">the current Enumerable field is variable count and determined by another field with numerical value, and the 
        /// numerical value is NOT directly reflect the original field length, have to use the argument 'countLinkExpression' together
        /// to detect the length, like field 'IamTheCountLink' has the value 8, and the 'countLinkExpression' contains "8:12;", then the 
        /// original field count is actually 12.</param>
        /// <param name="countLinkExpression"></param>
        /// <param name="index"></param>
        public EnumerableFormatAttribute(string countLink, string countLinkExpression, int index)
            : this()
        {
            base.LengthOrCountLink = countLink;
            base.LengthOrCountLinkExpression = countLinkExpression;
            base.Index = index;
        }

        /// <summary>
        /// If a fixed length field, then set the length value. otherwise, leave it.
        /// </summary>
        public int ElementLength { get; private set; }
    }

}
