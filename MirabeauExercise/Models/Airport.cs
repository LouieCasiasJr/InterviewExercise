using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace MirabeauExercise.Models
{
    public class Airport
    {
        public string iata { get; set; }
        public double lon { get; set; }
        public string iso { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public string continent { get; set; }
        public string type { get; set; }
        public double lat { get; set; }
        public string size { get; set; }
        private string _country;
        public string country
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_country))
                {
                    RegionInfo CT = new RegionInfo(string.Format("{0}-{1}", iso, iso));
                    _country = CT.DisplayName;
                }
                return _country;
            }
        }

        public static IEnumerable<SelectListItem> countries;
        public static void setAvailableCountriesList(IEnumerable<Airport> airports)
        {
            IEnumerable<SelectListItem> Countries = airports.Select(Airport => new SelectListItem { Text = Airport.country, Value = Airport.iso }).Distinct(new countryComparer()).ToList();
            Airport.countries = Countries.OrderBy(x => x.Text);
        }

        public static IEnumerable<Airport> GetAirports(string URL)
        {
            using (var client = new HttpClient(
                new WebRequestHandler() { CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable)}))
            {
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match",
                "0cee17c78e402a85bfd279f6eb5b277c2726194b");
                client.DefaultRequestHeaders.CacheControl =
                    new CacheControlHeaderValue() {Public = true, MaxAge = TimeSpan.FromMinutes(5)};
                var response = client.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    if (!string.Equals("HIT", response.Headers.GetValues("x-cache").FirstOrDefault()))
                        response.Content.Headers.Add("from-feed", "*");

                    var responsecontent = response.Content;
                    string data = responsecontent.ReadAsStringAsync().Result;

                    return !string.IsNullOrWhiteSpace(data)
                        ? JsonConvert.DeserializeObject<IEnumerable<Airport>>(data)
                        : Enumerable.Empty<Airport>();
                }

                return Enumerable.Empty<Airport>();
            }
        }
    }

    public class countryComparer : IEqualityComparer<SelectListItem>
    {
        public bool Equals(SelectListItem x, SelectListItem y)
        {
            if (x.Text == y.Text)
                return true;
            else
                return false;
        }

        public int GetHashCode(SelectListItem obj)
        {
            return obj.Text.GetHashCode();
        }
    }
}