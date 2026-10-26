using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revmax_Interface_Promun
{
    public class InvoiceData
    {
        public string InvoiceBaseCurrency { get; set; }
        public string BranchName { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerVATNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTelephoneNumber { get; set; }
        public string CustomerTIN { get; set; }
        public string CustomerEmail { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceTaxAmount { get; set; }
        public string InvoiceFlag { get; set; }
        public string Cashier { get; set; }
        public string InvoiceComment { get; set; }
        public string ItemsXML { get; set; }
        public string CurrenciesXML { get; set; }
        public string VerificationLink { get; set; }
        public string CustomerNum { get; internal set; }
        public string Discount { get; internal set; }
        public string DeviceId { get; internal set; }
        public string OriginalInvoiceNumber { get; internal set; }
        public string ReceiptGlobalNumber { get; internal set; }
        public string ReasonForCreditNote { get; internal set; }
        public string InvoiceStatus { get; internal set; }
        public string Tendered { get; internal set; }
        public string Change { get; internal set; }
        public string Rate { get; internal set; }
        public string Equivalence { get; internal set; }
        public string BalanceBefore { get; internal set; }
        public string BalanceAfter { get; internal set; }
        public string Account { get; internal set; }



    }
}
