using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace StockOpenClosePrice
{
    class Program
    {
        static DateTime parseDate(string date)
        {
            var splitedDate = date.Split('-');
            var day = int.Parse(splitedDate[0]);
            var month = splitedDate[1];
            var year = int.Parse(splitedDate[2]);
            
            // Here I could use the Globalization to get the month name by the current location, but to avoid problems at the plataform, was better get the month name hardcoded like that.
            var monthNumber = 0;
            if (month.Equals("January")) monthNumber = 1;
            if (month.Equals("February")) monthNumber = 2;
            if (month.Equals("March")) monthNumber = 3;
            if (month.Equals("April")) monthNumber = 4;
            if (month.Equals("May")) monthNumber = 5;
            if (month.Equals("June")) monthNumber = 6;
            if (month.Equals("July")) monthNumber = 7;
            if (month.Equals("August")) monthNumber = 8;
            if (month.Equals("September")) monthNumber = 9;
            if (month.Equals("October")) monthNumber = 10;
            if (month.Equals("November")) monthNumber = 11;
            if (month.Equals("December")) monthNumber = 12;

            return new DateTime(year, monthNumber, day);
        }

        static List<string> getDatesToSearch(DateTime firstDate, DateTime lastDate, string weekDay)
        {
            var result = new List<string>();

            var date = firstDate;
            do
            {
                if (date.DayOfWeek.ToString().Equals(weekDay))
                {
                    result.Add(date.ToString("d-MMMM-yyyy", System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("en")));
                }

                date = date.AddDays(1);
            }
            while (date <= lastDate);
            
            return result;
        }

        static void openAndClosePrices(string firstDate, string lastDate, string weekDay)
        {
            var firstDateParse = parseDate(firstDate);
            var lastDateParse = parseDate(lastDate);

            var datesToSearch = getDatesToSearch(firstDateParse, lastDateParse, weekDay);

            var result = new List<JArray>();

            using (var http = new HttpClient())
            {
                foreach (var date in datesToSearch)
                {
                    var uri = new Uri("https://jsonmock.hackerrank.com/api/stocks/?date="+ date);
                    var response = http.GetAsync(uri).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        result.Add(content["data"]);
                    }
                }
            }

            foreach (var item in result)
            {
                var extractedValue = item.FirstOrDefault();
                if (extractedValue == null) continue;
                
                Console.WriteLine(string.Format("{0} {1} {2}", extractedValue["date"], extractedValue["open"], extractedValue["close"]));
            }
        }

        static void Main(string[] args)
        {
            openAndClosePrices("1-January-2000", "22-February-2000", "Monday");
            Console.WriteLine("--------------------------------");
            openAndClosePrices("26-March-2001", "15-August-2001", "Wednesday");
            Console.ReadLine();
        }
    }
}
