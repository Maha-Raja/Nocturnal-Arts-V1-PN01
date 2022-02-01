using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Nat.PlannerApp.Services
{
    public class GoogleCalendarApi
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };
        private static string ApplicationName = "Paintception";
        private static string ServiceAccountEmail = "calendarapi@paintception.iam.gserviceaccount.com";
        private static X509Certificate2 certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);

        private static CalendarService GetService()
        {
            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(ServiceAccountEmail)
               {
                   Scopes = Scopes
               }.FromCertificate(certificate));

            // Create Google Calendar API service
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public static async Task<Calendar> CreateCalendarAsync(string title)
        {
            var service = GetService();
            //Create New Calendar
            //var AllowedConferenceSolutionTypes = new List<string>();
            //AllowedConferenceSolutionTypes.Add("hangoutsMeet");
            return await service.Calendars.Insert(new Calendar()
            {
                Summary = title,
                //ConferenceProperties = new ConferenceProperties()
                //{
                //    AllowedConferenceSolutionTypes = AllowedConferenceSolutionTypes
                //}
            }).ExecuteAsync();
        }

        public static async Task<Event> CreateEventAsync(string calenderId, string title, DateTime start, DateTime end)
        {
            var service = GetService();
            //var entryPoints = new List<EntryPoint>();
            //entryPoints.Add(new EntryPoint()
            //{
            //    EntryPointType = "video",
            //    Password = "pakistan1"
            //});
            var eventrequest = service.Events.Insert(new Event()
            {
                Start = new EventDateTime()
                {
                    DateTime = start
                },
                End = new EventDateTime()
                {
                    DateTime = end
                },
                ConferenceData = new ConferenceData()
                {
                    CreateRequest = new CreateConferenceRequest()
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        //ConferenceSolutionKey = new ConferenceSolutionKey()
                        //{
                        //    Type = "hangoutsMeet"
                        //},
                    },
                    //EntryPoints = entryPoints
                }
            }, "vo2ilphnjd2au4nep9ruhtmnf8@group.calendar.google.com");
            eventrequest.ConferenceDataVersion = 1;
            return await eventrequest.ExecuteAsync();
        }
    }
}
