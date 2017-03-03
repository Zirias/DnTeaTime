namespace PalmenIt.CoreTypes
{
    public class ValidationError<TValue> : Error
    {
        public string Name { get; }
        public TValue Value { get; }
        public string Description { get; }

        public ValidationError(string name, TValue value, string description) : base()
        {
            Name = name;
            Value = value;
            Description = description;
        }

        public override string Message
        {
            get
            {
                return string.Format("Validation error for `{0}'. The Value `{1}' had the following problem: {2}",
                    Name, Value, Description);
            }
        }
    }
}
