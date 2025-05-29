# Syfte

Att säkerställa att applikationens kärnlogik och gränssnitt fungerar enligt specifikation, samt att spelreglerna följs.

## Testområden

| Område             | Testtyp            | Ansvarig/verktyg       |
|--------------------|-------------------|------------------------|
| Kortlek            | Enhetstest         | xUnit, Moq             |
| Handvärde          | Enhetstest         | xUnit                  |
| Spelregler         | Enhet/integration  | xUnit, Moq             |
| Satsning/betalning | Enhetstest         | xUnit                  |
| UI/flow            | Selenium/manuellt  | Selenium, manuell test |
| CI/Regression      | Automatisk         | Azure DevOps           |

## Prioriteringar

- Blackjack-logik (21 på två kort)
- Dealer-regler (måste stanna på minst 17)
- Vinst-/förlustberäkning och utbetalning
- Korrekt visning av kort och resultat i UI

## Exempel på teststrategi för UI (Selenium)

- Automatisera hela användarflödet: insats → dela ut kort → Hit/Stand → kontroll av resultat.
- Selenium hittar alla knappar (Deal, Hit, Stand), fyller i insats och spelar automatiskt två rundor.
- Tester pausar och väntar på att UI ska uppdateras och kontrollerar rätt resultat.

## Ej testat (utanför scope)

- Responsivitet på små skärmar/mobiler (testat endast desktop)
- Säkerhet/felhantering för multiuser/scaling
