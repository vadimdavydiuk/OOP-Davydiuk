# Самостійна робота №8: Узагальнений FIFO-кеш із тестами

Умови — у PDF поруч із проєктом. Для цієї роботи фіксованого варіанта за номером студента немає. Назву проєкту взято з методички; у №9 назва не задана, обрано IndependentWork9.

## Запуск

Потрібен .NET SDK 10. З папки `IndependentWorks`:

```powershell
dotnet run --project IndependentWork8/IndependentWork7 --configuration Release
```

Тести: `dotnet test IndependentWork8/IndependentWork7.Tests` (з папки IndependentWorks).



Детальний опис, висновки та відповіді на контрольні запитання — у [REPORT.md](REPORT.md).

## Фактичний вивід

Час і випадкові затримки можуть відрізнятися на іншому запуску.

```text
Самостійна №8: FIFO-кеш
A = Перший; Count = 2
Після додавання C: A = False, B = True, C = True
Ключ 'A' відсутній у кеші.
```
