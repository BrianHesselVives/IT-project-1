using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Masseur
{
    public int Id { get; set; }

    public bool Actief { get; set; }

    public DateOnly? Einddienstverband { get; set; }

    public string? Beschrijving { get; set; }

    public string? IdAspNetUsers { get; set; }

    public virtual AspNetUser? IdAspNetUsersNavigation { get; set; }

    public virtual ICollection<MasseurTypeMassage> MasseurTypeMassages { get; set; } = new List<MasseurTypeMassage>();

    public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();

    public virtual ICollection<Schema> Schemas { get; set; } = new List<Schema>();
}
