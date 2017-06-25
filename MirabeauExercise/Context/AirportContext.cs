using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MirabeauExercise.Models;

namespace MirabeauExercise.Context
{
    public class AirportContext : DbContext
    {
        public DbSet<Airport> Airports { get; set; }
    }
}