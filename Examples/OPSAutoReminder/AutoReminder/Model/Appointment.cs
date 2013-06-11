using System;
using OzCommon.Utils;

namespace AutoReminder.Model
{
    public class Appointment : EventArgs
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Duration { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreationTime { get; set; }

        public ObservableCollectionEx<Attendee> Attendees { get; set; }

        public Appointment()
        {
            Attendees = new ObservableCollectionEx<Attendee>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Appointment;

            if (other == null)
            {
                return false;
            }

            return CreationTime == other.CreationTime;
        }
    }
}
