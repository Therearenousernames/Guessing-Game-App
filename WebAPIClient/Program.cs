using System.Net.Http.Headers;
using System.Text.Json;
using GuessGameAPI;


// Waiting for the api to start up
Console.WriteLine("Please wait for bit. API is starting up..");
Console.WriteLine();

// HttpClient creation and setup
using HttpClient client = new();
client.BaseAddress = new Uri("http://localhost:5053/api/game"); 
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");


// Running of the game
await GamePlay(client);

static async Task GamePlay(HttpClient client)
{
    
    bool Replay = false;
    int wonGames = 0; int lostGames = 0;

    do
    {
        Model result = await Start(client);
        List<int> list = await Guess(client, result);
        wonGames += list[0];
        lostGames += list[1];
        Replay = ReplayGame();
        Console.WriteLine();
    } while (Replay);
    
    Console.WriteLine($"You won {wonGames} games. You lost {lostGames} games. With a total of {wonGames + lostGames} games played. ");
    Console.WriteLine("Bye!");
}



// Sending the first request to the api
static async Task<Model?> Start(HttpClient client)
{
    HttpResponseMessage message = await client.GetAsync(client.BaseAddress);
    Console.WriteLine(message.Content);
    if (message.IsSuccessStatusCode)
    {
        var result = await GetResult<Model>(message);
        Console.WriteLine(result?.Message);
        return result;        
    }

    //TODO: ADD ERROR HANDLING

    return null;
}




// Gets user input
static int UserInput()
{ 
    Console.WriteLine("Please input a guess between the range of 1 to 20:");
    var userGuess = Console.ReadLine();

    // check if the user input is not null and is a valid number then return a string? or int?
    int num;
    while (!int.TryParse(userGuess, out num))
    {
        Console.WriteLine("Please enter a digit");
        Console.WriteLine("Input your guess: ");
        userGuess = Console.ReadLine();
    }
    return num;
}


// Play again
static bool ReplayGame()
{
    Console.WriteLine("Do you want to play again (y/n)? ");

    while (true)
    {
        var keyStroke = Console.ReadKey();
        if (keyStroke.Key == ConsoleKey.Y)
        {
            return true;
        }
        else if (keyStroke.Key == ConsoleKey.N)
        {
            return false;
        }
        else
        {
            Console.Write("\nPlease input either y or n: ");
        }
    }
}

// Sends the requests for game play
static async Task<List<int>> Guess(HttpClient client, Model result)
{
    var tries = result?.Tries;
    int win = 0;
    int lose = 0;
    
    while (tries > 0)
    {
        string guess = UserInput().ToString();
        HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/{result?.Id}/{guess}");

        if (message.IsSuccessStatusCode)
        {
            var newMessage = await GetResult<Model>(message);
            Console.WriteLine(newMessage?.Message);
            var updatedTries = newMessage?.Tries;
            var isPlaying = newMessage?.Playing;
            var winOrLose = newMessage?.Lost;
            if (isPlaying == false)
            {

                if ((bool)winOrLose)
                {
                    lose++;
                }
                else
                {
                    win++;
                }
                return new List<int>()
                {
                    win, lose
                };
            }
            else
            {
                tries = updatedTries;
            }
        }
    }
    // TO DO: ERROR HANDLING
    return null;

}



// Deserializes 
static async Task<T?> GetResult<T>(HttpResponseMessage message)
{
    var content = await message.Content.ReadAsStringAsync();
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var result = JsonSerializer.Deserialize<T>(content, options);
    return result;
}




    