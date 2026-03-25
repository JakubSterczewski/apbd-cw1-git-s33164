Projekt realizujący symulację wypożyczalni sprzętu.
Abstrakcyjna klasa User, gdyż nie chcemy tworzyć użytkowników bez specjalizacji (employee oraz Student, które dziedziczą po tej klasie wraz z nadpisywaniem maxRentals).
Equipments - przedmioty (Laptop, Camera, Projector) dziedziczą po tej abstract klasie, tak samo jak w przypadku Users. Equipments posiada ogólne pola, które każdy przedmiot posiada (Name itd) oraz pole Id tworzące przy pomocy klasy Guid.
Rental - klasa do tworzenia obiektów w danej strukturze (User, Equipment, daty, itd.)
RentalService - klasa przechowująca całą logikę wypożyczalnie, cały sprzęt i historię wypożyczeń. W klasie tej mamy metody do wypożyczania sprzętu (sprawdzając dostępność i limity użytkownika) oraz oddawanie wypożyczonych przedmiotów. Również w klasie tej mamy metody pomocnicze, takie jak zwracające wypożyczenia danego użytkownika, opóźnione, wszystkie wypożyczenia, raporty oraz dostępne przedmioty.

Kohezja - RentalService skupia się wyłącznie na zarządzaniu procesem wypożyczeń, unikając mieszania logiki z innymi domenami.
Coupling - User, Equipment są niezależnie wykorzystywane w RentalService
Odpowiedzialność klas - Rental odpowiada za śledzenie stanu konkretnego wypożyczenia i dostarcza informacje o jego statusie.

Wybrałem taki podział klas, plików, aby rozdzielić wszystkie klasy na jak najmniejsze ich funkcjonalności, aby jedna klasa była odpowiedzialna za jedną rzecz.