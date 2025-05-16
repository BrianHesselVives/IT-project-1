using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class ReservatiePromotieCode
{
    public int IdReservaties { get; set; }

    public string IdPromotieCode { get; set; } = null!;

    public DateOnly DatumToepassing { get; set; }

    public virtual PromotieCode IdPromotieCodeNavigation { get; set; } = null!;
}
