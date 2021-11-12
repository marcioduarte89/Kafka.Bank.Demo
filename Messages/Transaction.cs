using System;

namespace Messages
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public decimal Value { get; set; }
    }
}
