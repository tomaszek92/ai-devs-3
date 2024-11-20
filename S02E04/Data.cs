namespace Task9;

public static class Data
{
    public static readonly Dictionary<string, string> Parsed = new()
    {
        ["2024-11-12_report-13.png"] = """
                                       ## REPAIR NOTE

                                       FROM: Repair department

                                       Godzina 08:15. Rozpoczęto naprawę anteny nadawczej w sektorze komunikacyjnym. Uszkodzenie powstało wskutek długotrwałej ekspozycji na warunki atmosferyczne, co osłabiło sygnał przesyłowy. Wymieniono główny element przekaźnika oraz dokręcono panel stabilizacyjny. Test przesyłu sygnału zakończony pozytywnie. Antena działa zgodnie ze specyfikacją.

                                       ---

                                       APPROVED BY Joseph N.
                                       """,
        ["2024-11-12_report-15.png"] = """
                                       Sure, here is the text from the image:

                                       ---

                                       REPAIR NOTE

                                       FROM: Repair department

                                       Godzina II:50. W czujniku ruchu wykryto usterkę spowodowaną zwarciem kabli. Przyczyną była mała mysz, która dostała się między przewody, powodując chwilowe przerwy w działaniu sensorów. Odłączono zasilanie, usunięto ciało obce i zabezpieczono osłony kabli przed dalszymi uszkodzeniami. Czujnik ponownie skalibrowany i sprawdzony pod kątem poprawności działania.

                                       APPROVED BY Joseph N.
                                       """,
        ["2024-11-12_report-14.png"] = """
                                       Sure, here's the text from the image:

                                       ---

                                       REPAIR NOTE

                                       FROM: Repair department

                                       Godzina 15:45. Zakończono aktualizację systemu komunikacji jednostek mobilnych. Dodano możliwość dynamicznego przydzielania kanałów w zależności od obciążenia oraz zaimplementowano protokół szyfrowania QII dla bezpieczniejszej wymiany danych. Test komunikacji między jednostkami wykazał pełną kompatybilność z nowym systemem. Monitorowanie aktywne w trybie bieżącym.

                                       APPROVED BY Joseph N.
                                       """,
        ["2024-11-12_report-16.png"] = """
                                       REPAIR NOTE

                                       FROM: Repair department

                                       Godzina 13:30. Przeprowadzono aktualizację modułu AI analizującego wzorce ruchu. Wprowadzono dodatkowe algorytmy umożliwiające szybsze przetwarzanie i bardziej precyzyjną analizę zachowań niepożądanych. Aktualizacja zakończona sukcesem, wydajność systemu wzrosła o 18%, co potwierdzają pierwsze testy operacyjne. Algorytmy działają w pełnym zakresie

                                       APPROVED BY Joseph N.
                                       """,
        ["2024-11-12_report-17.png"] = """
                                       **REPAIR NOTE**

                                       FROM: Repair department

                                       Godzina 09:20. Przeprowadzono procedurę wymiany przestarzałych ogniw w jednostkach mobilnych. Dotychczasowe ogniwa wykazywały obniżoną wydajność, wpływającą na zdolność operacyjną jednostek w dłuższych trasach patrolowych. Nowe ogniwa zostały zainstalowane zgodnie z wytycznymi technicznymi, a czas pracy jednostek uległ zwiększeniu o 15%. Monitorowanie w toku.

                                       ________________________________________

                                       APPROVED BY Joseph N.
                                       """,
        ["2024-11-12_report-10-sektor-C1.mp3"] =
            "Boss, we found one guy hanging around the gate. He was tinkering with something on the alarm equipment. He wouldn't say what he was doing here or who he was. He was arrested. After this incident, the squad went back to patrolling the area.",
        ["2024-11-12_report-11-sektor-C2.mp3"] =
            "I know I shouldn't be calling about this, but the mood in our brigade is deteriorating. I think it's down to our running out of pineapple pizza. Robots can survive for months without it, but we humans unfortunately cannot. On behalf of the whole team, I would like to request a pizza delivery. We've heard that there is a delivery man in the area named Matthew who can not only deliver such a pizza to us, but also bake it. Perhaps it would be worth recruiting him to our team?",
        ["2024-11-12_report-12-sektor_A1.mp3"] =
            "Boss, as directed, we searched the tenements in the nearby town for rebels. We were unable to find anyone. It appears that the town has been abandoned for a long time. We have already drawn up plans to search more towns in the coming days.        ",
        ["2024-11-12_report-05-sektor_C1.txt"] =
            "Godzina 04:02. Bez wykrycia aktywności organicznej lub technologicznej. Sensor dźwiękowy i detektory ruchu w pełnej gotowości. Bez niepokojących sygnałów w trakcie patrolu. Kontynuuję monitorowanie.",
        ["2024-11-12_report-03-sektor_A3.txt"] =
            "Godzina 01:30. Przebieg patroli nocnych na poziomie ściśle monitorowanym. Czujniki pozostają aktywne, a wytyczne dotyczące wykrywania życia organicznego – bez rezultatów. Stan patrolu bez zakłóceń.",
        ["2024-11-12_report-06-sektor_C2.txt"] =
            "Godzina 22:50. Sektor północno-zachodni spokojny, stan obszaru stabilny. Skanery temperatury i ruchu wskazują brak wykrycia. Jednostka w pełni operacyjna, powracam do dalszego patrolu.",
        ["2024-11-12_report-00-sektor_C4.txt"] =
            "Godzina 22:43. Wykryto jednostkę organiczną w pobliżu północnego skrzydła fabryki. Osobnik przedstawił się jako Aleksander Ragowski. Przeprowadzono skan biometryczny, zgodność z bazą danych potwierdzona. Jednostka przekazana do działu kontroli. Patrol kontynuowany.",
        ["2024-11-12_report-04-sektor_B2.txt"] =
            "Godzina 23:45. Patroluje zachodnią część terenu; brak anomalii ani odchyleń od normy. Sektor bezpieczny, wszystkie kanały komunikacyjne czyste. Przechodzę do następnego punktu.",
        ["2024-11-12_report-01-sektor_A1.txt"] =
            "Godzina 03:26. Wstępny alarm wykrycia – ruch organiczny. Analiza wizualna i sensoryczna wykazała obecność lokalnej zwierzyny leśnej. Fałszywy alarm. Obszar bezpieczny, wracam na trasę patrolu. Spokój przywrócony.",
        ["2024-11-12_report-02-sektor_A3.txt"] =
            "Godzina 02:15. Obszar patrolu nocnego cichy, bez wykrycia aktywności organicznej ani mechanicznej. Prowadzony monitoring peryferii obiektu. Kontynuacja zadań.",
        ["2024-11-12_report-07-sektor_C4.txt"] =
            "Godzina 00:11. Czujniki dźwięku wykryły ultradźwiękowy sygnał, pochodzenie: nadajnik ukryty w zielonych krzakach, nieopodal lasu. Przeprowadzono analizę obiektu. Analiza odcisków palców wskazuje osobę o imieniu Barbara Zawadzka, skorelowano z bazą urodzeń. Nadajnik przekazany do działu śledczego. Obszar zabezpieczony, patrol zakończony bez dalszych incydentów",
        ["2024-11-12_report-09-sektor_C2.txt"] =
            "Godzina 03:45. Patrol na peryferiach zachodnich zakończony. Czujniki nie wykazały żadnych niepokojących sygnałów. Obszar bez anomalii, kończę bieżący cykl i przechodzę do kolejnego sektora.",
        ["2024-11-12_report-08-sektor_A1.txt"] =
            "Godzina 01:00. Monitoring obszaru patrolowego: całkowity brak ruchu. Względna cisza, czujniki nie wykazały aktywności. Kontynuuję obserwację terenu według wyznaczonych wytycznych.",
    };
}