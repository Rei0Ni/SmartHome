# SmartHome
---

The **Smart Home System** is a comprehensive platform for managing, monitoring, and controlling smart devices within a home or business environment. It provides role-based access to users, enables device control, and maintains detailed logs for accountability and analysis.

---

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [System Architecture](#system-architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [Contact](#contact)

---

## Features

- **Role-based Access:**
  - **Admin/Technician:** Add, update, and remove devices, view logs, and manage system settings.
  - **Normal User:** Control devices and view statuses.
  - **Guest:** Limited access to assigned devices and areas.

- **Device Management:**
  - Add, edit, and remove devices with custom parameters.
  - Supports various device types, including lamps, fans, AC units, and temperature sensors.

- **Real-time Monitoring:**
  - Track the current state of all devices.
  - Log all device state changes and commands.

- **Logs Management:**
  - Comprehensive logs for actions, errors, and warnings.
  - Filters for efficient log search and review.

- **System Configuration:**
  - Manage system preferences, including themes, time zones, and log configurations.

---

## Technologies Used

- **Frontend:** Blazor Hybrid  
- **Backend:** .NET Core  
- **Database:** MongoDB  
- **Communication Protocol:** Serial Communication (Arduino/ESP32)  
- **Collaboration Tool:** Jira  

---

## System Architecture

The system consists of the following key components:

1. **Frontend:** Provides an intuitive user interface for interacting with the system.  
2. **Backend:** Handles business logic, user management, and device control.  
3. **Database:** Stores user profiles, device details, logs, and system configurations.  
4. **Control Board:** Communicates with devices to execute commands.  

---

## Installation

### Prerequisites

- **Backend:**
  - .NET Core SDK
  - MongoDB installed and running
- **Frontend:**
  - Blazor Hybrid environment
- **Control Board:**
  - Arduino/ESP32 board
  - Serial communication setup

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/Rei0Ni/SmartHome.git
   cd SmartHome
   ```
2. Set up the backend:

    - Navigate to the backend directory.
    - Install dependencies:
      ```bash
      dotnet restore
      ```

    - Run the backend:
      ```bash
      dotnet run
      ```

## Usage
  1. Login: Access the system using credentials (role-based access).
  2. Add Devices: Admin/Technician can register new devices in specific areas.
  3. Control Devices: Users can issue commands to devices via the UI.
  4. Monitor Logs: View logs to track actions, errors, and warnings.
  5. Configure System: Update preferences such as themes, time zones, and log settings.

## Contributing

We welcome contributions! To contribute:

  1. Fork the repository.
  2. Create a new branch for your feature or bug fix.
  3. Commit your changes with clear messages.
  4. Submit a pull request.

## Contact

For questions or support, please contact:

  Abdelrahman Hamdy Hashim <br>
    - Email: [abdelrahman.hamdy.hashim@gmail.com](mailto:abdelrahman.hamdy.hashim@gmail.com) <br>
    - GitHub: [Rei0Ni](https://github.com/Rei0Ni) <br>
    - LinkedIn: [Profile](https://www.linkedin.com/in/abdelrahman-hamdy-hashim) <br>
