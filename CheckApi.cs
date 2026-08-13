using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:5001/api/HomeSettings");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
    }
}
