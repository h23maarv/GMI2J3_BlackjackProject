# Automatiska tester

- Alla enhetstester (xUnit) går igenom i pipelinen.
- Totalt antal tester: ca 10–15 stycken för kärnlogik och edge-cases.
- Seleniumtestet körs automatiskt och klarar att spela två rundor utan manuella fel.

# Manuella tester

- Genomförda UI-tester på desktop i Edge och Chrome.
- Alla huvudflöden fungerar: insats, dela ut kort, Hit, Stand, resultat, Spela igen.
- Test av gränsfall, t.ex. blackjack, bust, push.

# Buggar/upptäckta problem

- Initialt hanterades inte blackjack direkt vid utdelning – detta åtgärdades.
- Tidigare visades 11/12/13 istället för Knekt/Dam/Kung – detta fixades.
- UI/ikoner behövde designjusteringar, nu åtgärdat.

# Kodtäckning

- Majoriteten av kärnlogikens metoder har täckning via tester. (Exakt % saknas men alla huvudvägar är testade.)

# Kvalitet/sammanfattning

- Applikationen är stabil vid normal användning.
- Ingen känd blockerande bugg i nuvarande version.
- Klart för leverans.