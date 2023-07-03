# apiproject

Instructies

  - Om te gebruiken, open de folder "addressapi" in command line en typ 
  "dotnet run"
  - U kunt de api testen door Swagger UI op https://localhost:7195/swagger.

Informatie

  - Deze project is gebouwd in Visual Studio Code.
  - Als u deze project wilt bouwen, heeft u een Google Maps DistanceMatrix API key nodig voor string apiKey in AddressController.GetDistance om de afstand te berekenen.
  - Zoekwaardes zijn hoofdlettergevoelig.
  - Adresvelden beginnen met een hoofdletter:
    - Straat
    - Huisnummer
    - Postcode
    - Plaats
    - Land

Onderdelen waar ik trots op ben:
  
  - Wat
    - IEnumerableExtensions.OrderBy
  - Waarom
    - Dit is een extensie voor OrderBy, het maakt het mogelijk om een enumerable te sorteren in oplopende en aflopende volgorde op de gegeven adresveld. Dit maakt het sorteren van addressen generiek en dynamisch, als de adresveld niet te vinden is is de enumerable niet gesorteerd.
  
  - Wat
    - QueryHelpers.CreateSearchQuery
  - Waarom
    - Deze method zoekt door alle properties in een DbSet en maakt zo een dynamisch expression met een contains method voor elke string. Met deze method worden alle adresvelden doorgezoekt, zelfs als nieuwe adresvelden zijn toegevoegt.


Onderdelen waar ik niet tevreden over ben:

  - Wat
    - GetDistance
  - Waarom
    - Er is geen verwerking van regionale adresformaten.
  - Voorbeeld:
    - Verenigde staten:
      - HOUSE_NUMBER STREET_NAME
      - LOCALITY POSTAL_CODE
    - Nederland:
      - STREET_NAME HOUSE_NUMBER
      - POSTAL_CODE LOCALITY

  - Wat
    - GetAddresses
  - Waarom
    - De zoekwaarde is hoofdlettergevoelig. De case zegt niet of de zoekwaarde wel of niet hoofdlettergevoelig moet zijn, dus ik het niet veranderd.
  - Voorbeeld:
    - Zoekwaarde street is niet hetzelfde als Straat
