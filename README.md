# Pallas.DotNet

Pallas.DotNet is a .NET wrapper around the [Pallas Rust library](https://github.com/txpipe/pallas), which provides building blocks for the Cardano blockchain ecosystem. This library allows .NET developers to access the functionality of Pallas in a seamless and straightforward manner.

## Introduction

Pallas.DotNet is an early version of the library, designed to bridge the gap between .NET and the Cardano ecosystem by wrapping the powerful Pallas Rust library. Pallas itself is a collection of modules that re-implement common Ouroboros/Cardano logic in native Rust, making it a foundational layer for building higher-level applications like explorers, wallets, and potentially even a full node in the future.

## Features

While Pallas.DotNet is still in its infancy, the current version includes the following features:

- ChainSync
- GetTip
- Query UtxOByAddress

More features and modules will be wrapped in future updates as the library evolves.

## Getting Started

### Installation

To install Pallas.DotNet, you can use the following NuGet command:

```sh
dotnet add package pallas-dotnet --version 0.1.0
```

### Usage

Here's a simple example of how to use Pallas.DotNet to follow the Cardano blockchain:

```csharp
var nodeClient = new NodeClient();
var tip = await nodeClient.ConnectAsync("/tmp/node.socket", NetworkMagic.PREVIEW);

await nodeClient.StartChainSyncAsync(new Point(
    54131816,
    new Hash("34c65aba4b299113a488b74e2efe3a3dd272d25b470d25f374b2c693d4386535")
));

```
