using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World");
            await GetWelcomeMessage();
        }

        static async Task GetWelcomeMessage()
        {

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5053");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/game");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);

                    
                } else
                {
                    Console.WriteLine("Didn't work");
                }
            }
            catch (Exception e)
            {

                throw ;
            }
        }
    }
}

