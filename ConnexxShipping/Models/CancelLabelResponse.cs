// ReSharper disable once CheckNamespace
namespace ConnexxShipping
{
    /// <summary>
    /// The main result that comes back when canceling labels.
    /// We only store "data" from the response, which is an array of these results.
    /// </summary>
    public class CancelLabelResponse
    {
        public string Barcode { get; set; }
        public string CarrierShipmentID { get; set; }
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
