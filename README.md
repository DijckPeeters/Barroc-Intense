# ☕ Barroc Intense – Interne Medewerkers Applicatie

## 📖 Beschrijving

Deze applicatie is ontwikkeld als **intern systeem voor medewerkers van Barroc Intense**, een koffiebedrijf gespecialiseerd in koffieproducten, service en zakelijke dienstverlening.

De applicatie ondersteunt medewerkers bij interne processen zoals:

* Beheer van interne gegevens
* Interne administratie
* Medewerkerstoegang tot het systeem
* Digitale ondersteuning van bedrijfsprocessen

De applicatie is **uitsluitend bedoeld voor intern gebruik** en niet voor klanten of extern publiek.

---

## ⚙️ Systeemvereisten

De volgende software is vereist om de applicatie te kunnen gebruiken:

### Software

* **Visual Studio Code** (code editor)
* **Laragon** (local development environment)

  * PHP
  * MySQL
  * Apache/Nginx
* Webbrowser (Chrome of Edge aanbevolen)

> Laragon verzorgt automatisch:
>
> * PHP-configuratie
> * Database (MySQL)
> * Webserver

---

## 📥 Installatie-instructies (stap-voor-stap)

### Stap 1 – Project plaatsen

1. Download of clone het project
2. Plaats de projectmap in:

```
C:\laragon\www\
```

Voorbeeld:

```
C:\laragon\www\barroc-intense
```

---

### Stap 2 – Laragon starten

1. Start **Laragon**
2. Klik op **Start All**

---

### Stap 3 – Database instellen

1. Open Laragon
2. Klik op **Database** → **Open**
3. Maak een nieuwe database aan

Naamvoorbeeld:

```
barroc_intense
```

4. Importeer het `.sql` databasebestand (indien aanwezig)

---

### Stap 4 – Environment configuratie

Indien het project een Laravel-project is:

1. Kopieer:

```
.env.example
```

2. Hernoem naar:

```
.env
```

3. Pas de databaseconfiguratie aan:

```
DB_DATABASE=barroc_intense
DB_USERNAME=root
DB_PASSWORD=
```

---

### Stap 5 – Dependencies installeren

Open de terminal in de projectmap en voer uit:

```
composer install
```

```
npm install
```

```
npm run build
```

---

### Stap 6 – Laravel configuratie

```
php artisan key:generate
```

---

## ▶️ Applicatie starten

Start de applicatie via de browser:

```
http://barroc-intense.test
```

of

```
http://localhost/barroc-intense/public
```

(afhankelijk van Laragon-configuratie)

---

## 🔐 Test-inloggegevens

Gebruik de volgende testaccount:

```
Gebruikersnaam: marc
Wachtwoord: 1234
```

---

## ⚠️ Bekende beperkingen & aandachtspunten

* Alleen bedoeld voor **intern gebruik**
* Geen productiebeveiliging
* Basis-authenticatie
* Geen encryptie van gevoelige data
* Geen automatische back-ups
* Geen logging-systeem
* Geen foutmonitoring
* Niet geoptimaliseerd voor mobiel gebruik
* Beveiliging is niet productiegeschikt
* Geen rechtenstructuur (admin/medewerker) tenzij toegevoegd

---

## 🛠 Ontwikkelomgeving

Aanbevolen setup:

* Visual Studio Code
* Laragon
* PHP 8+
* MySQL
* Node.js
* npm

---

## 👨‍💻 Informatie voor developers

* Project is opgezet als leer- en ontwikkelproject
* Structuur is modulair opgezet
* Gericht op uitbreidbaarheid
* Geschikt voor doorontwikkeling
* Nieuwe functionaliteiten kunnen worden toegevoegd via:

  * Controllers
  * Views
  * Routes
  * Database migrations
  * Models

---

## 🚀 Toekomstige uitbreidingen (optioneel)

Mogelijke uitbreidingen:

* Rollen- en rechtenstructuur
* Beveiliging (JWT / OAuth)
* Logging
* Back-ups
* API-koppelingen
* Rapportagesysteem
* Dashboard met statistieken
* Productbeheer
* Orderbeheer
* Klantbeheer

---

## 📄 Licentie

Dit project is ontwikkeld als schoolproject en leerproject.
Niet bedoeld voor commercieel gebruik of productieomgevingen zonder verdere beveiliging en optimalisatie.

---

© Barroc Intense – Interne Medewerkers Applicatie
