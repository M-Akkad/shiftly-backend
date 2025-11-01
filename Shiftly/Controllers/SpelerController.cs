using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL.Services;

namespace Shiftly.Controllers
{
    /// <summary>
    /// Speler endpoints voor het beheren van spelers en hun afwezigheden
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpelerController : ControllerBase
    {
        private readonly SpelerService _spelerService;
        private readonly AfwezigheidService _afwezigheidService;

        public SpelerController(SpelerService spelerService, AfwezigheidService afwezigheidService)
        {
            _spelerService = spelerService;
            _afwezigheidService = afwezigheidService;
        }

        /// <summary>
        /// Haal alle spelers op
        /// </summary>
        /// <response code="200">Lijst van spelers succesvol opgehaald</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SpelerDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SpelerDTO>>> GetAllSpelers()
        {
            var spelers = await _spelerService.GetAllAsync();
            return Ok(spelers);
        }

        /// <summary>
        /// Haal een specifieke speler op
        /// </summary>
        /// <remarks>
        /// Haalt de details van één speler op aan de hand van het speler id.
        /// </remarks>
        /// <param name="id">De unieke ID van de speler</param>
        /// <response code="200">Speler succesvol opgehaald</response>
        /// <response code="404">Speler niet gevonden</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SpelerDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SpelerDTO>> GetSpeler(int id)
        {
            var speler = await _spelerService.GetByIdAsync(id);
            if (speler == null)
                return NotFound(new { message = "Speler niet gevonden" });

            return Ok(speler);
        }

        /// <summary>
        /// Haal wedstrijden van een speler op
        /// </summary>
        /// <remarks>
        /// **UC-05: Eigen wedstrijden bekijken**
        ///
        /// Haalt alle wedstrijden op waar deze speler aan toegewezen is.
        /// </remarks>
        /// <param name="spelerId">De unieke ID van de speler</param>
        /// <response code="200">Lijst van wedstrijden succesvol opgehaald</response>
        /// <response code="404">Speler niet gevonden</response>
        [HttpGet("{spelerId}/wedstrijden")]
        [ProducesResponseType(typeof(IEnumerable<WedstrijdDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WedstrijdDTO>>> GetSpelerWedstrijden(int spelerId)
        {
            var speler = await _spelerService.GetByIdAsync(spelerId);
            if (speler == null)
                return NotFound(new { message = "Speler niet gevonden" });

            var wedstrijden = await _spelerService.GetMijnWedstrijdenAsync(spelerId);
            return Ok(wedstrijden);
        }

        /// <summary>
        /// Registreer afwezigheid voor een wedstrijd
        /// </summary>
        /// <remarks>
        /// **UC-06: Afwezigheid doorgeven**
        ///
        /// Laat een speler afwezigheid doorgeven voor een wedstrijd.
        ///
        /// **BELANGRIJKE 7-DAGEN REGEL:**
        /// - Afwezigheid kan alleen doorgegeven worden voor wedstrijden die verder dan 7 dagen in de toekomst liggen
        /// - Wedstrijden binnen 7 dagen: 400 Bad Request
        /// - Wedstrijden > 7 dagen in toekomst: 200 OK
        /// 
        /// </remarks>
        /// <param name="spelerId">De unieke ID van de speler</param>
        /// <param name="wedstrijdId">De unieke ID van de wedstrijd</param>
        /// <response code="200">Afwezigheid succesvol geregistreerd</response>
        /// <response code="400">Ongeldige operatie (bijv. binnen 7 dagen, al afwezig)</response>
        /// <response code="404">Speler of wedstrijd niet gevonden</response>
        [HttpPost("{spelerId}/wedstrijden/{wedstrijdId}/afwezigheid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterAfwezigheid(int spelerId, int wedstrijdId)
        {
            var speler = await _spelerService.GetByIdAsync(spelerId);
            if (speler == null)
                return NotFound(new { message = "Speler niet gevonden" });

            var wedstrijden = await _spelerService.GetMijnWedstrijdenAsync(spelerId);
            var wedstrijd = wedstrijden.FirstOrDefault(w => w.ID == wedstrijdId);

            if (wedstrijd == null)
                return NotFound(new { message = "Wedstrijd niet gevonden voor deze speler" });

            // Check 7-dagen regel
            if (!_afwezigheidService.KanAfwezigheidDoorgeven(wedstrijd.Datum, wedstrijd.Tijd))
            {
                return BadRequest(new { message = "Je kunt je afwezigheid alleen doorgeven voor wedstrijden die verder dan één week in de toekomst liggen." });
            }

            var (success, message) = await _afwezigheidService.RegistreerAfwezigheidAsync(wedstrijdId, spelerId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        /// <summary>
        /// Draai afwezigheid terug naar aanwezig
        /// </summary>
        /// <remarks>
        /// Wijzigt de status van een speler van "Afwezig" terug naar "Aanwezig".
        /// </remarks>
        /// <param name="spelerId">De unieke ID van de speler</param>
        /// <param name="wedstrijdId">De unieke ID van de wedstrijd</param>
        /// <response code="200">Status succesvol gewijzigd naar Aanwezig</response>
        /// <response code="400">Ongeldige operatie</response>
        [HttpDelete("{spelerId}/wedstrijden/{wedstrijdId}/afwezigheid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelAfwezigheid(int spelerId, int wedstrijdId)
        {
            var (success, message) = await _afwezigheidService.WijzigNaarAanwezigAsync(wedstrijdId, spelerId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
