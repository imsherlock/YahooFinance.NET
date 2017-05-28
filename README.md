# YahooFinance.NET
Download historical end of day stock data and historical dividend data via the Yahoo Finance API

Install via NUGET
```
Install-Package YahooFinance.NET
```

Usage
```csharp
using YahooFinance.NET;

string exchange = "ASX";
string symbol = "AFI";

YahooFinanceClient yahooFinance = new YahooFinanceClient();
string yahooStockCode = yahooFinance.GetYahooStockCode(exchange, symbol);
List<YahooHistoricalPriceData> yahooPriceHistory = yahooFinance.GetDailyHistoricalPriceData(yahooStockCode);
List<YahooHistoricalDividendData> yahooDividendHistory = yahooFinance.GetHistoricalDividendData(yahooStockCode);
YahooRealTimeData yahooRealTimeData = yahooFinance.GetRealTimeData(yahooStockCode);
```
