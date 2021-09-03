using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using QuickType;

namespace MyFunctionProj
{
    public static class CheckHoliday
    {
        [FunctionName("CheckHoliday")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request.");

            var tempPath = System.IO.Path.GetTempPath();
            var tempFile = Path.Combine(tempPath, "holiday.json");

            if (!File.Exists(tempFile))
            {
                using HttpClient client = new HttpClient();
                var result = await client.GetFromJsonAsync<Holiday>("https://data.taipei/api/v1/dataset/29d9771d-c0ee-40d4-8dfb-3866b0b7adaa?scope=resourceAquire&offset=958&limit=1000");
                log.LogInformation($"Writing cache holiday.json to: {tempFile}");
                File.WriteAllText(tempFile, result.ToJson());
            }
            else
            {
                log.LogInformation($"Load holiday.json from cache: {tempFile}");
            }

            var data = Holiday.FromJson(File.ReadAllText(tempFile));

            string date = req.Query["date"];

            DateTime dt;
            if (DateTime.TryParse(date, out dt))
            {
                var dateStr = dt.ToString("yyyy/M/d");

                var item = data.Result.Results.FirstOrDefault(p => p.Date == dateStr);

                if (item == null)
                {
                    item = new ResultElement();
                }

                if (item.Name == "軍人節")
                {
                    item.IsHoliday = IsHoliday.否;
                }

                // 如果 item
                return new OkObjectResult(item);
            }
            else
            {
                return new BadRequestObjectResult(new { });
            }


            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            // return new OkObjectResult(data);
        }
    }
}
