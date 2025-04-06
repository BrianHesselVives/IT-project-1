using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Prijs
{
    public int Id { get; set; }

    public float Prijzen { get; set; }

    public DateOnly Startdatum { get; set; }

    public int IdTypeMassage { get; set; }

    public virtual TypeMassage IdTypeMassageNavigation { get; set; } = null!;

    public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();
}
