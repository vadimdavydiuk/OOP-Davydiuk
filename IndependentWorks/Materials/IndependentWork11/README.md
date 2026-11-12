# Самостійна робота №11: Інвертований індекс та продуктивність

Умови — у PDF поруч із проєктом. Для цієї роботи фіксованого варіанта за номером студента немає. Назву проєкту взято з методички; у №9 назва не задана, обрано IndependentWork9.

## Запуск

Потрібен .NET SDK 10. З папки `IndependentWorks`:

```powershell
dotnet run --project IndependentWork11/InvertedIndexDemo --configuration Release
```

Для автоматичного прогону без інтерактивного пошуку додайте `-- --demo`.

Детальний опис, висновки та відповіді на контрольні запитання — у [REPORT.md](REPORT.md).

## Фактичний вивід

Час і випадкові затримки можуть відрізнятися на іншому запуску.

```text
Самостійна №11: Інвертований індекс
Документів: 5; унікальних слів: 37
Без стоп-слів: c#, modern, object, oriented, programming, language, developed, by, microsoft
AND programming language: [1] Introduction to C#, [2] Java Basics, [3] Python Overview
AND c# async: [4] C# Advanced Features
OR python async: [3] Python Overview, [4] C# Advanced Features, [5] Comparing Languages
Вимірювання: медіана 5 серій по 20 запитів, після прогріву; побудова окремо.
N=10: побудова=0,071 мс; linear=0,0470 мс; index=0,0013 мс; знайдено=6; окупність≈2 запитів
N=100: побудова=0,619 мс; linear=0,2856 мс; index=0,0031 мс; знайдено=60; окупність≈3 запитів
N=1000: побудова=4,393 мс; linear=3,4148 мс; index=0,0532 мс; знайдено=600; окупність≈2 запитів
N=10000: побудова=68,183 мс; linear=19,8236 мс; index=0,1895 мс; знайдено=6000; окупність≈4 запитів
```
