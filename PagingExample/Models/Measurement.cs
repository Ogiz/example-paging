using System;
using NPoco;

namespace PagingExample.Models
{
    public class Measurement
    {
        [Column("podatek_meritev_id")]
        public int Id { get; set; }

        [Column("naziv_test")]
        public string TestCode { get; set; }

        [Column("izhodna_sarza")]
        public string BatchNumber { get; set; }

        [Column("serija_meritve")]
        public int MeasurementBatch { get; set; }

        [Column("zap_stevilka_meritve")]
        public int MeasurementId { get; set; }

        [Column("vrednost_meritve")]
        public decimal Value { get; set; }

        [Column("datum_meritve")]
        public DateTime Date { get; set; }

        [Column("oznaka")]
        public string MeasuringParameter { get; set; }
    }
}