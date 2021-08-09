using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T1019796.Data
{
    public static partial class AppointmentCollection
    {

        public static IEnumerable<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        {
            var result = GenerateAppointments().Where(p => p.StartDate >= startDate && p.EndDate <= endDate);
            return result;
        }

        private static List<Appointment> GenerateAppointments()
        {
            DateTime date = DateTime.Now.Date;
            var dataSource = new List<Appointment>();
            for (int i = 0; i < 100; i++)
            {
                Appointment appt = new Appointment
                {
                    AppointmentId = i,
                    Caption = $"Appointment{i}",
                    Status = i % 2,
                    Label = i % 2,
                    StartDate = date + new TimeSpan(i, i % 24, i % 60, i % 60)
                };
                appt.EndDate = appt.StartDate.AddHours(3);
                dataSource.Add(appt);
            }
            return dataSource;
        }
    }
}

