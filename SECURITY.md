# Security Policy

This policy defines how to report security vulnerabilities in Bitwarden Vault Manager, requests private coordinated disclosure, and applies to the latest maintained release distributed through GitHub Releases and source builds from the `master` branch.

## 📑 Table of Contents

- [Supported Versions](#-supported-versions)
- [Reporting a Vulnerability](#-reporting-a-vulnerability)
- [Scope](#-scope)
- [Disclosure Policy](#-disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Latest version | Any other distribution channels | ❌ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/bitwarden-vault-manager/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Vulnerabilities in the Bitwarden Vault Manager CLI application and its bundled dependencies
- Issues that permit unauthorised access to vault data, code execution, privilege escalation, or disclosure of sensitive information

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Security issues in Bitwarden-hosted services, browser extensions, mobile applications, or other third-party products
- Reports that depend upon unsupported forks, modified third-party distribution packages, or deliberately insecure local configuration

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.
