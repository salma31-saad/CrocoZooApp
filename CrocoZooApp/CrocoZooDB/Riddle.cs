using System;
using System.Collections.Generic;

namespace CrocoZooApp.CrocoZooDB;

public partial class Riddle
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public int? CorrectAnimalId { get; set; }

    public virtual Animal? CorrectAnimal { get; set; }
}
