using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Betaling
{
    /// <summary>
    /// UUID
    /// </summary>
    public string Id { get; set; } = null!;

    public DateOnly DatumBetaling { get; set; }

    public string Betaalmethode { get; set; } = null!;

    public string? TransactieReferentie { get; set; }

    public float BetaaldBedrag { get; set; }

    public string? Opmerking { get; set; }

    public int IdReservaties { get; set; }

    public virtual Reservaty IdReservatiesNavigation { get; set; } = null!;
}
