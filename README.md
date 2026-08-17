[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/bitwarden-vault-manager)](https://github.com/hmlendea/bitwarden-vault-manager/releases/latest)
[![Build Status](https://github.com/hmlendea/bitwarden-vault-manager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/bitwarden-vault-manager/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/bitwarden-vault-manager)](https://github.com/hmlendea/bitwarden-vault-manager/blob/master/LICENSE)

# Bitwarden Vault Manager

Bitwarden Vault Manager is a console application for analysing exported Bitwarden vault data and identifying weak spots such as reused passwords, weak passwords, missing email-address fields, and items without TOTP.

## 📑 Table of Contents

- [Capabilities](#-capabilities)
- [Use Cases](#-use-cases)
- [Usage](#-usage)
- [Command Reference](#-command-reference)
- [Known Limitations](#-known-limitations)
- [Installation](#-installation)
  - [Manual Installation](#manual-installation)
- [Compatibility](#-compatibility)
- [Privacy and Data](#-privacy-and-data)
- [Development](#-development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Run](#run)
  - [Test](#test)
  - [Coverage](#coverage)
  - [Continuous Integration](#continuous-integration)
  - [Release](#release)
  - [Dependencies](#dependencies)
- [Project Structure](#-project-structure)
  - [Projects and Packages](#projects-and-packages)
  - [Directories](#directories)
- [Architecture](#-architecture)
- [Contributing](#-contributing)
- [Security](#-security)
- [Project Engagement](#-project-engagement)
- [License](#-license)

## ✨ Capabilities

- Load a Bitwarden JSON export from the local filesystem for in-memory analysis.
- List unique usernames, email addresses, phone numbers, and password lengths across vault items.
- Detect weak passwords with a built-in password-strength heuristic.
- Identify reused passwords and show which accounts share them.
- Find items without TOTP and items with misconfigured login metadata.
- Generate TOTP association URLs for supported services such as Steam, Blizzard, and Gemini.

## 🎯 Use Cases

- **Vault hygiene review:** Inspect a Bitwarden export to identify weak passwords, reused credentials, and missing metadata.
- **Account clean-up:** Locate all accounts tied to a specific email address, phone number, username, or password.
- **TOTP migration support:** Extract TOTP association URLs from compatible entries for downstream authenticator workflows.

## 🚀 Usage

Run the application with the path to a Bitwarden JSON export file:

```bash
dotnet run --project BitwardenVaultManager/BitwardenVaultManager.csproj -- /path/to/bitwarden-export.json
```

After startup, choose one of the registered menu commands to inspect the loaded vault.

## ⌨️ Command Reference

| Command | Description |
|---------|-------------|
| `get-email-addresses` | List all discovered email addresses and the number of associated accounts. |
| `get-email-address-usages` | List the accounts associated with a specific email address. |
| `get-phone-numbers` | List all discovered phone numbers and the number of associated accounts. |
| `get-phone-number-usages` | List the accounts associated with a specific phone number. |
| `get-items-by-password-length` | List items that use passwords of a specific length. |
| `get-items-without-2fa` | List weak-password login items that do not have TOTP configured. |
| `get-misconfigured-items` | List login items that do not expose an email address field. |
| `get-password-lengths` | Show password lengths and the number of logins for each length. |
| `get-password-usages` | List the accounts that use a specific password. |
| `get-passwords-containing` | List the accounts whose passwords contain a specific text fragment. |
| `get-reused-passwords` | Print passwords that are reused across multiple accounts. |
| `get-totp-urls` | Generate TOTP association URLs for items that contain TOTP secrets. |
| `get-usernames` | List all discovered unique usernames and the number of associated accounts. |
| `get-username-usages` | List the accounts associated with a specific username. |
| `get-weak-passwords` | List accounts that currently use weak passwords. |

## ⚠️ Known Limitations

- The application analyses Bitwarden JSON exports only; it does not connect directly to Bitwarden-hosted services.
- Analysis is read-only and does not write changes back to the vault export.
- Sensitive values such as usernames, passwords, and TOTP URLs may be printed to the terminal when the corresponding commands are used.

## 📦 Installation

[![Obtain it from GitHub](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/github.png)](https://github.com/hmlendea/bitwarden-vault-manager/releases)

### Manual Installation

1. Download the appropriate archive or executable from the [GitHub Releases](https://github.com/hmlendea/bitwarden-vault-manager/releases) page.
2. Extract the downloaded archive if necessary.
3. Run the executable and pass the path to a Bitwarden JSON export file.

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| `.NET` | `10.0` | The solution targets `net10.0`. |
| Bitwarden vault export format | Current JSON export shape represented by `DataAccess.DataObjects` | Changes to the export shape require mapping and test updates. |

## 🛡️ Privacy and Data

| Data | Purpose | Storage | Retention | Optional |
|------|---------|---------|-----------|----------|
| Bitwarden vault export contents | In-memory credential analysis | Process memory only | Retained for the duration of the process | No |
| Terminal output | Present analysis findings to the operator | Terminal session and terminal history outside the application boundary | Controlled by the operator environment | No |

## 🛠️ Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

Clone the repository and restore the solution dependencies:

```bash
git clone git@github.com:hmlendea/bitwarden-vault-manager.git
cd bitwarden-vault-manager
dotnet restore BitwardenVaultManager.slnx
```

### Build

```bash
dotnet build BitwardenVaultManager.slnx
```

### Run

```bash
dotnet run --project BitwardenVaultManager/BitwardenVaultManager.csproj -- /path/to/bitwarden-export.json
```

### Test

```bash
dotnet test BitwardenVaultManager.UnitTests/BitwardenVaultManager.UnitTests.csproj
```

### Coverage

```bash
dotnet test BitwardenVaultManager.UnitTests/BitwardenVaultManager.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput=TestResults/coverage/
```

The coverage report is written to `BitwardenVaultManager.UnitTests/TestResults/coverage/coverage.json`.

### Continuous Integration

The primary GitHub Actions workflow restores dependencies, builds the solution, and runs the automated tests. You can reproduce those checks locally with:

```bash
dotnet restore BitwardenVaultManager.slnx
dotnet build BitwardenVaultManager.slnx --no-restore
dotnet test BitwardenVaultManager.UnitTests/BitwardenVaultManager.UnitTests.csproj --no-build --verbosity normal
```

### Release

The repository includes `release.sh`, which delegates to the upstream deployment script used by the project maintainer.

```bash
bash ./release.sh 1.5.1
```

This script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

### Dependencies

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `NuciCLI` | `3.0.1` | Runtime | Console helpers for terminal interaction. |
| `NuciCLI.Menus` | `1.5.5` | Runtime | Menu host and command registration for the interactive CLI. |
| `NuciDAL` | `3.2.1` | Runtime | Project dependency declared by the production application. |
| `NUnit` | `4.3.2` | Development | Unit-testing framework for the test project. |
| `Moq` | `4.20.72` | Development | Test doubles for service and adapter seams. |
| `coverlet.collector` | `6.0.4` | Development | Coverage collection during test execution. |

## 🗂️ Project Structure

The repository is organised as a small solution with one production console application and one dedicated unit-test project.

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `BitwardenVaultManager/BitwardenVaultManager.csproj` | .NET console application | Loads a Bitwarden export and exposes analysis commands through a terminal menu. |
| `BitwardenVaultManager.UnitTests/BitwardenVaultManager.UnitTests.csproj` | .NET test project | Verifies application behaviour, mappings, helpers, CLI output, and coverage-sensitive contracts. |

### Directories

| Directory | Purpose |
|-----------|---------|
| `BitwardenVaultManager/DataAccess` | File loading and exported vault transport objects. |
| `BitwardenVaultManager/Menus` | Interactive CLI menu and command presentation. |
| `BitwardenVaultManager/Service` | Core analysis services, models, helpers, and mappings. |
| `BitwardenVaultManager.UnitTests` | Automated unit tests for the production project. |
| `.github/workflows` | Continuous-integration workflow definitions. |

## 🏗️ Architecture

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality

## 🔒 Security

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/bitwarden-vault-manager/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0`.
See [LICENSE](./LICENSE) for further information.
