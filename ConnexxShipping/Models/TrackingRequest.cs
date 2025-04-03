using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ConnexxShipping
{
    /// <summary>
    /// Body for the POST /tracking call.
    /// </summary>
    public class TrackingRequest
    {
        public List<string> TrackingBarCodes { get; set; }
    }

    /// <summary>
    /// Wrapper for the final tracking response we return to the caller.
    /// </summary>
    public class TrackingResponse
    {
        public List<TrackingBarcodeData> ValidBarcodes { get; set; }
        public List<string> InvalidBarcodes { get; set; }
    }

    /// <summary>
    /// Actual data from "data" in the response wrapper.
    /// We store "invalidBarcodes" and "validBarcodes" with events.
    /// </summary>
    public class TrackingData
    {
        public List<string> InvalidBarcodes { get; set; }
        public List<TrackingBarcodeData> ValidBarcodes { get; set; }
    }

    public class TrackingBarcodeData
    {
        public string Barcode { get; set; }
        public List<TrackingEvent> Events { get; set; } = new List<TrackingEvent>();
    }

    public class TrackingEvent
    {
        public string StatusTime { get; set; }
        public string StatusCode { get; set; }
        public string TrackingEventDescription { get; set; }
    }
}
