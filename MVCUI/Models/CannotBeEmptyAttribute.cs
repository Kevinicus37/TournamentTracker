using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCUI.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CannotBeEmptyAttribute : ValidationAttribute
    {
        private const string defaultError = "Must select at least one {0}.";

        public CannotBeEmptyAttribute() : base(defaultError) //
        {
        }

        public override bool IsValid(object value)
        {
            IList list = value as IList;
            return (list != null && list.Count > 0);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(this.ErrorMessageString, name);
        }
    }
}