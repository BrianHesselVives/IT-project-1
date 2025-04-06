using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class TypeGebruiker
{
    public string Id { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
}
