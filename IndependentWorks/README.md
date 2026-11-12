# IndependentWorks — самостійні роботи з ООП

Роботи №1–3 та №8–15 розміщені в папках IndependentWorkN. У кожній роботі №8–15: Program.cs з усіма класами, IndependentWorkN.csproj, readme.md із поясненнями та image.png із виводом програми. Папки bin і obj створюються під час збірки.

Умови PDF, початкові звіти, журнали й CSV збережено окремо в Materials/IndependentWorkN. Тести кешу винесені в IndependentWork8.Tests, щоб не входити до консольної програми.

## Запуск

Потрібен .NET SDK 10. У папці потрібної роботи виконайте `dotnet run --configuration Release`. Для №11 доступний інтерактивний пошук; параметр `-- --demo` запускає демонстрацію без очікування вводу.

З папки IndependentWorks:

```powershell
dotnet build IndependentWorks.sln --configuration Release
dotnet test IndependentWork8.Tests --configuration Release
./verify.ps1
```

Рішення та verify.ps1 перевіряють роботи №8–15. Проєкти №1–3 збережені без змін. Для першого відновлення потрібні NuGet-пакети xUnit і Polly.Core 8.9.1. Мережеві сценарії програм імітовані.
