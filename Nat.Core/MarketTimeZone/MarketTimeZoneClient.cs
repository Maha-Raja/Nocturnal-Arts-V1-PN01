using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.Lookup;
using Nat.Core.MarketTimeZone.Model;
using Nat.Core.ServiceClient;
using Nat.Core.Caching;

namespace Nat.Core.MarketTimeZone
{
    public class MarketTimeZoneClient
    {
        public static async Task<MarketTimeZoneModel> GetMarketTimeAsync(DateTime? date, string marketCode, MarketTimeZoneModel fetchedModel = null)
        {
            if (marketCode != null)
            {
                MarketTimeZoneModel marketTimeZone = new MarketTimeZoneModel();
                string format = @"M/d/yyyy h:m:s tt";
                string utcTime = (date ?? DateTime.UtcNow).ToString(format);
                TimeSpan marketTimezone;
                DateTimeOffset marketTime;
                //Get market dictionary to get timezone cache function
                IDictionary<string, MarketLocationViewModel> market = await GetAllMarketsAsync();
                MarketLocationViewModel marketLocation = market[marketCode];
                

                string str = market[marketCode].Timezone;
                string Timezone = str.Substring(0, 6);

                //Get Lookup dictionary to get timezone Visible Value
                var TimezoneLookup = await GetTimeZoneLookupAsync();
                if ((bool)marketLocation.DaylightSavingApplicable && date > marketLocation.DaylightStartTime && date < marketLocation.DaylightEndTime)
                {
                    marketTimeZone.TimeZone = TimezoneLookup.ContainsKey(marketLocation.Timezone) ? TimezoneLookup[marketLocation.Timezone].StringAttribute2 : null;
                    
                    marketTimezone = TimeSpan.Parse(TimezoneLookup[marketLocation.Timezone].StringAttribute1);                    
                    DateTimeOffset eventTime = DateTimeOffset.ParseExact(utcTime, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    marketTime = eventTime.ToOffset(marketTimezone);
                }
                else
                {
                    marketTimeZone.TimeZone = TimezoneLookup.ContainsKey(marketLocation.Timezone) ? TimezoneLookup[marketLocation.Timezone].VisibleValue : null;
                    marketTimezone = TimeSpan.Parse(TimezoneLookup[marketLocation.Timezone].HiddenValue.Substring(0,6));
                    DateTimeOffset eventTime = DateTimeOffset.ParseExact(utcTime, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    marketTime = eventTime.ToOffset(marketTimezone);
                }
                
                marketTimeZone.MarketTime = marketTime.DateTime;
                return marketTimeZone;

            }
            else
            {
                MarketTimeZoneModel marketTimeZone = new MarketTimeZoneModel();
                marketTimeZone.MarketTime = date.Value;
                return marketTimeZone;
            }

        }


        public static async Task<IDictionary<string, LookupViewModel>> GetTimeZoneLookupAsync()
        {
            async Task<IDictionary<string, LookupViewModel>> GetLookupTimezone()
            {
                var lookupModel = await LookupClient.ReadAsync<LookupViewModel>("TIMEZONES");
                if (lookupModel != null)
                {
                    return lookupModel;
                }
                else
                {
                    throw new NullReferenceException("Null value returned from DB");
                }
            }
            return await Caching.Caching.GetObjectFromCacheAsync<IDictionary<string, LookupViewModel>>("lookups/Timzones", 5, GetLookupTimezone);
        }

        public static async Task<IDictionary<string, MarketLocationViewModel>> GetAllMarketsAsync()
        {
            async Task<IDictionary<string, MarketLocationViewModel>> GetMarketDictionary()
            {
                var marketlist = await NatClient.ReadAsync<IEnumerable<MarketLocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
                if (marketlist.status.IsSuccessStatusCode)
                {
                    return marketlist.data.ToDictionary(item => item.LocationShortCode, item => item);
                }
                else
                {
                    throw new ApplicationException("Failed to fetch Markets");
                }
            }
            return await Caching.Caching.GetObjectFromCacheAsync<IDictionary<string, MarketLocationViewModel>>("market/dictionary", 5, GetMarketDictionary);
        }
    }
}