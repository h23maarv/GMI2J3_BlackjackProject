# Övergripande teststrategi

- Vi har använt **Test-Driven Development (TDD)** så långt som möjligt, det vill säga skrivit tester före implementation.
- Alla kärnkomponenter är utvecklade med enhetstester (**xUnit**).
- **Mocking** (Moq) används för beroenden, särskilt i `GameService` och `DeckService`.
- UI testas både **manuellt** och med **Selenium WebDriver** för att automatisera användarflöden.
- En **CI-pipeline** (Azure DevOps) är byggd och kör automatiskt alla tester vid varje push till GitHub.
- Kodtäckning har mätts periodvis (ange procent om ni vill, annars utelämna).
- Buggfixar och förbättringar har hanterats genom **issues** och **pull requests**.

## Testnivåer

- **Unit testing**: Alla logikklasser (`DeckService`, `HandService`, `GameService`, `BettingService`)
- **Integration testing**: Tester där flera tjänster samverkar, t.ex. `GameService` + `HandService`.
- **UI/acceptanstest**: Manuella tester samt Seleniumtest för hela spelrundor.
- **Regressionstestning**: Automatiska tester körs vid varje commit i pipelinen.

## Verktyg

- **xUnit** för enhetstestning
- **Moq** för mocking av beroenden
- **Selenium WebDriver** för automatiska UI-tester
- **Azure DevOps** för CI/CD
- **GitHub** för versionshantering
