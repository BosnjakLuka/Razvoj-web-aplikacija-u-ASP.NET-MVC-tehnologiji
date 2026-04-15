using System;
using System.Collections.Generic;

namespace planinarenje.Models
{
    public class KorisnikDetailsViewModel
    {
        public int IdKorisnik { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public DateTime? DatumRodenja { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string? BrojMobitela { get; set; }
        public string? ProfilnaSlika { get; set; }
        public bool StatusAktivan { get; set; }

        public KnjizicaViewModel? Knjizica { get; set; }
        public List<PosjetViewModel> Posjeti { get; set; } = new();
        public List<KorisnikMedaljaViewModel> Medalje { get; set; } = new();
    }

    public class KnjizicaViewModel
    {
        public int IdKnjizica { get; set; }
        public string NazivAplikacije { get; set; } = string.Empty;
        public DateTime DatumIzdavanja { get; set; }
    }

    public class PosjetViewModel
    {
        public int IdPosjet { get; set; }
        public DateTime DatumPosjeta { get; set; }
        public string NazivKontrolneTocke { get; set; } = string.Empty;
        public string SlikaUrl { get; set; } = string.Empty;
        public string NazivRute { get; set; } = string.Empty;
    }

    public class KorisnikMedaljaViewModel
    {
        public string NazivMedalje { get; set; } = string.Empty;
        public DateTime DatumOsvajanja { get; set; }
        public string Opis { get; set; } = string.Empty;
    }
}