using System;

namespace Lab7V5
{
    // Базовий клас Document
    public class Document
    {
        public string Title { get; set; }

        public Document(string title)
        {
            Title = title;
        }

        // Віртуальний метод базового класу
        public virtual void Open()
        {
            Console.WriteLine($"[Document] Відкриття документа: \"{Title}\"");
        }
    }

    // Похідний клас PDF (перевизначає метод за допомогою override)
    public class PDF : Document
    {
        public int PageCount { get; set; }

        public PDF(string title, int pageCount) : base(title)
        {
            PageCount = pageCount;
        }

        // Перевизначення методу (Поліморфізм)
        public override void Open()
        {
            Console.WriteLine($"[PDF] Відкриття PDF-документа: \"{Title}\" ({PageCount} стор.) у PDF Reader.");
        }
    }

    // Похідний клас Text (приховує метод за допомогою new)
    public class Text : Document
    {
        public string Encoding { get; set; }

        public Text(string title, string encoding) : base(title)
        {
            Encoding = encoding;
        }

        // Приховування методу базового класу (Без поліморфізму)
        public new void Open()
        {
            Console.WriteLine($"[Text] Відкриття текстового документа: \"{Title}\" (Кодування: {Encoding}) у Блокноті.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення об'єктів
            PDF pdfDoc = new PDF("Інструкція.pdf", 24);
            Text textDoc = new Text("Замітки.txt", "UTF-8");

            // Приведення до базового типу (Upcasting)
            Document docRef1 = pdfDoc;
            Document docRef2 = textDoc;

            Console.WriteLine("--- Виклики через посилання базового типу (Document) ---");
            // override: викликається перевизначений метод похідного класу PDF (динамічне зв'язування)
            docRef1.Open(); 

            // new: викликається метод базового класу Document, бо поліморфізм відсутній (статичне зв'язування)
            docRef2.Open(); 

            Console.WriteLine("\n--- Виклики через посилання похідних типів (з явним приведенням) ---");
            // Явне приведення та виклик методів відповідних типів
            ((PDF)docRef1).Open();
            ((Text)docRef2).Open();
        }
    }
}