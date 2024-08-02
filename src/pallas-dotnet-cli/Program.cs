using System.Diagnostics;
using PallasDotnet;
using PallasDotnet.Models;
using Spectre.Console;

static double GetCurrentMemoryUsageInMB()
{
    Process currentProcess = Process.GetCurrentProcess();

    // Getting the physical memory usage of the current process in bytes
    long memoryUsed = currentProcess.WorkingSet64;

    // Convert to megabytes for easier reading
    double memoryUsedMb = memoryUsed / 1024.0 / 1024.0;

    return memoryUsedMb;
}

var nodeClient = new NodeClient();
var tip = await nodeClient.ConnectAsync("/tmp/node.socket", NetworkMagic.PREVIEW);


nodeClient.Disconnected += (sender, args) =>
{
    ConsoleHelper.WriteLine($"Disconnected ", ConsoleColor.DarkRed);
};

nodeClient.Reconnected += (sender, args) =>
{
    ConsoleHelper.WriteLine($"Reconnected ", ConsoleColor.DarkGreen);
};

nodeClient.ChainSyncNextResponse += (sender, args) =>
{
    var nextResponse = args.NextResponse;

    if (nextResponse.Action == NextResponseAction.Await)
    {
        ConsoleHelper.WriteLine("Awaiting...", ConsoleColor.DarkGray);
        return;
    }

    var blockHash = nextResponse.Block.Hash.ToHex();

    // Create a table for the block
    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.Title($"[bold yellow]Block: {blockHash}[/]");
    table.AddColumn(new TableColumn("[u]Action[/]").Centered());
    table.AddColumn(new TableColumn($"[u]{nextResponse.Action}[/]").Centered());

    // Add rows to the table for the block details with colors
    table.AddRow("[blue]Block Number[/]", nextResponse.Block.Number.ToString());
    table.AddRow("[blue]Slot[/]", nextResponse.Block.Slot.ToString());
    table.AddRow("[blue]TX Count[/]", nextResponse.Block.TransactionBodies.Count().ToString());

    // Calculate input count, output count, assets count, and total ADA output
    int inputCount = 0, outputCount = 0, assetsCount = 0, datumCount = 0;
    ulong totalADAOutput = 0;

    foreach (var transactionBody in nextResponse.Block.TransactionBodies)
    {
        inputCount += transactionBody.Inputs.Count();
        outputCount += transactionBody.Outputs.Count();
        assetsCount += transactionBody.Outputs.Sum(o => o.Amount.MultiAsset.Count);
        datumCount += transactionBody.Outputs.Count(o => o.Datum != null);
        transactionBody.Outputs.ToList().ForEach(o => totalADAOutput += o.Amount.Coin);
    }

    // Add the calculated data with colors
    table.AddRow("[green]Input Count[/]", inputCount.ToString());
    table.AddRow("[green]Output Count[/]", outputCount.ToString());
    table.AddRow("[green]Assets Count[/]", assetsCount.ToString());
    table.AddRow("[green]Datum Count[/]", datumCount.ToString());

    var totalADAFormatted = (totalADAOutput / 1000000m).ToString("N6") + " ADA";
    table.AddRow("[green]Total ADA Output[/]", totalADAFormatted);
    table.AddRow("[yellow]Memory[/]", GetCurrentMemoryUsageInMB().ToString("N2") + " MB");
    table.AddRow("[yellow]Time[/]", DateTime.Now.ToString("HH:mm:ss.fff"));

    // Render the table to the console
    AnsiConsole.Write(table);

    nextResponse.Block.TransactionBodies
        .Where(tx => tx.Id.ToHex() == "4c4afd467a06e93891221141305c9242ed633e0788c9d8eb10fd66a3117e77b6").ToList()
        .SelectMany(tx => tx.Inputs).ToList()
        .ForEach(input => {
            Console.WriteLine("---------------------------");
            Console.WriteLine(Convert.ToHexString(input.Id.Bytes));
            Console.WriteLine("---------------------------");
        });

};

await nodeClient.StartChainSyncAsync(new Point(
    54131816,
    new Hash("34c65aba4b299113a488b74e2efe3a3dd272d25b470d25f374b2c693d4386535")
));

while (true)
{
    await Task.Delay(1000);
}