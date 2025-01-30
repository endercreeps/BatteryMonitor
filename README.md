
# BatteryMonitorApp 📊🔋

BatteryMonitorApp is a lightweight Windows system tray application that monitors battery level and network connection. It triggers an API request when the battery goes outside the defined range [45;80] and ensures the device is connected to an authorized network before activation. The app runs in the background with minimal UI (system tray icon only).

## Features ✨
- **Battery Monitoring**: Automatically keeps track of your battery level.
- **Network Connection Check**: Ensures the device is connected to an authorized network.
- **API Trigger**: Sends an API request when the battery level is outside the defined range [45;80].
- **System Tray Icon**: Runs silently in the background with a system tray icon.

## Getting Started 🚀

### Prerequisites
- Windows operating system
- .NET Framework (version required by the application)

### Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/endercreeps/BatteryMonitor.git
    ```
2. Navigate to the project directory:
    ```sh
    cd BatteryMonitor
    ```
3. Open the solution file in Visual Studio and build the project.
4. Run the application.

### Usage
- The application will start minimized to the system tray.
- Right-click on the system tray icon to access options and settings.

## Configuration ⚙️
- Define your battery range and authorized networks in the settings.

## Contributing 🤝
Contributions are welcome! Please open an issue or submit a pull request.

## License 📄
This project is licensed under the MIT License.

## Contact 📧
If you have any questions or feedback, feel free to reach out.

