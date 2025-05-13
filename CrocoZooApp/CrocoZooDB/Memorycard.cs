using System;
using System.Collections.Generic;

namespace CrocoZooApp.CrocoZooDB;

public partial class Memorycard
{
    public int Id { get; set; }

    public int? AnimalId { get; set; }

    public string? PairType { get; set; }

    public virtual Animal? Animal { get; set; }
}
