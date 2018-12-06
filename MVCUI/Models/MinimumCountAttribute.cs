using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCUI.Models
{

    [AttributeUsage(AttributeTargets.Property)]
    public class MinimumCountAttribute : ValidationAttribute
    {
        private int minCount = 0;
        private const string defaultError = "There must be at least {0}.";


        public MinimumCountAttribute(int min) : base(defaultError) //
        {
            minCount = min;
        }

        public override bool IsValid(object value)
        {
            IList list = value as IList;
            return (list != null && list.Count >= minCount);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(this.ErrorMessageString, minCount);
        }
    }
}