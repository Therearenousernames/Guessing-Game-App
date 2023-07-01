using DependencyInjectionExample;
using System;

public class MessageWriter : IMessageWriter
{
	public void Write(string message)
	{
		Console.WriteLine($"MessageWrite.Write(message: \"{message}\"");
	}
}


public class Worker : BackgroundService
{
	private readonly MessageWriter _writer = new MessageWriter();

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while(!stoppingToken.IsCancellationRequested)
		{
			_.Write($"Worker running at: {DateTimeOffSet.Now}");
			await Task.Delay(1000, stoppingToken);
		}
	}
}


namespace DependencyInjectionExample;
public interface IMessageWriter
{
	void Write(string message);
}

