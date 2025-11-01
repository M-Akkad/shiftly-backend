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

    public async Task<(bool Success, string Message)> RegistreerAfwezigheidAsync(int wedstrijdId, int spelerId)
    {
        // Haal wedstrijd op
        var wedstrijd = await _wedstrijdRepository.GetByIdAsync(wedstrijdId);
        if (wedstrijd == null)
        {
            return (false, "Wedstrijd niet gevonden");
        }

        // Check of wedstrijd minimaal 7 dagen in de toekomst ligt
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

       
        wedstrijdSpeler.Status = "Afwezig";
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        return (true, "Je afwezigheid is succesvol geregistreerd.");
    }


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

        var wedstrijdSpeler = await _wedstrijdSpelerRepository.GetByWedstrijdAndSpelerAsync(wedstrijdId, spelerId);
        if (wedstrijdSpeler == null)
        {
            return (false, "Je bent niet toegewezen aan deze wedstrijd");
        }

        wedstrijdSpeler.Status = "Aanwezig";
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        return (true, "Status gewijzigd naar Aanwezig.");
    }

    public bool KanAfwezigheidDoorgeven(DateTime wedstrijdDatum, TimeSpan wedstrijdTijd)
    {
        var wedstrijdDateTime = wedstrijdDatum.Date + wedstrijdTijd;

        var minimaalDatum = DateTime.Now.AddDays(7);

        return wedstrijdDateTime > minimaalDatum;
    }
}
