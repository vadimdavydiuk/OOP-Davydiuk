using System;
using System.Collections.Generic;

namespace lab14v5
{
    // Клас для представлення повідомлення
    public class Message
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public string Text { get; set; }

        public Message(int id, string recipient, string text)
        {
            Id = id;
            Recipient = recipient;
            Text = text;
        }

        public override string ToString() => $"[ID: {Id}] Одержувач: {Recipient} | Текст: \"{Text}\"";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №14 (Варіант 5: Система повідомлень) ===");
            Console.WriteLine("Тема: HashSet / Queue / Stack: практичні сценарії\n");

            // ==========================================
            // 1. HASHSET: Унікальні одержувачі
            // ==========================================
            Console.WriteLine("--- 1. Використання HashSet<T> (Унікальні одержувачі) ---");
            HashSet<string> uniqueRecipients = new HashSet<string>();

            // Додаємо одержувачів (включно з дублікатами)
            string[] incomingRecipients = { "ivan@gmail.com", "maria@ukr.net", "ivan@gmail.com", "petro@gmail.com", "maria@ukr.net" };
            
            Console.WriteLine("Додаємо одержувачів до системи (з повторами):");
            foreach (var email in incomingRecipients)
            {
                bool added = uniqueRecipients.Add(email);
                Console.WriteLine($" - Спроба додати '{email}': {(added ? "успішно додано" : "дублікат, проігноровано")}");
            }

            Console.WriteLine($"\nКількість унікальних одержувачів у HashSet: {uniqueRecipients.Count}");
            Console.WriteLine("Список унікальних одержувачів:");
            foreach (var recipient in uniqueRecipients)
            {
                Console.WriteLine($"   * {recipient}");
            }

            // Перевірка наявності елемента за O(1)
            string searchEmail = "petro@gmail.com";
            Console.WriteLine($"Чи є одержувач '{searchEmail}' у базі? {uniqueRecipients.Contains(searchEmail)}");
            Console.WriteLine();


            // ==========================================
            // 2. QUEUE: Черга повідомлень для відправки (FIFO)
            // ==========================================
            Console.WriteLine("--- 2. Використання Queue<T> (Черга повідомлень - FIFO) ---");
            Queue<Message> messageQueue = new Queue<Message>();

            // Додаємо повідомлення в чергу (Enqueue)
            messageQueue.Enqueue(new Message(1, "ivan@gmail.com", "Привіт, Іване! Це тестове повідомлення 1."));
            messageQueue.Enqueue(new Message(2, "maria@ukr.net", "Добрий день, Маріє! Звіт готовий."));
            messageQueue.Enqueue(new Message(3, "petro@gmail.com", "Петре, нагадуємо про зустріч о 15:00."));

            Console.WriteLine($"Кількість повідомлень у черзі на відправку: {messageQueue.Count}");

            // Використання Peek для перегляду першого в черзі без видалення
            if (messageQueue.Count > 0)
            {
                Console.WriteLine($"[Peek] Наступне повідомлення до відправки: {messageQueue.Peek()}");
            }

            // Обробка черги (дисципліна FIFO: First In, First Out)
            Console.WriteLine("\nРозпочинаємо розсилку повідомлень (процес Dequeue):");
            Stack<Message> sentHistory = new Stack<Message>();

            while (messageQueue.Count > 0)
            {
                Message currentMessage = messageQueue.Dequeue();
                Console.WriteLine($" -> Надіслано: {currentMessage}");
                
                // Зберігаємо надіслане повідомлення в історію (Stack)
                sentHistory.Push(currentMessage);
            }

            Console.WriteLine($"Повідомлень у черзі після відправки: {messageQueue.Count}\n");


            // ==========================================
            // 3. STACK: Історія надісланих повідомлень (LIFO)
            // ==========================================
            Console.WriteLine("--- 3. Використання Stack<T> (Історія надісланих - LIFO / Undo) ---");
            Console.WriteLine($"Кількість записів в історії відправлених: {sentHistory.Count}");

            // Перегляд останнього надісланого повідомлення без видалення (Peek)
            if (sentHistory.Count > 0)
            {
                Console.WriteLine($"[Peek] Останнє надіслане повідомлення (на верхівці стеку): {sentHistory.Peek()}");
            }

            // Демонстрація операції скасування (Undo / Pop) останньої відправки
            Console.WriteLine("\nІмітація операції 'Відкликати останнє надіслане повідомлення' (Undo через Pop):");
            if (sentHistory.Count > 0)
            {
                Message recalledMessage = sentHistory.Pop();
                Console.WriteLine($" <!> Відкликано повідомлення: {recalledMessage}");
            }

            Console.WriteLine("\nПоточна історія надісланих повідомлень (від найновіших до найдавніших):");
            foreach (var msg in sentHistory)
            {
                Console.WriteLine($"   * {msg}");
            }
        }
    }
}