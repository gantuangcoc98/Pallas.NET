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
var result = await nodeClient.GetUtxoByAddressCborAsync("addr_test1qr302ykx22zmhecwpml2mnjyysfghdl4m55ekj95a2jhwguaxpxg84qj6xxsv5kag39zm2q5pwp3uf6nx6fjhjrlsd7s7v0v26");
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
    table.AddRow("[yellow]Time[/]",  DateTime.Now.ToString("HH:mm:ss.fff"));

    // Render the table to the console
    AnsiConsole.Write(table);

    foreach(TransactionBody tx in nextResponse.Block.TransactionBodies
        .Where(tx => tx.Id.ToHex() == "9f6a17fbf8e6c4f1283757744c2a66985f5407fee6ba831aa7b12d5feef1b824")
        .Select(tx => tx)
    )
    {
        Console.WriteLine("---------------------------");
        Console.WriteLine(Convert.ToHexString(tx.Raw));
        Console.WriteLine(tx.MetaData?.ToString() ?? "null");
        Console.WriteLine("---------------------------");
    }
    
};

await nodeClient.StartChainSyncAsync(new Point(
    118779478,
    new Hash("574f5bd419cc46614cc57edddc421a0f33fc0010c6c846eac42d68dc46c1078e")
));

while (true)
{
    await Task.Delay(1000);
}