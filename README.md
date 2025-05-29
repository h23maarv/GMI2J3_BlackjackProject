# Blackjack WebApp – GMI2J3 mjukvarutestningsprojekt

**Av Grupp 6:**  
William Ekengren, Mattias Arvidsson & William Ekbladh

**Länk till Azure DevOps:**  
[https://dev.azure.com/h23wieke/ULTRA_BLACKJACK_WIN_EVERY_HAND](https://dev.azure.com/h23wieke/ULTRA_BLACKJACK_WIN_EVERY_HAND)

---

## Om projektet

Detta är en webbapplikation för det klassiska kortspelet **Blackjack**, utvecklad i utbildningssyfte för kursen *Software Testing 1 (GMI2J3)* vid Högskolan i Dalarna. Projektet har stort fokus på testdriven utveckling (TDD), enhetstestning, automatisering och modern .NET-utveckling.

---

## Funktioner

- Spela Blackjack mot datorn via ett webbaserat och mobilvänligt gränssnitt.
- Sätt valfri insats (senaste insats sparas under sessionen).
- Svenska namn för alla sviter och valörer (Ess, Knekt, Dam, Kung).
- Riktiga kortsymboler (♥ ♦ ♣ ♠) och kortdesign i UI.
- Spelregler enligt klassisk blackjack:
  - Ess = 1 eller 11 poäng (det som är mest fördelaktigt)
  - Knekt/Dam/Kung = 10 poäng
  - 2–10 = nominellt värde
- Automatisk detektering av "bust" (>21 poäng).
- Resultat, utbetalning och vinst/förlust visas efter varje hand.
- Totalt saldo för sessionen visas och summeras tills sidan uppdateras/stängs.
- Robust MVC-uppdelning och testbar kod.
- Visuell feedback för blackjack, oavgjort och vinst/förlust.
- Modern, responsiv layout med blackjackbordskänsla.

---

## Teknik

- **Backend:** ASP.NET Core MVC (.NET 9), C#
- **Frontend:** Razor Views, Bootstrap-inspirerad stil, anpassad CSS, Unicode-symboler
- **Testning:** xUnit, Moq, FluentAssertions, Selenium WebDriver
- **Sessionshantering:** ASP.NET Core Session
- **CI/CD:** Azure DevOps (pipeline kopplad till GitHub)
- **Designmönster:** Dependency Injection, TDD, SOLID

---

## Komma igång

**Krav:**  
- .NET 9 SDK (eller senare)  
- Visual Studio 2022, VS Code eller Jetbrains Rider

**Kloning och installation:**
```bash```
git clone [repo-url]
cd Blackjack
dotnet restore
dotnet run --project src/Blackjack.Web

Gå till http://localhost:5094 i din webbläsare.

src/
  Blackjack.Core/   // Domänlogik: models, services, interfaces
  Blackjack.Tests/  // Enhets- & UI-tester (xUnit, Moq, Selenium)
  Blackjack.Web/    // Web-app: controllers, views, viewmodels

## Testning

Projektet har omfattande tester för all kärnlogik och UI-flöde.

### Enhetstester

- **DeckServiceTests.cs:** Tester för skapande, blandning och dragning från kortlek.
- **HandServiceTests.cs:** Tester för handvärdering, flera ess, klädda kort, bust, blackjack m.m.
- **GameServiceTests.cs:** Tester för spelmekanik, deal, hit, stand, vinst/förlust, push, blackjack.

### Selenium/acceptanstest

- **SeleniumTests.cs:** Automatiserar ett riktigt användarflöde via webbläsare (Edge) med Selenium WebDriver.
    - *Exempel:* Spelar två omgångar, väljer insats, klickar på Hit/Stand tills handen är klar, verifierar resultat och sessionens vinst/förlust.

---

### Kör testerna så här

- **Visual Studio:**  
  Test → Test Explorer → Run all tests
- **CLI:**  
  `dotnet test`
- **CI:**  
  Körs automatiskt vid push till GitHub via Azure DevOps

---

### Viktiga testområden (alla täckta med tester)

- **Kortlek:** Skapande, dragning, blandning, unika kort
- **Handvärde:** Korrekt poäng vid alla möjliga kombinationer
- **Spelregler:** Deal, Hit, Stand, korrekt hantering av bust, blackjack och push
- **UI/acceptans:** Användarflöde, vinst/förlust/utbetalning, saldo
- **Felhantering:** Ex. dra kort från tom kortlek, spela utan att dealat

---

### Exempel på testfall

| Testnamn                                             | Syfte                                             | Förväntat resultat                     |
|------------------------------------------------------|---------------------------------------------------|----------------------------------------|
| DealInitialHands_GivesTwoCardsToPlayerAndDealer      | Kontroll av att två kort delas ut till båda parter| Båda har två kort                      |
| PlayerHit_AddsCardToPlayerHand                       | Spelaren får ett kort till vid Hit                | Handen ökar med ett kort               |
| DealerPlay_StopsWhenDealerReaches17OrMore            | Dealern drar tills minst 17 poäng                 | Dealerns handvärde ≥ 17                |
| EvaluateOutcome_PlayerGetsBlackjack_ReturnsPlayerWin | Vinst när spelare får blackjack på två kort       | Resultat: PlayerWin                    |
| EvaluateOutcome_Push_ReturnsPush                     | Testar oavgjort (push) vid lika värde             | Resultat: Push                         |
| Can_Play_Two_Games (Selenium)                        | Automatiserat UI-flöde, spelar två hela rundor    | Rätt vinst/förlust visas, UI fungerar  |

---

## Projektets syfte

Projektet är en del av kursmomentet i mjukvarutestning och demonstrerar:

- Par- och mobbprogrammering
- Testdriven utveckling (TDD) i praktiken
- Enhetstestning, mockning och integrationstestning
- CI/CD med DevOps (Azure DevOps + GitHub)
- Acceptanstestning med Selenium
- Testbar design och robust kodstruktur

---

## Bidrag och kontakt

Kontakta valfri gruppmedlem vid frågor, eller lämna en issue i GitHub-repot.

---

## Licens

För utbildningsbruk, *Software Testing 1* – Högskolan Dalarna.
