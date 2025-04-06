using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Schema
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Naam { get; set; } = null!;

    public DateOnly StartDatum { get; set; }

    public DateOnly? EindDatum { get; set; }

    public int IdMasseur { get; set; }

    public virtual Masseur IdMasseurNavigation { get; set; } = null!;

    public virtual ICollection<RegulierTijdslot> RegulierTijdslots { get; set; } = new List<RegulierTijdslot>();

    public virtual ICollection<UitzonderingTijdslot> UitzonderingTijdslots { get; set; } = new List<UitzonderingTijdslot>();
}
