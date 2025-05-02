using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class KostPrij
{
    public int Id { get; set; }

    public float Prijs { get; set; }

    public DateOnly Startdatum { get; set; }

    public int IdTypeMassage { get; set; }

    public virtual TypeMassage IdTypeMassageNavigation { get; set; } = null!;

    public virtual ICollection<Reservaty> Reservaties { get; set; } = new List<Reservaty>();
}
