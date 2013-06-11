using System;

namespace AutoReminder.Model
{
    public class ReminderAction
    {
        private TimeSpan _timeBeforeDeadline;
        
        public ReminderActionType ActionType { get; set; }

        public TimeSpan TimeBeforeDeadLineExact
        {
            get { return TimeSpan.Parse(TimeBeforeDeadline); }
        }

        public string TimeBeforeDeadline
        {
            get { return _timeBeforeDeadline.ToString(); }
            set { _timeBeforeDeadline = TimeSpan.Parse(value); }
        }

        public ReminderAction()
        { }

        public ReminderAction(ReminderActionType actionType, TimeSpan timeSpan)
        {
            ActionType = actionType;
            _timeBeforeDeadline = timeSpan;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ReminderAction;

            if (other == null)
            {
                return false;
            }

            return ActionType == other.ActionType && TimeBeforeDeadLineExact == other.TimeBeforeDeadLineExact;
        }
    }
}
