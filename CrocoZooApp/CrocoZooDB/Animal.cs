using System;
using System.Collections.Generic;

namespace CrocoZooApp.CrocoZooDB;

public partial class Animal
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public string SoundPath { get; set; } = null!;

    public virtual ICollection<Memorycard> Memorycards { get; set; } = new List<Memorycard>();

    public virtual ICollection<Riddle> Riddles { get; set; } = new List<Riddle>();
}
