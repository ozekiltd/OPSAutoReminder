using System.Collections.Generic;

namespace AutoReminder.Model
{
    public class Attendee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public List<string> Addresses  { get; private set; }
        public List<string> EmailAddresses { get; private set; }
        public List<string> PhoneNumbers { get; private set; }

        public Attendee()
        {
            Addresses = new List<string>();
            EmailAddresses = new List<string>();
            PhoneNumbers = new List<string>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Attendee;

            if (other == null)
            {
                return false;
            }

            return FullName == other.FullName;
        }
    }
}
