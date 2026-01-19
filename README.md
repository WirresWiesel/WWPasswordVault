Gerne 🙂
Hier ist eine kurze, saubere README, wie sie gut zu einem GitHub-Projekt passt – nicht zu lang, aber professionell.

🔐 WWPasswordVault

WWPasswordVault is a modern password vault built with C# and WinUI 3.
It securely stores user credentials using strong cryptographic primitives and a clean, maintainable architecture based on MVVM.

✨ Features

Secure master-password authentication

Password hashing with salt and configurable iterations

Encrypted local vault storage (JSON-based)

Session-based lock / unlock handling

Clean separation between Core and UI (WinUI 3) projects

🧱 Architecture

The solution is split into two main layers:

WWPasswordVault.Core
Contains cryptography, authentication, storage, and domain models.

WWPasswordVault.WinUI
WinUI 3 frontend using MVVM, navigation services, and session management.

The application follows a state-driven design, where navigation and UI behavior react to session state changes instead of direct callbacks.

🚧 Project Status

This project is currently under active development and serves as:

a learning project for modern WinUI 3 + MVVM

a foundation for a secure local password manager

⚠️ Disclaimer

This project is for educational and experimental purposes.
It has not been audited and should not be used for storing sensitive production credentials.

<img width="1900" height="1012" alt="image" src="https://github.com/user-attachments/assets/3aa30e93-21e7-43b1-9b36-4c8126950bfd" />

<img width="1904" height="1014" alt="image" src="https://github.com/user-attachments/assets/4db07e17-8d05-450d-9c5e-43424a584df0" />

<img width="1902" height="1012" alt="image" src="https://github.com/user-attachments/assets/ea02f558-be11-46e4-a903-beac2eee4e27" />
