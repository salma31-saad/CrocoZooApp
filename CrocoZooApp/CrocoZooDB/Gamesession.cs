using System;
using System.Collections.Generic;

namespace CrocoZooApp.CrocoZooDB;

public partial class Gamesession
{
    public int Id { get; set; }

    public int? PlayerId { get; set; }

    public string GameMode { get; set; } = null!;

    public int? Score { get; set; }

    public DateTime? DatePlayed { get; set; }

    public virtual Player? Player { get; set; }
}
