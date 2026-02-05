

☕ Barroc Intense CRM & Maintenance System
Welkom bij de officiële repository van het Barroc Intense CRM-systeem. Dit is een Windows-desktopapplicatie gebouwd met WinUI 3 en .NET, ontworpen om de bedrijfsprocessen van Barroc Intense te stroomlijnen: van inkoop en voorraadbeheer tot klantenservice en onderhoud op locatie.

🏗️ Project Structuur
De applicatie is opgedeeld in drie kernmodules, elk met hun eigen verantwoordelijkheden:

1. Inkoop & Logistiek (Purchasing)
Beheert de stroom van goederen en de status van de voorraad.

InkoopDashBoard: Centraal overzicht voor inkoopmedewerkers.

StockPage: Real-time weergave van de actuele voorraadniveaus.

IngredientListPage: Gedetailleerd inzicht in de onderdelen/ingrediënten per koffiemachine.

DeliveryPage: Beheer van binnenkomende leveringen met statusfiltering (Onderweg/Geleverd).

2. Maintenance & Service (Onderhoud)
Ondersteunt monteurs bij keuringen en reparaties.

FormulierPage: Digitaal werkbon-systeem waar monteurs storingen registreren, voorraad afschrijven en een digitale handtekening van de klant kunnen vastleggen via een interactief canvas.

3. Klantenservice (Customer Support)
Het eerste aanspreekpunt voor klanten en storingsmeldingen.

KlantenservicePage: Registratie van nieuwe meldingen, toewijzen van monteurs en het beheren van prioriteiten.

🚀 Key Features
Entity Framework Core Integratie: Voor robuuste en snelle database-interactie.

Interactieve Handtekening: Monteurs kunnen op een canvas tekenen om werkbonnen direct te accorderen.

Dynamische Voorraad: Automatische afschrijving van materialen zodra een onderhoudsbeurt is voltooid.

Modern UI/UX: Een strak "Dark-Mode" design met de kenmerkende Barroc Intense geel/zwarte branding.

🛠️ Installatie & Setup
Vereisten
Visual Studio 2022 met de Windows App SDK workload.

.NET 6 SDK of hoger.

SQLite of LocalDB (geconfigureerd via AppDbContext).

Stappen
Clone de repository:

Bash
git clone https://github.com/jouw-gebruikersnaam/barroc-intense.git
Open de solution (.sln) in Visual Studio.

Herstel de NuGet-pakketten.

Voer de database-migraties uit (indien van toepassing):

PowerShell
Update-Database
Druk op F5 om de applicatie te starten.

🧪 Testing & Kwaliteit
De applicatie bevat uitgebreide testscenario's voor de kritieke paden:

Data Validatie: Controle op numerieke invoer bij prijzen en hoeveelheden.

Navigatie Tests: Validatie van parameter-doorvoer tussen pagina's (zoals ProductID naar Ingredienten).

CRUD Functionaliteit: Testen van het toevoegen, wijzigen en verwijderen van leveringen en meldingen.

🤝 Bijdragen
Maak een Feature branch aan.

Commit je wijzigingen.

Open een Pull Request.

⚖️ Licentie
Dit project is ontwikkeld voor Barroc Intense. Alle rechten voorbehouden.
