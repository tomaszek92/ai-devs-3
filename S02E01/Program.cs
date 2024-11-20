﻿using System.Net.Http.Json;
using OpenAI.Audio;
using OpenAI.Chat;

var transcriptions = new Dictionary<string, string>
{
    {
        "Michał",
        "Gość miał ambicje, znałem go w sumie od dzieciństwa, w zasadzie to znałem, bo trochę nam się kontakt urwał, ale jak najbardziej, pracowaliśmy razem. On zawsze chciał pracować na jakiejś znanej uczelni, po studiach pamiętam, został na uczelni i robił doktorat z sieci neuronowych i uczenia maszynowego, potem przeniósł się na inną uczelnię i pracował chwilę w Warszawie, ale to był tylko epizod z Warszawy. On zawsze mówił, że zawsze musi pracować na jakiejś ważnej uczelni, bo w tym środowisku bufonów naukowych to się prestiż liczy. Mówił, królewska uczelnia, to jest to, co chce osiągnąć, na tym mu zależało. Mówił, ja się tam dostanę, zobaczysz, no i będę tam wykładał. Z tego co wiem, no to osiągnął swój cel, no i brawa dla niego. Lubię ludzi, którzy jak się uprzą, że coś zrobią, to po prostu to zrobią, ale to nie było łatwe, ale gościowi się udało i to wcale nie metodą po trupach do celu. Andrzej był okej, szanował ludzi, marzył o tej uczelni i z tego co wiem, to na niej wylądował. Nie miałem z nim już kontaktu, ale widziałem, że profil na Linkedin zaktualizował. Nie powiedzieliście mi, dlaczego go szukacie, bo praca na uczelni to nie jest coś zabronionego, prawda? A, z rzeczy ważnych, to chciałbym wiedzieć, dlaczego jestem tu, gdzie jestem i kiedy się skończy to przesłuchanie. Dostaję pytania chyba od dwóch godzin i w sumie powiedziałem już wszystko, co wiem."
    },
    {
        "Adrian",
        " No pewnie. Obserwowałem jego dokonania i muszę przyznać, że zrobił na mnie wrażenie. Ja mam taką pamięć opartą na wrażeniach. I wrażenie mi pozostało po pierwszym spotkaniu. Nie wiem kiedy to było, ale on był taki... taki nietypowy. Później zresztą zastanawiałem się, jak to jest możliwe, że robi tak wiele rzeczy. Nieprzeciętny, ale swój. Znany w końcu to Andrzej. Naukowiec. Później chyba zniknął z miejsc, gdzie go śledziłem. Przy okazji jakiejś konferencji czy eventu chyba widziałem go, ale nie udało mi się z nim porozmawiać. Nie, nie mamy żadnego kontaktu. Nie jest moją rodziną, więc dlaczego miałbym ukrywać? Ja go tylko obserwowałem. Różnych ludzi się obserwuje. To nie zbrodnia, prawda? Kiedy w końcu zostawicie mnie w spokoju?"
    },
    {
        "Rafał",
        "Andrzejek, Andrzejek! Myślę, że osiągnął to co chciał. Jagiełło byłby z niego bardzo dumny. Chociaż, nie wiem, może coś mi się myli. Jagiełło chyba nie był jego kolegą i raczej nie miał z tą uczelnią wiele wspólnego. To tylko nazwa. Taka nazwa. To był jakiś wielki gość, bardziej co ją założył. Ale co to ma do rzeczy? Ale czy Andrzejek go znał? Chyba nie, ale nie wiem, bo Andrzejek raczej nie żył w czternastym wieku. Kto go tam wie? Mógł odwiedzić czternasty wiek. Ja bym odwiedził. Tego instytutu i tak wtedy nie było. To nowe coś. Ta ulica od matematyka co wpada w komendanta to chyba dwudziesty wiek. Ten czas mi się miesza. Wszystko jest takie nowe. To jest nowy, lepszy świat. Podoba ci się świat, w którym żyjesz? Andrzej zawsze był dziwny, kombinował coś i mówił, że podróże w czasie są możliwe. Razem pracowaliśmy nad tymi podróżami. To wszystko, co teraz się dzieje i ten stan, w którym jestem, to jest wina tych wszystkich podróży, tych tematów, tych rozmów. Ostatecznie nie wiem, czy Andrzejek miał rację i czy takie podróże są możliwe. Jeśli kiedykolwiek spotkacie takiego podróżnika, dajcie mi znać. Proszę, to by oznaczało, że jednak nie jestem szalony, ale jeśli taki ktoś wróci w czasie i pojawi się akurat dziś, to by znaczyło, że ludzie są zagrożeni. Jesteśmy zagrożeni. Andrzej jest zagrożony. Andrzej nie jest zagrożony. Andrzej jest zagrożony. Ale jeśli ktoś wróci w czasie i pojawi się akurat dziś, to by znaczyło, że ludzie są zagrożeni. Jesteśmy zagrożeni. Andrzej jest zagrożony. Andrzej nie jest zagrożony. To Andrzej jest zagrożeniem. To Andrzej jest zagrożeniem. Andrzej nie jest zagrożony. Andrzej jest zagrożeniem."
    },
    {
        "Monika",
        "Ale wy tak na serio pytacie? Bo nie znać Andrzeja Maja w naszych kręgach to naprawdę byłoby dziwne. Tak, znam go. Podobnie jak pewnie kilka tysięcy innych uczonych go zna. Andrzej pracował z sieciami neuronowymi. To prawda. Był wykładowcą w Krakowie. To także prawda. Z tego co wiem, jeszcze przynajmniej pół roku temu tam pracował. Wydział czy tam Instytut Informatyki i Matematyki Komputerowej czy jakoś tak. Nie pamiętam jak się to dokładnie teraz nazywa, ale w każdym razie gość pracował z komputerami i sieciami neuronowymi. No chyba jesteście w stanie skojarzyć fakty. Nie? Komputery, sieci neuronowe? To się łączy. Bezpośrednio z nim nie miałam kontaktu. Może raz na jakimś sympozjum naukowym pogratulowałam mu świetnego wykładu, ale to wszystko co nas łączyło. Jeden uścisk dłoni, nigdy nie weszliśmy do wspólnego projektu, nigdy nie korespondowałam z nim. Tak naprawdę znam go jako celebrytę ze świata nauki, ale to wszystko co mogę wam powiedzieć."
    },
    {
        "Agnieszka",
        "Może go znałam, a może nie. Kto wie? Zacznijmy od tego, że nie macie prawa mnie tutaj przetrzymywać. Absolutnie nic złego nie zrobiłam. Trzymacie mnie tutaj niezgodnie z prawem. Wiem, że teraz wszystko się zmienia na świecie i roboty dyktują, jak ma być, ale o ile się nie mylę, dawne prawo nadal obowiązuje. Mamy tutaj jakąś konstytucję, prawda? Chcę rozmawiać z adwokatem. Maja znałam, to prawda. Było to kilka lat temu. Pracowaliśmy razem w Warszawie, ale na tym nasza znajomość się skończyła. Byliśmy w tej samej pracy. Czy to jest jakieś przestępstwo? To jest coś niedozwolonego w naszym kraju? Za to można wsadzać ludzi do więzienia? On wjechał z Warszawy, nie ma go tam. Z tego, co wiem, pojechał do Krakowa. Wykładać tam chciał chyba coś z informatyki czy matematyki. Nie wiem, jak to się skończyło. Może to były tylko plany?"
    },
    {
        "Adam",
        "Andrzej Maj? No coś kojarzę. Był taki gość. Pamiętam. Pracował u nas w biurze. Był project managerem. Chociaż, moment, może to jednak był Arkadiusz Maj? Też na literę A. Mógłbym się pomylić. No jednak tak, Arkadiusz. Z Arkadiuszem współpracowałem w Wałbrzychu. Pamiętam, że był naprawdę wrednym facetem. Normalnie nie chciałbyś z takim pracować. Jak coś było do zrobienia, to albo stosował typową spychologię, albo zamiatał sprawę pod dywan. Nigdy człowieka nie docenił. Wszystkie zasługi brał na siebie. Był naprawdę beznadziejny. Arkadiusza pamiętam jak dziś, więc jeśli chcecie go aresztować, to jak najbardziej. Jestem za. Takich ludzi powinno się zamykać, a nie mnie, bo ja jestem niewinny. Jak chcecie, to ja wam mogę adres nawet podać. Stefana Batorego, 68D. Tylko D jak Danuta, bo pod B mieszka jego ciocia, a ona była fajna. Jak będziecie Arkadiusza aresztować, to proszę powiedzcie mu z pozdrowieniami od Adama. A on będzie wiedział, o kogo chodzi."
    },
};

// var transcriptions = await ExtractTranscriptionsFromAudioAsync();

var prompt =
    "Na podstawie tekstu wydobądź informację na jakiej ulicy znajduje się uczelnia, na której wykłada Andrzej Maj. Przeprowadź pełną analizę i odpowiedz na pytanie.";

foreach (var (name, transcription) in transcriptions)
{
    prompt += $"\n\n{name}:\n{transcription}";
}

Console.WriteLine($"Prompt: {prompt}");
Console.WriteLine();

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var chatResult = await chatClient.CompleteChatAsync(
    ChatMessage.CreateUserMessage(prompt));

Console.WriteLine($"Chat result: {chatResult.Value.Content[0].Text}");

var key = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!);
using var httpClient = new HttpClient();

var apiResponse = await httpClient.PostAsJsonAsync(
    "https://centrala.ag3nts.org/report",
    new { task = "mp3", apikey = key, answer = chatResult.Value.Content[0].Text });

Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");

return;

async Task<Dictionary<string, string>> ExtractTranscriptionsFromAudioAsync()
{
    Dictionary<string, string> result = new();

    var audioClient = new AudioClient("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

    foreach (var filePath in Directory.EnumerateFiles("interrogations"))
    {
        Console.WriteLine($"Transcribing {filePath}");

        await using var stream = File.OpenRead(filePath);
        var clientResultAudioTranscription = await audioClient.TranscribeAudioAsync(stream, filePath);
        Console.WriteLine($"Transcription: {clientResultAudioTranscription.Value.Text}");

        var name = Path.GetFileNameWithoutExtension(filePath);
        result.Add(name, clientResultAudioTranscription.Value.Text);

        Console.WriteLine();
    }

    return result;
}