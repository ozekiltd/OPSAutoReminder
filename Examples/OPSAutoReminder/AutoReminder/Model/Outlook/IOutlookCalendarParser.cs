using System;
using AutoReminder.Model.Settings;

namespace AutoReminder.Model.Outlook
{
    interface IOutlookCalendarParser
    {
        event EventHandler<Appointment> AppointmentFound;
        event EventHandler AppointmentsParsed;

        void Start(AppPreferences settins);
        void Cancel();

        bool Parsing { get; }
    }
}
