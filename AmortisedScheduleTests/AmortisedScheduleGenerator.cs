namespace AmortisedScheduleTests
{
    using System;
    using System.Collections.Generic;
    using Common.Extensions;

    public class AmortisedScheduleGenerator
    {
        private const int MonthsPerYear = 12;
        private readonly DateTime disbursalDate;
        private readonly List<DateTime> bankHolidays;

        public AmortisedScheduleGenerator(decimal principal, double annualInterestRate, int numberOfPayments, int amortisationInYears, DateTime disbursalDate, List<DateTime> bankHolidays)
        {
            this.Principal = principal;
            this.AnnualInterestRate = annualInterestRate;
            this.NumberOfPayments = numberOfPayments;
            this.AmortisationInYears = amortisationInYears;

            this.disbursalDate = disbursalDate;
            this.bankHolidays = bankHolidays;

            this.GenerateScheduleItems();
        }

        public decimal Principal { get; }
        public double AnnualInterestRate { get; }
        public int NumberOfPayments { get; }
        public int AmortisationInYears { get; }
        public List<RepaymentDto> MonthlyScheduleItems { get; set; } = new List<RepaymentDto>();

        public double MonthlyApr() => Math.Pow(1 + this.AnnualInterestRate, 1.0 / MonthsPerYear) - 1;

        public decimal BalloonPercentage() => 1 - (decimal)this.NumberOfPayments / MonthsPerYear / this.AmortisationInYears;

        public decimal BalloonAmount() => this.BalloonPercentage() * this.Principal;

        public double Payment() => Math.Abs(((double)this.BalloonAmount() - (double)this.Principal * Math.Pow(this.MonthlyApr() + 1.0, this.NumberOfPayments)) / (Math.Pow(this.MonthlyApr() + 1.0, this.NumberOfPayments) - 1) * this.MonthlyApr());

        private void GenerateScheduleItems()
        {
            var balance = this.Principal;
            for (var i = 0; i < this.NumberOfPayments; i++)
            {
                var interest = (decimal)this.MonthlyApr() * balance;
                this.MonthlyScheduleItems.Add(CreateRepayment(this.disbursalDate.AddMonths(i + 1).FirstFutureWorkingDay(this.bankHolidays), (decimal)this.Payment() - interest, interest, i + 1));

                balance -= this.MonthlyScheduleItems[i].PrincipalAmount;
            }

            this.MonthlyScheduleItems[^1].PrincipalAmount += balance;
        }

        private static RepaymentDto CreateRepayment(DateTime paymentDate, decimal principal, decimal interest, int sequence)
        {
            return new RepaymentDto
            {
                Date = paymentDate,
                InterestAmount = interest,
                IsExpected = true,
                LoanScheduleRepaymentStatusId = 1, //LoanScheduleRepaymentStatusId.Created
                MonthNumber = sequence,
                Payer = "Borrower",
                PaymentTypeId = 1, //PaymentTypeId.DirectDebit
                PrincipalAmount = principal,
                Processed = false,
                SequenceNumber = sequence,
            };
        }
    }
}
