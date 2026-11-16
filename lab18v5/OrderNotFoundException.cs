using System;
using System.Runtime.Serialization;

namespace lab18v5
{
    [Serializable]
    public class OrderNotFoundException : Exception
    {
        public int OrderId { get; }

        public OrderNotFoundException()
            : base("Замовлення не знайдено.") { }

        public OrderNotFoundException(int orderId)
            : base($"Замовлення з ID {orderId} не знайдено в системі.")
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(int orderId, string message)
            : base(message)
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(int orderId, string message, Exception innerException)
            : base(message, innerException)
        {
            OrderId = orderId;
        }

        // Конструктор для серіалізації (вимога критеріїв оцінювання на 5 балів)
        protected OrderNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OrderId = info.GetInt32(nameof(OrderId));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(OrderId), OrderId);
        }
    }
}