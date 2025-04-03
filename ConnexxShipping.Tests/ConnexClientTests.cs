using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Xunit;

namespace ConnexxShipping.Tests
{
    public class ConnexxxClientTests
    {
        private const string BearerToken = "test-token";

        [Fact]
        public async Task GetAvailableServicesAsync_ReturnsExpectedServices()
        {
            // Arrange
            var mockJson = @"
            {
                ""message"": """",
                ""statusCode"": 200,
                ""data"": [
                    {
                        ""serviceId"": ""UPS_EXPRESS_SAVER_138621"",
                        ""carrierName"": ""UPS"",
                        ""serviceName"": ""EXPRESS SAVER"",
                        ""associatedDepot"": ""Manchester - Unit A ..."",
                        ""directionType"": ""EXPORT""
                    },
                    {
                        ""serviceId"": ""UPS_STANDARD_137916"",
                        ""carrierName"": ""UPS"",
                        ""serviceName"": ""STANDARD"",
                        ""associatedDepot"": ""Manchester - Unit A ..."",
                        ""directionType"": ""EXPORT""
                    }
                ]
            }";

            var handler = new MockHttpMessageHandler(request =>
            {
                if (request.Method == HttpMethod.Get && request.RequestUri.AbsolutePath.EndsWith("carriers-services"))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(mockJson)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var client = new ConnexxClient(BearerToken, new ConnexxOptions { BaseUrl = "https://fake-base-url" }, handler);
            var services = await client.GetAvailableServicesAsync();

            Assert.Equal(2, services.Count);
            Assert.Equal("UPS_EXPRESS_SAVER_138621", services[0].ServiceId);
            Assert.Equal("EXPRESS SAVER", services[0].ServiceName);
            Assert.Equal("UPS_STANDARD_137916", services[1].ServiceId);
            Assert.Equal("STANDARD", services[1].ServiceName);
        }

        [Fact]
        public async Task CreateLabelsAsync_ReturnsExpectedResponse()
        {
            var mockJson = @"
            {
                ""message"": """",
                ""statusCode"": 201,
                ""data"": {
                    ""request"": {
                        ""id"": ""881bf487-cfb8-42fb-bca4-6424fd2ea453"",
                        ""createdDate"": ""2025-03-11T11:41:09.157Z""
                    },
                    ""commercialInvoicePdfUrls"": [],
                    ""combinedPdfUrl"": ""https://somewhere/combined.pdf"",
                    ""shipments"": [
                        {
                            ""id"": ""d60fa439-8436-4e3b-bc08-ebd1d6c817a0"",
                            ""carrierName"": ""UPS"",
                            ""serviceName"": ""STANDARD MULTI BOX"",
                            ""customerReference"": ""ITD Test"",
                            ""packages"": [
                                {
                                    ""index"": 0,
                                    ""height"": 30,
                                    ""weight"": 1000,
                                    ""length"": 30,
                                    ""width"": 30,
                                    ""trackingCode"": ""1Z3Y741A6794478600"",
                                    ""trackingUrl"": ""https://www.ups.com/track?trackNums=1Z3Y741A6794478600""
                                }
                            ]
                        }
                    ],
                    ""totalLabelsRequested"": 1,
                    ""totalLabelsErrored"": 0,
                    ""status"": ""COMPLETE"",
                    ""errors"": []
                }
            }";

            var handler = new MockHttpMessageHandler(request =>
            {
                if (request.Method == HttpMethod.Post && request.RequestUri.AbsolutePath.EndsWith("shipments"))
                {
                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(mockJson)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var client = new ConnexxClient(BearerToken, new ConnexxOptions { BaseUrl = "https://fake-base-url" }, handler);

            var requestModel = new CreateLabelRequest
            {
                LabelSize = "4x6",
                DoNotUseWebhook = true,
                Shipments = new List<Shipment> {}
            };

            var resp = await client.CreateLabelsAsync(requestModel);

            Assert.NotNull(resp);
            Assert.Equal("COMPLETE", resp.Status);
            Assert.Single(resp.Shipments);

            var shipment = resp.Shipments[0];
            Assert.Equal("UPS", shipment.CarrierName);
            Assert.Single(shipment.Packages);
            Assert.Equal("1Z3Y741A6794478600", shipment.Packages[0].TrackingCode);
        }

        [Fact]
        public async Task GetTrackingAsync_ReturnsTrackingData()
        {
            var mockJson = @"
            {
                ""message"": """",
                ""statusCode"": 201,
                ""data"": {
                    ""invalidBarcodes"": [],
                    ""validBarcodes"": [
                        {
                            ""barcode"": ""1Z3Y741A6794478600"",
                            ""events"": [
                                {
                                    ""statusTime"": ""2025-03-11T11:41:09.157Z"",
                                    ""statusCode"": ""Label Printed"",
                                    ""trackingEvent"": ""Label Printed""
                                }
                            ]
                        }
                    ]
                }
            }";

            var handler = new MockHttpMessageHandler(request =>
            {
                if (request.Method == HttpMethod.Post && request.RequestUri.AbsolutePath.EndsWith("tracking"))
                {
                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(mockJson)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var client = new ConnexxClient(BearerToken, new ConnexxOptions { BaseUrl = "https://fake-base-url" }, handler);

            var barcodes = new List<string> { "1Z3Y741A6794478600" };
            var tracking = await client.GetTrackingAsync(barcodes);

            Assert.NotNull(tracking);
            Assert.Empty(tracking.InvalidBarcodes);
            Assert.Single(tracking.ValidBarcodes);

            var vb = tracking.ValidBarcodes[0];
            Assert.Equal("1Z3Y741A6794478600", vb.Barcode);
            Assert.Single(vb.Events);
            Assert.Equal("Label Printed", vb.Events[0].StatusCode);
        }

        [Fact]
        public async Task CancelLabelAsync_ReturnsExpectedResult()
        {
            var mockJson = @"
            {
                ""message"": """",
                ""statusCode"": 201,
                ""data"": [
                    {
                        ""barcode"": ""1Z3Y741A6794478600"",
                        ""carrierShipmentID"": null,
                        ""success"": true,
                        ""successMessage"": ""Shipment cancelled"",
                        ""errorMessage"": """"
                    }
                ]
            }";

            var handler = new MockHttpMessageHandler(request =>
            {
                if (request.Method == HttpMethod.Post && request.RequestUri.AbsolutePath.EndsWith("shipments/cancel"))
                {
                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(mockJson)
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var client = new ConnexxClient(BearerToken, new ConnexxOptions { BaseUrl = "https://fake-base-url" }, handler);

            var barcodesToCancel = new List<string> { "1Z3Y741A6794478600" };

            var cancelResult = await client.CancelLabelAsync(barcodesToCancel);

            Assert.Single(cancelResult);
            Assert.True(cancelResult[0].Success);
            Assert.Equal("Shipment cancelled", cancelResult[0].SuccessMessage);
            Assert.Equal("1Z3Y741A6794478600", cancelResult[0].Barcode);
        }
    }
}
