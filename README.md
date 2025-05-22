# Blackjack WebApp – GMI2J3 mjukvarutestningsprojekt
Utav Grupp 6 bestående utav: William Ekengren, Mattias Arvidsson & William Ekbladh

Länk till Azure: https://dev.azure.com/h23wieke/ULTRA_BLACKJACK_WIN_EVERY_HAND

Detta är en webbapplikation för det klassiska kortspelet **Blackjack**, utvecklad i utbildningssyfte för kursen *Software Testing 1 (GMI2J3)* vid Högskolan i Dalarna. Projektet fokuserar på testdriven utveckling (TDD), enhetstestning och användning av moderna .NET-tekniker.

## Funktioner

- Spela Blackjack mot datorn via ett webbaserat gränssnitt.
- Sätt valfri insats (med minnesfunktion för senaste satsning).
- Spelregler enligt klassisk blackjack:
    - Ess = 1 eller 11 poäng (det som är mest fördelaktigt).
    - Knekt/Dam/Kung = 10 poäng.
    - 2–10 = nominellt värde.
- Svenska namn för alla sviter och valörer.
- Automatisk detektering av "bust" (>21 poäng).
- Resultat, utbetalning och vinst/förlust visas efter varje hand.
- Totalt saldo (sessionens vinst/förlust) visas och summeras tills sidan uppdateras eller webbläsaren stängs.
- Robust MVC-uppdelning och testbar kod.

## Teknik

- **Backend:** ASP.NET Core MVC (.NET 9), C#
- **Frontend:** Razor Pages, Bootstrap-stil (valfritt), Unicode-kortsymboler
- **Testning:** xUnit, Moq, FluentAssertions
- **Sessionshantering:** ASP.NET Core Session
- **Designmönster:** Dependency Injection, TDD, SOLID-principer

## Komma igång

1. **Krav:**
   - .NET 9 SDK (eller senare)
   - Visual Studio 2022+ eller VS Code

2. **Kloning och installation:**
   ```bash
   git clone [repo-url]
   cd Blackjack
   dotnet restore

3. **Starta applikationen:**
dotnet run --project src/Blackjack.Web
Gå till http://localhost:5094 i din webbläsare.

4. **Köra tester:**
dotnet test

## Katalogstruktur (utdrag)

src/
  Blackjack.Core/      // Domänlogik: models, services, interfaces
  Blackjack.Web/       // Web-app: controllers, views, models
  Blackjack.Tests/     // Enhetstester

## Exempel på användning

Ange insats, tryck Dela ut kort.

Spela genom att välja Ta kort (Hit) eller Stanna (Stand).

Dina och dealerns kort visas med både svenska namn och symbol.

När handen är klar visas vinst/förlust och totalt saldo.

## Projektets syfte

Projektet är en del av kursmomentet i mjukvarutestning och demonstrerar:

Par- och mobprogrammering

Enhetstestning med mockning

Tillämpning av DevOps-praxis (kan enkelt kopplas till Azure DevOps/GitHub Actions)

Dokumenterad teststrategi och rapport (se separat dokumentation)