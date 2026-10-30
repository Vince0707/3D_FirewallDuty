# Firewall Duty: 3D Cybersecurity Simulation

A first-person mobile game (Android) where the player investigates cybersecurity threats in an office and a bank. Each environment has 10 story missions. The threats covered are phishing, malware, spam, fake pop-ups, social engineering and unauthorized access.

## Documentation
- [Software features](docs/SOFTWARE_FEATURES.md)
- [Technical documentation](docs/TECHNICAL_DOCUMENTATION.md): architecture, backend, mission system, build and configuration
- [Game flowchart](docs/flowchart.html): download the file and open it in a browser

## Requirements
- Unity **6000.4.6f1** (Unity 6), with Android Build Support for builds
- Universal Render Pipeline 17, Input System package (the legacy Input API is disabled)

## Open the project
1. Clone the repository.
2. In Unity Hub, choose **Add project from disk** and select the cloned folder. The first import takes a while because Unity regenerates the `Library` folder.
3. Open `Assets/Scenes/MainMenu_Scene.unity` and press Play to start from the menu. The mission scenes are `Company_scene` (office) and `Bank_scene` (bank).

## Login and online features
Login, leaderboard and cloud save use Firebase, with Google sign-in. The real keys are not in this repository. To enable them, copy each example file in `Assets/Resources/MenuUI/`, remove `.example` from the name and fill in your own values:

| File | Needs |
|------|-------|
| `firebase_config.example.json` | Firebase `projectId`, `apiKey`, `webClientId` |
| `google_device_config.example.json` | Google OAuth `clientId` and `clientSecret` |
| `facebook_config.example.json` | Facebook `appId` and `clientToken` (Facebook sign-in is not active in the game) |

Without them the game still opens, but login and online features will not work. Never commit the real files (they are in `.gitignore`).

## Code layout
Only `Assets/Scripts/` is first-party code. Everything else under `Assets/` is third-party asset-store content.
- `Assets/Scripts/Menu/`: menu screens, login, trivia, leaderboard, achievements, sounds
- `Assets/Scripts/Mission/`: mission session, scenarios, timer, investigation screens, Cyber Tablet, floor-arrow guide
- `Assets/Scripts/`: player controller, joystick, interaction, doors, NPC behaviours
- `Assets/Scripts/Editor/`: editor tools used to set up scenes
