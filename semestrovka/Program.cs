namespace semestrovka;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var currentDirectory = "../../../";
        var server = new ServerHandler(currentDirectory);
        await server.Start();
    }
}