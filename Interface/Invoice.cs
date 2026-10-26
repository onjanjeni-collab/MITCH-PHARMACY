using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revmax_Interface_Promun
{
    public class Invoice
    {
        internal string CustomerNumber;

        public string Currency { get; set; }
        public string BranchName { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerVATNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTelephoneNumber { get; set; }
        public string CustomerBPN { get; set; }
        public string CustomerTIN { get; set; }
        public string CustomerEmail { get; set; }
        public string OriginalInvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceTaxAmount { get; set; }
        public string InvoiceFlag { get; set; }
        public string Cashier { get; set; }
        public string VerificationCode { get; set; }
        public string QRCode { get; set; }
        public string InvoiceComment { get; set; }
        public string ItemsXML { get; set; }
        public string CurrenciesXML { get; set; }
        public string VerificationLink { get; set; }
        public string CustomerNum { get; internal set; }
        public string Discount { get; internal set; }
        public string DeviceId { get; internal set; }
        public string ReceiptGlobalNumber { get; internal set; }
        public string ReasonForCreditNote { get; internal set; }
        public List<MraReceiptLine> ReceiptLines { get; set; }
        public string LevyBreakDownJson { get; set; }
    }

    public class MraReceiptLine
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal TotalVAT { get; set; }
        public string TaxRateId { get; set; }
        public bool IsProduct { get; set; }
    }
}
