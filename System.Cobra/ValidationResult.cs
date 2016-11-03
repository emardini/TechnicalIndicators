using System.Collections.Generic;
using System.Linq;

namespace System.Cobra
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            errorMessages = new List<string>();
        }

        private List<string> errorMessages;

        public bool IsValid { get { return !errorMessages.Any(); } }

        public IEnumerable<string> ErrorMessages
        {
            get
            {
                return errorMessages;
            }
        }

        public void AddErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!errorMessages.Any(x => x.Equals(message, StringComparison.OrdinalIgnoreCase))) {
                errorMessages.Add(message);
            }           
        }

        public override string ToString()
        {
            return $"IsValid:{this.IsValid}, ErrorMessages:[{string.Join(",", this.ErrorMessages)}]";
        }
    }
}
