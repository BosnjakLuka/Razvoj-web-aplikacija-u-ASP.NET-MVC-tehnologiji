using System;

namespace planinarenje.Models
{
    public class KorisnikIndexCardViewModel
    {
        public int IdKorisnik { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public DateTime DatumRegistracije { get; set; }
        public bool StatusAktivan { get; set; }
    }
}