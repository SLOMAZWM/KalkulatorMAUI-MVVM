using System;

namespace KalkulatorMAUI_MVVM.Models
{
    public class ExchangeRatePairResponse
    {
        public string Result { get; set; }
        public string Documentation { get; set; }
        public string TermsOfUse { get; set; }
        public double ConversionRate { get; set; }
        public long TimeLastUpdateUnix { get; set; }
        public string TimeLastUpdateUtc { get; set; }
        public long TimeNextUpdateUnix { get; set; }
        public string TimeNextUpdateUtc { get; set; }
        public string BaseCode { get; set; }
        public string TargetCode { get; set; }
    }
}
