using System;
using System.Windows;
using AutoReminder.Model.Outlook;
using AutoReminder.Model.Settings;
using AutoReminder.Utils.MessageModifier;
using AutoReminder.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using OPSSDK;
using OzCommon.Model;
using OzCommon.Model.Mock;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.View;
using OzCommon.ViewModel;

namespace AutoReminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private PreferencesWindow _preferencesWindow;
        private WaitWindow _loadingWindow;
        private SingletonApp _app;
        private DialogService _dialogService;
        private Client _client;

        public App()
        {
            _app = new SingletonApp("OPSAutoReminder");

            InitDependencies();
        }

        private void InitDependencies()
        {
            _dialogService = new DialogService();
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IDialogService>(() => _dialogService);
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IOutlookCalendarParser>(() => new OutlookCalendarParser());
            //GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IOutlookCalendarParser>(() => new MockOutlookCalendarParser());
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IGenericSettingsRepository<AppPreferences>>(() => new SettingsRepository());
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<ITemplateModifier>(() => new OutlookTemplateModifier());
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IUserInfoSettingsRepository>(() => new UserInfoSettingsRepository());
            _client = new Client();
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IClient>(() => _client);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);

            _app.OnStartup(e);

            base.OnStartup(e);

            MainWindow = new LoginWindow();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);
            base.OnExit(e);
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                                                  {
                                                      if (notificationMessage.Notification == Messages.ShowSettings)
                                                      {
                                                          _preferencesWindow = new PreferencesWindow();
                                                          _preferencesWindow.Owner = (Window) notificationMessage.Sender;
                                                          _preferencesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                                          _preferencesWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.DismissSettingsWindow)
                                                      {
                                                          _preferencesWindow.Close();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                                                      {
                                                          var mainWindow = new AutoReminderMainWindow();
                                                          mainWindow.Show();

                                                          Application.Current.MainWindow = mainWindow;
                                                      }
                                                      else if (notificationMessage.Notification == Messages.ShowWaitWindowLoading)
                                                      {
                                                          _loadingWindow = new WaitWindow("Importing from Microsoft Outlook...");
                                                          _loadingWindow.Owner = Application.Current.MainWindow;
                                                          _loadingWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.DismissWaitWindow)
                                                      {
                                                          if (_loadingWindow != null)
                                                              _loadingWindow.Close();

                                                          _loadingWindow = null;
                                                      }
                                                      else if (notificationMessage.Notification == Messages.ShowAboutWindow)
                                                      {
                                                          var aboutWindow = new AboutWindow("Auto Reminder");
                                                          aboutWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == MainViewModel.ShowApiExtensionWarning)
                                                      {
                                                          MessageBox.Show(
                                                              "API extension ID is not set in General settings or it is invalid. Calls and messages will not be initiated on reminder actions.",
                                                              "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                      }
                                                      else if (notificationMessage.Notification == MainViewModel.ShowAttendeesWarning)
                                                      {
                                                          MessageBox.Show(
                                                              "No attendees for appointment",
                                                              "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                      }
                                                      else if (notificationMessage.Notification == MainViewModel.ShowImportingFailed)
                                                      {
                                                          MessageBox.Show(
                                                              "Importing appointments from outlook failed",
                                                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                      }
                                                  }));
        }
    }

}
