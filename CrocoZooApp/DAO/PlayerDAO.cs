using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoZooApp.CrocoZooDB;
using Microsoft.EntityFrameworkCore;


namespace CrocoZooApp.DAO
{
    public class PlayerDAO
    {
        private readonly ImportSyouDbContext _context;

        public PlayerDAO(ImportSyouDbContext context)
        {
            _context = context;
        }

        public List<Player> GetAll()
        {
            return _context.Players.ToList();
        }

        public Player? GetById(int id)
        {
            return _context.Players.Find(id);
        }

        public void Add(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        public void Update(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var player = _context.Players.Find(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                _context.SaveChanges();
            }
        }

        public Player? GetByName(string name)
        {
            return _context.Players.FirstOrDefault(p => p.Name == name);
        }
    }
}
