// See https://aka.ms/new-console-template for more information
using System.Text;
using Newtonsoft.Json;
using System.Configuration;


string model = ReadSetting("model");
string apiToken = ReadSetting("apiToken");
string jsonPacket = "{\"model\":\"" + model + "\",\"prompt\":\"" + args[0] + "\",\"temperature\":1,\"max_tokens\":100}";
string bearer = "Bearer " + apiToken;

if (args.Length > 0)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("authorization", bearer);

    var content = new StringContent(jsonPacket, Encoding.UTF8, "application/json");

    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);
    string responseString = await response.Content.ReadAsStringAsync();

    try
    {
        var dynData = JsonConvert.DeserializeObject<dynamic>(responseString);
        Console.ForegroundColor = ConsoleColor.Green;
        string reply = dynData!.choices[0].text;
        var fixedMsg = reply.Split("\n").Last();

        System.Console.WriteLine($"----> GPT-3 response is: ");
        System.Console.WriteLine(fixedMsg);
        Console.ResetColor();
        System.Console.WriteLine("");
    }
    catch (Exception ex)
    {
        System.Console.WriteLine("An error occured: " + ex.Message);
    }

}
else
{
    System.Console.WriteLine("-----> you need to provide some input");
}


static string ReadSetting(string key)
{
    try
    {
        var appSettings = ConfigurationManager.AppSettings;
        string result = appSettings[key] ?? "Not Found";
        return result;
    }
    catch (ConfigurationErrorsException)
    {
        throw new Exception("Error reading app settings");
    }
}