using DTO;
using DAL.Repositories;
using DAL.Models;

namespace BLL.Services;

public class SpelerService
{
    private readonly SpelerRepository _spelerRepository;
    private readonly WedstrijdRepository _wedstrijdRepository;

    public SpelerService(
        SpelerRepository spelerRepository,
        WedstrijdRepository wedstrijdRepository)
    {
        _spelerRepository = spelerRepository;
        _wedstrijdRepository = wedstrijdRepository;
    }


    public async Task<SpelerDTO?> GetByIdAsync(int spelerId)
    {
        var speler = await _spelerRepository.GetByIdAsync(spelerId);
        if (speler == null) return null;

        return MapToDTO(speler);
    }


    public async Task<SpelerDTO?> GetByEmailAsync(string email)
    {
        var speler = await _spelerRepository.GetByEmailAsync(email);
        if (speler == null) return null;

        return MapToDTO(speler);
    }


    public async Task<List<SpelerDTO>> GetAllAsync()
    {
        var spelers = await _spelerRepository.GetAllAsync();
        return spelers.Select(MapToDTO).ToList();
    }

    public async Task<List<WedstrijdDTO>> GetMijnWedstrijdenAsync(int spelerId)
    {
        // Haal alle wedstrijden op waar deze speler aan toegewezen is
        var wedstrijden = await _wedstrijdRepository.GetWedstrijdenBySpelerAsync(spelerId);

        return wedstrijden.Select(w => MapWedstrijdToDTO(w, spelerId)).ToList();
    }

    #region Mapping Helpers

    private SpelerDTO MapToDTO(Speler speler)
    {
        return new SpelerDTO
        {
            ID = speler.ID,
            Naam = speler.Naam,
            Email = speler.Email,
            Positie = speler.Positie,
            Rugnummer = speler.Rugnummer,
            Wachtwoord = string.Empty // Never expose password hash
        };
    }

    private WedstrijdDTO MapWedstrijdToDTO(Wedstrijd wedstrijd, int spelerId)
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
                Speler = ws.Speler != null ? new SpelerDTO
                {
                    ID = ws.Speler.ID,
                    Naam = ws.Speler.Naam,
                    Email = ws.Speler.Email,
                    Positie = ws.Speler.Positie,
                    Rugnummer = ws.Speler.Rugnummer,
                    Wachtwoord = string.Empty
                } : null
            }).ToList()
        };
    }

    #endregion
}
