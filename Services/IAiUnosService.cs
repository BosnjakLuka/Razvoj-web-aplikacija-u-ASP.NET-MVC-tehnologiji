using planinarenje.Models.Ai;

namespace planinarenje.Services;

/// <summary>
/// Servis koji iz korisnikova opisa na prirodnom jeziku izvlači strukturirane
/// podatke o planinarskom posjetu. Implementacija koristi vanjski LLM (Gemini).
/// </summary>
public interface IAiUnosService
{
    /// <summary>
    /// Je li servis konfiguriran (postoji API ključ). Ako nije, UI može sakriti AI dio
    /// ili prikazati poruku, a ručni unos i dalje radi.
    /// </summary>
    bool JeDostupan { get; }

    /// <summary>
    /// Izvuče prijedlog podataka o posjetu iz slobodnog teksta.
    /// Nikad ne baca iznimku prema pozivatelju — kod greške vraća prijedlog s ispunjenom
    /// <see cref="AiPosjetPrijedlog.Napomena"/>, tako da forma uvijek ostane upotrebljiva.
    /// </summary>
    Task<AiPosjetPrijedlog> IzvuciPodatkeAsync(string upit, CancellationToken cancellationToken = default);
}
