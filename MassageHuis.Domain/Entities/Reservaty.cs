using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Reservaty
{
    public int Id { get; set; }

    public DateOnly DatumCreatie { get; set; }

    public DateOnly DatumReservatie { get; set; }

    public string? IdAspNetUsers { get; set; }

    public string? IdPromotieCode { get; set; }

    public int IdTypeMassage { get; set; }

    public int IdRegulierTijdslot { get; set; }

    public int IdPrijs { get; set; }

    public string Status { get; set; } = null!;

    public float TeBetalenBedrag { get; set; }

    public virtual ICollection<Betaling> Betalings { get; set; } = new List<Betaling>();

    public virtual AspNetUser? IdAspNetUsersNavigation { get; set; }

    public virtual KostPrij IdPrijsNavigation { get; set; } = null!;

    public virtual PromotieCode? IdPromotieCodeNavigation { get; set; }

    public virtual RegulierTijdslot IdRegulierTijdslotNavigation { get; set; } = null!;

    public virtual TypeMassage IdTypeMassageNavigation { get; set; } = null!;
}
