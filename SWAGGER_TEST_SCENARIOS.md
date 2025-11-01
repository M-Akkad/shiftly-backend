# Shiftly API - Swagger Test Scenario's

## 📋 Inhoudsopgave
1. [Setup](#setup)
2. [Basis Informatie Ophalen](#basis-informatie-ophalen)
3. [UC-03: Wedstrijd Aanmaken en Beheren](#uc-03-wedstrijd-aanmaken-en-beheren)
4. [UC-04: Spelers Toewijzen aan Wedstrijd](#uc-04-spelers-toewijzen-aan-wedstrijd)
5. [UC-05: Eigen Wedstrijden Bekijken](#uc-05-eigen-wedstrijden-bekijken)
6. [UC-06: Afwezigheid Doorgeven](#uc-06-afwezigheid-doorgeven)
7. [Edge Cases en Validatie Tests](#edge-cases-en-validatie-tests)

---

## Setup

### Applicatie Starten
```bash
cd C:\Users\Akkad\Documents\Project_ruha\Shiftly
dotnet run --project Shiftly/Shiftly.csproj
```

### Swagger UI Openen
De applicatie opent automatisch in je browser op: `https://localhost:7061/`

Swagger UI is geconfigureerd op de **root URL** (niet op `/swagger`).

### Test Data
Bij elke start wordt de database opnieuw aangemaakt met de volgende test data:

**Admins:**
- ID 1: Jan de Trainer (jan@shiftly.nl)
- ID 2: Piet Beheerder (piet@shiftly.nl)

**Spelers (8 stuks):**
- ID 1: Mohammed Ali - Aanvaller #9
- ID 2: Sara de Vries - Middenvelder #10
- ID 3: Ahmed Hassan - Verdediger #5
- ID 4: Lisa Jansen - Keeper #1
- ID 5: Kevin Bakker - Aanvaller #11
- ID 6: Fatima El Amrani - Middenvelder #8
- ID 7: Thomas Visser - Verdediger #3
- ID 8: Yasmin Özdemir - Aanvaller #7

**Wedstrijden (3 stuks):**
- ID 1: FC Utrecht (over 10 dagen, 14:00)
- ID 2: PSV (over 17 dagen, 16:30)
- ID 3: Feyenoord (over 24 dagen, 13:00)

---

## Basis Informatie Ophalen

### Test 1: Haal alle spelers op
**Doel:** Verkrijg een overzicht van alle spelers

**Stappen:**
1. Klik op `GET /api/Speler`
2. Klik op "Try it out"
3. Klik op "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat 8 spelers
- Elke speler heeft: ID, Naam, Email, Positie, Rugnummer

**📝 Noteer:** Speler ID voor latere tests (bijv. ID 1)

---

### Test 2: Haal alle admins op
**Doel:** Verkrijg een overzicht van alle admins

**Stappen:**
1. Klik op `GET /api/Admin`
2. "Try it out" → "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat 2 admins
- Admin 1: Jan de Trainer
- Admin 2: Piet Beheerder

**📝 Noteer:** Admin ID voor latere tests (bijv. ID 1)

---

### Test 3: Haal alle wedstrijden op
**Doel:** Verkrijg een overzicht van alle wedstrijden

**Stappen:**
1. Klik op `GET /api/Wedstrijd`
2. "Try it out" → "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat 3 wedstrijden
- Wedstrijden zijn gesorteerd op datum en tijd
- Elke wedstrijd bevat: Datum, Tijd, Locatie, Tegenstander, Admin info, Lijst van spelers

**📝 Noteer:** Wedstrijd ID voor latere tests (bijv. ID 1)

---

### Test 4: Haal specifieke speler op
**Doel:** Details van één speler opvragen

**Stappen:**
1. Klik op `GET /api/Speler/{id}`
2. "Try it out"
3. Vul in bij `id`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: Mohammed Ali - Aanvaller #9

---

### Test 5: Haal specifieke wedstrijd op
**Doel:** Details van één wedstrijd opvragen

**Stappen:**
1. Klik op `GET /api/Wedstrijd/{id}`
2. "Try it out"
3. Vul in bij `id`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat wedstrijd details + admin + alle toegewezen spelers met hun status

---

## UC-03: Wedstrijd Aanmaken en Beheren

### Test 6: Nieuwe wedstrijd aanmaken
**Doel:** Maak een nieuwe wedstrijd aan als admin

**Stappen:**
1. Klik op `POST /api/Wedstrijd`
2. "Try it out"
3. Vul de volgende JSON in bij Request body:
```json
{
  "datum": "2025-12-01T00:00:00",
  "tijd": "15:00:00",
  "locatie": "Eigen Stadion",
  "tegenstander": "Ajax",
  "adminID": 1
}
```
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `201 Created`
- Response bevat de nieuwe wedstrijd met een ID (waarschijnlijk ID 4)
- Header `Location` bevat URL naar de nieuwe wedstrijd

**📝 Noteer:** ID van nieuwe wedstrijd voor latere tests

---

### Test 7: Wedstrijd met datum in verleden (negatieve test)
**Doel:** Controleer validatie van datum

**Stappen:**
1. Klik op `POST /api/Wedstrijd`
2. "Try it out"
3. Vul in:
```json
{
  "datum": "2020-01-01T00:00:00",
  "tijd": "15:00:00",
  "locatie": "Test Locatie",
  "tegenstander": "Test Team",
  "adminID": 1
}
```
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message: "Wedstrijddatum moet in de toekomst liggen"

---

### Test 8: Haal wedstrijden van admin op
**Doel:** Bekijk welke wedstrijden een admin heeft aangemaakt

**Stappen:**
1. Klik op `GET /api/Admin/{adminId}/wedstrijden`
2. "Try it out"
3. Vul in bij `adminId`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat alleen wedstrijden waar `adminID = 1`

---

## UC-04: Spelers Toewijzen aan Wedstrijd

### Test 9: Haal beschikbare spelers voor wedstrijd op
**Doel:** Bekijk welke spelers nog toegewezen kunnen worden

**Stappen:**
1. Klik op `GET /api/Wedstrijd/{wedstrijdId}/beschikbare-spelers`
2. "Try it out"
3. Vul in bij `wedstrijdId`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat spelers die NIET aan wedstrijd 1 toegewezen zijn
- Bijvoorbeeld spelers 5, 6, 7, 8 (afhankelijk van seed data)

**📝 Noteer:** Een beschikbare speler ID (bijv. ID 5)

---

### Test 10: Wijs speler toe aan wedstrijd
**Doel:** Voeg een speler toe aan een wedstrijd

**Stappen:**
1. Klik op `POST /api/Wedstrijd/{wedstrijdId}/spelers/{spelerId}`
2. "Try it out"
3. Vul in:
   - `wedstrijdId`: `1`
   - `spelerId`: `5` (of een andere beschikbare speler)
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: `{ "message": "Speler succesvol toegewezen aan wedstrijd" }`

---

### Test 11: Controleer dat speler toegevoegd is
**Doel:** Verifieer dat de toewijzing gelukt is

**Stappen:**
1. Klik op `GET /api/Wedstrijd/{id}`
2. Vul in bij `id`: `1`
3. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- In de lijst `wedstrijdSpelers` staat nu speler 5
- Status van speler 5 is "Aanwezig"

---

### Test 12: Dubbele toewijzing (negatieve test)
**Doel:** Controleer dat spelers niet dubbel toegewezen kunnen worden

**Stappen:**
1. Klik op `POST /api/Wedstrijd/{wedstrijdId}/spelers/{spelerId}`
2. Vul in:
   - `wedstrijdId`: `1`
   - `spelerId`: `5` (dezelfde als vorige test)
3. "Execute"

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message: "Speler is al toegewezen aan deze wedstrijd"

---

### Test 13: Verwijder speler van wedstrijd
**Doel:** Haal een speler van een wedstrijd af

**Stappen:**
1. Klik op `DELETE /api/Wedstrijd/{wedstrijdId}/spelers/{spelerId}`
2. "Try it out"
3. Vul in:
   - `wedstrijdId`: `1`
   - `spelerId`: `5`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: `{ "message": "Speler succesvol verwijderd van wedstrijd" }`

---

### Test 14: Controleer dat speler verwijderd is
**Stappen:**
1. `GET /api/Wedstrijd/1/beschikbare-spelers`

**✅ Verwacht resultaat:**
- Speler 5 staat weer in de lijst van beschikbare spelers

---

## UC-05: Eigen Wedstrijden Bekijken

### Test 15: Haal wedstrijden van speler op
**Doel:** Bekijk aan welke wedstrijden een speler is toegewezen

**Stappen:**
1. Klik op `GET /api/Speler/{spelerId}/wedstrijden`
2. "Try it out"
3. Vul in bij `spelerId`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response bevat wedstrijden waar speler 1 aan toegewezen is
- Elke wedstrijd toont de status van de speler (Aanwezig/Afwezig)

---

### Test 16: Speler zonder wedstrijden
**Doel:** Test met speler die aan geen wedstrijden is toegewezen

**Stappen:**
1. `GET /api/Speler/{spelerId}/wedstrijden`
2. Vul in bij `spelerId`: `8` (of een speler die niet toegewezen is)
3. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: lege lijst `[]`

---

## UC-06: Afwezigheid Doorgeven

### Test 17: Geldige afwezigheid doorgeven
**Doel:** Speler meldt afwezigheid voor wedstrijd (> 7 dagen in toekomst)

**Voorwaarde:** Gebruik een wedstrijd die verder dan 7 dagen in de toekomst ligt (bijv. wedstrijd 1, 2 of 3)

**Stappen:**
1. Eerst controleren dat speler toegewezen is: `GET /api/Speler/1/wedstrijden`
2. Klik op `POST /api/Speler/{spelerId}/wedstrijden/{wedstrijdId}/afwezigheid`
3. "Try it out"
4. Vul in:
   - `spelerId`: `1`
   - `wedstrijdId`: `1` (of een wedstrijd > 7 dagen)
5. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: `{ "message": "Afwezigheid succesvol doorgegeven" }`

---

### Test 18: Controleer afwezigheid status
**Doel:** Verifieer dat de status is gewijzigd

**Stappen:**
1. `GET /api/Speler/1/wedstrijden`
2. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Bij wedstrijd 1 staat nu status "Afwezig" in de wedstrijdSpelers lijst

---

### Test 19: Dubbele afwezigheid (negatieve test)
**Doel:** Controleer dat je niet tweemaal afwezigheid kunt doorgeven

**Stappen:**
1. `POST /api/Speler/1/wedstrijden/1/afwezigheid` (opnieuw)
2. "Execute"

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message: "Status is al Afwezig"

---

### Test 20: Afwezigheid terugdraaien
**Doel:** Wijzig status terug van Afwezig naar Aanwezig

**Stappen:**
1. Klik op `DELETE /api/Speler/{spelerId}/wedstrijden/{wedstrijdId}/afwezigheid`
2. "Try it out"
3. Vul in:
   - `spelerId`: `1`
   - `wedstrijdId`: `1`
4. "Execute"

**✅ Verwacht resultaat:**
- Status code: `200 OK`
- Response: `{ "message": "Status succesvol gewijzigd naar Aanwezig" }`

---

### Test 21: Controleer dat status teruggezet is
**Stappen:**
1. `GET /api/Speler/1/wedstrijden`

**✅ Verwacht resultaat:**
- Status van wedstrijd 1 is weer "Aanwezig"

---

### Test 22: 7-dagen regel testen
**Doel:** Controleer dat afwezigheid binnen 7 dagen geblokkeerd wordt

**⚠️ Opmerking:** Deze test is alleen uit te voeren als je een wedstrijd binnen 7 dagen hebt. In de standaard seed data liggen alle wedstrijden > 7 dagen in de toekomst.

**Alternatief:** Maak handmatig een wedstrijd aan die binnen 7 dagen valt:
1. `POST /api/Wedstrijd`
```json
{
  "datum": "2025-11-05T00:00:00",
  "tijd": "15:00:00",
  "locatie": "Test Locatie",
  "tegenstander": "Test Team",
  "adminID": 1
}
```
2. Wijs speler toe: `POST /api/Wedstrijd/{nieuwWedstrijdId}/spelers/1`
3. Probeer afwezigheid door te geven: `POST /api/Speler/1/wedstrijden/{nieuwWedstrijdId}/afwezigheid`

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message: "Je kunt je afwezigheid alleen doorgeven voor wedstrijden die verder dan één week in de toekomst liggen."

---

## Edge Cases en Validatie Tests

### Test 23: Niet-bestaande speler
**Stappen:**
1. `GET /api/Speler/999`

**✅ Verwacht resultaat:**
- Status code: `404 Not Found`
- Error message: "Speler niet gevonden"

---

### Test 24: Niet-bestaande wedstrijd
**Stappen:**
1. `GET /api/Wedstrijd/999`

**✅ Verwacht resultaat:**
- Status code: `404 Not Found`
- Error message: "Wedstrijd niet gevonden"

---

### Test 25: Niet-bestaande admin
**Stappen:**
1. `GET /api/Admin/999`

**✅ Verwacht resultaat:**
- Status code: `404 Not Found`
- Error message: "Admin niet gevonden"

---

### Test 26: Wedstrijd aanmaken met ongeldige admin
**Stappen:**
1. `POST /api/Wedstrijd`
```json
{
  "datum": "2025-12-01T00:00:00",
  "tijd": "15:00:00",
  "locatie": "Test",
  "tegenstander": "Test",
  "adminID": 999
}
```

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message: "Admin niet gevonden"

---

### Test 27: Speler toewijzen aan niet-bestaande wedstrijd
**Stappen:**
1. `POST /api/Wedstrijd/999/spelers/1`

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message bevat informatie over niet-bestaande wedstrijd

---

### Test 28: Niet-bestaande speler toewijzen
**Stappen:**
1. `POST /api/Wedstrijd/1/spelers/999`

**✅ Verwacht resultaat:**
- Status code: `400 Bad Request`
- Error message bevat informatie over niet-bestaande speler

---

## Complete Test Flow - Happy Path

**Scenario:** Admin maakt wedstrijd, wijst spelers toe, speler bekijkt wedstrijden en geeft afwezigheid door

### Stap 1: Admin maakt nieuwe wedstrijd
```
POST /api/Wedstrijd
{
  "datum": "2025-12-15T00:00:00",
  "tijd": "19:00:00",
  "locatie": "ArenA",
  "tegenstander": "FC Utrecht",
  "adminID": 1
}
```
✅ Wedstrijd aangemaakt met ID 4

### Stap 2: Admin bekijkt beschikbare spelers
```
GET /api/Wedstrijd/4/beschikbare-spelers
```
✅ Lijst met alle 8 spelers

### Stap 3: Admin wijst 4 spelers toe
```
POST /api/Wedstrijd/4/spelers/1
POST /api/Wedstrijd/4/spelers/2
POST /api/Wedstrijd/4/spelers/3
POST /api/Wedstrijd/4/spelers/4
```
✅ Alle 4 spelers toegevoegd

### Stap 4: Speler 1 bekijkt eigen wedstrijden
```
GET /api/Speler/1/wedstrijden
```
✅ Ziet wedstrijd 4 in de lijst met status "Aanwezig"

### Stap 5: Speler 1 geeft afwezigheid door
```
POST /api/Speler/1/wedstrijden/4/afwezigheid
```
✅ Status gewijzigd naar "Afwezig"

### Stap 6: Admin controleert wedstrijd
```
GET /api/Wedstrijd/4
```
✅ Ziet dat speler 1 status "Afwezig" heeft

### Stap 7: Speler 1 kan toch, draait afwezigheid terug
```
DELETE /api/Speler/1/wedstrijden/4/afwezigheid
```
✅ Status terug naar "Aanwezig"

---

## Handige Tips

### Response Codes Begrijpen
- **200 OK**: Succesvol opgehaald/gewijzigd
- **201 Created**: Nieuw item aangemaakt
- **400 Bad Request**: Validatiefout (bijv. verkeerde data, 7-dagen regel)
- **404 Not Found**: Item bestaat niet

### Database Resetten
Als je opnieuw wilt beginnen met verse test data:
1. Stop de applicatie (Ctrl+C)
2. Verwijder de database: `rm Shiftly/shiftly.db` (of verwijder handmatig)
3. Start de applicatie opnieuw: `dotnet run --project Shiftly/Shiftly.csproj`

De `DbInitializer` maakt automatisch nieuwe test data aan!

### Swagger UI Features
- **Try it out**: Activeert het formulier om data in te vullen
- **Execute**: Voert de request uit
- **Clear**: Reset het formulier
- **Response body**: Toont het JSON antwoord
- **Response headers**: Toont HTTP headers
- **Request URL**: Toont de volledige URL die aangeroepen is

---

## Checklist Complete Test Run

Gebruik deze checklist om een volledige test run te doen:

- [ ] Test 1-5: Basis informatie ophalen (Spelers, Admins, Wedstrijden)
- [ ] Test 6-8: UC-03 Wedstrijd aanmaken en beheren
- [ ] Test 9-14: UC-04 Spelers toewijzen aan wedstrijd
- [ ] Test 15-16: UC-05 Eigen wedstrijden bekijken
- [ ] Test 17-22: UC-06 Afwezigheid doorgeven (inclusief 7-dagen regel)
- [ ] Test 23-28: Edge cases en validatie
- [ ] Complete Happy Path scenario

**Geschatte tijd:** 30-45 minuten voor volledige test run

---

## Problemen?

Als je errors krijgt:
1. Controleer of de applicatie draait (check terminal output)
2. Controleer of je op `https://localhost:7061` bent
3. Check of de database correct is aangemaakt (zie console output bij start)
4. Bij twijfel: reset de database (zie hierboven)

Veel succes met testen! 🚀
