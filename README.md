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

## Katalogstruktur (utdrag)

src/
  Blackjack.Core/      // Domänlogik: models, services, interfaces
  Blackjack.Web/       // Web-app: controllers, views, models
  Blackjack.Tests/     // Enhetstester

## Testning

Projektet innehåller omfattande enhetstester för både kortlekslogik, handvärdering och spelmekanik. Testerna finns i filerna:

- `DeckServiceTests.cs`
- `HandServiceTests.cs`
- `GameServiceTests.cs`

## Så här kör du testerna

**För att köra alla tester:**
Test -> Test Explorer -> Run all tests

**Projektet är omfattande testat med hjälp av xUnit, Moq och FluentAssertions.**

- Alla viktiga delar (kortlek, händer, spelregler) har enhetstester.
- Testfallen täcker såväl normala scenarier som felhantering (exception).
- Testerna kan köras med kommandot `dotnet test`.
- Testerna verifierar bland annat:
    - Att en ny kortlek alltid har 52 kort.
    - Att ess, klädda kort och övriga kort ger rätt värde i en hand.
    - Att programmet hanterar oavgjort, vinst och förlust korrekt.
    - Att dealern stannar på rätt värde och att spelet skyddar mot felanvändning.

## Testade delar

### DeckService:

- Skapar en komplett kortlek med alla färger och valörer.
- Drar kort korrekt (minskar antal, tar bort rätt kort, hanterar tom kortlek).
- Blandar kortleken slumpmässigt och kan återskapa en ny kortlek (reset).
- Jämför att olika blandningar ger olika resultat.

### HandService:

- Räknar ut korrekt värde för en hand med olika kombinationer av ess, klädda kort och vanliga kort.
- Hanterar flera ess och olika scenarios som blackjack, bust och tom hand.
- Ger rätt värde på klädda kort (10 poäng) och ess (1 eller 11 poäng).

### GameService:

- Delar ut kort korrekt till spelare och dealer.
- Hanterar spelarens och dealerns drag och logik för när dealern måste dra kort.
- Avgör vinnare och korrekt hantering av oavgjort, vinst, förlust och blackjack.
- Skyddar mot ogiltiga drag om spelet inte initierats.

### Exempel på testfall

- Skapa en kortlek och säkerställ att det finns 52 unika kort.
- Testa att handvärdering fungerar med ess och klädda kort i olika kombinationer.
- Simulera hela spelet och kontrollera att utfall blir rätt (vinst, förlust, push).
- Kontroll av att programmet kastar rätt undantag vid ogiltiga operationer (t.ex. dra kort från tom kortlek).

## Exempel på användning

- Ange insats, tryck Dela ut kort.
- Spela genom att välja Ta kort (Hit) eller Stanna (Stand).
- Dina och dealerns kort visas med både svenska namn och symbol.
- När handen är klar visas vinst/förlust och totalt saldo.

## Projektets syfte

**Projektet är en del av kursmomentet i mjukvarutestning och demonstrerar:**

- Par- och mobprogrammering
- Enhetstestning med mockning
- Tillämpning av DevOps-praxis (kan enkelt kopplas till Azure DevOps/GitHub Actions)