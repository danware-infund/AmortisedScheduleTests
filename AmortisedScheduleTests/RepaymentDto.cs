namespace AmortisedScheduleTests
{
    using System;

    public class RepaymentDto
    {
        private DateTime date;

        /// <summary>Gets or sets a value for the date.</summary>
        public DateTime Date
        {
            get => this.date.Date;
            set => this.date = value;
        }

        /// <summary>Gets or sets a value for the sequence number.</summary>
        public int SequenceNumber { get; set; }

        /// <summary>Gets or sets a value for the principal amount.</summary>
        public decimal PrincipalAmount { get; set; }

        /// <summary>Gets or sets a value for the interest amount.</summary>
        public decimal InterestAmount { get; set; }

        /// <summary>Gets or sets a value for the payment type ID.</summary>
        public int PaymentTypeId { get; set; }

        /// <summary>Gets or sets a value for the loan schedule repayment status ID.</summary>
        public int LoanScheduleRepaymentStatusId { get; set; }

        /// <summary>Gets or sets a value indicating whether a repayment is expected.</summary>
        public bool IsExpected { get; set; } = true;

        /// <summary>
        /// Gets or sets a value identifying the party responsible for paying the item.
        /// </summary>
        public string Payer { get; set; }

        public bool Processed { get; set; } = false;

        public int MonthNumber { get; set; }
    }
}
