using Newtonsoft.Json;
using System.Collections.Generic;

namespace Revmax_Interface_Promun.Services
{
    public class ActivatedTerminalResponse
    {
        [JsonProperty("activatedTerminal")]
        public ActivatedTerminal ActivatedTerminal { get; set; }

        [JsonProperty("configuration")]
        public MRAConfiguration Configuration { get; set; }
    }

    public class ActivatedTerminal
    {
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; }

        [JsonProperty("terminalCredentials")]
        public TerminalCredentials TerminalCredentials { get; set; }
    }

    public class TerminalCredentials
    {
        [JsonProperty("jwtToken")]
        public string JwtToken { get; set; }

        [JsonProperty("secretKey")]
        public string SecretKey { get; set; }
    }

    public class TerminalCredentialsResponse
    {
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; }

        [JsonProperty("jwtToken")]
        public string JwtToken { get; set; }

        [JsonProperty("secretKey")]
        public string SecretKey { get; set; }
    }

    public class MRAConfiguration
    {
        [JsonProperty("globalConfiguration")]
        public GlobalConfiguration GlobalConfiguration { get; set; }

        [JsonProperty("terminalConfiguration")]
        public TerminalConfiguration TerminalConfiguration { get; set; }

        [JsonProperty("taxpayerConfiguration")]
        public TaxpayerConfiguration TaxpayerConfiguration { get; set; }
    }

    public class GlobalConfiguration
    {
        [JsonProperty("taxrates")]
        public List<TaxRate> TaxRates { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class TaxRate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }
    }

    public class TerminalConfiguration
    {
        [JsonProperty("terminalLabel")]
        public string TerminalLabel { get; set; }

        [JsonProperty("isActiveTerminal")]
        public bool IsActiveTerminal { get; set; }
    }

    public class TaxpayerConfiguration
    {
        [JsonProperty("tin")]
        public string Tin { get; set; }

        [JsonProperty("isVATRegistered")]
        public bool IsVATRegistered { get; set; }
    }

    public class InitializeResponse
    {
        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("configuration")]
        public MRAConfiguration Configuration { get; set; }

        [JsonProperty("taxpayerDetails")]
        public TaxpayerConfiguration TaxpayerDetails { get; set; }
    }

    public class TerminalSiteProduct
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("quantity")]
        public decimal? Quantity { get; set; }

        [JsonProperty("unitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("siteId")]
        public string SiteId { get; set; }

        [JsonProperty("minimumStockLevel")]
        public decimal? MinimumStockLevel { get; set; }

        [JsonProperty("taxRateId")]
        public string TaxRateId { get; set; }

        [JsonProperty("isProduct")]
        public bool? IsProduct { get; set; }
    }
}
