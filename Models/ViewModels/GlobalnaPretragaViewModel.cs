namespace planinarenje.Models.ViewModels;

// Jedna stavka rezultata globalne pretrage (jedan zapis nekog entiteta).
public class PretragaStavka
{
    public string Naziv { get; set; } = string.Empty;
    public string? Podnaslov { get; set; }
    public string Controller { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Tip { get; set; } = string.Empty;
    public int Skor { get; set; }

    // Polja koja su sudjelovala u pretrazi (osim Naziva) - prikazuju se u rezultatu
    // da je vidljivo zašto je zapis pogodio upit (npr. Opis, Koordinate, Adresa...).
    public List<KeyValuePair<string, string>> Polja { get; set; } = new();
}

// Grupa rezultata za jedan tip entiteta (npr. "Područja").
public class PretragaGrupa
{
    public string Naziv { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Ikona { get; set; } = string.Empty;
    public List<PretragaStavka> Stavke { get; set; } = new();

    // Ukupan broj poklapanja u toj grupi (može biti veći od broja prikazanih Stavki).
    public int Ukupno { get; set; }
}

// Model za cross-entity (globalnu) pretragu.
public class GlobalnaPretragaViewModel
{
    public const int MinDuljinaUpita = 2;

    public string? Upit { get; set; }
    public List<PretragaGrupa> Grupe { get; set; } = new();

    public int UkupnoPrikazano => Grupe.Sum(g => g.Stavke.Count);
    public int UkupnoPoklapanja => Grupe.Sum(g => g.Ukupno);
    public bool UpitJeValjan => !string.IsNullOrWhiteSpace(Upit) && Upit.Trim().Length >= MinDuljinaUpita;
}
