#!/bin/bash

echo $(pwd)

# Function to build and copy for Linux
build_for_linux() {
    cargo build --release --manifest-path ./src/pallas-dotnet-rs/Cargo.toml
    cp ./src/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so "./src/pallas-dotnet/libpallas_dotnet_rs.so"
    rnet-gen ./src/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so > "./src/pallas-dotnet/PallasDotnetWrapper.cs"
}

# Function to build and copy for macOS
build_for_macos() {
    cargo build --release --manifest-path ./src/pallas-dotnet-rs/Cargo.toml
    cp ./src/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib "./src/pallas-dotnet/libpallas_dotnet_rs.dylib"
    rnet-gen ../pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib > "./src/pallas-dotnet/PallasDotnetWrapper.cs"
}

# Check the operating system
OS="`uname`"
case $OS in
  'Linux')
    # Linux-specific commands
    build_for_linux
    ;;
  'Darwin')
    # macOS-specific commands
    build_for_macos
    ;;
  *) ;;
esac
