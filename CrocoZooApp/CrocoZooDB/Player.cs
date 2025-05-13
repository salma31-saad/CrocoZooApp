using System;
using System.Collections.Generic;

namespace CrocoZooApp.CrocoZooDB;

public partial class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Age { get; set; }

    public virtual ICollection<Gamesession> Gamesessions { get; set; } = new List<Gamesession>();
}
