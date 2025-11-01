using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class WedstrijdRepository
{
    private readonly ShiftlyContext _context;

    public WedstrijdRepository(ShiftlyContext context)
    {
        _context = context;
    }

    public async Task<Wedstrijd?> GetByIdAsync(int id)
    {
        return await _context.Wedstrijden.FindAsync(id);
    }

    public async Task<Wedstrijd?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Wedstrijden
            .Include(w => w.Admin)
            .Include(w => w.WedstrijdSpelers)
                .ThenInclude(ws => ws.Speler)
            .FirstOrDefaultAsync(w => w.ID == id);
    }

    public async Task<List<Wedstrijd>> GetAllWedstrijdenAsync()
    {
        var wedstrijden = await _context.Wedstrijden
            .Include(w => w.Admin)
            .Include(w => w.WedstrijdSpelers)
                .ThenInclude(ws => ws.Speler)
            .ToListAsync();

        return wedstrijden
            .OrderBy(w => w.Datum)
            .ThenBy(w => w.Tijd)
            .ToList();
    }

    public async Task<List<Wedstrijd>> GetWedstrijdenBySpelerAsync(int spelerId)
    {
        var wedstrijden = await _context.Wedstrijden
            .Include(w => w.Admin)
            .Include(w => w.WedstrijdSpelers)
                .ThenInclude(ws => ws.Speler)
            .Where(w => w.WedstrijdSpelers.Any(ws => ws.SpelerID == spelerId))
            .ToListAsync();

        return wedstrijden
            .OrderBy(w => w.Datum)
            .ThenBy(w => w.Tijd)
            .ToList();
    }

    public async Task AddAsync(Wedstrijd wedstrijd)
    {
        await _context.Wedstrijden.AddAsync(wedstrijd);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
