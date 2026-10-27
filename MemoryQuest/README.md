# MemoryQuest

MemoryQuest on .NET MAUI rakendus, mis on ehitatud mänguks, kus kasutaja valib profiili, mängib mälumängu ja salvestab oma statistikat kohalikku SQLite andmebaasi.

## Kuidas rakendus üles ehitati

Projekt on loodud .NET MAUI abil, kasutades:
- MAUI app shelli navigatsiooniks
- XAML-i kasutajaliidese loomiseks
- C#-ga loogika ja andmetöötlust
- SQLite andmebaasi kohaliku andmete salvestamiseks
- resx lokaliseerimise faile keelte toetamiseks

### Peamised osad

- `MemoryQuest/AppShell.xaml` – põhine navigatsioonirakenduse shell
- `MemoryQuest/Views/HomePage.xaml` ja `.xaml.cs` – avaekraan
- `MemoryQuest/Views/ProfileSelectionPage.xaml` ja `.xaml.cs` – profiili valik, loomine ja kustutamine
- `MemoryQuest/Views/MainPage.xaml` ja `.xaml.cs` – mängupõhimõte, skoor ja statistika
- `MemoryQuest/Views/SettingsPage.xaml` ja `.xaml.cs` – keeleseadete leht
- `MemoryQuest/Services/SqlProfileService.cs` – andmebaasi teenus profiilide ja statistika jaoks
- `MemoryQuest/Services/AppSettingsService.cs` – keele- ja rakenduse seaded
- `MemoryQuest/Localization/AppResources.resx` ja `AppResources.et.resx` – lokaliseeritud tekstid

## Kuidas rakendus töötab

### 1. Avaekraan
Kui rakendus käivitub, luuakse `AppShell` ja avatakse HomePage.
Kasutaja saab:
- alustada mängu
- minna seadetesse

### 2. Profiili valik
Kui kasutaja klõpsab Play, avatakse profiilide valimise ekraan.
Seal saab:
- luua uut profiili
- valida olemasolevat profiili
- kustutada profiili
- jätkata mängu

Profiilide andmed säilitatakse SQLite andmebaasis.

### 3. Mänguava
Kui profiil on valitud, luuakse `MainPage` ja mäng algab.
Mängu ajal:
- arvutatakse skoor
- loendatakse käike
- võetakse arvesse paaride arvu
- pärast mängu lõppu salvestatakse statistika aktiivsele profiilile andmebaasi

### 4. Seaded
Settings lehel saab valida keelt:
- Eesti
- English

Keel on salvestatud `Preferences`-sse ja rakendatakse rakenduse kultuurile kohe.

## Andmebaasi info

Rakendus kasutab SQLite-d, mitte SQL Serverit.

Andmebaas asub selles asukohas:
- `FileSystem.AppDataDirectory/MemoryQuest.db`

See tähendab, et andmed jäävad kasutaja seadmesse ja pole sõltuvad arvutist või välisest serverist.

## Lokaliseerimine

Rakenduse tekstid on eraldatud resx-failidesse:
- `MemoryQuest/Localization/AppResources.resx` (inglise keel)
- `MemoryQuest/Localization/AppResources.et.resx` (eesti keel)

Kui kasutaja keelt vahetab, uuendatakse kõikide lehtede tekstid vastavalt valitud kultuurile.

## Kuidas käivitada

1. Ava projekt lahendus.
2. Veendu, et .NET MAUI tööriistad on installitud.
3. Käivita projekt Visual Studio kaudu.
4. Valige soovitud sihtplatform (Windows, Android jne).
5. Rakendus käivitub ja loob vajadusel SQLite andmebaasi automaatselt.

## Tehnoloogiad

- .NET 10
- .NET MAUI
- C#
- XAML
- SQLite
- Resource files (`.resx`) for localization

## Koko kokkuvõte

See rakendus on lihtne mälumängumäng, mille eesmärk on luua kasutaja profiile, mängida mälumängu ja hoida statistikat lokaalsetes andmetes. Rakendus ühendab endas MAUI UI, SQLite andmetöötluse ja keeletoetuse ühes projektis.

{
