#!/bin/bash

# Function to build and copy for Linux
build_for_linux() {
    cargo build --release --manifest-path ../../rust/pallas-dotnet-rs/Cargo.toml
    cp ../../rust/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so "./lib/libpallas_dotnet_rs.so"
    rnet-gen ../../rust/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.so > "./PallasDotnetWrapper.cs"
}

# Function to build and copy for macOS
build_for_macos() {
    cargo build --release --manifest-path ../../rust/pallas-dotnet-rs/Cargo.toml
    cp ../../rust/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib "./lib/libpallas_dotnet_rs.dylib"
    rnet-gen ../../rust/pallas-dotnet-rs/target/release/libpallas_dotnet_rs.dylib > "./PallasDotnetWrapper.cs"
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
