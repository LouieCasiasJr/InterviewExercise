using System;
using System.Device.Location;

namespace MirabeauExercise.Models
{
    public class Distance
    {
        public string origin { get; set; }
        public string oCountry { get; set; }

        public string destination { get; set; }
        public string dCountry { get; set; }

        public GeoCoordinate oCoord { get; set; }
        public GeoCoordinate dCoord { get; set; }

        public double distance { get { return oCoord.GetDistanceTo(dCoord); } }
        public double distanceKM { get { return Math.Round((distance / 1000), 2); } }
        public double distanceMi { get { return Math.Round((distance / 1609.344), 2); } }

        public Distance(Airport origin, Airport destination)
        {;
            if (origin != null && destination != null)
            {
                this.origin = origin.name ?? origin.iata;
                oCountry = origin.country;
                this.destination = destination.name ?? destination.iata;
                dCountry = destination.country;
                oCoord = new GeoCoordinate(origin.lat, origin.lon);
                dCoord = new GeoCoordinate(destination.lat, destination.lon);
            }
        }
    }
}