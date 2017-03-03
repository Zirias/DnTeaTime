namespace PalmenIt.CoreTypes
{
    public class Error
    {
        private readonly string _message;

        public Error(string message)
        {
            _message = message ?? string.Empty;
        }

        public Error() : this(null)
        { }

        public virtual string Message { get { return _message; } }
    }
}
