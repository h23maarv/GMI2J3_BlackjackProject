# Exempel på testfall

## Automatiska tester (xUnit/Moq)

| Testnamn                                              | Syfte                                                 | Förväntat resultat                |
|-------------------------------------------------------|-------------------------------------------------------|-----------------------------------|
| DealInitialHands_GivesTwoCardsToPlayerAndDealer       | Kontrollera att två kort ges till spelare och dealer  | Båda får 2 kort                   |
| PlayerHit_AddsCardToPlayerHand                        | Kontrollera att spelaren får ett kort till            | Spelarens hand ökar med ett kort  |
| DealerPlay_StopsWhenDealerReaches17OrMore             | Kontroll att dealern drar tills minst 17              | Dealerhandens värde ≥ 17          |
| EvaluateOutcome_PlayerGetsBlackjack_ReturnsPlayerWin  | Kontroll av vinst på blackjack                        | Resultat: PlayerWin               |
| EvaluateOutcome_Push_ReturnsPush                      | Testa oavgjort när båda har lika hand                 | Resultat: Push                    |

## UI/Selenium

| Steg                                        | Syfte                              | Förväntat resultat                     |
|---------------------------------------------|------------------------------------|----------------------------------------|
| Starta spel, fyll i insats, klicka "Deal"   | Simulera start                     | Två kort visas, dealer visar ett kort  |
| Klicka "Hit" tills > 21                     | Testa bust                         | Resultat: "Dealern vann"               |
| Spela en runda där du får blackjack         | Testa blackjack                    | Resultat: "Du fick blackjack"          |
| Klicka "Stanna" efter några kort            | Testa stand                        | Resultat visas                         |
| Klicka "Spela igen"                         | Ny runda startar                   | Nya kort delas ut                      |
