using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class UitzonderingTijdslot
{
    public int Id { get; set; }

    public DateOnly Datum { get; set; }

    public TimeOnly Startijd { get; set; }

    public TimeOnly Eindtijd { get; set; }

    public string TypeUitzondering { get; set; } = null!;

    public int IdSchema { get; set; }

    public virtual Schema IdSchemaNavigation { get; set; } = null!;
}
