using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ConnexxShipping
{
    public class CreateLabelResponse
    {
        public RequestInfo Request { get; set; }
        public List<CommercialInvoicePdfUrls> CommercialInvoicePdfUrls { get; set; }
        public string CombinedPdfUrl { get; set; }
        public List<ShipmentResult> Shipments { get; set; }
        public int TotalLabelsRequested { get; set; }
        public int TotalLabelsErrored { get; set; }
        public string Status { get; set; }
        public List<string> Errors { get; set; }
    }

    public class RequestInfo
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CommercialInvoicePdfUrls
    {
        public string ShipmentID { get; set; }
        public string CommercialInvoicePdfUrl { get; set; }
        public string CommercialInvoiceBase64 { get; set; }
    }

    public class ShipmentResult
    {
        public string Id { get; set; }
        public string CarrierName { get; set; }
        public string ServiceName { get; set; }
        public string CustomerReference { get; set; }
        public string LabelBase64Eltron { get; set; }
        public string LabelBase64Citizen { get; set; }
        public string ShipmentLabelBase64Pdf { get; set; }
        public string ShipmentLabelUrl { get; set; }
        public string ShipmentTrackingCode { get; set; }
        public List<PackageResult> Packages { get; set; }
    }

    public class PackageResult
    {
        public int Index { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public string TrackingCode { get; set; }
        public string TrackingUrl { get; set; }
        public string LabelUrl { get; set; }
        public string LabelBase64Pdf { get; set; }
        public string LabelBase64Zpl { get; set; }
        public string Labelbase64Png { get; set; }
    }
}
