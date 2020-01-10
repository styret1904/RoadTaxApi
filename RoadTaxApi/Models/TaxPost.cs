using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoadTaxApi.Utilities;

namespace RoadTaxApi.Models
{
    public class TaxPost
    {
        public long Id { get; set; }

        public string RegNumber { get; set; }

        public VehicleType VehicleType { get; set; }

        public int Fee { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
