using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoZooApp.CrocoZooDB;
using Microsoft.EntityFrameworkCore;

namespace CrocoZooApp.DAO
{
    public class MemoryCardDAO 
    {
        private readonly ImportSyouDbContext _context;

        public MemoryCardDAO(ImportSyouDbContext context)
        {
            _context = context;
        }

        public List<Memorycard> GetAll()
        {
            return _context.Memorycards.Include(c => c.Animal).ToList();
        }

        public Memorycard? GetById(int id)
        {
            return _context.Memorycards.Include(c => c.Animal).FirstOrDefault(c => c.Id == id);
        }

        public List<Memorycard> GetRandomPairs(int pairCount)
        {
            var pairs = _context.Memorycards
                .Include(c => c.Animal)
                .Where(c => c.PairType != null)
                .GroupBy(c => c.PairType)
                .OrderBy(_ => Guid.NewGuid())
                .Take(pairCount)
                .SelectMany(g => g.Take(2)) // 2 cartes par paire
                .ToList();

            return pairs;
        }
    }
}
