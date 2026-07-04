# Multimeter Display - UNI-T UT161E

A Windows desktop application that displays real-time readings from a UNI-T UT161E multimeter via USB serial connection.

## Features

- ✅ Live multimeter reading display
- ✅ Automatic COM port detection
- ✅ Selectable COM port dropdown
- ✅ Real-time serial data parsing
- ✅ Connection status indicator
- ✅ Large, easy-to-read display

## Requirements

- Windows 10 or later
- .NET 6.0 Runtime
- UNI-T UT161E multimeter with USB connection

## Installation

### Option 1: Run from Source

1. Install [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Clone or download this project
3. Open terminal in project directory
4. Run:
   ```bash
   dotnet run
   ```

### Option 2: Build Release

1. Install .NET 6.0 SDK
2. Run:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true
   ```
3. Executable will be in `bin/Release/net6.0-windows/win-x64/publish/MultimeterDisplay.exe`

## Usage

1. **Connect Multimeter**: Plug UNI-T UT161E into USB port
2. **Launch Application**: Run the program
3. **Select COM Port**: Choose the port from dropdown (e.g., COM3)
4. **Click Connect**: Button will turn green when connected
5. **View Reading**: Large display shows real-time multimeter readings

## Serial Protocol

- **Baud Rate**: 19200
- **Data Format**: 8N1 (8 bits, no parity, 1 stop bit)
- **Packet**: 14 bytes (ASCII) + newline
- **Update Rate**: ~2 readings per second

### Packet Format
```
+00.0000 V DC
-014.326 mA AC
+00002.37 Ω
```

## Troubleshooting

### No COM Ports Found
- Ensure multimeter is connected via USB
- Check Device Manager for the COM port
- Try "Refresh Ports" button

### Connection Failed
- Verify correct COM port is selected
- Check multimeter is turned on
- Try disconnecting and reconnecting USB cable
- Restart the application

### No Reading Updates
- Ensure multimeter is in appropriate measurement mode
- Check multimeter display is showing values
- Try different USB cable or port

## Building

Requirements:
- Visual Studio 2022 or later
- .NET 6.0 SDK or later

## License

MIT

## Support

For issues or questions, check the UNI-T UT161E manual or multimeter documentation.