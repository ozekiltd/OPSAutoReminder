using OzCommon.Utils;

namespace AutoReminder.Model.Settings
{
    public class AppPreferences
    {
        public static AppPreferences DefaultPreferences
        {
            get
            {
                return new AppPreferences
                                      {
                                          EmailSelectionType = SelectionType.First,
                                          SmsPhoneSelectionType = SelectionType.First,
                                          CallPhoneSelectionType = SelectionType.First,
                                          EmailMessageTemplate =
                                              "Dear $fullName,\r\n\r\nThis is a reminder e-mail about the upcoming \"$subject\" event which will start at $startTime. The event will take place at $location " +
                                              "and will last about $duration minutes. The event's topic and description can be read below. \r\n\r\n$description",
                                          SmsMessageTemplate =
                                              "This is a reminder sms for the upcoming \"$subject\" event starting at $startTime, $location.",
                                          CallTemplate =
                                              "This is a reminder call for the upcoming $subject event starting at $startTime, $location"
                                      };
            }
        }

        public ObservableCollectionEx<ReminderAction> ReminderActions { get; set; }
        public ObservableCollectionEx<ReminderActionEntry> ReminderActionEntries { get; set; } 
        public ObservableCollectionEx<Appointment> Appointments { get; set; }

        public string EmailMessageTemplate { get; set; }
        public string SmsMessageTemplate { get; set; }
        public string CallTemplate { get; set; }
        public string ApiExtensionId { get; set; }
        public string SenderEmailAddress { get; set; }

        public SelectionType SmsPhoneSelectionType { get; set; }
        public SelectionType CallPhoneSelectionType { get; set; }
        public SelectionType EmailSelectionType { get; set; }

        public AppPreferences()
        {
            ReminderActions = new ObservableCollectionEx<ReminderAction>();
            ReminderActionEntries = new ObservableCollectionEx<ReminderActionEntry>();
            Appointments = new ObservableCollectionEx<Appointment>();
        }

        public AppPreferences(AppPreferences preferences)
        {
            ReminderActions = new ObservableCollectionEx<ReminderAction>();
            ReminderActionEntries = new ObservableCollectionEx<ReminderActionEntry>();
            Appointments = new ObservableCollectionEx<Appointment>();

            ReminderActionEntries.AddItems(preferences.ReminderActionEntries);
            ReminderActions.AddItems(preferences.ReminderActions);
            Appointments.AddItems(preferences.Appointments);

            EmailMessageTemplate = preferences.EmailMessageTemplate;
            SmsMessageTemplate = preferences.SmsMessageTemplate;
            CallTemplate = preferences.CallTemplate;
            ApiExtensionId = preferences.ApiExtensionId;

            SmsPhoneSelectionType = preferences.SmsPhoneSelectionType;
            CallPhoneSelectionType = preferences.CallPhoneSelectionType;
            EmailSelectionType = preferences.EmailSelectionType;

            SenderEmailAddress = preferences.SenderEmailAddress;
        }
    }
}
