using System.Text.Json;

namespace PallasDotnet.Models;

public record TransactionBody(
    Hash Id,
    ulong Index,
    IEnumerable<TransactionInput> Inputs,
    IEnumerable<TransactionOutput> Outputs,
    Dictionary<Hash,Dictionary<Hash,long>> Mint,
    JsonElement? MetaData,
    byte[] Raw
);