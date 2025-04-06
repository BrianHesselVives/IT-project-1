using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class TypeMassage
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public bool Actief { get; set; }

    public virtual ICollection<MasseurTypeMassage> MasseurTypeMassages { get; set; } = new List<MasseurTypeMassage>();

    public virtual ICollection<Prijs> Prijs { get; set; } = new List<Prijs>();

    public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();
}
