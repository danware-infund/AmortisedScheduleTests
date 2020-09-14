namespace AmortisedScheduleTests
{
    using System;
    using System.Collections.Generic;
    using Common.Extensions;
    using Microsoft.VisualBasic;

    public class AmortisedScheduleGenerator
    {
        private const int MonthsPerYear = 12;
        private readonly DateTime disbursalDate;
        private readonly List<DateTime> bankHolidays;

        public AmortisedScheduleGenerator(decimal principal, double apyInterestRate, int numberOfPayments, int amortisationInYears, DateTime disbursalDate, List<DateTime> bankHolidays)
        {
            this.Principal = principal;
            this.ApyInterestRate = apyInterestRate;
            this.NumberOfPayments = numberOfPayments;
            this.AmortisationInYears = amortisationInYears;

            this.disbursalDate = disbursalDate;
            this.bankHolidays = bankHolidays;

            this.GenerateScheduleItems();
        }

        public decimal Principal { get; }
        public double ApyInterestRate { get; }
        public int NumberOfPayments { get; }
        public int AmortisationInYears { get; }
        public List<ScheduleItem> MonthlyScheduleItems { get; set; } = new List<ScheduleItem>();

        public double MonthlyApr() => Math.Pow(1 + this.ApyInterestRate, 1.0 / MonthsPerYear) - 1;

        public decimal BalloonPercentage() => 1 - (decimal)this.NumberOfPayments / MonthsPerYear / this.AmortisationInYears;

        public decimal BalloonAmount() => this.BalloonPercentage() * this.Principal;

        public double Payment() => Math.Abs(Financial.Pmt(this.MonthlyApr(), this.NumberOfPayments, (double)this.Principal, (double)-this.BalloonAmount()));

        private void GenerateScheduleItems()
        {
            var balance = this.Principal;
            for (var i = 0; i < this.NumberOfPayments; i++)
            {
                var interest = (decimal)this.MonthlyApr() * balance;
                this.MonthlyScheduleItems.Add(new ScheduleItem(interest, (decimal)this.Payment() - interest, this.disbursalDate.AddMonths(i + 1).FirstFutureWorkingDay(this.bankHolidays)));

                balance -= this.MonthlyScheduleItems[i].Principal;
            }

            this.MonthlyScheduleItems[^1].Principal += balance;
        }
    }
}
