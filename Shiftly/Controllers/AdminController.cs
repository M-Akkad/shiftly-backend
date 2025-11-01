using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL.Services;

namespace Shiftly.Controllers
{
    /// <summary>
    /// Admin endpoints voor het beheren van administrators
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly WedstrijdService _wedstrijdService;

        public AdminController(AdminService adminService, WedstrijdService wedstrijdService)
        {
            _adminService = adminService;
            _wedstrijdService = wedstrijdService;
        }

        /// <summary>
        /// Haal alle admins op
        /// </summary>
        /// <remarks>
        /// Haalt een lijst van alle geregistreerde administrators op.
        /// </remarks>
        /// <response code="200">Lijst van admins succesvol opgehaald</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AdminDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAsync();
            return Ok(admins);
        }

        /// <summary>
        /// Haal een specifieke admin op
        /// </summary>
        /// <remarks>
        /// Haalt de details van een admin op aan de hand van het admin id.
        /// </remarks>
        /// <param name="id">De unieke ID van de admin</param>
        /// <response code="200">Admin succesvol opgehaald</response>
        /// <response code="404">Admin niet gevonden</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AdminDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDTO>> GetAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound(new { message = "Admin niet gevonden" });

            return Ok(admin);
        }

        /// <summary>
        /// Haal alle wedstrijden van een admin op
        /// </summary>
        /// <remarks>
        /// Haalt alle wedstrijden op die door deze admin beheerd worden.
        ///
        /// </remarks>
        /// <param name="adminId">De unieke ID van de admin</param>
        /// <response code="200">Lijst van wedstrijden succesvol opgehaald</response>
        /// <response code="404">Admin niet gevonden</response>
        [HttpGet("{adminId}/wedstrijden")]
        [ProducesResponseType(typeof(IEnumerable<WedstrijdDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WedstrijdDTO>>> GetAdminWedstrijden(int adminId)
        {
            var admin = await _adminService.GetByIdAsync(adminId);
            if (admin == null)
                return NotFound(new { message = "Admin niet gevonden" });

            var wedstrijden = await _wedstrijdService.GetAllWedstrijdenAsync();
            var adminWedstrijden = wedstrijden.Where(w => w.AdminID == adminId);

            return Ok(adminWedstrijden);
        }
    }
}
