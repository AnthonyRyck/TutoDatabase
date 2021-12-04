public static class ConsoleExtension
{
	public static void ToConsoleInfo(this string message)
	{
		ForegroundColor = ConsoleColor.White;
		WriteLine(message);
	}

	public static void ToConsoleResult(this string message)
	{
		ForegroundColor = ConsoleColor.Green;
		WriteLine(message);
	}
}