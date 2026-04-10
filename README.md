# WayBeyond

A Windows desktop application built with WPF and .NET 6 for processing and managing client debt collection files. WayBeyond automates the ingestion, transformation, and reporting of debtor account data from multiple healthcare clients, with support for configurable file formats, remote file transfers, and specialized reporting workflows.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Configuration](#configuration)
  - [Setting Up a Client File Format](#setting-up-a-client-file-format)
  - [Setting Up a Drop Format](#setting-up-a-drop-format)
  - [Remote Connections](#remote-connections)
- [Usage](#usage)
  - [Local Client Loads](#local-client-loads)
  - [Epic Client Loads](#epic-client-loads)
  - [Reporting](#reporting)
- [Data Models](#data-models)
- [License](#license)

---

## Overview

WayBeyond is designed for debt collection agencies that receive account referral files from healthcare clients. The application:

- Reads and parses client files in various formats (XLS, XLSX, CSV)
- Maps file columns to debtor record fields using configurable format definitions
- Processes accounts for multiple Epic EHR clients (NorLea, RGH, AANMA, etc.)
- Generates drop files for downstream collection workflows
- Produces reporting exports including Transunion lists, bad debt, charity, PIF, cancels, and inventory reports
- Manages remote file connections (SFTP/FTP) for automated file retrieval

---

## Features

- **Configurable Client File Formats** — Define column mappings per client without code changes
- **Configurable Drop Formats** — Define output file structure for each client
- **Epic EHR Integration** — Dedicated processing pipelines for Epic placement files
- **Local & Remote File Loading** — Pick up files locally or pull from remote connections
- **Bad Email Tracking** — Maintain a list of known bad email domains and addresses
- **Processed File Batch History** — Track which file batches have been processed
- **Texas Tech Reporting** — Specialized reporting module for Texas-region clients
- **Encrypted Configuration** — Sensitive connection settings are encrypted at rest
- **MVVM Architecture** — Clean separation of UI and business logic using WPF + MVVM pattern

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | WPF (.NET 6, Windows) |
| Architecture | MVVM |
| ORM | Entity Framework Core 7 |
| Database | SQLite (with SQLCipher encryption) |
| Excel Processing | Excel Interop / ExcelService |
| Remote Transfer | Custom SFTP/FTP Transfer service |
| DI Container | Manual composition via `ContainerHelper` |

---

## Project Structure

```
WayBeyond/
├── WayBeyond.sln
├── ClientFormat.md                  # Documentation for setting up client file formats
├── WayBeyond.Data/                  # Data layer — EF Core models and DbContext
│   ├── Context/
│   │   └── BeyondContext.cs         # SQLite DbContext
│   └── Models/                      # Entity models (Account, Debtor, Client, etc.)
└── WayBeyond.UX/                    # WPF UI layer
    ├── MainWindow.xaml / ViewModel  # Shell and navigation
    ├── File/
    │   ├── Drops/                   # Drop format management (input & output formats)
    │   ├── Email/                   # Bad email address management
    │   ├── Location/                # File location management
    │   ├── Maintenance/             # Client maintenance
    │   ├── Remote/                  # Remote connection management
    │   └── Settings/                # Application settings
    ├── Processing/
    │   ├── EpicLoads/               # Epic EHR client file processing
    │   └── LocalLoads/              # Local/manual file processing
    ├── Reporting/
    │   ├── ProcessedFilesView       # Processed batch history
    │   └── TexasTech/               # Texas Tech reporting module
    ├── Services/                    # Business logic services
    │   ├── BeyondRepository.cs      # Main data access repository
    │   ├── ClientProcess.cs         # Core client file processing logic
    │   ├── NorLeaClientProcess.cs   # NorLea-specific Epic processing
    │   ├── RghClientProcess.cs      # RGH-specific Epic processing
    │   ├── AanmaClientProcess.cs    # AANMA-specific Epic processing
    │   ├── Transfer.cs              # Remote file transfer service
    │   ├── ExcelService.cs          # Excel file reading service
    │   ├── DropFileWrite.cs         # Drop file output writer
    │   ├── TexasTechService.cs      # Texas Tech reporting service
    │   └── ConfigurationEncryptionService.cs  # Settings encryption
    └── Helpers/                     # MVVM helpers (BindableBase, RelayCommand, etc.)
```

---

## Getting Started

### Prerequisites

- Windows 10 or later
- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) with the **.NET desktop development** workload
- Microsoft Excel installed (required for Excel file processing via Interop)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/jsjoblom79/waybeyondnew.git
   cd waybeyondnew
   ```

2. **Open the solution:**
   ```
   WayBeyond.sln
   ```

3. **Restore NuGet packages:**
   Visual Studio will restore packages automatically on build. Or via CLI:
   ```bash
   dotnet restore
   ```

4. **Build the solution:**
   ```bash
   dotnet build
   ```

5. **Run the application:**
   Set `WayBeyond.UX` as the startup project and press **F5**, or:
   ```bash
   dotnet run --project WayBeyond.UX
   ```

The application will create and seed its SQLite database on first run using the configured connection string in `App.config`.

---

## Configuration

### Setting Up a Client File Format

Client file formats define how the application reads an incoming client referral file. Navigate to **File → Drops → Client File Format**.

1. Click **+ File Format** to add a new format.
2. Fill in the format header fields:
   - **File Format Name** *(required)*
   - **File Start Line** — the row number where account records begin
   - **Client Debtor Number Column** *(required)* — a column present on every record, used to count accounts
3. Click **Add** to save the format header to the database.
4. Click the **edit (pencil)** icon on the format you just created.
5. Add detail fields on the right side of the screen:
   - **Field** — the target debtor record field (from a predefined list)
   - **File Column** — the column letter (e.g., `A`, `B`, `C`) — always use alphabetic characters even for CSV files
   - **Column Type** — `string`, `int`, `double`, `datetime`, or `long`
   - **Special Case** — used to split combined fields: `split1` (before comma), `split2` (after comma), etc.
6. Click **Add File Format Detail** for each field.

> **Note:** The application opens XLS, XLSX, and CSV files via Excel Interop. Always use alphabetic column identifiers.

### Setting Up a Drop Format

Drop formats define the structure of the output file sent downstream after processing. Configuration follows a similar pattern to file formats and is accessible from the same **File → Drops** menu.

### Remote Connections

Navigate to **File → Remote Connections** to configure SFTP or FTP locations. Connection credentials are encrypted at rest using the `ConfigurationEncryptionService`. Once a connection is saved, associate it with a **File Location** under **File → File Locations**.

---

## Usage

### Local Client Loads

1. Navigate to the **Local Loads** section.
2. Select the incoming file from the file picker.
3. Select the matching client from the client list.
4. Click **Process** to parse the file and load records into the database.
5. Use **Clear** to reset selections.

### Epic Client Loads

1. Navigate to the **Epic Loads** section.
2. The view auto-loads files found at configured Epic placement file locations.
3. Click **Process** to run all detected Epic client files through their respective processing pipelines.

Supported Epic clients are identified by filename patterns:
- `norlea` → NorLea Community Hospital
- `rghosp` → Roswell General Hospital
- `anesphesia` → AANMA Anesthesia
- `faithcommunity` → Faith Community (in progress)

### Reporting

Navigate to the **Reporting** section to generate and export the following:

| Report | Description |
|---|---|
| Processed Files | History of all processed file batches |
| Inventory | Active accounts with no payment activity |
| Bad Debt | Accounts flagged as over 200% FPL |
| Charity | Accounts flagged as 0–200% FPL |
| Cancels | Accounts closed with CCR status in the last month |
| PIF | Paid-in-full accounts closed in the last month |
| Transunion | Accounts eligible for credit reporting |

The **Texas Tech** reporting module provides a specialized workflow for Texas-region clients, including TransUnion export preparation, expired account tracking, and Scode updates from TUResult data.

---

## Data Models

Key entities managed by the application:

| Model | Description |
|---|---|
| `Client` | Debt collection client configuration |
| `FileFormat` / `FileFormatDetail` | Incoming file column mapping definitions |
| `DropFormat` / `DropFormatDetail` | Output file structure definitions |
| `FileLocation` | Watched directories (local or remote) |
| `RemoteConnection` | SFTP/FTP connection settings |
| `Debtor` | Debtor contact and account information |
| `Account` | Individual account referral record |
| `TexasDebtor` / `Patient` | Texas Tech-specific debtor and patient records |
| `TUResult` | TransUnion scoring results |
| `ProcessedFileBatch` | Audit log of processed file batches |
| `BadEmailAddresses` | Known invalid email addresses/domains |
| `Setting` | Application-level configuration key/value pairs |

---

## License

This project is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for details.
