
# Greek Weather Console App

A C# Console Application built with .NET that provides live weather information for cities across Greece using the Open-Meteo APIs.

---

## Overview

The application functions in a simple two-step pipeline:
1. **Geocoding:** Translates user-entered Greek city names into geographic coordinates (latitude and longitude).
2. **Weather Retrieval:** Queries weather servers using those coordinates to fetch current meteorological data.

---

## Features

* **Localized Search:** Specifically configured for Greek locations (`countryCode=GR`).
* **Real-time Data:** Fetches current temperature (∞C), relative humidity (%), wind speed (km/h), and weather condition codes.
* **Greek Weather Descriptions:** Automatically maps WMO numeric weather codes to human-readable Greek descriptions (e.g., Clear sky, Rain, Thunderstorm).
* **Interactive Loop:** Allows users to perform multiple continuous queries until typing `‘≈Àœ”` to exit.
* **Input Validation & Safety:** Ignores blank inputs and safely handles Greek characters and URL formatting.
* **Exception Handling:** Gracefully handles network failures, JSON parsing errors, and invalid city entries without crashing.

---

## Prerequisites

Before running this project, ensure you have the following installed:
* [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or higher
* An IDE such as Visual Studio 2022, Visual Studio Code, or Rider

---

## Getting Started

### 1. Clone the Repository

Open your terminal or command prompt and run:
```bash
git clone https://github.com/p-kyriakos/Greek_Weather_Console_App
```

### 2. Navigate to the Project Folder

Change your current directory to the cloned repository folder:
```bash
cd Greek_Weather_Console_App
```

### 3. Restore Dependencies

Download and restore all the required NuGet packages for the project:
```bash
dotnet restore
```

### 4. Build the Application

Compile the project to ensure there are no compilation errors:
```bash
dotnet build
```

### 5. Run the Application

Launch the console application directly from your terminal:
```bash
dotnet run --project Weather_Console_App
```

---

## How to Use

1. Once the application starts, type the name of any Greek city (e.g., ¡ËﬁÌ·, »ÂÛÛ·ÎÔÌﬂÍÁ) and press Enter to see the live weather data.
2. To exit the interactive loop and close the application, simply type ‘≈Àœ”.

