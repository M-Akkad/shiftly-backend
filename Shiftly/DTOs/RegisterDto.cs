namespace Shiftly.DTOs;

public record RegisterDto(string Naam, string Email, string Wachtwoord, string ConfirmWachtwoord, string Positie, string Rugnummer, int? TeamId = null, bool IsCoach = false);
