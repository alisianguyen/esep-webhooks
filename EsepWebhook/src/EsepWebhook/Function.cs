using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(object input, ILambdaContext text)
    {
        text.Logger.LogInformation($"FunctionHandler received: {input}");

        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        var user = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            info = new StringContent(payload, Encoding.UTF8, "application/json")
        };
    
        var answer = user.Send(request);
        using var viewer = new StreamReader(answer.Content.ReadAsStream());
            
        return viewer.ReadToEnd();
    }
}
