using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCUI.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CountLimitAttribute : ValidationAttribute
    {
        private const string defaultError = "There cannot be more than {0}.";
        private int maxCount;

        public CountLimitAttribute(int max) : base(defaultError) //
        {
            maxCount = max;
        }

        public override bool IsValid(object value)
        {
            IList list = value as IList;
            return (list != null && list.Count <= maxCount);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(this.ErrorMessageString, maxCount);
        }
    }
}