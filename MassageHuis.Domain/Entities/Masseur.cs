using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Masseur
{
    public int Id { get; set; }

    public string GebruikerId { get; set; } = null!;

    public bool Actief { get; set; }

    public DateOnly? Einddienstverband { get; set; }

    public string? Beschrijving { get; set; }

    public virtual Gebruiker Gebruiker { get; set; } = null!;

    public virtual ICollection<MasseurTypeMassage> MasseurTypeMassages { get; set; } = new List<MasseurTypeMassage>();

    public virtual ICollection<Schema> Schemas { get; set; } = new List<Schema>();
}
