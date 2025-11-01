using DTO;
using DAL.Repositories;
using DAL.Models;

namespace BLL.Services;

public class AdminService
{
    private readonly AdminRepository _adminRepository;

    public AdminService(AdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }


    public async Task<AdminDTO?> GetByIdAsync(int id)
    {
        var admin = await _adminRepository.GetByIdAsync(id);
        if (admin == null) return null;

        return MapToDTO(admin);
    }


    public async Task<AdminDTO?> GetByEmailAsync(string email)
    {
        var admin = await _adminRepository.GetByEmailAsync(email);
        if (admin == null) return null;

        return MapToDTO(admin);
    }


    public async Task<List<AdminDTO>> GetAllAsync()
    {
        var admins = await _adminRepository.GetAllAsync();
        return admins.Select(MapToDTO).ToList();
    }

    private AdminDTO MapToDTO(Admin admin)
    {
        return new AdminDTO
        {
            ID = admin.ID,
            Naam = admin.Naam,
            Email = admin.Email,
            Wachtwoord = string.Empty // Never expose password hash
        };
    }
}
