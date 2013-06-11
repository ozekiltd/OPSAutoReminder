using System;
using System.Linq;
using AutoReminder.Model;
using AutoReminder.Model.Outlook;
using AutoReminder.Model.Reminder;
using AutoReminder.Model.Settings;
using GalaSoft.MvvmLight.Messaging;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.ViewModel;

namespace AutoReminder.ViewModel
{
    public class MainViewModel : CommonMainViewModel<Appointment>
    {
        public static string ImportFromOutlook = Guid.NewGuid().ToString();
        public static string GetReminderEntries = Guid.NewGuid().ToString();
        public static string BuildReminderEntries = Guid.NewGuid().ToString();
        public static string ShowApiExtensionWarning = Guid.NewGuid().ToString();
        public static string ShowAttendeesWarning = Guid.NewGuid().ToString();
        public static string ShowImportingFailed = Guid.NewGuid().ToString();

        private IOutlookCalendarParser _outlookParser;
        private IGenericSettingsRepository<AppPreferences> _settingsRepository;
        private Reminder _reminder;

        public ObservableCollectionEx<Appointment> Appointments { get; set; }
        public ObservableCollectionEx<ReminderAction> ReminderActions { get; set; }
        public ObservableCollectionEx<ReminderActionEntry> ReminderActionLogEntries { get; set; }

        public MainViewModel()
        {
            _outlookParser = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IOutlookCalendarParser>();
            _outlookParser.AppointmentFound += OutlookParserOnAppointmentFound;
            _outlookParser.AppointmentsParsed += OutlookParserOnAppointmentsParsed;
            _settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
            _reminder = new Reminder();
            _reminder.ReminderActionDone += ReminderActionDone;

            Appointments = new ObservableCollectionEx<Appointment>();
            ReminderActions = new ObservableCollectionEx<ReminderAction>();

            InitSettings();
            BuildReminderActionEntries();

            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);
        }

        private void OutlookParserOnAppointmentsParsed(object sender, EventArgs eventArgs)
        {
            Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));

            var settings = _settingsRepository.GetSettings();
            settings.Appointments = Appointments;

            _settingsRepository.SetSettings(settings);
            BuildReminderActionEntries();
        }

        private void OutlookParserOnAppointmentFound(object sender, Appointment appointment)
        {
            var existingAppointments = Appointments.Where(ap => ap.CreationTime == appointment.CreationTime && ap.Subject == appointment.Subject);

            foreach (var existingAppointment in existingAppointments.ToList())
                Appointments.Remove(existingAppointment);

            Appointments.Add(appointment);
        }

        private void InitSettings()
        {
            var settings = _settingsRepository.GetSettings();

            if (settings != null)
            {
                ReminderActions = settings.ReminderActions;

                foreach (var appointment in settings.Appointments.Where(appointment => appointment.StartTime > DateTime.Now))
                    OutlookParserOnAppointmentFound(null, appointment);

                settings.Appointments = Appointments;
                _settingsRepository.SetSettings(settings);
            }
            else
                _settingsRepository.SetSettings(AppPreferences.DefaultPreferences);
        }

        private void InitStart()
        {
            var settings = _settingsRepository.GetSettings();
            _outlookParser.Start(settings);
            Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindowLoading));
        }

        private void GetReminderActionEntries()
        {
            ReminderActionLogEntries = _settingsRepository.GetSettings().ReminderActionEntries;
            RaisePropertyChanged("ReminderActionLogEntries");
        }

        private void BuildReminderActionEntries()
        {
            var settings = _settingsRepository.GetSettings();

            settings = ReminderActionEntryBuilder.BuildEntries(settings);
            _settingsRepository.SetSettings(settings);

            GetReminderActionEntries();
        }


        private void MessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == ImportFromOutlook)
                InitStart();
            else if (notificationMessage.Notification == GetReminderEntries)
                GetReminderActionEntries();
            else if (notificationMessage.Notification == BuildReminderEntries)
                BuildReminderActionEntries();
        }

        private void ReminderActionDone(object sender, EventArgs e)
        {
            GetReminderActionEntries();
        }
    }
}