using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class RegulierTijdslot
{
    public int Id { get; set; }

    public int IdSchema { get; set; }

    public int Dag { get; set; }

    public TimeOnly StartTijd { get; set; }

    public TimeOnly EindTijd { get; set; }

    public virtual Schema IdSchemaNavigation { get; set; } = null!;

    public virtual ICollection<Reservaty> Reservaties { get; set; } = new List<Reservaty>();
}
