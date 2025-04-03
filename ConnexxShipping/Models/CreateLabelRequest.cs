using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ConnexxShipping
{
    /// <summary>
    /// Represents the full JSON body sent when creating labels.
    /// Adjust this to match your shipping needs exactly.
    /// </summary>
    public class CreateLabelRequest
    {
        public string LabelSize { get; set; } = "4x6";
        public bool DoNotUseWebhook { get; set; } = true;
        public List<Shipment> Shipments { get; set; } = new List<Shipment>();
    }

    public class Shipment
    {
        public string ServiceId { get; set; }
        public EvriService EvriService { get; set; }
        public string WarehouseName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerReference { get; set; }
        public string ReasonForExport { get; set; }
        public string FromAddressFirstName { get; set; }
        public string FromAddressLastName { get; set; }
        public string FromAddressCompany { get; set; }
        public string FromAddressPhone { get; set; }
        public string FromAddressEmail { get; set; }
        public string FromAddressStreet1 { get; set; }
        public string FromAddressStreet2 { get; set; }
        public string FromAddressCity { get; set; }
        public string FromAddressCountyState { get; set; }
        public string FromAddressZip { get; set; }
        public string FromAddressCountryIso { get; set; }
        public string FromAddressEoriNumber { get; set; }
        public string FromAddressVatNumber { get; set; }
        public string ToAddressFirstName { get; set; }
        public string ToAddressLastName { get; set; }
        public string ToAddressCompany { get; set; }
        public string ToAddressPhone { get; set; }
        public string ToAddressEmail { get; set; }
        public string ToAddressStreet1 { get; set; }
        public string ToAddressStreet2 { get; set; }
        public string ToAddressCity { get; set; }
        public string ToAddressCountyState { get; set; }
        public string ToAddressZip { get; set; }
        public string ToAddressCountryIso { get; set; }
        public string ToAddressEoriNumber { get; set; }
        public string ToAddressVatNumber { get; set; }
        public List<Package> Packages { get; set; } = new List<Package>();
    }
    public class EvriService
    {
        public bool Signature { get; set; }
    }

    public class Package
    {
        public int PackageType { get; set; } // 3 = Parcel, 2 = Pak, 1 = Envelope
        public decimal PackageLength { get; set; }
        public decimal PackageWidth { get; set; }
        public decimal PackageHeight { get; set; }
        public decimal PackageWeight { get; set; }
        public List<PackageItem> PackageItems { get; set; }
    }

    public class PackageItem
    {
        public string ItemHsCode { get; set; }
        public string ItemSku { get; set; }
        public decimal ItemValue { get; set; }
        public decimal ItemWeight { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQuantity { get; set; }
        public string ItemValueCurrency { get; set; }
        public string ItemOriginCountryIso { get; set; }
        public string ManufacturerDetails { get; set; }
    }
}
