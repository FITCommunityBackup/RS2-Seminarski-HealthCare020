﻿namespace HealthCare020.Core.Request
{
    public class LicniPodaciSearchRequest
    {
        public int? Id { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        public string Adresa { get; set; }

        public char Pol { get; set; }

        public string BrojTelefona { get; set; }

        public string Grad { get; set; }
    }
}