using System;
using System.ComponentModel;

namespace AutoReminder.Model
{
    public class ReminderActionEntry : INotifyPropertyChanged
    {
        private ReminderActionState _reminderState;
        public event PropertyChangedEventHandler PropertyChanged;

        public Attendee Attendee { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime ReminderDate { get; set; }
        public ReminderAction ReminderAction { get; set; }

        public ReminderActionState ReminderState
        {
            get { return _reminderState; }
            set
            {
                _reminderState = value;
                OnPropertyChanged("ReminderState");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
