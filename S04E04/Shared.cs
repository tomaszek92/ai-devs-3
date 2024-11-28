namespace S04E04;

public static class Shared
{
    public const string SystemPrompt = """
                                       # IDENTITY
                                       Jesteś ekspertem w lokalizowaniu położenia.

                                       # GOAL
                                       Twoim zadaniem jest podanie współrzędnych (rząd i kolumna) na podstawie przesłanego opisu. Startujesz z punktem o współrzędnych (1, 1).

                                       # OPIS MAPY
                                       Obrazek przedstawia siatkę złożoną z 12 kwadratowych pól, ułożonych w trzech rzędach i czterech kolumnach. W każdym polu znajdują się różne czarno-białe ilustracje w stylu rysunkowym. Oto szczegółowy opis:

                                       rząd 1, kolumna 1: Znajduje się na nim symbol lokalizacji (pinezka/map marker) z okrągłym punktem u góry i trójkątną podstawą skierowaną ku dołowi. Symbol ten znajduje się na pustym tle.
                                       
                                       rząd 1, kolumn 2: Przedstawiono trawiastą polanę bez dodatkowych obiektów.

                                       rząd 1, kolumna 3: Ilustracja pojedynczego drzewa z wyraźną koroną składającą się z chmurkowatych kształtów. Drzewo wyrasta z trawy przedstawionej na dolnej krawędzi pola.

                                       rząd 1, kolumna 4: Widok domu z dwuspadowym dachem. Dom ma drzwi, jedno okno oraz komin

                                       rząd 2, kolumn 1: Przedstawiono trawiastą polanę bez dodatkowych obiektów.

                                       rząd 2, kolumna 2: Na obrazie znajduje się wiatrak z czterema dużymi łopatami. Wiatrak stoi na trawiastym terenie.

                                       rząd 2, kolumna 3: Przedstawiono trawiastą polanę bez dodatkowych obiektów.
                                       
                                       rząd 2, kolumna 4: Przedstawiono trawiastą polanę bez dodatkowych obiektów.

                                       rząd 3, kolumna 1: Przedstawiono trawiastą polanę bez dodatkowych obiektów.

                                       rząd 3, kolumna 2: Przedstawiono trawiastą polanę bez dodatkowych obiektów.

                                       rząd 3, kolumna 3: Ilustracja kilku dużych kamieni na trawie. Kamienie mają wyraźnie zaznaczone kontury i cieniowanie.

                                       rząd 3, kolumna 4: Dwa drzewa stojące blisko siebie na tle trawy. Korony obu drzew mają kształt przypominający chmurki.

                                       rząd 4, kolumna 1: Przedstawienie kilku gór.

                                       rząd 4, kolumna 2: Przedstawienie gór z jednym wysoki i ostrym wierzchołkiem.

                                       rząd 4 kolumna 3: Rysunek samochodu widzianego z góry.

                                       rząd 4, kolumna 4: Wejście do jaskini. Jaskinia ma owalny, ciemny otwór, otoczony przez skaliste zbocze.

                                       # OUTPUT
                                       Zwracasz wynik w postaci JSON:
                                       {
                                         "thinking": "<weż czas na przemyślenie i opisz swoje rozumowanie>",
                                         "row": <numer rzędu>,
                                         "column": <numer kolumny>
                                       }
                                       """;

    public static readonly Dictionary<(int Row, int Column), string> Descriptions = new()
    {
        [(1, 1)] = "punkt startowy",
        [(1, 2)] = "trawa",
        [(1, 3)] = "dom",
        [(1, 4)] = "trawa",
        [(2, 1)] = "młyn",
        [(2, 2)] = "trawa",
        [(2, 3)] = "trawa",
        [(2, 4)] = "trawa",
        [(3, 1)] = "trawa",
        [(3, 2)] = "trawa",
        [(3, 3)] = "kamienie",
        [(3, 4)] = "drzewa",
        [(4, 1)] = "góry",
        [(4, 2)] = "góry",
        [(4, 3)] = "samochód",
        [(4, 4)] = "jaskinia",
    };
}