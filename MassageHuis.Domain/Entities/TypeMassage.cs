using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class TypeMassage
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public bool Actief { get; set; }

    public virtual ICollection<KostPrij> KostPrijs { get; set; } = new List<KostPrij>();

    public virtual ICollection<MasseurTypeMassage> MasseurTypeMassages { get; set; } = new List<MasseurTypeMassage>();

    public virtual ICollection<Reservaty> Reservaties { get; set; } = new List<Reservaty>();
}
