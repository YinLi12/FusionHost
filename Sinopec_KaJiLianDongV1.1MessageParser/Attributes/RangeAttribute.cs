using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessageParser
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// default have 3 types of validation, the value based on 0, if need extend, then increase the value
        /// </summary>
        private int rangValidationType = -1;
        private string errorMsg = string.Empty;
        private string validatedValue = string.Empty;

        private int intMin = -1;
        private int intMax = -1;

        private decimal decimalMin = -1;
        private decimal decimalMax = -1;

        private IEnumerable<int> allowedIntValues = null;

        private Type validatingType = null;
        private string regexString = string.Empty;
        private RegexOptions regexOptions;

        /// <summary>
        /// Range validation for int type values
        /// </summary>
        /// <param name="min">min values</param>
        /// <param name="max">max values</param>
        /// <param name="errorMessage">error message to show, support String.Format, the {0}, {1} and {2} are auto set to concrete value, min and max.</param>
        public RangeAttribute(int min, int max, string errorMessage)
        {
            this.intMin = min;
            this.intMax = max;
            this.errorMsg = errorMessage;
            this.rangValidationType = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="errorMessage">error message to show, support String.Format, the {0}, {1} and {2} are auto set to concrete value, min and max.</param>
        public RangeAttribute(decimal min, decimal max, string errorMessage)
        {
            this.decimalMin = min;
            this.decimalMax = max;
            this.errorMsg = errorMessage;
            this.rangValidationType = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="regexString"></param>
        /// <param name="errorMessage">error message to show, support String.Format, the {0}, {1} and {2} are auto set to concrete value, type.Name and regexString.</param>
        public RangeAttribute(Type type, string regexString, RegexOptions regexOptions, string errorMessage)
        {
            this.validatingType = type;
            this.regexString = regexString;
            this.regexOptions = regexOptions;
            this.errorMsg = errorMessage;
            this.rangValidationType = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="errorMessage">error message to show, support String.Format, the {0}, {1} are auto set to concrete value, allowed ints string.</param>
        public RangeAttribute(int[] allowedValues, string errorMessage)
        {
            this.allowedIntValues = allowedValues;
            this.errorMsg = errorMessage;
            this.rangValidationType = 3;
        }

        public new string ErrorMessage
        {
            get
            {
                switch (this.rangValidationType)
                {
                    case 0:
                        return string.Format(this.errorMsg, this.validatedValue, this.intMin, this.intMax);
                    case 1:
                        return string.Format(this.errorMsg, this.validatedValue, this.decimalMin, this.decimalMax);
                    case 2:
                        return string.Format(this.errorMsg, this.validatedValue, this.validatingType, this.regexString);
                    case 3:
                        return string.Format(this.errorMsg, this.validatedValue, this.allowedIntValues.Cast<string>().Aggregate((p, acc) => p + ", " + acc));
                }

                return this.errorMsg;
            }
            //private set { this.errorMsg = value; }
        }

        public override bool IsValid(object value)
        {
            this.validatedValue = value.ToString();
            switch (this.rangValidationType)
            {
                case 0:
                    int targetInt = -1;
                    if (!int.TryParse(value.ToString(), out targetInt))
                    {
                        return false;
                    }

                    return targetInt >= this.intMin && targetInt <= this.intMax;
                case 1:
                    decimal targetDecimal = -1;
                    if (!decimal.TryParse(value.ToString(), out targetDecimal))
                    {
                        return false;
                    }

                    return targetDecimal >= this.decimalMin && targetDecimal <= this.decimalMax;
                case 2:
                    return Regex.IsMatch(value.ToString(), this.regexString, this.regexOptions);
                case 3:
                    int tempInt = -1;
                    if (!int.TryParse(value.ToString(), out tempInt))
                    {
                        return false;
                    }

                    return this.allowedIntValues.Contains(tempInt);
            }

            return false;
        }
    }
}
