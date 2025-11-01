using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class AdminRepository
{
    private readonly ShiftlyContext _context;

    public AdminRepository(ShiftlyContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        return await _context.Admins.FindAsync(id);
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        return await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<List<Admin>> GetAllAsync()
    {
        return await _context.Admins.ToListAsync();
    }

    public async Task AddAsync(Admin admin)
    {
        await _context.Admins.AddAsync(admin);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
