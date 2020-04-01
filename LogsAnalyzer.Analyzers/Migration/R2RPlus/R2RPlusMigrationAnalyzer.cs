using LogsAnalyzer.Infrastructure.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Analyzers.Migration.R2RPlus {
    public class R2RPlusMigrationAnalyzer : BaseLogAnalyzer {
        public readonly List<R2RPlusMigrationAnalysis> R2RPlusMigrationResults = new List<R2RPlusMigrationAnalysis>();
        private readonly List<Func<string, R2RPlusMigrationAnalysis, bool>> _logParsers = new List<Func<string, R2RPlusMigrationAnalysis, bool>>();

        private readonly List<Func<string, long, string, bool>> _preParsers = new List<Func<string, long, string, bool>>();

        private readonly List<BaseEntity> _parsedExtras = new List<BaseEntity>();
        private readonly List<BaseEntity> _parsedProducts = new List<BaseEntity>();
        private readonly List<ProductInventory> _parsedProductInventories = new List<ProductInventory>();


        private const string MIG_LOG_MARKER = @"\[MIG:(.*?)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\]";

        private string _lastInsertedBookingError;
        private InsertedRatePlan _lastInsertedRatePlan;

        private class InsertedRatePlan {
            public readonly string Name;
            public readonly string Code;
            public InsertedRatePlan(string name, string code) {
                Name = name;
                Code = code;
            }
        }

        private class BaseEntity {
            public readonly string Id;
            public readonly string Name;
            public BaseEntity(string id, string name) {
                Id = id;
                Name = name;
            }
        }

        private class ProductInventory : BaseEntity {
            public readonly string ProductId;
            public ProductInventory(string id, string name, string productId) : base(id, name) {
                ProductId = productId;
            }
        }


        public R2RPlusMigrationAnalyzer() {
            _logParsers.Add(tryParseStartMigration);
            _logParsers.Add(tryParseEndMigration);
            _logParsers.Add(tryParseBusinessUpdate);
            _logParsers.Add(tryParseInsertBooking);
            _logParsers.Add(tryParseInsertProduct);
            _logParsers.Add(tryParseInsertRatePlan);
            _logParsers.Add(tryParseInsertInactiveProduct);
            _logParsers.Add(tryParseInsertInventory);
            _logParsers.Add(tryParseInsertProductInventory);
            _logParsers.Add(tryParseInsertExtra);

            _preParsers.Add(tryParseBookingControllerPostLastError);
            _preParsers.Add(tryParseRatePlan);
            _preParsers.Add(tryParseExtra);
            //_preParsers.Add(tryParseProduct);
            _preParsers.Add(tryParseProductInventory);
        }



        public override bool Analyze(string lineText, long lineNumber, string sourceName) {
            foreach (var preParser in _preParsers) {
                if (preParser(lineText, lineNumber, sourceName)) {
                    break;
                }
            }

            R2RPlusMigrationAnalysis analysis;
            string logMessage;
            if (!tryParseMigrationLog(lineText, lineNumber, sourceName, out logMessage, out analysis)) {
                return false;
            }

            foreach (var parser in _logParsers) {
                if (parser(logMessage, analysis)) {
                    return true;
                }
            }
            return false;
        }

        private bool tryParseProduct(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "\"Name\":\"(.*?)\".*ProductTypeId.*AccommodationProductType.*Id\":\"(.*?)\"");
            if (m.Success) {
                var id = m.Groups[2].Value.Trim();
                if (!_parsedProducts.Any(e => e.Id == id)) {
                    _parsedProducts.Add(new BaseEntity(id, m.Groups[1].Value.Trim()));
                }
            }
            return m.Success;
        }

        private bool tryParseProductInventory(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "AccommodationProductId\":\"(.*?)\".*Id\":\"(.*?)\"");
            if (m.Success) {
                var inventoryId = m.Groups[2].Value.Trim();
                if (!_parsedProductInventories.Any(p => p.Id == inventoryId)) {
                    _parsedProductInventories.Add(new ProductInventory(inventoryId, string.Empty, m.Groups[1].Value.Trim()));
                }

            }
            return m.Success;
        }

        private bool tryParseExtra(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "\"Name\":\"(.*?)\",.*BookingExtraTypeId.*Id\":\"(.*?)\"");
            if (m.Success) {
                var id = m.Groups[2].Value.Trim();
                if (!_parsedExtras.Any(e => e.Id == id)) {
                    _parsedExtras.Add(new BaseEntity(id, m.Groups[1].Value.Trim()));
                }
            }
            return m.Success;
        }

        private bool tryParseRatePlan(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "{\"ProductId\".*\"Name\":\"(.*?)\".*\"Code\":\"(.*?)\"");
            if (m.Success) {
                _lastInsertedRatePlan = new InsertedRatePlan(m.Groups[1].Value, m.Groups[2].Value);
            }
            return m.Success;
        }


        #region Parsers
        private bool tryParseMigrationLog(string lineText, long lineNumber, string sourceName,
                                          out string logMessage, out R2RPlusMigrationAnalysis analysis) {
            analysis = null;
            logMessage = string.Empty;
            var m = Regex.Match(lineText, $"{MIG_LOG_MARKER}(.*)");
            if (m.Success) {
                analysis = new R2RPlusMigrationAnalysis(m);
                analysis.StartLineNumber = lineNumber;
                analysis.EndLineNumber = lineNumber;
                analysis.Source = sourceName;
                logMessage = m.Groups[6].Value;
            }
            return m.Success;
        }

        private bool tryParseStartMigration(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Migration START");
            if (m.Success) {
                R2RPlusMigrationResults.Add(analysis);
            }
            return m.Success;
        }
        private bool tryParseEndMigration(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Migration END. Result: (.*)");
            if (m.Success) {
                var result = new R2RPlusMigrationAnalysis(m);
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    theAnalysis.HadErrors = m.Groups[1].Value.ToUpper() != "TRUE";
                    theAnalysis.EndLineNumber = analysis.EndLineNumber;
                }
            }
            return m.Success;
        }

        private bool tryParseBookingControllerPostLastError(string lineText, long lineNumber, string sourceName) {
            var m = Regex.Match(lineText, "Exception.*\"Message\":\"(.*?)\".*BookingsController.Post\\(Guid businessId, Account account\\)");
            if (m.Success) {
                _lastInsertedBookingError = m.Groups[1].Value;
            }
            return m.Success;
        }

        private bool tryParseBusinessUpdate(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $"Successfully updated business");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    theAnalysis.WasBusinessUpdated = true;
                }
            }
            return m.Success;
        }
        private bool tryParseInsertBooking(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of R booking \[(.*?)\] via \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[3].Value.ToUpper() == "TRUE";
                    var insertedBooking = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        RPlusId = m.Groups[2].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    if (!isOk && !string.IsNullOrEmpty(_lastInsertedBookingError)) {
                        insertedBooking.StatusMessage = $"Probable cause: {_lastInsertedBookingError}";
                        _lastInsertedBookingError = null;
                    }
                    else {
                        insertedBooking.StatusMessage = m.Groups[4].Value;
                    }
                    theAnalysis.InsertedBookings.Add(insertedBooking);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertProduct(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of R Product \[(.*?)\|(.*?)\] via \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[4].Value.ToUpper() == "TRUE";
                    var inserted = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        Name = m.Groups[2].Value,
                        RPlusId = m.Groups[3].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    inserted.StatusMessage = m.Groups[5].Value;
                    theAnalysis.InsertedProducts.Add(inserted);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertInactiveProduct(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Created R\+ inactive product \[(.*?)\] for.*?\[(.*?)\((.*)\)\].*\[(.*?)\]");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var inserted = new InsertedInactiveProduct {
                        RId = m.Groups[4].Value,
                        RPlusId = m.Groups[1].Value,
                        RatePlanId = m.Groups[3].Value,
                        RatePlanName = m.Groups[2].Value.Trim(),
                        LineNumber = analysis.StartLineNumber
                    };
                    theAnalysis.InsertedInactiveProducts.Add(inserted);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertRatePlan(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of Rate Plan \[(.*?)\] with Id \[(.*?)\]: \[(.*?)\] (.*)");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[3].Value.ToUpper() == "TRUE";
                    var insertedData = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        RPlusId = m.Groups[2].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    insertedData.StatusMessage = m.Groups[4].Value;
                    if (_lastInsertedRatePlan != null) {
                        insertedData.Name = $"{_lastInsertedRatePlan.Name} ({_lastInsertedRatePlan.Code})";
                    }
                    theAnalysis.InsertedRatePlans.Add(insertedData);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertInventory(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of R Inventory/Product \[(.*?)\|(.*?)\].*via \[(.*?)\].*\[(.*)\]");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[4].Value.ToUpper() == "TRUE";
                    var inserted = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        Name = m.Groups[2].Value,
                        RPlusId = m.Groups[3].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    inserted.StatusMessage = m.Groups[5].Value;
                    theAnalysis.InsertedInventory.Add(inserted);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertProductInventory(string logMessage, R2RPlusMigrationAnalysis analysis) {
            // Unmatched closing square bracket in ID is sort of a bug; correct when logging code is modified.
            var m = Regex.Match(logMessage, $@"Insert result of product inventory \[(.*?): \[(.*?)\]");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var invRId = m.Groups[1].Value.Trim();
                    var inventory = _parsedProductInventories.FirstOrDefault(i => i.Id == invRId);
                    string productName = "Unknown";
                    if (inventory != null) {
                        var insertedProduct = theAnalysis.InsertedProducts.FirstOrDefault(p => p.RPlusId == inventory.ProductId);
                        if (insertedProduct != null) {
                            productName = insertedProduct.Name;
                        }
                    }

                    var isOk = m.Groups[2].Value.ToUpper() == "TRUE";
                    var inserted = new InsertedRPlusData {
                        RPlusId = m.Groups[1].Value,
                        Name = productName,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    theAnalysis.InsertedProductInventory.Add(inserted);
                }
            }
            return m.Success;
        }

        private bool tryParseInsertExtra(string logMessage, R2RPlusMigrationAnalysis analysis) {
            var m = Regex.Match(logMessage, $@"Insert result of AccommodationBookingExtra Id \[(.*?)\] with Id \[(.*?)\]: \[(.*?)\]");
            if (m.Success) {
                var theAnalysis = R2RPlusMigrationResults.LastOrDefault(a => a.LogId == analysis.LogId);
                if (theAnalysis != null) {
                    var isOk = m.Groups[3].Value.ToUpper() == "TRUE";
                    var inserted = new InsertedRPlusData {
                        RId = m.Groups[1].Value,
                        RPlusId = m.Groups[2].Value,
                        IsOk = isOk,
                        LineNumber = analysis.StartLineNumber
                    };
                    var extra = _parsedExtras.FirstOrDefault(e => e.Id == inserted.RPlusId);
                    if (extra != null) {
                        inserted.Name = extra.Name;
                    }
                    theAnalysis.InsertedAccommodationBookingExtras.Add(inserted);
                }
            }
            return m.Success;
        }
        #endregion
    }
}
