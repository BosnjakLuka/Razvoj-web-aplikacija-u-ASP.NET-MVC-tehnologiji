using System;
using System.Collections.Generic;
using planinarenje.Entiteti;

namespace planinarenje.Models
{
    public class PosjetIndexViewModel
    {
        public int IdPosjet { get; set; }
        public string ImePrezimeKorisnika { get; set; } = string.Empty;
        public string NazivKontrolneTocke { get; set; } = string.Empty;
        public string NazivRute { get; set; } = string.Empty;
        public DateTime DatumVrijemePosjeta { get; set; }
        public string Dozivljaj { get; set; } = string.Empty;
        public bool JeLiPotvrdenPosjet { get; set; }
    }

    public class PosjetDetailsViewModel
    {
        public int IdPosjet { get; set; }
        
        public int IdKorisnik { get; set; }
        public string ImePrezimeKorisnika { get; set; } = string.Empty;
        
        public int IdKnjizica { get; set; }
        public string NazivKnjizice { get; set; } = string.Empty;

        public int IdKontrolnaTocka { get; set; }
        public string NazivKontrolneTocke { get; set; } = string.Empty;

        public int IdRuta { get; set; }
        public string NazivRute { get; set; } = string.Empty;

        public DateTime DatumVrijemePosjeta { get; set; }
        public int? VrijemeUsponaMin { get; set; }
        public string Dozivljaj { get; set; } = string.Empty;
        public string? OpisIskustva { get; set; }
        public string UneseniGUID { get; set; } = string.Empty;
        public bool JeLiPotvrdenPosjet { get; set; }
        public DateTime DatumKreiranjaZapisa { get; set; }

        public List<FotografijaPosjetaViewModel> Fotografije { get; set; } = new();
    }

    public class FotografijaPosjetaViewModel
    {
        public string NazivDatoteke { get; set; } = string.Empty;
        public string PutanjaUrl { get; set; } = string.Empty;
        public string? Opis { get; set; }
    }
}