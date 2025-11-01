using DTO;
using DAL.Repositories;

namespace BLL.Services;

public class AfwezigheidService
{
    private readonly WedstrijdSpelerRepository _wedstrijdSpelerRepository;
    private readonly WedstrijdRepository _wedstrijdRepository;

    public AfwezigheidService(
        WedstrijdSpelerRepository wedstrijdSpelerRepository,
        WedstrijdRepository wedstrijdRepository)
    {
        _wedstrijdSpelerRepository = wedstrijdSpelerRepository;
        _wedstrijdRepository = wedstrijdRepository;
    }

    /// <summary>
    /// UC-06: Registreer afwezigheid voor een wedstrijd met validatie
    /// Business rules:
    /// - B-06.01: Afwezigheid kan alleen doorgegeven worden voor wedstrijden
    ///            die minimaal 7 dagen in de toekomst liggen
    /// </summary>
    public async Task<(bool Success, string Message)> RegistreerAfwezigheidAsync(int wedstrijdId, int spelerId)
    {
        // Haal wedstrijd op
        var wedstrijd = await _wedstrijdRepository.GetByIdAsync(wedstrijdId);
        if (wedstrijd == null)
        {
            return (false, "Wedstrijd niet gevonden");
        }

        // B-06.01: Check of wedstrijd minimaal 7 dagen in de toekomst ligt
        if (!KanAfwezigheidDoorgeven(wedstrijd.Datum, wedstrijd.Tijd))
        {
            return (false, "Je kunt je afwezigheid alleen doorgeven voor wedstrijden die verder dan één week in de toekomst liggen.");
        }

        // Haal WedstrijdSpeler relatie op
        var wedstrijdSpeler = await _wedstrijdSpelerRepository.GetByWedstrijdAndSpelerAsync(wedstrijdId, spelerId);
        if (wedstrijdSpeler == null)
        {
            return (false, "Je bent niet toegewezen aan deze wedstrijd");
        }

        // Update status naar "Afwezig" (EF Core tracks changes automatically)
        wedstrijdSpeler.Status = "Afwezig";
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        // K-06.01: Success message
        return (true, "Je afwezigheid is succesvol geregistreerd.");
    }

    /// <summary>
    /// Wijzig status terug naar "Aanwezig"
    /// Zelfde 7-dagen regel als afwezig melden
    /// </summary>
    public async Task<(bool Success, string Message)> WijzigNaarAanwezigAsync(int wedstrijdId, int spelerId)
    {
        // Haal wedstrijd op
        var wedstrijd = await _wedstrijdRepository.GetByIdAsync(wedstrijdId);
        if (wedstrijd == null)
        {
            return (false, "Wedstrijd niet gevonden");
        }

        // Check of wijziging nog mogelijk is (7-dagen regel)
        if (!KanAfwezigheidDoorgeven(wedstrijd.Datum, wedstrijd.Tijd))
        {
            return (false, "Je kunt je status niet meer wijzigen binnen 7 dagen voor de wedstrijd.");
        }

        // Haal WedstrijdSpeler relatie op
        var wedstrijdSpeler = await _wedstrijdSpelerRepository.GetByWedstrijdAndSpelerAsync(wedstrijdId, spelerId);
        if (wedstrijdSpeler == null)
        {
            return (false, "Je bent niet toegewezen aan deze wedstrijd");
        }

        // Update status naar "Aanwezig" (EF Core tracks changes automatically)
        wedstrijdSpeler.Status = "Aanwezig";
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        return (true, "Status gewijzigd naar Aanwezig.");
    }

    /// <summary>
    /// Helper method: Check of afwezigheid doorgegeven kan worden
    /// Regel: Wedstrijd moet minimaal 7 dagen in de toekomst liggen
    /// </summary>
    public bool KanAfwezigheidDoorgeven(DateTime wedstrijdDatum, TimeSpan wedstrijdTijd)
    {
        // Bereken exacte wedstrijd datum+tijd
        var wedstrijdDateTime = wedstrijdDatum.Date + wedstrijdTijd;

        // Bereken minimale datum (nu + 7 dagen)
        var minimaalDatum = DateTime.Now.AddDays(7);

        // Wedstrijd moet later zijn dan minimale datum
        return wedstrijdDateTime > minimaalDatum;
    }
}
