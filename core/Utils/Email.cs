using System.Linq;

namespace Ara3D.Utils
{
    /// <summary>
    /// Wraps a string instance intended to be used for email. 
    /// </summary>
    public struct Email
    {
        public Email(string value) => Value = value;
        public string Value { get; }
        public bool IsValid() => IsValid(Value);
        public static bool IsValid(string email) => email != null && email.Length >= 5 && email.Count(x => x == '@') == 1 && email.Contains(".") && !email.Contains(' ');
        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string value) => new Email(value);
        public static Email Default = "username@example.com";
    }

}
