using System;

namespace Producer.API.DTOs
{
    public class Transaction
    {
        public int UserId { get; set; }

        public decimal Value { get; set; }
    }
}
