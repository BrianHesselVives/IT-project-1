using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class MasseurTypeMassage
{
    public int IdMasseur { get; set; }

    public int? Column { get; set; }

    public int IdTypeMassage { get; set; }

    public bool Actief { get; set; }

    public virtual Masseur IdMasseurNavigation { get; set; } = null!;

    public virtual TypeMassage IdTypeMassageNavigation { get; set; } = null!;
}
