using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class SpelerRepository
{
    private readonly ShiftlyContext _context;

    public SpelerRepository(ShiftlyContext context)
    {
        _context = context;
    }

    public async Task<Speler?> GetByIdAsync(int id)
    {
        return await _context.Spelers.FindAsync(id);
    }

    public async Task<Speler?> GetByEmailAsync(string email)
    {
        return await _context.Spelers
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<List<Speler>> GetAllAsync()
    {
        return await _context.Spelers
            .OrderBy(s => s.Naam)
            .ToListAsync();
    }

    public async Task<List<Speler>> GetSpelersNietInWedstrijdAsync(int wedstrijdId)
    {
        var toegewezenSpelerIds = await _context.WedstrijdSpelers
            .Where(ws => ws.WedstrijdID == wedstrijdId)
            .Select(ws => ws.SpelerID)
            .ToListAsync();

        return await _context.Spelers
            .Where(s => !toegewezenSpelerIds.Contains(s.ID))
            .OrderBy(s => s.Naam)
            .ToListAsync();
    }

    public async Task AddAsync(Speler speler)
    {
        await _context.Spelers.AddAsync(speler);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
