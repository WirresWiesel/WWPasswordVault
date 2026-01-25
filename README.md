🔐 WWPasswordVault

WWPasswordVault is a modern password vault built with C# and WinUI 3.
It securely stores user credentials using strong cryptographic primitives and a clean, maintainable architecture based on MVVM.

✨ Features

Secure master-password authentication

Password hashing with salt and configurable iterations

Encrypted local vault storage (JSON-based)

Session-based lock / unlock handling

Clean separation between Core and UI (WinUI 3) projects

Improved credential organization with multi-category support
  - Assign multiple categories to a single vault entry
  - Categories can be reused across entries
  - Better filtering and overview for large vaults

  🗂 Category Management

WWPasswordVault supports multiple categories per vault entry, allowing better organization of credentials such as:

  - Websites

  - Games & Launchers

  - Financial and insurance accounts

  - Work-related or personal credentials

Categories are treated as structured data instead of free text, enabling cleaner UI logic, easier filtering, and future extensibility (e.g. tags, icons, or category-based views).

🧱 Architecture

The solution is split into two main layers:

WWPasswordVault.Core
Contains cryptography, authentication, storage, and domain models.

WWPasswordVault.WinUI
WinUI 3 frontend using MVVM, navigation services, and session management.

The application follows a state-driven design, where navigation and UI behavior react to session state changes instead of direct callbacks.

🚧 Project Status

This project is currently under active development and serves as:

  - a learning project for modern WinUI 3 + MVVM

  - a foundation for a secure local password manager

⚠️ Disclaimer

This project is for educational and experimental purposes.
It has not been audited and should not be used for storing sensitive production credentials.

<img width="1900" height="1012" alt="image" src="https://github.com/user-attachments/assets/3aa30e93-21e7-43b1-9b36-4c8126950bfd" />

<img width="1900" height="1011" alt="image" src="https://github.com/user-attachments/assets/a61126c5-0226-42a9-a741-25d45cbc412f" />

<img width="1903" height="1006" alt="image" src="https://github.com/user-attachments/assets/56d9f016-360d-4a97-ade1-8464e2a216d9" />
