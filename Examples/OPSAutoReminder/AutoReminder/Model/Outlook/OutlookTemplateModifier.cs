using System.Linq;
using AutoReminder.Utils.MessageModifier;

namespace AutoReminder.Model.Outlook
{
    class OutlookTemplateModifier : TemplateModifier
    {
        public OutlookTemplateModifier()
        {
            //Appointment parameters
            Register(new MessageReplacer("$location"));
            Register(new MessageReplacer("$subject"));
            Register(new MessageReplacer("$duration"));
            Register(new MessageReplacer("$description"));
            Register(new MessageReplacer("$startTime"));
            Register(new MessageReplacer("$endTime"));
            Register(new MessageReplacer("$creationTime"));

            //Attendee parameters
            Register(new MessageReplacer("$emailAddress"));
            Register(new MessageReplacer("$phoneNumber"));
            Register(new MessageReplacer("$address"));
            Register(new MessageReplacer("$firstName"));
            Register(new MessageReplacer("$lastName"));
            Register(new MessageReplacer("$fullName"));
        }

        public MessageContext CreateCurrentContext(Appointment appointment, Attendee attendee)
        {
            var messageContext = new MessageContext();

            //Appointment parameters
            messageContext.SetValue("$location", appointment.Location);
            messageContext.SetValue("$subject", appointment.Subject);
            messageContext.SetValue("$description", appointment.Description);
            messageContext.SetValue("$startTime", appointment.StartTime.ToString());
            messageContext.SetValue("$endTime", appointment.EndTime.ToString());
            messageContext.SetValue("$creationTime", appointment.CreationTime.ToString());
            messageContext.SetValue("$duration", appointment.Duration.ToString());

            //Attendee parameters
            messageContext.SetValue("$emailAddress", attendee.EmailAddresses.FirstOrDefault());
            messageContext.SetValue("$phoneNumber", attendee.PhoneNumbers.FirstOrDefault());
            messageContext.SetValue("$address", attendee.Addresses.FirstOrDefault());
            messageContext.SetValue("$firstName", attendee.FirstName);
            messageContext.SetValue("$lastName", attendee.LastName);
            messageContext.SetValue("$fullName", attendee.FullName);

            return messageContext;
        }
    }
}
