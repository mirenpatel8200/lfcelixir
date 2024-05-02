using System;

namespace Elixir.Models
{
    public class SettingsEntry
    {
        public int? SettingsId { get; set; }
        public string PairName { get; set; }
        public string PairValue { get; set; }
        public string SettingsDescription { get; set; }
        public DateTime? PayPalTokenExpirationDT { get; set; }
    }
}
