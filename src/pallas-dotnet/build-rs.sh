#!/bin/bash

# Function to build and copy for Linux
build_for_linux() {
    cargo build --release --manifest-path ../pallas-dotnet-rs/Cargo.toml
    cp ../pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so "./libpallas_dotnet_rs.so"
    rnet-gen ../pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so > "./PallasDotnetWrapper.cs"
}

# Function to build and copy for macOS
build_for_macos() {
    cargo build --release --manifest-path ../pallas-dotnet-rs/Cargo.toml
    cp ../pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib "./libpallas_dotnet_rs.dylib"
    rnet-gen ../pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib > "./PallasDotnetWrapper.cs"
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
