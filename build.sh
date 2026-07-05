#!/bin/bash
# Build script for UNI-T 161E Multimeter Application
# This script builds the project and creates an executable

echo ""
echo "========================================"
echo "UNI-T 161E Multimeter - Build Script"
echo "========================================"
echo ""

# Check if dotnet CLI is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed or not in PATH"
    echo "Please install .NET 6.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Display dotnet version
echo "Detected .NET SDK:"
dotnet --version
echo ""

# Clean previous build
echo "[1/3] Cleaning previous build..."
dotnet clean -c Release > /dev/null 2>&1
rm -rf bin/Release > /dev/null 2>&1
echo "Done."
echo ""

# Restore dependencies
echo "[2/3] Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "Error: Failed to restore packages"
    exit 1
fi
echo "Done."
echo ""

# Build the project
echo "[3/3] Building Release executable..."
dotnet publish -c Release -o "bin/Release/publish" --self-contained false
if [ $? -ne 0 ]; then
    echo "Error: Build failed"
    exit 1
fi
echo "Done."
echo ""

# Check if executable was created
if [ -f "bin/Release/publish/UniT161E.exe" ]; then
    echo ""
    echo "========================================"
    echo "Build Successful!"
    echo "========================================"
    echo ""
    echo "Executable created at:"
    echo "bin/Release/publish/UniT161E.exe"
    echo ""
    echo "File size: $(du -h bin/Release/publish/UniT161E.exe | cut -f1)"
    echo ""
    echo "Note: .NET 6.0 Runtime must be installed on target systems"
    echo "Download from: https://dotnet.microsoft.com/download/dotnet/6.0"
    echo ""
else
    echo "Error: Executable not found after build"
    exit 1
fi
