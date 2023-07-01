using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using GuessGameAPI;
using GuessGameAPI.Models;


// Waiting for the api to start up
Console.WriteLine("Please wait for bit. API is starting up..");
Console.WriteLine();


// HttpClient creation and setup
using HttpClient client = new();
client.BaseAddress = new Uri("http://localhost:5053/api/data");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");


await GameStart(client);
Console.WriteLine();

static async Task GameStart(HttpClient client)
{
    
    int wonGames = 0; int lostGames = 0;

    string message = "What do want to do: \n" +
        "-To delete user press D\n" +
        "-To view user history press V\n"+
        "-To delete user history press A\n" +
        "-To play a new game press S\n" +
        "-To register press R\n" +
        "-To exit press E\n";


    Console.WriteLine(message);
    while (true)
    {
        var keyStroke = Console.ReadKey();
        Console.WriteLine();
        if (keyStroke.Key == ConsoleKey.D)
        {
            await DeleteUsernameAndHistory(client);

        } else if (keyStroke.Key == ConsoleKey.S)
        {
            Model result = await Start(client, false);
            List<int> list = await Guess(client, result);
            wonGames += list[0];
            lostGames += list[1];
        } else if (keyStroke.Key == ConsoleKey.V)
        {
            await GetUserHistory(client);
        }
        else if (keyStroke.Key == ConsoleKey.A)
        {
            await DeleteUserHistory(client);
        } else if (keyStroke.Key == ConsoleKey.R)
        {
            Model result = await Start(client, true);
            List<int> list = await Guess(client, result);
            wonGames += list[0];
            lostGames += list[1];

        } else if (keyStroke.Key == ConsoleKey.E)
        {
            Console.WriteLine("So sad to see you go...");
            break;
        }
        else
        {
            Console.Write(message);
        }
        Console.WriteLine($"\n{message}");
    }
    Console.WriteLine($"Player stats: You won {wonGames} games. You lost {lostGames} games. With a total of {wonGames + lostGames} games played. ");
    Console.WriteLine("Bye!");
}


static string UsernameCheck(string username)
{
    List<char> chars = new()
    {
        '{', '}', '|', '^', '~', '[', ']', '`', '/'
    };


    foreach (char x in chars)
    {
        if (username.Contains(x))
        {
            Console.WriteLine($"Username cannot contain {x}.\nPlease enter another username.\n");
            while (username.Contains(x) || username == "")
            {
                Console.Write("Please enter a username: ");
                username = Console.ReadLine();
            }
        }
    }

    if (username == "")
    {
        Console.WriteLine($"Username cannot be an empty string.\nPlease enter another username.\n");

        while (username == "")
        {
            Console.Write("Please enter a username: ");
            username = Console.ReadLine();
        }
    }
    return username;
}



// Connect to the API and get a username assigned to you.
static async Task<Model?> ValidateUsername(HttpClient client, bool register)
{
    HttpResponseMessage message; Task<HttpResponseMessage> response;

    Console.WriteLine("These characters are illegal characters: '{', '}', '|', '^', '~', '[', ']', '`'. Please refrain from using them in your username.\n");
    Console.Write("Please enter a username: ");
    var username = Console.ReadLine();
    Console.WriteLine();


    // error handling on this side of the program because username cannot change url
    if (username != null)
    {
        var checkedUsername = UsernameCheck(username);
        if (register)
        {
            message = await client.GetAsync($"{client.BaseAddress}/register/{checkedUsername}");
        }
        else
        {
            message = await client.GetAsync($"{client.BaseAddress}/{checkedUsername}");
        }
        response = WhileNotValid(client, message);
        message = response.Result;
        var result = await GetResult<Model>(message);
        Console.Write($"{result?.Message}\n");
       return result;
    }
    return null;
   
}



static async Task<HttpResponseMessage> WhileNotValid(HttpClient client, HttpResponseMessage message)
{
    string checkedUsername;
    while (message.StatusCode != HttpStatusCode.OK)
    {
        var decoded = await GetResult<Model>(message);
        Console.WriteLine($"The username {decoded?.Message}\n");
        checkedUsername = CheckedString();
        message = await client.GetAsync($"{client.BaseAddress}/register/{checkedUsername}");
    }
    return message;
}



static async Task GetUserHistory(HttpClient client)
{
    Model result;
    string checkedUsername = CheckedString();
    HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/history/{checkedUsername}");
    result = await GetResult<Model?>(message);
    if (message.IsSuccessStatusCode)
    {
        List<GameState> states = result?.GameStates;
        Console.WriteLine($"Game history for {checkedUsername}");
        if (states.Count == 0)
        {
            Console.WriteLine($"Game history for user {checkedUsername} is empty");
        }
        else
        {
            foreach (GameState state in states)
            {
                Console.WriteLine($"State Id: {state.Id}  Number to guess: {state.NumberToGuess}  Won/Lost: {state.WonLoss}  Remaining Tries: {state.Tries}");
            }
        }
    } else
    {
        Console.WriteLine($"{result?.Message}");
    }
}

static string CheckedString()
{
    Console.WriteLine("Please enter a username: ");
    string username = Console.ReadLine();
    Console.WriteLine();
    return UsernameCheck(username);
}

static async Task DeleteUserHistory(HttpClient client)
{
    Model result;
    HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/deletehistory/{CheckedString()}");
    result = await GetResult<Model>(message);
    Console.WriteLine(result?.Message);

}

static async Task DeleteUsernameAndHistory(HttpClient client)
{
    Model result;
    var checkedUsername = CheckedString();
    HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/deleteusernandhistory/{checkedUsername}");
    result = await GetResult<Model>(message);
    Console.WriteLine(result?.Message);

}

static async Task<Model> Start(HttpClient client, bool act)
{
    Model result = await ValidateUsername(client, act);
    HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/start/{result?.Username}");
    result = await GetResult<Model>(message);
    Console.WriteLine($"{result?.Message}");
    return result;
}


static int UserInput()
{
    Console.WriteLine("Please input a guess between the range of 1 to 20:");
    var userGuess = Console.ReadLine();
    int num;
    while (!int.TryParse(userGuess, out num))
    {
        Console.WriteLine("Please enter a digit");
        Console.WriteLine("Input your guess: ");
        userGuess = Console.ReadLine();
    }
    return num;
}


static async Task<List<int>> Guess(HttpClient client, Model result)
{
    var tries = result?.Tries;
    int win = 0;
    int lose = 0;

    while (tries >= 0)
    {
        string guess = UserInput().ToString();
        HttpResponseMessage message = await client.GetAsync($"{client.BaseAddress}/{result?.Username}/{result?.Id}/{guess}");
        var newMessage = await GetResult<Model>(message);
        Console.WriteLine(newMessage?.Message);
        if (message.IsSuccessStatusCode)
        { 
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
                return new List<int>() { win, lose };
            }
            else
            {
                tries = updatedTries;
            }
        }
    } 
    return new List<int> { win, lose };
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





