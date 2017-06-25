using MirabeauExercise.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MirabeauExercise.Controllers
{
    public class AirportsController : Controller
    {
        private static int pageSize = 50;
        private static string JSONFeed = "https://raw.githubusercontent.com/jbrooksuk/JSON-Airports/master/airports.json";

        public ActionResult Index(int? page, string currentFilter, string country, int? exPage, string origin)
        {
            page = page ?? 1;
            if (!string.IsNullOrWhiteSpace(origin))
                ViewBag.origin = origin;

            ResetPageAndCarryCountryFilter(ref page, currentFilter, ref country);

            IEnumerable<Airport> data = Airport.GetAirports(JSONFeed);

            data = FilterEUAirports(country, data);
 
            int pageNum = (int)(page);
            return View(data.ToPagedList((pageNum), pageSize));
        }

        private void ResetPageAndCarryCountryFilter(ref int? page, string currentFilter, ref string country)
        {
            if (country != null)
                page = 1;
            else
                country = currentFilter;
            ViewBag.CurrentFilter = country;
        }

        private static IEnumerable<Airport> FilterEUAirports(string country, IEnumerable<Airport> data)
        {
            IEnumerable<Airport> EU =
                            data.Where(x => string.Equals(x.continent, "EU", StringComparison.OrdinalIgnoreCase)).OrderBy(d => d.iata);
            Airport.setAvailableCountriesList(EU);

            if (!string.IsNullOrWhiteSpace(country))
                EU = EU.Where(d => d.iso == country);

            return EU;
        }

        [Route("Airports/Distance")]
        public ActionResult Distance(string origin, string destination, int? page, string currentFilter)
        {
            if (string.IsNullOrWhiteSpace(destination) ^ string.IsNullOrWhiteSpace((origin)))
                return RedirectToAction("Index", new { page=page, currentFilter=currentFilter, country = (string)null, exPage=(int?)null, origin=origin });

            IEnumerable<Airport> data = Airport.GetAirports(JSONFeed);
            Airport Origin = data.Where(x => x.iata == origin).FirstOrDefault();
            Airport Destination = data.Where(x => x.iata == destination).FirstOrDefault();

            Distance distance = new Distance(Origin, Destination);

            return View(distance);
        }
    }
}