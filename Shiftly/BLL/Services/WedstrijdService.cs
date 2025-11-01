using DTO;
using DAL.Repositories;
using DAL.Models;

namespace BLL.Services;

public class WedstrijdService
{
    private readonly WedstrijdRepository _wedstrijdRepository;
    private readonly WedstrijdSpelerRepository _wedstrijdSpelerRepository;
    private readonly SpelerRepository _spelerRepository;

    public WedstrijdService(
        WedstrijdRepository wedstrijdRepository,
        WedstrijdSpelerRepository wedstrijdSpelerRepository,
        SpelerRepository spelerRepository)
    {
        _wedstrijdRepository = wedstrijdRepository;
        _wedstrijdSpelerRepository = wedstrijdSpelerRepository;
        _spelerRepository = spelerRepository;
    }

    public async Task<WedstrijdDTO?> GetByIdAsync(int id)
    {
        var wedstrijd = await _wedstrijdRepository.GetByIdAsync(id);
        if (wedstrijd == null) return null;

        return MapToDTO(wedstrijd);
    }

    public async Task<WedstrijdDTO?> GetByIdWithDetailsAsync(int id)
    {
        var wedstrijd = await _wedstrijdRepository.GetByIdWithDetailsAsync(id);
        if (wedstrijd == null) return null;

        return MapToDTOWithDetails(wedstrijd);
    }


    public async Task<List<WedstrijdDTO>> GetAllWedstrijdenAsync()
    {
        var wedstrijden = await _wedstrijdRepository.GetAllWedstrijdenAsync();
        return wedstrijden.Select(MapToDTOWithDetails).ToList();
    }

    public async Task<(bool Success, string Message, WedstrijdDTO? Wedstrijd)> VoegWedstrijdToeAsync(WedstrijdDTO wedstrijdDto)
    {
        if (string.IsNullOrWhiteSpace(wedstrijdDto.Locatie))
        {
            return (false, "Alle wedstrijdgegevens moeten worden ingevuld", null);
        }

        if (string.IsNullOrWhiteSpace(wedstrijdDto.Tegenstander))
        {
            return (false, "Alle wedstrijdgegevens moeten worden ingevuld", null);
        }

        var wedstrijdDateTime = wedstrijdDto.Datum.Date + wedstrijdDto.Tijd;
        if (wedstrijdDateTime <= DateTime.Now)
        {
            return (false, "De datum en tijd moeten in de toekomst liggen", null);
        }

        // Maak object aan
        var wedstrijd = new Wedstrijd
        {
            Datum = wedstrijdDto.Datum,
            Tijd = wedstrijdDto.Tijd,
            Locatie = wedstrijdDto.Locatie,
            Tegenstander = wedstrijdDto.Tegenstander,
            AdminID = wedstrijdDto.AdminID
        };

        // Opslaan in database
        await _wedstrijdRepository.AddAsync(wedstrijd);
        await _wedstrijdRepository.SaveChangesAsync();

        var resultDto = MapToDTO(wedstrijd);
        return (true, "Wedstrijd succesvol aangemaakt!", resultDto);
    }


    public async Task<(bool Success, string Message)> WijsSpelerToeAsync(int wedstrijdId, int spelerId)
    {
        // Haal wedstrijd op met details
        var wedstrijd = await _wedstrijdRepository.GetByIdWithDetailsAsync(wedstrijdId);
        if (wedstrijd == null)
        {
            return (false, "Wedstrijd niet gevonden");
        }

        var speler = await _spelerRepository.GetByIdAsync(spelerId);
        if (speler == null)
        {
            return (false, "Speler niet gevonden");
        }

        var bestaandeAssignment = await _wedstrijdSpelerRepository.GetByWedstrijdAndSpelerAsync(wedstrijdId, spelerId);
        if (bestaandeAssignment != null)
        {
            return (false, "Deze speler is al ingepland");
        }

        var isBeschikbaar = await _wedstrijdSpelerRepository.IsSpelerBeschikbaarAsync(
            spelerId,
            wedstrijd.Datum,
            wedstrijd.Tijd);

        if (!isBeschikbaar)
        {
            return (false, "Deze speler is al ingepland op hetzelfde moment");
        }

        // Voeg speler toe met status "Aanwezig"
        var wedstrijdSpeler = new WedstrijdSpeler
        {
            WedstrijdID = wedstrijdId,
            SpelerID = spelerId,
            Status = "Aanwezig"
        };

        await _wedstrijdSpelerRepository.AddAsync(wedstrijdSpeler);
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        return (true, "Speler succesvol toegewezen!");
    }

    public async Task<(bool Success, string Message)> VerwijderSpelerVanWedstrijdAsync(int wedstrijdId, int spelerId)
    {
        var wedstrijdSpeler = await _wedstrijdSpelerRepository.GetByWedstrijdAndSpelerAsync(wedstrijdId, spelerId);

        if (wedstrijdSpeler == null)
        {
            return (false, "Speler niet gevonden op deze wedstrijd");
        }

        _wedstrijdSpelerRepository.Delete(wedstrijdSpeler);
        await _wedstrijdSpelerRepository.SaveChangesAsync();

        return (true, "Speler verwijderd van wedstrijd");
    }


    public async Task<List<SpelerDTO>> GetBeschikbareSpelers(int wedstrijdId)
    {
        var spelers = await _spelerRepository.GetSpelersNietInWedstrijdAsync(wedstrijdId);
        return spelers.Select(MapSpelerToDTO).ToList();
    }

    #region Mapping Helpers

    private WedstrijdDTO MapToDTO(Wedstrijd wedstrijd)
    {
        return new WedstrijdDTO
        {
            ID = wedstrijd.ID,
            Datum = wedstrijd.Datum,
            Tijd = wedstrijd.Tijd,
            Locatie = wedstrijd.Locatie,
            Tegenstander = wedstrijd.Tegenstander,
            AdminID = wedstrijd.AdminID
        };
    }

    private WedstrijdDTO MapToDTOWithDetails(Wedstrijd wedstrijd)
    {
        return new WedstrijdDTO
        {
            ID = wedstrijd.ID,
            Datum = wedstrijd.Datum,
            Tijd = wedstrijd.Tijd,
            Locatie = wedstrijd.Locatie,
            Tegenstander = wedstrijd.Tegenstander,
            AdminID = wedstrijd.AdminID,
            Admin = wedstrijd.Admin != null ? new AdminDTO
            {
                ID = wedstrijd.Admin.ID,
                Naam = wedstrijd.Admin.Naam,
                Email = wedstrijd.Admin.Email,
                Wachtwoord = string.Empty
            } : null,
            WedstrijdSpelers = wedstrijd.WedstrijdSpelers?.Select(ws => new WedstrijdSpelerDTO
            {
                WedstrijdID = ws.WedstrijdID,
                SpelerID = ws.SpelerID,
                Status = ws.Status,
                Speler = ws.Speler != null ? MapSpelerToDTO(ws.Speler) : null
            }).ToList()
        };
    }

    private SpelerDTO MapSpelerToDTO(Speler speler)
    {
        return new SpelerDTO
        {
            ID = speler.ID,
            Naam = speler.Naam,
            Email = speler.Email,
            Positie = speler.Positie,
            Rugnummer = speler.Rugnummer,
            Wachtwoord = string.Empty
        };
    }

    #endregion
}
