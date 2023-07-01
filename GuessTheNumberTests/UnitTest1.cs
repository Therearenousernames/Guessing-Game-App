using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Services;
using NSubstitute;
using Xunit;
using GameCore;
using FluentAssertions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GuessTheNumberTests
{
    [TestClass]
    public class GuessTests
    {
        readonly int random = 10;

        // Declare method for standard output
        static string CapturedStdOut(Action callback)
        {
            TextWriter textWriter = new StringWriter();
            using var newStdOut = new StringWriter();
            Console.SetOut(newStdOut);

            callback.Invoke();
            var capturedOutput = newStdOut.ToString();

            Console.SetOut(textWriter);
            return capturedOutput;
        }


        [DataTestMethod]
        [DataRow("20")]
        [DataRow("19")]
        [DataRow("18")]
        public void ValidateWhenUserInputIsTooHigh(string input)
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns(input);
            Play play = new(userInputService);

            // Running the method
            string message = play.Validate(int.Parse(input), random);

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.Equal($"\n{input} is too high\nRemaining tries: {play.limit}\n", message);
            message.Should().Be($"\n{input} is too high\nRemaining tries: {play.limit}\n");
        }


        [DataTestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("3")]
        public void ValidateWhenUserInputIsTooLow(string input)
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns(input);
            Play play = new(userInputService);

            // Running the method
            string message = play.Validate(int.Parse(input), random);

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.Equal($"\n{input} is too low\nRemaining tries: {play.limit}\n", message);
            message.Should().Be($"\n{input} is too low\nRemaining tries: {play.limit}\n");
        }

        [DataTestMethod]
        [DataRow("10")]
        public void ValidateWhenUserInputIsCorrect(string input)
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns(input);
            Play play = new(userInputService);

            // Running the method
            string message = play.Validate(int.Parse(input), random);

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.Equal($"HOOOOOORRRAAAAYYYYYYY...Well done. Your guess of {input} is correct", message);
            message.Should().Be($"HOOOOOORRRAAAAYYYYYYY...Well done. Your guess of {input} is correct");
        }

        [TestMethod]
        public void GenerateRandomReturnsTypeInt()
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns("12");
            Play play = new(userInputService);

            // Running the methods
            int generatedNumber = play.GenerateRandom();
            bool isNumerical = Microsoft.VisualBasic.Information.IsNumeric(generatedNumber);

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.True(isNumerical);
            Assert.AreNotEqual(generatedNumber, "12");
            isNumerical.Should().BeTrue();
            generatedNumber.GetType().Should().Be(typeof(int));
            
        }

        [TestMethod]
        public void GenerateRandomReturnsAValueThatIsBetweenSpecifiedRange()
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns("5");
            Play play = new(userInputService);

            // Running the methods
            int generatedNumber = play.GenerateRandom();

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.True(0 < generatedNumber && generatedNumber < 21);
            generatedNumber.Should().BeInRange(0, 21);

        }

        [DataTestMethod]
        [DataRow("1")]
        [DataRow("20")]
        [DataRow("13")]
        public void GetUserInputUserEntersAValidUserInput(string input)
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadLine().Returns(input);
            Play play = new(userInputService);

            // Running the method
            int actual = play.GetUserInput();

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.Equal(int.Parse(input), actual);
            actual.Should().Be(int.Parse(input));
        }


        [TestMethod]
        public void ReplayGameStandardOutput()
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadKey().Returns(new ConsoleKeyInfo('Y', ConsoleKey.Y, false, false, false));
            Play play = new(userInputService);

            // Running the methods
            var capturedStdOut = CapturedStdOut(() =>
            {
                play.ReplayGame().Should().BeTrue();

            });

            // Asserting with Xunit & FluentAssertions
            Xunit.Assert.Equal("Do you want to play again (y/n)? ", capturedStdOut);
            capturedStdOut.Should().Be("Do you want to play again (y/n)? ");
            
        }

        [TestMethod]
        public void ReplayGameUserInputYReturnsTrue()
        {
            // Arrange data using NSubstitute
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadKey().Returns(new ConsoleKeyInfo('Y', ConsoleKey.Y, false, false, false));
            Play play = new(userInputService);

            // Running the methods
            bool actual = play.ReplayGame();

            // Asserting
            Xunit.Assert.True(actual);
            actual.Should().BeTrue();
        }

        [TestMethod]
        public void ReplayGameUserInputNReturnsFalse()
        {
            // Arrange data
            var userInputService = Substitute.For<IUserInput>();
            userInputService.ReadKey().Returns(new ConsoleKeyInfo('N', ConsoleKey.N, false, false, false));
            Play play = new(userInputService);

            // Running the methods
            bool actual = play.ReplayGame();

            // Asserting
            Xunit.Assert.False(actual);
            actual.Should().BeFalse();
        }
    }
}