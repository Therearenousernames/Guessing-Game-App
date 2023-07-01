using GuessGameAPI;
using GuessGameAPI.Controllers;
using GuessGameAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;


namespace DatabaseControllerTests
{
    

    [TestClass]
    public class DataBaseImplementationTests
    {
        DataController dataController = new();

        [DataTestMethod]
        [DataRow("k")]
        public void RegisteredUserLoginReturnsModelWithWelcomeBackMessage(string input)
        {
            IActionResult actionResult = dataController.DatabaseSearch(input);
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Model? model = (Model)okObjectResult?.Value;

            model?.Message.Should().Be($"Welcome back {input}! May the forces be with you...\n");
            model?.Message.Should().BeOfType<string>();

        }



        [DataTestMethod]
        [DataRow("Ethan")]
        public void NewUserRegisterReturnsModelWithMessageOfUsernameBeingRegister(string input)
        {
           
            IActionResult actionResult = dataController.GetUserName("Ethan");
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Model? model = (Model)okObjectResult?.Value;

            model?.Message.Should().Be($"Username {input} has been registered.");
            model?.Message.Should().BeOfType<string>();

        }


        [DataTestMethod]
        [DataRow("Ethan")]
        [DataRow("k")]
        public void NewUserRegisterExistingName(string input)
        {
            
            IActionResult actionResult = dataController.GetUserName(input);
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Model? model = (Model?)okObjectResult?.Value;

            model?.Message.Should().Be($"{input} already exists. Please try again.");
        }

        [DataTestMethod]
        [DataRow("Ethan")]
        [DataRow("k")] 
        public void StartingTheGame(string input)
        {
            IActionResult actionResult = dataController.Get(input);
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Model? model = (Model?)okObjectResult?.Value;
            
            int? id = model?.Id;

            model.Message.Should().Be($"A user id of {id} has been allocated to you and random number has been generated. You have 5 tries left.");
            model?.Id.Should().Be(id);
            model?.Tries.Should().Be(5);
        }


        [DataTestMethod]
        [DataRow("k")]
        [DataRow("Ethan")]
        public void GetUserHistory(string input)
        {
            IActionResult actionResult = dataController.GetUserHistory(input);
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Model? model = (Model?)okObjectResult?.Value;

            model.GameStates.Should().BeOfType<List<GameState>>();

        }
    }
}