using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace magazynek.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableFieldAttribute : Attribute
    {
        public enum InputType
        {
            Text,
            Number,
            Checkbox,
            Dropdown
        }
        public string Label { get; }
        public bool IsEditable { get; }
        public InputType Type { get; }

        public EditableFieldAttribute(string Label, bool IsEditable = true, InputType Type = InputType.Text)
        {
            this.Label = Label;
            this.IsEditable = IsEditable;
            this.Type = Type;
        }
    }
}