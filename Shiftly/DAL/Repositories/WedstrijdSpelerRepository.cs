using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class WedstrijdSpelerRepository
{
    private readonly ShiftlyContext _context;

    public WedstrijdSpelerRepository(ShiftlyContext context)
    {
        _context = context;
    }

    public async Task<WedstrijdSpeler?> GetByWedstrijdAndSpelerAsync(int wedstrijdId, int spelerId)
    {
        return await _context.WedstrijdSpelers
            .FirstOrDefaultAsync(ws => ws.WedstrijdID == wedstrijdId && ws.SpelerID == spelerId);
    }

    public async Task<List<WedstrijdSpeler>> GetByWedstrijdAsync(int wedstrijdId)
    {
        return await _context.WedstrijdSpelers
            .Include(ws => ws.Speler)
            .Where(ws => ws.WedstrijdID == wedstrijdId)
            .ToListAsync();
    }

    public async Task<bool> IsSpelerBeschikbaarAsync(int spelerId, DateTime datum, TimeSpan tijd)
    {
        // Check if player is already assigned to another match at the same date and time
        var conflictingAssignment = await _context.WedstrijdSpelers
            .Include(ws => ws.Wedstrijd)
            .AnyAsync(ws => ws.SpelerID == spelerId
                && ws.Wedstrijd!.Datum == datum
                && ws.Wedstrijd.Tijd == tijd);

        return !conflictingAssignment;
    }

    public async Task AddAsync(WedstrijdSpeler wedstrijdSpeler)
    {
        await _context.WedstrijdSpelers.AddAsync(wedstrijdSpeler);
    }

    public void Delete(WedstrijdSpeler wedstrijdSpeler)
    {
        _context.WedstrijdSpelers.Remove(wedstrijdSpeler);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
