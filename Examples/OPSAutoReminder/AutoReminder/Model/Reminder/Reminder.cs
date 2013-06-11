using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using AutoReminder.Model.Outlook;
using AutoReminder.Model.Settings;
using AutoReminder.Utils.MessageModifier;
using AutoReminder.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using OPSSDK;
using OPSSDKCommon.Model.Message;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.ViewModel;
using Ozeki.Media.MediaHandlers;
using Timer = System.Timers.Timer;

namespace AutoReminder.Model.Reminder
{
    public class Reminder
    {
        public event EventHandler ReminderActionDone;

        private IGenericSettingsRepository<AppPreferences> _settingsRepository;
        private OutlookTemplateModifier _templateModifier;
        private Timer _reminderTimer;
        private IClient _client;
        private IDialogService _dialogService;

        public Reminder()
        {
            _settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
            _client = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IClient>();
            _dialogService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IDialogService>();
            _client.ErrorOccurred += ClientOnErrorOccurred;

            _templateModifier = (OutlookTemplateModifier)GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ITemplateModifier>();

            _reminderTimer = new Timer(2500);
            _reminderTimer.Elapsed += ReminderTimerOnElapsed;
            _reminderTimer.Start();

        }


        private void ClientOnErrorOccurred(object sender, ErrorInfo errorInfo)
        {
            switch (errorInfo.Type)
            {
                case ErrorType.ConnectionFailure:
                    _dialogService.ShowMessageBox("Network connection error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case ErrorType.VersionMismatch:
                    _dialogService.ShowMessageBox("Version mismatch error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    _dialogService.ShowMessageBox("Unknown error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        private void ReminderTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _reminderTimer.Stop();

            var settings = _settingsRepository.GetSettings();
            var appointments = settings.Appointments;
            var entries = settings.ReminderActionEntries.ToList();

            try
            {
                foreach (var reminderActionEntry in entries.Where(entry => entry.ReminderState == ReminderActionState.Pending))
                {
                    if (DateTime.Now < reminderActionEntry.ReminderDate)
                        continue;

                    try
                    {
                        InitiateReminderAction(reminderActionEntry.Appointment, reminderActionEntry.ReminderAction,
                                               reminderActionEntry.Attendee, settings);
                    }
                    catch (Exception)
                    {
                    }

                    reminderActionEntry.ReminderState = ReminderActionState.Done;

                    settings.Appointments = appointments;
                    settings.ReminderActionEntries = new ObservableCollectionEx<ReminderActionEntry>();
                    settings.ReminderActionEntries.AddItems(entries);

                    _settingsRepository.SetSettings(settings);

                    OnReminderActionDone(EventArgs.Empty);
                }
            }
            catch (Exception) { }

            _reminderTimer.Start();
        }

        private void InitiateReminderAction(Appointment appointment, ReminderAction reminderAction, Attendee attendee, AppPreferences settings)
        {

            IAPIExtension apiExtension = null;
            if (settings.ApiExtensionId != null)
            {
                apiExtension = _client.GetAPIExtension(settings.ApiExtensionId);
                if (apiExtension == null)
                {
                    Messenger.Default.Send(new NotificationMessage(MainViewModel.ShowApiExtensionWarning));
                }
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage(MainViewModel.ShowApiExtensionWarning));
            }

            if (apiExtension == null)
            {
                return;
            }

            List<string> emailAddresses = new List<string>();
            List<string> smsNumbers = new List<string>();
            List<string> callNumbers = new List<string>();

            if (settings.EmailSelectionType == SelectionType.First)
            {
                var email = attendee.EmailAddresses.FirstOrDefault();
                if (email != null)
                {
                    emailAddresses.Add(email);
                }
            }
            else
            {
                emailAddresses.AddRange(attendee.EmailAddresses);
            }

            if (settings.SmsPhoneSelectionType == SelectionType.First)
            {
                var num = attendee.PhoneNumbers.FirstOrDefault();
                if (num != null)
                {
                    smsNumbers.Add(num);
                }
            }
            else
            {
                emailAddresses.AddRange(attendee.PhoneNumbers);
            }

            if (settings.CallPhoneSelectionType == SelectionType.First)
            {
                var num = attendee.PhoneNumbers.FirstOrDefault();
                if (num != null)
                {
                    callNumbers.Add(num);
                }
            }
            else
            {
                callNumbers.AddRange(attendee.PhoneNumbers);
            }


            switch (reminderAction.ActionType)
            {
                case ReminderActionType.Call:
                    {
                        foreach (var number in callNumbers)
                        {

                            var call = apiExtension.CreateCall(number.Replace(" ", ""));
                            ManualResetEvent callDone = new ManualResetEvent(false);
                            call.CallStateChanged += (sender, args) =>
                                                        {
                                                            if (args.Item.IsInCall())
                                                            {
                                                                TextToSpeech speech = new TextToSpeech();
                                                                speech.Stopped += (o, eventArgs) => call.HangUp();
                                                                call.ConnectAudioSender(speech);

                                                                var message = settings.CallTemplate;
                                                                var context = _templateModifier.CreateCurrentContext(appointment, attendee);
                                                                message = _templateModifier.Modify(message, context);
                                                                speech.AddAndStartText(message);
                                                            }
                                                            else if (args.Item.IsCallEnded())
                                                            {
                                                                callDone.Set();
                                                            }
                                                        };
                            call.CallErrorOccurred += (sender, args) => callDone.Set();
                            call.Start();
                            callDone.WaitOne();
                        }

                        break;
                    }

                case ReminderActionType.Sms:
                    {
                        foreach (var number in smsNumbers)
                        {
                            var message = settings.SmsMessageTemplate;
                            var context = _templateModifier.CreateCurrentContext(appointment, attendee);
                            message = _templateModifier.Modify(message, context);
                            apiExtension.SendMessage(new SMSMessage(number, message));
                        }

                        break;
                    }

                case ReminderActionType.Email:
                    {
                        foreach (var addr in emailAddresses)
                        {
                            var message = settings.EmailMessageTemplate;
                            var context = _templateModifier.CreateCurrentContext(appointment, attendee);
                            message = _templateModifier.Modify(message, context);
                            apiExtension.SendMessage(new EmailMessage(addr, "Reminder for appointment", message,
                                                                      string.IsNullOrEmpty(settings.SenderEmailAddress) ? "unknown@unknown.com" : settings.SenderEmailAddress));   
                        }

                        break;
                    }
            }

        }

        private void OnReminderActionDone(EventArgs e)
        {
            var handler = ReminderActionDone;
            if (handler != null) handler(this, e);
        }
    }
}
