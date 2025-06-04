# 💆‍♀️ Website MassageHuis

Welkom bij het MassageHuis Reserveringssysteem, een modern online platform voor het beheren van massageafspraken. Dit systeem stelt klanten in staat om gemakkelijk massages te boeken, masseurs om hun agenda's te beheren, en beheerders/uitbaters om het gehele proces te overzien.

## ✨ Kenmerken

* **Gebruikersauthenticatie & Autorisatie:** Robuuste beveiliging met ASP.NET Core Identity, inclusief rollen voor:
    * `Klant`: Gebruikers die massages kunnen boeken.
    * `Masseur`: Professionele masseurs die hun beschikbaarheid beheren.
    * `Uitbater`: De eigenaar van het MassageHuis met overkoepelende beheerrechten.
    * `Admin`: Full-access systeembeheerder.
* **Klantfunctionaliteit:**
    * Bladeren door beschikbare masseurs op basis van actieve schema's.
    * Interactieve kalenderweergave van de beschikbaarheid van masseurs.
    * Eenvoudig boeken en beheren van massageafspraken.
    * Persoonlijke profielbeheer.
* **Masseurfunctionaliteit:**
    * Beheer van persoonlijke werkschema's (reguliere tijdsloten).
    * Instellen van uitzonderingen (vrije dagen, verlof).
    * Inzicht in aankomende reserveringen.
* **Beheerder/Uitbaterfunctionaliteit:**
    * Centraal beheer van alle masseurs en hun gegevens.
    * Overzicht van alle reserveringen in het systeem.
    * Beheer van algemene uitzonderingstijdsloten (bijv. feestdagen, bedrijfssluiting).
* **Dynamische Kalender:** Een intuïtieve kalenderweergave voor het selecteren van beschikbare tijdsloten.
* **E-mailnotificaties:** Integratie voor het verzenden van e-mails, bijvoorbeeld voor reserveringsbevestigingen.
* **Database Integratie:** Efficiënte gegevensopslag en -ophaling via Entity Framework Core.

## 🚀 Technologieën

* **Backend:**
    * [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) (C#)
    * [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
    * [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) (ORM)
    * [AutoMapper](https://automapper.org/) (Object-object mapping)
* **Frontend:**
    * [Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/) (.NET)
    * [Bootstrap 5](https://getbootstrap.com/) (Responsief UI-framework)
    * JavaScript
* **Database:** (Standaard geconfigureerd met SQL Server, maar kan worden aangepast voor andere databases die door EF Core worden ondersteund).
* **Email:** Abstraheren van een e-mailverzendservice (`IEmailSend`).
