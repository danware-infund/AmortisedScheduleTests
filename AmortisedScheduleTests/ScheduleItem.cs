namespace AmortisedScheduleTests
{
    using System;

    public class ScheduleItem
    {
        public ScheduleItem(decimal interest, decimal principal, DateTime paymentDate)
        {
            this.Interest = interest;
            this.Principal = principal;
            this.PaymentDate = paymentDate;
        }

        public decimal Interest { get; set; }
        public decimal Principal { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
