using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class PromotieCode
{
    public string Id { get; set; } = null!;

    public DateOnly Vervaldatum { get; set; }

    public string Code { get; set; } = null!;

    public float Bedrag { get; set; }

    public string? IdAspNetUsers { get; set; }

    public string Status { get; set; } = null!;

    public string Type { get; set; } = null!;

    public float? ResterendWaarde { get; set; }

    public virtual AspNetUser? IdAspNetUsersNavigation { get; set; }

    public virtual ICollection<ReservatiePromotieCode> ReservatiePromotieCodes { get; set; } = new List<ReservatiePromotieCode>();

    public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();
}
