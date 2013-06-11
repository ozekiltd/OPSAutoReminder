using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System;
using AutoReminder.Model.Settings;
using AutoReminder.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Outlook;

namespace AutoReminder.Model.Outlook
{
    public class OutlookCalendarParser : IOutlookCalendarParser
    {
        public event EventHandler<Appointment> AppointmentFound;
        public event EventHandler AppointmentsParsed;

        public bool Parsing { get; private set; }

        public void Start(AppPreferences settings)
        {
            if (Parsing)
                OnAppointmentsParsed(EventArgs.Empty);

            Parsing = true;

            Task.Factory.StartNew(() => InternalStart(settings));
        }

        public void Cancel()
        {
            Parsing = false;
        }

        private void InternalStart(AppPreferences settings)
        {
            NameSpace oNS = null;
            try
            {
                Application app;

                var outlookProcesses = Process.GetProcessesByName("Outlook.exe");
                if (outlookProcesses.Length == 0)
                    app = new Application();
                else
                    app = (Application) System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application");

                oNS = app.GetNamespace("mapi");

                oNS.Logon(Missing.Value, Missing.Value, true, true);
                var oCalendar = oNS.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);
                var oItems = oCalendar.Items;
                oItems.IncludeRecurrences = true;

                foreach (AppointmentItem oItem in oItems)
                {
                    if (!Parsing)
                        break;

                    if (oItem.Start < DateTime.Now)
                        continue;

                    var appointment = new Appointment
                                          {
                                              Location = oItem.Location,
                                              Description = oItem.Body,
                                              Subject = oItem.Subject,
                                              CreationTime = oItem.CreationTime,
                                              StartTime = oItem.Start,
                                              EndTime = oItem.End,
                                              Duration = oItem.Duration
                                          };

                    foreach (Recipient recipient in oItem.Recipients)
                    {
                        var contact = recipient.AddressEntry.GetContact();
                        var attendee = new Attendee
                                           {
                                               FirstName = contact.FirstName,
                                               LastName = contact.LastName,
                                               FullName = contact.FullName
                                           };

                        #region Field parsing

                        if (!string.IsNullOrEmpty(contact.Email1Address) && contact.Email1AddressType == "SMTP")
                            attendee.EmailAddresses.Add(contact.Email1Address);

                        if (!string.IsNullOrEmpty(contact.Email2Address) && contact.Email2AddressType == "SMTP")
                            attendee.EmailAddresses.Add(contact.Email2Address);

                        if (!string.IsNullOrEmpty(contact.Email3Address) && contact.Email3AddressType == "SMTP")
                            attendee.EmailAddresses.Add(contact.Email3Address);

                        {
                            if (!string.IsNullOrEmpty(contact.BusinessTelephoneNumber))
                                attendee.PhoneNumbers.Add(contact.BusinessTelephoneNumber);

                            if (!string.IsNullOrEmpty(contact.Business2TelephoneNumber))
                                attendee.PhoneNumbers.Add(contact.Business2TelephoneNumber);
                        }

                        {
                            if (!string.IsNullOrEmpty(contact.HomeTelephoneNumber))
                                attendee.PhoneNumbers.Add(contact.HomeTelephoneNumber);

                            if (!string.IsNullOrEmpty(contact.Home2TelephoneNumber))
                                attendee.PhoneNumbers.Add(contact.Home2TelephoneNumber);
                        }

                        if (!string.IsNullOrEmpty(contact.BusinessAddress))
                            attendee.Addresses.Add(contact.BusinessAddress.Replace("\r\n", " "));

                        if (!string.IsNullOrEmpty(contact.HomeAddress))
                            attendee.Addresses.Add(contact.HomeAddress.Replace("\r\n", " "));

                        #endregion

                        appointment.Attendees.Add(attendee);
                    }

                    OnAppointmentFound(appointment);
                }


            }
            catch (System.Exception e)
            {
                Messenger.Default.Send(new NotificationMessage(MainViewModel.ShowImportingFailed)); 
            }
            finally
            {
                if (oNS != null)
                {
                    oNS.Logoff();
                }
            }

            Parsing = false;

            OnAppointmentsParsed(EventArgs.Empty);
        }

        private void OnAppointmentsParsed(EventArgs e)
        {
            EventHandler handler = AppointmentsParsed;
            if (handler != null) handler(this, e);
        }

        private void OnAppointmentFound(Appointment e)
        {
            EventHandler<Appointment> handler = AppointmentFound;
            if (handler != null) handler(this, e);
        }
    }
}
