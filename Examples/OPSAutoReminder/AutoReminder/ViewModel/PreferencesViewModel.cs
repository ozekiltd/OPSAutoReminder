using System;
using System.Linq;
using AutoReminder.Model;
using AutoReminder.Model.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.ViewModel;

namespace AutoReminder.ViewModel
{
    class PreferencesViewModel : ViewModelBase
    {
        private IGenericSettingsRepository<AppPreferences> _settingsRepository;

        public RelayCommand Ok { get; set; }
        public RelayCommand Cancel { get; set; }
        public RelayCommand Add { get; set; }
        public RelayCommand Delete { get; set; }

        public AppPreferences Preferences { get; set; }

        public ReminderActionType ReminderActionType { get; set; }
        public ReminderAction SelectedReminderAction { get; set; }
        public int ReminderDays { get; set; }
        public int ReminderHours { get; set; }
        public int ReminderMinutes { get; set; }
        public int ReminderSeconds { get; set; }

        public PreferencesViewModel()
        {
            _settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();

            Preferences = new AppPreferences(_settingsRepository.GetSettings());
            InitCommands();
        }

        private void InitCommands()
        {
            Ok = new RelayCommand(() =>
                                      {
                                          _settingsRepository.SetSettings(Preferences);

                                          
                                          Messenger.Default.Send(new NotificationMessage(Messages.DismissSettingsWindow));
                                          Messenger.Default.Send(new NotificationMessage(MainViewModel.BuildReminderEntries));
                                          if (string.IsNullOrEmpty(Preferences.ApiExtensionId))
                                          {
                                              Messenger.Default.Send(new NotificationMessage(MainViewModel.ShowApiExtensionWarning));
                                          }
                                      });

            Cancel = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage(Messages.DismissSettingsWindow)));

            Add = new RelayCommand(InitAdd);
            Delete = new RelayCommand(InitDelete);
        }

        private void InitDelete()
        {
            if (Preferences.ReminderActions.Contains(SelectedReminderAction))
                Preferences.ReminderActions.Remove(SelectedReminderAction);

            RaisePropertyChanged("Preferences.ReminderActions");
        }

        private void InitAdd()
        {
            if (ReminderDays == 0 && ReminderHours == 0 && ReminderMinutes == 0 && ReminderSeconds == 0)
                return;

            var span = new TimeSpan(ReminderDays, ReminderHours, ReminderMinutes, ReminderSeconds);
            var reminderAction = new ReminderAction(ReminderActionType, span);

            if (!Preferences.ReminderActions.Contains(reminderAction))
                Preferences.ReminderActions.Add(reminderAction);

            if (Preferences.Appointments.Any(a => a.Attendees.Count == 0))
            {
                Messenger.Default.Send(new NotificationMessage(MainViewModel.ShowAttendeesWarning));   
            }

            RaisePropertyChanged("Preferences.ReminderActions");
        }
    }
}
