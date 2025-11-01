using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL.Services;

namespace Shiftly.Controllers
{
    /// <summary>
    /// Wedstrijd beheer endpoints voor het plannen en beheren van voetbalwedstrijden
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WedstrijdController : ControllerBase
    {
        private readonly WedstrijdService _wedstrijdService;

        public WedstrijdController(WedstrijdService wedstrijdService)
        {
            _wedstrijdService = wedstrijdService;
        }

        /// <summary>
        /// Haal alle wedstrijden op
        /// </summary>
        /// <remarks>
        /// **UC-03: Wedstrijd aanmaken en beheren**
        ///
        /// Haalt alle wedstrijden op uit het systeem, gesorteerd op datum en tijd.
        /// </remarks>
        /// <response code="200">Lijst van wedstrijden succesvol opgehaald</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WedstrijdDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WedstrijdDTO>>> GetAllWedstrijden()
        {
            var wedstrijden = await _wedstrijdService.GetAllWedstrijdenAsync();
            return Ok(wedstrijden);
        }

        /// <summary>
        /// Haal een specifieke wedstrijd op met volledige details
        /// </summary>
        /// <remarks>
        /// Haalt een wedstrijd op inclusief:
        /// - Admin informatie
        /// - Alle toegewezen spelers met hun status (Aanwezig/Afwezig)
        /// - Locatie en tegenstander details
        /// </remarks>
        /// <param name="id">De unieke ID van de wedstrijd</param>
        /// <response code="200">Wedstrijd succesvol opgehaald</response>
        /// <response code="404">Wedstrijd niet gevonden</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WedstrijdDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WedstrijdDTO>> GetWedstrijd(int id)
        {
            var wedstrijd = await _wedstrijdService.GetByIdWithDetailsAsync(id);
            if (wedstrijd == null)
                return NotFound(new { message = "Wedstrijd niet gevonden" });

            return Ok(wedstrijd);
        }

        /// <summary>
        /// Voeg een nieuwe wedstrijd toe
        /// </summary>
        /// <remarks>
        /// **UC-03: Wedstrijd aanmaken en beheren**
        ///
        /// Maakt een nieuwe wedstrijd aan in het systeem.
        ///
        /// **Validatieregels:**
        /// - Datum moet in de toekomst liggen
        /// - Admin moet bestaan
        /// - Tijd moet geldig zijn (HH:mm:ss format)
        /// 
        /// {
        ///   "datum": "2025-12-01T00:00:00",
        ///   "tijd": "15:00:00",
        ///   "locatie": "Eigen Stadion",
        ///   "tegenstander": "Ajax",
        ///   "adminID": 1
        /// }
        /// ```
        /// 3. Verwacht: 201 Created met de nieuwe wedstrijd
        /// </remarks>
        /// <param name="wedstrijd">De wedstrijd gegevens</param>
        /// <response code="201">Wedstrijd succesvol aangemaakt</response>
        /// <response code="400">Ongeldige invoer (bijv. datum in verleden)</response>
        [HttpPost]
        [ProducesResponseType(typeof(WedstrijdDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WedstrijdDTO>> CreateWedstrijd(WedstrijdDTO wedstrijd)
        {
            var (success, message, result) = await _wedstrijdService.VoegWedstrijdToeAsync(wedstrijd);

            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(GetWedstrijd), new { id = result?.ID }, result);
        }

        /// <summary>
        /// Haal beschikbare spelers voor een wedstrijd op
        /// </summary>
        /// <remarks>
        /// **UC-04: Spelers toewijzen aan wedstrijd**
        ///
        /// Haalt alle spelers op die NIET aan deze wedstrijd zijn toegewezen.        ///
        /// </remarks>
        /// <param name="wedstrijdId">De unieke ID van de wedstrijd</param>
        /// <response code="200">Lijst van beschikbare spelers opgehaald</response>
        /// <response code="404">Wedstrijd niet gevonden</response>
        [HttpGet("{wedstrijdId}/beschikbare-spelers")]
        [ProducesResponseType(typeof(IEnumerable<SpelerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SpelerDTO>>> GetBeschikbareSpelers(int wedstrijdId)
        {
            var wedstrijd = await _wedstrijdService.GetByIdWithDetailsAsync(wedstrijdId);
            if (wedstrijd == null)
                return NotFound(new { message = "Wedstrijd niet gevonden" });

            var beschikbareSpelers = await _wedstrijdService.GetBeschikbareSpelers(wedstrijdId);
            return Ok(beschikbareSpelers);
        }

        /// <summary>
        /// Wijs een speler toe aan een wedstrijd
        /// </summary>
        /// <remarks>
        /// **UC-04: Spelers toewijzen aan wedstrijd**
        ///
        /// Voegt een speler toe aan een wedstrijd met status "Aanwezig".
        ///
        /// **Validatieregels:**
        /// - Speler moet bestaan
        /// - Wedstrijd moet bestaan
        /// - Speler mag niet al toegewezen zijn
        /// </remarks>
        /// <param name="wedstrijdId">De unieke ID van de wedstrijd</param>
        /// <param name="spelerId">De unieke ID van de speler</param>
        /// <response code="200">Speler succesvol toegewezen</response>
        /// <response code="400">Ongeldige operatie (bijv. speler al toegewezen)</response>
        [HttpPost("{wedstrijdId}/spelers/{spelerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignSpeler(int wedstrijdId, int spelerId)
        {
            var (success, message) = await _wedstrijdService.WijsSpelerToeAsync(wedstrijdId, spelerId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        /// <summary>
        /// Verwijder een speler van een wedstrijd
        /// </summary>
        /// <remarks>
        /// Verwijdert de toewijzing van een speler aan een wedstrijd.
   
        /// </remarks>
        /// <param name="wedstrijdId">De unieke ID van de wedstrijd</param>
        /// <param name="spelerId">De unieke ID van de speler</param>
        /// <response code="200">Speler succesvol verwijderd van wedstrijd</response>
        /// <response code="400">Ongeldige operatie</response>
        [HttpDelete("{wedstrijdId}/spelers/{spelerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveSpeler(int wedstrijdId, int spelerId)
        {
            var (success, message) = await _wedstrijdService.VerwijderSpelerVanWedstrijdAsync(wedstrijdId, spelerId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
