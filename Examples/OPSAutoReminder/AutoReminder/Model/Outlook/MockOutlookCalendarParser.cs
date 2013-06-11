using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoReminder.Model.Settings;
using OzCommon.Utils;

namespace AutoReminder.Model.Outlook
{
    class MockOutlookCalendarParser : IOutlookCalendarParser
    {
        public event EventHandler<Appointment> AppointmentFound;
        public event EventHandler AppointmentsParsed;
        public void Start(AppPreferences settins)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          Parsing = true;

                                          Thread.Sleep(2000);
                                          var handler = AppointmentFound;
                                          if (handler != null)
                                          {
                                              var appointment = new Appointment()
                                                                {
                                                                    Attendees = new ObservableCollectionEx<Attendee>(),
                                                                    CreationTime = DateTime.Now.Subtract(TimeSpan.FromHours(48.0)),
                                                                    Description = "test description",
                                                                    Duration = 3600,
                                                                    EndTime = DateTime.Now.Subtract(TimeSpan.FromHours(4.0)),
                                                                    Location = "nowhere",
                                                                    StartTime = DateTime.Now.Subtract(TimeSpan.FromHours(5.0)),
                                                                    Subject = "test subject"
                                                                };

                                              var attendee = new Attendee()
                                                                 {
                                                                     FirstName = "John",
                                                                     LastName = "Doe",
                                                                 };
                                              attendee.Addresses.Add("125 sterg str.");
                                              attendee.EmailAddresses.Add("sdfdfg@rtrth.rh");
                                              attendee.PhoneNumbers.Add("110");
                                              appointment.Attendees.Add(attendee);
                                              handler(this, appointment);
                                          }

                                          Parsing = false;
                                          var h = AppointmentsParsed;
                                          if (h != null)
                                          {
                                              h(this, EventArgs.Empty);
                                          }
                                      });
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public bool Parsing { get; private set; }
    }
}
