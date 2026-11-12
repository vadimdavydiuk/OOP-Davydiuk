# Звіт з аналізу інкапсуляції в Open-Source проєкті

## 1. Обраний проєкт
- **Назва:** .NET Runtime (Repository for the .NET runtime and libraries)
- **Посилання на GitHub:** [github.com/dotnet/runtime](https://github.com/dotnet/runtime)

---

## 2. Аналіз інкапсуляції

### Клас: `List<T>`
- **Посилання на файл:** [`src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs)
- **Опис класу:** Динамічний масив, що надає методи для впорядкованого списку об'єктів з доступом за індексом.
- **Поля:** 
  Клас строго інкапсулює свій внутрішній стан. Основне сховище даних `_items` та лічильник розміру `_size` є приватними, що унеможливлює зовнішнє пошкодження буфера.
  ```csharp
  private T[] _items;
  private int _size;
  private int _version;
  [NonSerialized]
  private object? _syncRoot;
  ```
  ![alt text](image.png)