# Taiwan Holiday Function App

## Basic Usage

- Example

    ```sh
    curl -s "https://holidaybook.azurewebsites.net/api/checkholiday?code=will.fans&date=2021-06-14"
    ```

- Bash

    ```sh
    curl -s "https://holidaybook.azurewebsites.net/api/checkholiday?code=will.fans&date=`date -Idate`"
    ```

## Development

```sh
git clone https://github.com/doggy8088/TaiwanHolidayFuncApp.git
cd TaiwanHolidayFuncApp

func start
```

- Usage:

  http://localhost:7071/api/CheckHoliday?date=2021-6-14

- Result

  ```sh
  {
      "description": "全國各機關學校放假一日。",
      "holidayCategory": "放假之紀念日及節日",
      "isHoliday": 1,
      "date": "2021/6/14",
      "_id": 1016,
      "name": "端午節"
  }
  ```

## Build

```sh
dotnet build -c Release
```

## Publish

1. Login with Azure CLI first

   ```sh
   az login
   az account set -s "SubscriptionName or SubscriptionID"
   ```

2. Publish to a Function App

   ```sh
   func azure functionapp publish holidaybook
   ```

   > 這個步驟會自動取得名為 `default` 的 Function Key

3. Built-in log streaming

    ```sh
    func azure functionapp logstream holidaybook
    ```

### Code Snippets

- Sample Code: [HttpClientExt_Newtonsoft.Json.cs](https://gist.github.com/doggy8088/8bfc6613e34c4d3c5db6348c24b46759#file-httpclientext_newtonsoft-json-cs-L29-L41)

  ```cs
  using Newtonsoft.Json;

  public static class HttpClientExt
  {
      public static async System.Threading.Tasks.Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUrl, T theObj)
      {
          return await client.PostAsync(requestUrl, new StringContent(JsonConvert.SerializeObject(theObj), Encoding.UTF8, "application/json"));
      }

      public static async System.Threading.Tasks.Task<T> GetFromJsonAsync<T>(this HttpClient client, string requestUrl)
      {
          return JsonConvert.DeserializeObject<T>(await client.GetStringAsync(requestUrl));
      }
  }
  ```

## Issues

- Function App - Hot Reload

  [Hot reload · Issue #1239 · Azure/azure-functions-core-tools](https://github.com/Azure/azure-functions-core-tools/issues/1239)

  ```xml
  <Target Name="RunFunctions">
      <Exec Command="func start" />
  </Target>
  ```

  ```sh
  dotnet watch msbuild /t:RunFunctions
  ```

## Function App Links

- [Work with Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=linux%2Ccsharp%2Cbash&WT.mc_id=DT-MVP-4015686)

  ```json
  {
    "IsEncrypted": false,
    "Values": {
      "FUNCTIONS_WORKER_RUNTIME": "<language worker>",
      "AzureWebJobsStorage": "<connection-string>",
      "AzureWebJobsDashboard": "<connection-string>",
      "MyBindingConnection": "<binding-connection-string>",
      "AzureWebJobs.HttpExample.Disabled": "true"
    },
    "Host": {
      "LocalHttpPort": 7071,
      "CORS": "*",
      "CORSCredentials": false
    },
    "ConnectionStrings": {
      "SQLConnectionString": "<sqlclient-connection-string>"
    }
  }
  ```

- [Register Azure Functions binding extensions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-register#extension-bundles?WT.mc_id=DT-MVP-4015686)

- [Connectors for Azure Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/apis-list)


## Azure Logic App

- Debugging Tool
  - [Webhook.site - Test, process and transform emails and HTTP requests](https://webhook.site/)
  - [Troubleshoot and diagnose workflow failures - Azure Logic Apps](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-diagnosing-failures) -> [Perform runtime debugging](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-diagnosing-failures#perform-runtime-debugging)

- Functions in expressions
  - [Reference guide to using functions in expressions for Azure Logic Apps and Power Automate](https://docs.microsoft.com/en-us/azure/logic-apps/workflow-definition-language-functions-reference?WT.mc_id=DT-MVP-4015686)
  - [Custom date and time format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings?WT.mc_id=DT-MVP-4015686)

- Nest Logic App
  - [Multiple Triggers on a Logic App Workflow: Limitations and a Different Approach](https://powerobjects.com/tips-and-tricks/multiple-triggers-logic-app-workflow/)
  - [Call, trigger, or nest logic apps by using HTTPS endpoints in Azure Logic Apps](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-http-endpoint)

- 取得本地時間今天日期的 expression 語法

  ```js
  convertFromUtc(utcNow('yyyy-MM-ddTHH:mm:ssK'), 'Taipei Standard Time', 'yyyy-MM-dd')
  ```

- 呼叫外部 HTTP 服務

    > [Call service endpoints over HTTP or HTTPS from Azure Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-native-http)

    ```json
    {
        "inputs": {
            "method": "GET",
            "uri": "https://holidaybook.azurewebsites.net/api/checkholiday",
            "queries": {
                "code": "will.fans",
                "date": "@{convertFromUtc(utcNow(), 'Taipei Standard Time', 'yyyy-MM-dd')}"
            }
        }
    }
    ```

- 取得 HTTP action 的回應 HTTP Status Code 內容

  ```js
  outputs('HTTP')['statusCode']
  ```

- 取得 HTTP action 的回應 JSON 內容

  ```js
  body('HTTP')['isHoliday']
  ```

  > 回傳的 body 部分預設就是 JSON 反序列化之後的物件

## DataSource

- API
  - 2021 年所有假期的網址: https://data.taipei/api/v1/dataset/29d9771d-c0ee-40d4-8dfb-3866b0b7adaa?scope=resourceAquire&offset=958&limit=1000
- [臺北市資料大平臺](https://data.taipei/)
  - [臺北市政府行政機關辦公日曆表](https://data.taipei/#/dataset/detail?id=c30ca421-d935-4faa-b523-9c175c8de738)
