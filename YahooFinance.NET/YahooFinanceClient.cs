using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace YahooFinance.NET
{
	public class YahooFinanceClient
	{
		private const string BaseUrl = "http://real-chart.finance.yahoo.com/table.csv?s=";
	    private const string RealTimeUrl = "http://finance.yahoo.com/d/quotes.csv?s=";
	    private const string RealTimeSuffix = "&f=abl1pohgt1nsv";

        private enum HistoryType
		{
			DividendHistory = 1,
			Day,
			Week,
			Month,
		}

		public string GetYahooStockCode(string exchange, string code)
		{
			var exchangeHelper = new YahooExchangeHelper();
			return exchangeHelper.GetYahooStockCode(exchange, code);
		}

		public List<YahooHistoricalPriceData> GetDailyHistoricalPriceData(string yahooStockCode, DateTime? startDate = null, DateTime? endDate = null)
		{
			return GetHistoricalPriceData(yahooStockCode, HistoryType.Day, startDate, endDate);
		}

		public List<YahooHistoricalPriceData> GetWeeklyHistoricalPriceData(string yahooStockCode, DateTime? startDate = null, DateTime? endDate = null)
		{
			return GetHistoricalPriceData(yahooStockCode, HistoryType.Week, startDate, endDate);
		}

		public List<YahooHistoricalPriceData> GetMonthlyHistoricalPriceData(string yahooStockCode, DateTime? startDate = null, DateTime? endDate = null)
		{
			return GetHistoricalPriceData(yahooStockCode, HistoryType.Month, startDate, endDate);
		}

		public List<YahooHistoricalDividendData> GetHistoricalDividendData(string yahooStockCode, DateTime? startDate = null, DateTime? endDate = null)
		{
			var dividendHistoryCsv = GetHistoricalDataAsCsv(yahooStockCode, HistoryType.DividendHistory, startDate, endDate);

			var historicalDevidendData = new List<YahooHistoricalDividendData>();
			foreach (var line in dividendHistoryCsv.Split('\n').Skip(1))
			{
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				var values = line.Split(',');

				var newDividendData = new YahooHistoricalDividendData
				{
					Date = DateTime.Parse(values[0], CultureInfo.InvariantCulture),
					Dividend = decimal.Parse(values[1], CultureInfo.InvariantCulture),
				};
				historicalDevidendData.Add(newDividendData);
			}

			return historicalDevidendData;
		}

		private List<YahooHistoricalPriceData> GetHistoricalPriceData(string yahooStockCode, HistoryType historyType, DateTime? startDate, DateTime? endDate)
		{
			var historicalDataCsv = GetHistoricalDataAsCsv(yahooStockCode, historyType, startDate, endDate);

			var historicalPriceData = new List<YahooHistoricalPriceData>();
			foreach (var line in historicalDataCsv.Split('\n').Skip(1))
			{
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				var values = line.Split(',');

				var newPriceData = new YahooHistoricalPriceData
				{
					Date = DateTime.Parse(values[0], CultureInfo.InvariantCulture),
					Open = decimal.Parse(values[1], CultureInfo.InvariantCulture),
					High = decimal.Parse(values[2], CultureInfo.InvariantCulture),
					Low = decimal.Parse(values[3], CultureInfo.InvariantCulture),
					Close = decimal.Parse(values[4], CultureInfo.InvariantCulture),
					Volume = long.Parse(values[5], CultureInfo.InvariantCulture),
					AdjClose = decimal.Parse(values[6], CultureInfo.InvariantCulture)
				};
				historicalPriceData.Add(newPriceData);
			}

			return historicalPriceData;
		}

		private string GetHistoricalDataAsCsv(string yahooStockCode, HistoryType historyType, DateTime? startDate, DateTime? endDate)
		{
			var dateRangeOption = string.Empty;
			var addDateRangeOption = startDate.HasValue && endDate.HasValue;
			if (addDateRangeOption)
			{
				var startDateValue = startDate.Value;
				var endDateValue = endDate.Value;

				dateRangeOption = GetDateRangeOption(startDateValue, endDateValue);
			}

			var historyTypeOption = GetHistoryType(historyType);
			var options = $"{dateRangeOption}{historyTypeOption}";

			var historicalDataCsv = YahooApiRequest(yahooStockCode, options);

			return historicalDataCsv;
		}

	    public YahooRealTimeData GetRealTimeData(string yahooStockCode)
	    {
	        var RealTimeDataCsv = GetRealTimeDataAsCsv(yahooStockCode);


            var values = RealTimeDataCsv.Replace("\"", "").Split(',');


            var realTimeData = new YahooRealTimeData
                {
                    Ask = decimal.Parse(values[0], CultureInfo.InvariantCulture),
                    Bid = decimal.Parse(values[1], CultureInfo.InvariantCulture),
                    Last = decimal.Parse(values[2], CultureInfo.InvariantCulture),
                    PreviousClose = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                    Open = decimal.Parse(values[4], CultureInfo.InvariantCulture),
	                High = decimal.Parse(values[5], CultureInfo.InvariantCulture),
	                Low = decimal.Parse(values[6], CultureInfo.InvariantCulture),
                    LastTradeTime = DateTime.Parse(values[7], CultureInfo.InvariantCulture),
                    Name = values[8], 
                    Symbol = values[9],
	                Volume = long.Parse(values[10], CultureInfo.InvariantCulture),
                };

	        return realTimeData;
        }

        private string GetRealTimeDataAsCsv(string yahooStockCode)
	    {

	        var realTimeDataCsv = YahooRealTimeApiRequest(yahooStockCode);

	        return realTimeDataCsv;
	    }

        private string YahooApiRequest(string yahooStockCode, string options)
		{
			var requestUrl = $"{BaseUrl}{yahooStockCode}{options}";

			using (var client = new HttpClient())
			{
				using (var response = client.GetAsync(requestUrl).Result)
				{
					var historicalData = response.Content.ReadAsStringAsync().Result;

					if (response.IsSuccessStatusCode)
					{
						return historicalData;
					}

					return string.Empty;
				}
			}
		}
	    private string YahooRealTimeApiRequest(string yahooStockCode)
	    {
	        var requestUrl = $"{RealTimeUrl}{yahooStockCode}{RealTimeSuffix}";

	        using (var client = new HttpClient())
	        {
	            using (var response = client.GetAsync(requestUrl).Result)
	            {
	                var realTimeData = response.Content.ReadAsStringAsync().Result;

	                if (response.IsSuccessStatusCode)
	                {
	                    return realTimeData;
	                }

	                return string.Empty;
	            }
	        }
	    }

        private string GetHistoryType(HistoryType type)
		{
			var optionCode = string.Empty;
			switch (type)
			{
				case HistoryType.DividendHistory:
					optionCode = "v";
					break;
				case HistoryType.Day:
					optionCode = "d";
					break;
				case HistoryType.Week:
					optionCode = "w";
					break;
				case HistoryType.Month:
					optionCode = "m";
					break;
			}

			var option = $"&g={optionCode}";
			return option;
		}

		private string GetDateRangeOption(DateTime startDate, DateTime endDate)
		{
			var start = $"{GetStartDate(startDate)}";
			var end = $"{GetEndDate(endDate)}";

			var option = $"{start}{end}";
			return option;
		}

		private string GetStartDate(DateTime date)
		{
			// API uses zero-based month numbering
			var month = $"&a={date.Month - 1}";
			var day = $"&b={date.Day}";
			var year = $"&c={date.Year}";

			var option = $"{month}{day}{year}";
			return option;
		}

		private string GetEndDate(DateTime date)
		{
			// API uses zero-based month numbering
			var month = $"&d={date.Month - 1}";
			var day = $"&e={date.Day}";
			var year = $"&f={date.Year}";

			var option = $"{month}{day}{year}";
			return option;
		}
	}
}
