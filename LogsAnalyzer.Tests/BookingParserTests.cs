using LogAnalyzer.Analyzers.Bookings.Models;
using LogsAnalyzer.Analyzers.Bookings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogsAnalyzer.Tests {
    [TestClass]
    public class BookingParserTests {

        [TestMethod]
        public void itShouldParseSelfClosingBookingElement() {
            var parser = new BookingParser();
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}/>",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} />",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} id='1' />",
                $"othertextonsameline<{BookingParser.XmlTokens.ROOT_ELEMENT} id='1' />othertextonsameline"

            };
            foreach (var input in inputs) {
                parser.Accept(input);
                Assert.IsNotNull(parser.BookingAnalysis);
            }
        }

        [TestMethod]
        public void itShouldParseBookingWithStartAndEndTagsOnSameLine() {
            var parser = new BookingParser();
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}></{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}> </{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}><test a1='1'></test></{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"othertextonsameline<{BookingParser.XmlTokens.ROOT_ELEMENT}><test a1='1'></test></{BookingParser.XmlTokens.ROOT_ELEMENT}>othertextonsameline"
            };

            foreach (var input in inputs) {
                parser.Accept(input);
                Assert.IsNotNull(parser.BookingAnalysis);
            }
        }

        [TestMethod]
        public void itShouldParseBookingWithStartAndEndTagsOnDifferentLines() {
            var input1 = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var input2 = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} test='123'>",
                "<someChildElement />",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var inputs = new string[][] {
                input1,
                input2
            };

            var parser = new BookingParser();
            for (var k = 0; k < inputs.Length; k++) {
                for (var j = 0; j < inputs[k].Length; j++) {
                    parser.Accept(inputs[k][j]);
                    if (j == 0) {
                        Assert.IsNull(parser.BookingAnalysis);
                    }
                }
                Assert.IsNotNull(parser.BookingAnalysis);
            }
        }

        [TestMethod]
        public void itShouldReadTransactionId() {
            var clientTransactionId = "123";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} {BookingParser.XmlTokens.CLIENT_TRANSACTION_ID}='{clientTransactionId}'>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(clientTransactionId, parser.BookingAnalysis.TransactionId);

            // Should still work without TransactionId
            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrEmpty(parser.BookingAnalysis.TransactionId));
        }

        [TestMethod]
        public void itShouldReadMultipleBookingsFromSingleInput() {
            var clientTransactionId1 = "123";
            var clientTransactionId2 = "456";

            var inputs = new string[] {
                "first unrelated text in input",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} {BookingParser.XmlTokens.CLIENT_TRANSACTION_ID}='{clientTransactionId1}'>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                "some unrelated text in input",
                "some more unrelated text in input",
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} {BookingParser.XmlTokens.CLIENT_TRANSACTION_ID}='{clientTransactionId2}'>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                "last unrelated text in input"
            };

            var outputs = new List<BookingAnalysis>();
            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
                if (parser.BookingAnalysis != null) {
                    outputs.Add(parser.BookingAnalysis);
                }
            }

            Assert.AreEqual(2, outputs.Count);
            var firstBooking = outputs.FirstOrDefault();
            Assert.AreEqual(clientTransactionId1, firstBooking.TransactionId);
            var secondBooking = outputs.LastOrDefault();
            Assert.AreEqual(clientTransactionId2, secondBooking.TransactionId);
        }

        [TestMethod]
        public void itShouldReadTransactionTimestamp() {
            var timestamp = DateTime.Now.ToString();
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT} {BookingParser.XmlTokens.TIMESTAMP}='{timestamp}'>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(timestamp, parser.BookingAnalysis.Timestamp);

            // Should still work without Timestamp
            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrEmpty(parser.BookingAnalysis.Timestamp));
        }

        [TestMethod]
        public void itShouldReadDistributorShortName() {
            var distributor = "acme corp";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}  xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>",
                $"<POS xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<Source>",
                $"<BookingChannel>",
                $"<CompanyName {BookingParser.XmlTokens.DISTRIBUTOR}='{distributor}'></CompanyName>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(distributor, parser.BookingAnalysis.DistributorShortName);

            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.DistributorShortName));
        }


        [TestMethod]
        public void itShouldReadProductIdAndName() {
            var productId = Guid.NewGuid().ToString();
            var productName = "Water Villa - Standard Rate";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<ProductTypeRsrvs xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<ProductTypeRsrv>",
                $"<ProductType {BookingParser.XmlTokens.PRODUCT_ID}='{productId}'>",
                $"<Name>{productName}</Name>",
                $"</ProductType>",
                $"</ProductTypeRsrv>",
                $"</ProductTypeRsrvs>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(productName, parser.BookingAnalysis.ProductName);
            Assert.AreEqual(productId, parser.BookingAnalysis.ProductId);

            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.ProductName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.ProductId));
        }

        //[TestMethod]
        public void itShouldReadChannelCommissionAndPaymentOption() {
            var channelCommission = "143.44";
            var paymentOption = "dummyPaymentOption";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<ProductTypeRsrvs {BookingParser.XmlTokens.CHANNEL_COMMISSION}='{channelCommission}' {BookingParser.XmlTokens.PAYMENT_OPTION}='{paymentOption}'>",
                $"<ProductTypeRsrv>",
                $"<ProductType>",
                $"<Name>dummyProduct</Name>",
                $"</ProductType>",
                $"</ProductTypeRsrv>",
                $"</ProductTypeRsrvs>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(channelCommission, parser.BookingAnalysis.ChannelCommission);
            Assert.AreEqual(paymentOption, parser.BookingAnalysis.PaymentOption);

            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.ChannelCommission));
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.PaymentOption));
        }

        [TestMethod]
        public void itShouldReadStartAndEndDates() {
            var startDate = DateTime.Now.ToString();
            var endDate = DateTime.Now.AddDays(2).ToString();
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<ProductTypeRsrvs xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<ProductTypeRsrv>",
                $"<ProductType>",
                $"<Name>dummyProduct</Name>",
                $"<TimeSlots>",
                $"<TimeSlot {BookingParser.XmlTokens.START_DATE}='{startDate}' {BookingParser.XmlTokens.END_DATE}='{endDate}'>",
                $"</TimeSlot>",
                $"</TimeSlots>",
                $"</ProductType>",
                $"</ProductTypeRsrv>",
                $"</ProductTypeRsrvs>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(startDate, parser.BookingAnalysis.StartDate);
            Assert.AreEqual(endDate, parser.BookingAnalysis.EndDate);


            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.StartDate));
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.EndDate));
        }


        [TestMethod]
        public void itShouldReadPrimaryProviderAndCount() {
            var primaryProvider = "dummyProvider";
            var providerCount = 2;
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<AvailRequestSegments xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<UTSv_AvailRequestSegment>",
                $"<TPA_Extensions>",
                $"<SearchCriteria>",
                $"<ProviderID_List>",
                $"<ID>{primaryProvider}</ID>",
                $"<ID>dummy</ID>",
                $"</ProviderID_List>",
                $"</SearchCriteria>",
                $"</TPA_Extensions>",
                $"</UTSv_AvailRequestSegment>",
                $"</AvailRequestSegments>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(primaryProvider, parser.BookingAnalysis.PrimaryProvider);
            Assert.AreEqual(providerCount, parser.BookingAnalysis.ProviderCount);


            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.PrimaryProvider));
            Assert.AreEqual(0, parser.BookingAnalysis.ProviderCount);
        }

        [TestMethod]
        public void itShouldReadProductTotalAndExtrasTotal() {
            var productTotal = "475.50";
            var extrasTotal = "200.00";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<ProductTypeRsrvs xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<ProductTypeRsrv>",
                $"<ProductType>",
                $"<Name>dummyProduct</Name>",
                $"<TotalRate Amount='{productTotal}' />",
                $"<Supplements TotalRate='{extrasTotal}' />",
                $"</ProductType>",
                $"</ProductTypeRsrv>",
                $"</ProductTypeRsrvs>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(productTotal, parser.BookingAnalysis.ProductTotal);
            Assert.AreEqual(extrasTotal, parser.BookingAnalysis.ExtrasTotal);


            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.ProductTotal));
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.ExtrasTotal));
        }

        [TestMethod]
        public void itShouldReadCustomerFirstAndLastNames() {
            var firstName = "Julia";
            var lastName = "Harper";
            var inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<Customer xmlns='{BookingParser.XmlTokens.NS_EVIIVO}'>",
                $"<PersonName>",
                $"<GivenName>{firstName}</GivenName>",
                $"<Surname>{lastName}</Surname>",
                $"</PersonName>",
                $"</Customer>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };

            var parser = new BookingParser();
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.AreEqual(firstName, parser.BookingAnalysis.CustomerFirstName);
            Assert.AreEqual(lastName, parser.BookingAnalysis.CustomerLastName);


            inputs = new string[] {
                $"<{BookingParser.XmlTokens.ROOT_ELEMENT}>",
                $"<POS>",
                $"<Source>",
                $"<BookingChannel>",
                $"</BookingChannel>",
                $"</Source>",
                $"</POS>",
                $"</{BookingParser.XmlTokens.ROOT_ELEMENT}>"
            };
            foreach (var input in inputs) {
                parser.Accept(input);
            }

            Assert.IsNotNull(parser.BookingAnalysis);
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.CustomerFirstName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(parser.BookingAnalysis.CustomerLastName));
        }

        [TestMethod]
        public void itShouldReadExtraDetails() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldReadMiscellaneousTraceData() {
            throw new NotImplementedException();
        }

    }
}
