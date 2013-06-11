using System;
using System.Linq;
using AutoReminder.Model.Settings;

namespace AutoReminder.Model
{
    public class ReminderActionEntryBuilder
    {
        public static AppPreferences BuildEntries(AppPreferences settings)
        {
            foreach (var appointment in settings.Appointments)
                foreach (var reminderAction in settings.ReminderActions)
                    foreach (var attendee in appointment.Attendees)
                    {
                        if (settings.ReminderActionEntries.Any(
                                entry =>entry.Appointment.Equals(appointment) && entry.Attendee.Equals(attendee) &&
                                entry.ReminderAction.Equals(reminderAction)))
                            continue;

                        var reminderActionEntry = new ReminderActionEntry();

                        reminderActionEntry.Appointment = appointment;
                        reminderActionEntry.Attendee = attendee;
                        reminderActionEntry.ReminderAction = reminderAction;
                        reminderActionEntry.ReminderState = ReminderActionState.Pending;
                        reminderActionEntry.ReminderDate = appointment.StartTime.Subtract(reminderAction.TimeBeforeDeadLineExact);

                        settings.ReminderActionEntries.Add(reminderActionEntry);
                    }

            return settings;
        }
    }
}
