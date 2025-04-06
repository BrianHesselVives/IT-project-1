using System;
using System.Collections.Generic;

namespace MassageHuis.Entities;

public partial class Gebruiker
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Naam { get; set; } = null!;

    public string Voornaam { get; set; } = null!;

    public DateOnly? Geboortedatum { get; set; }

    public string IdTypeGebruiker { get; set; } = null!;

    public virtual TypeGebruiker IdTypeGebruikerNavigation { get; set; } = null!;

    public virtual ICollection<Masseur> Masseurs { get; set; } = new List<Masseur>();

    public virtual ICollection<PromotieCode> PromotieCodes { get; set; } = new List<PromotieCode>();

    public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();
}
