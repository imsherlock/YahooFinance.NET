# YahooFinance.NET
Download realtime and historical end of day stock data and historical dividend data via the Yahoo Finance API

## Install via NUGET
```
Install-Package YahooFinance.NET
```

## Usage
```csharp
using YahooFinance.NET;

string cookie = "YOUR_COOKIE";
string crumb = "YOUR_CRUMB";


string exchange = "ASX";
string symbol = "AFI";

YahooFinanceClient yahooFinance = new YahooFinanceClient(cookie, crumb);
string yahooStockCode = yahooFinance.GetYahooStockCode(exchange, symbol);
List<YahooHistoricalPriceData> yahooPriceHistory = yahooFinance.GetDailyHistoricalPriceData(yahooStockCode);
List<YahooHistoricalDividendData> yahooDividendHistory = yahooFinance.GetHistoricalDividendData(yahooStockCode);
YahooRealTimeData yahooRealTimeData = yahooFinance.GetRealTimeData(yahooStockCode);
```

## To get a cookie/crumb

[First try this script](https://github.com/IvanTrendafilov/YahooFinanceAPITokens)

### To get a cookie manually
1. Go to the Yahoo finance URL and search for a stock. eg. https://finance.yahoo.com/quote/AFI.AX/history?p=AFI.AX
2. In Chrome open Settings > Show advanced settings... > Privacy > Content Settings... > All cookies and site data... 
3. Find the site yahoo.com and the cookie name sould be 'B'
4. Copy the 'Content'
5. This is your cookie

### To get a crumb manually
1. Go to the Yahoo finance URL and search for a stock. eg. https://finance.yahoo.com/quote/AFI.AX/history?p=AFI.AX
2. Right click on 'Download Data' and copy the link address
3. Paste the link somewhere and your crumb will be at the and after the &crumb= eg. https://query1.finance.yahoo.com/v7/finance/download/AFI.AX?period1=1493432127&period2=1496024127&interval=1d&events=history&crumb=YOURCRUMB

## Building and testing the project via commandline
1. Open up a Powershell prompt and execute
```
PS> .\build.ps1
```

## Deploying the package to NuGet
1. Set the NuGet API key via the commandline if its not already set
```
nuget.exe setApiKey <API-Key> -Source https://www.nuget.org/api/v2/package
```
2. Increment the AssemblyVersion in AssemblyInfo.cs of the YahooFinance.NET project
3. Open up a Powershell prompt and execute
```
PS> .\build.ps1 -Target Deploy
```
