# UNI-T E61+ Multimeter Display - Quick Start Guide

## Overview

This is a professional Windows desktop application for real-time monitoring and logging of UNI-T E61+ digital multimeter measurements via USB serial connection.

## Key Features at a Glance

- **Real-time Display**: Large, easy-to-read measurement display
- **Data Logging**: Record measurements with automatic timestamps
- **Statistics**: Track Min, Max, and Average values
- **CSV Export**: Save data for analysis in Excel/other tools
- **Auto-reconnect**: Automatically recover from connection drops
- **Professional UI**: Dark theme, responsive design
- **Multi-baud**: Support for 9600-115200 baud rates

## Getting Started

### First Time Setup

1. **Install .NET 6.0 Runtime** (if not already installed)
   - Download from: https://dotnet.microsoft.com/download/dotnet/6.0
   - Choose "Runtime" (not SDK)

2. **Connect Your Multimeter**
   - Plug UNI-T E61+ into USB port
   - Wait for device recognition

3. **Launch Application**
   - Run `MultimeterDisplay.exe`
   - Application will auto-detect COM ports

4. **Configure Connection**
   - Select COM port from dropdown
   - Verify baud rate (default: 19200)
   - Click "Connect"

### First Measurement

1. Turn on your multimeter
2. Select measurement mode (Voltage, Current, Resistance, etc.)
3. Application will display readings automatically
4. Check that values match your multimeter display

## User Guide

### Connection Panel (Right Side)

**COM Port Selection**
- Lists all available serial ports
- Auto-populates when multimeter is connected
- Select the correct port for your device

**Baud Rate**
- Default: 19200 (recommended)
- Change only if connection fails
- Must match multimeter's output rate

**Refresh Ports**
- Updates available COM ports
- Use if multimeter not appearing in list

**Connect Button**
- Establishes serial connection
- Changes to green when connected
- Disables COM port and baud rate options

**Disconnect Button**
- Closes serial connection
- Clears current readings
- Re-enables configuration options

**Status Text**
- Shows connection state
- Displays error messages
- Color-coded (Green=OK, Red=Error, Orange=Warning)

### Data Display (Left Side)

**Current Reading**
- Large display of latest measurement
- Updates 2-4 times per second
- Shows value, unit, and mode

**Statistics Panel**
- **MIN**: Lowest value recorded (since last reset)
- **AVG**: Average of all values
- **MAX**: Highest value recorded
- **Samples**: Total number of measurements

**Recent Readings**
- Last 10 measurements displayed
- Includes timestamp and full data
- Scrollable for history viewing

### Data Logging Panel

**Start Recording**
- Begin data collection session
- Resets statistics and clears previous data
- Records all measurements with timestamps

**Stop Recording**
- Pause data collection
- Keeps current data in memory
- Can resume by clicking Start again

**Export to CSV**
- Saves all logged data to CSV file
- File saved to Desktop
- Named: `multimeter_YYYY-MM-DD_HH-mm-ss.csv`

**Clear Data**
- Removes all logged measurements
- Resets statistics (Min/Max/Avg)
- Keeps connection active

### Settings Panel

**Auto-reconnect** (Checkbox)
- If enabled: app attempts reconnection after disconnect
- If disabled: connection must be manually re-established
- Recommended: Keep enabled

**Auto-save Every 100 Samples** (Checkbox)
- Automatically exports data to CSV periodically
- Prevents data loss in case of crash
- Saves to Desktop with auto-generated filename

**Display Refresh Rate** (ms)
- Controls UI update frequency
- Lower = more responsive, higher CPU usage
- Default: 500ms (recommended)
- Range: 100-2000ms

## Common Tasks

### Log 1 Hour of Voltage Measurements

1. Connect multimeter to USB
2. Set multimeter to DC Voltage mode
3. Click "Connect" in application
4. Click "Start Recording"
5. Leave running for 1 hour
6. Click "Stop Recording"
7. Click "Export to CSV"
8. File saved to Desktop - open in Excel

### Find Peak Current in a Circuit

1. Set multimeter to DC Current (mA) mode
2. Connect application
3. Start recording
4. Activate circuit or test sequence
5. Stop recording
6. Review "MAX" value in Statistics panel
7. Export to CSV for detailed analysis

### Troubleshoot Serial Connection

1. Disconnect multimeter
2. Click "Refresh Ports" - should disappear
3. Reconnect multimeter
4. Click "Refresh Ports" - should reappear
5. If still not visible, check Device Manager
6. Try different USB port or cable

### Export Data to Excel

1. Click "Export to CSV"
2. Navigate to Desktop
3. Find `multimeter_*.csv` file
4. Open with Excel
5. Data ready for analysis/graphing

## Data Export Format

The CSV file contains these columns:

```
Timestamp,Value,Unit,Mode,Raw Data
2026-07-05 14:30:45.123,12.345,mV,DC,+12.3450 mV DC
2026-07-05 14:30:45.623,12.401,mV,DC,+12.4010 mV DC
```

- **Timestamp**: When measurement was taken
- **Value**: Numeric value only
- **Unit**: V, mA, Ω, nF, Hz, °C, etc.
- **Mode**: AC or DC
- **Raw Data**: Complete original packet from multimeter

## Tips & Tricks

### Reduce CPU Usage
- Increase Display Refresh Rate to 1000ms
- Disable Auto-save
- Close unnecessary applications

### Improve Serial Stability
- Use high-quality USB cable
- Avoid USB hubs - connect directly to computer
- Keep USB port clean
- Ensure multimeter has fresh batteries

### Maximize Recording Duration
- Enable Auto-save to prevent data loss
- Record to faster drive (SSD recommended)
- Monitor memory usage if recording > 100,000 samples
- Export and clear periodically

### Accurate Measurements
- Allow multimeter to warm up first
- Use appropriate measurement range
- Check probe connections before measuring
- Refer to multimeter manual for mode selection

## Troubleshooting

### "No COM ports found"
- **Solution 1**: Check Device Manager (Win+R → devmgmt.msc)
- **Solution 2**: Install CH340/PL2303 drivers if needed
- **Solution 3**: Try different USB port
- **Solution 4**: Restart computer

### Connection drops frequently
- **Solution 1**: Use shorter, higher-quality USB cable
- **Solution 2**: Connect directly to motherboard USB, not hub
- **Solution 3**: Try different baud rate
- **Solution 4**: Update multimeter drivers

### No readings updating
- **Solution 1**: Verify multimeter is turned on
- **Solution 2**: Check measurement mode on multimeter
- **Solution 3**: Verify probes are connected
- **Solution 4**: Try "Disconnect" then "Connect" again

### Application crashes
- **Solution 1**: Update to latest .NET 6.0 runtime
- **Solution 2**: Restart computer
- **Solution 3**: Check Windows event log
- **Solution 4**: Reinstall application

### High CPU usage
- **Solution 1**: Increase Display Refresh Rate
- **Solution 2**: Disable Auto-save feature
- **Solution 3**: Close other applications
- **Solution 4**: Check for USB driver issues

## Keyboard Shortcuts

- **F5**: Refresh COM ports
- **Ctrl+C**: Copy current reading to clipboard
- **Ctrl+E**: Export data
- **Ctrl+Q**: Quit application

## Performance Tips

**For Long Recording Sessions (> 1 hour)**
- Enable Auto-save (every 100 samples)
- Set Display Refresh to 1000ms or higher
- Monitor available disk space
- Periodically export and clear data

**For High-Frequency Readings**
- Reduce Display Refresh Rate to 200-300ms
- Disable unnecessary UI elements
- Close background applications
- Use SSD for data storage

**For Stable Connection**
- Use powered USB hub if connecting multiple devices
- Keep USB cables away from high-voltage lines
- Ensure grounding is proper
- Use ferrite clamps on cable if needed

## Advanced Features

### Auto-Export Workflow
1. Enable "Auto-save every 100 samples"
2. Files automatically saved to Desktop
3. Each export creates new timestamped file
4. Useful for long-term unattended monitoring

### Statistics Tracking
- Statistics reset when "Start Recording" is clicked
- Represents only the current recording session
- Average calculated as mean of all samples
- Min/Max represent extreme values in session

### Recent Readings Display
- Shows last 10 measurements with full precision
- Helpful for spotting value trends
- Timestamp shows exact collection moment
- List scrolls automatically with new data

## File Locations

- **Application**: `MultimeterDisplay.exe`
- **Exported Data**: Desktop folder (default)
- **Configuration**: Application directory (if added in future)
- **Logs**: Application directory (if enabled)

## Specifications

- **Minimum RAM**: 100MB
- **Minimum Disk**: 50MB
- **OS**: Windows 10 or later
- **Framework**: .NET 6.0
- **Supported Baud Rates**: 9600, 19200, 38400, 57600, 115200
- **Max Samples in Memory**: ~10,000
- **Max Recording Duration**: Limited by available RAM

## Support & Help

- **GitHub Issues**: Report bugs at project repository
- **Multimeter Manual**: Check UNI-T E61+ documentation
- **Serial Issues**: Refer to Windows Device Manager
- **Data Analysis**: Use Excel or Google Sheets with exported CSV

## Version Information

- **Current Version**: 1.0.0
- **Release Date**: July 5, 2026
- **Framework**: .NET 6.0
- **Status**: Stable

---

**Need Help?** Check the README.md file for additional technical details, or visit the GitHub repository for the latest version.
