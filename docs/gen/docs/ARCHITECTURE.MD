# Architecture Overview

## System Summary
This project is an ASP.NET Core (`net8.0`) Web API that captures camera frames, analyzes images with OpenAI vision, and optionally reports analysis results to Home Assistant.

At runtime, it combines:
- HTTP controllers for control/query endpoints
- Background hosted services for continuous camera ingestion
- Service-layer integrations for AI inference and Home Assistant updates
- Configuration-driven camera and prompt behavior

## High-Level Component Layout

### API Layer (`Controllers/`)
- `AIImageAnalyzeController`: one-off image analysis endpoint (`POST /AIImageAnalyze`) for uploaded images.
- `CameraServiceController`: camera service introspection and control (`/services`, `/response/{id}`, `/image/{id}`, enable/disable analyzer).
- `ConfigurationController`: exposes configured tracked objects.
- `HomeAssistantController`: utility endpoints to push values/reports to Home Assistant.

### Application Services (`Services/`)
- `CameraServiceFactory` (`BackgroundService`):
  - Reads configured cameras from `Cameras` config section.
  - Creates one `CameraService` instance per camera.
  - Starts all camera workers.
- `CameraService` (`BackgroundService`):
  - Validates camera reachability (ping).
  - Runs `ffmpeg` to pull a continuous frame stream as PNG images.
  - Detects complete PNG frames from stream bytes.
  - Stores latest image/report and optionally triggers async AI analysis.
- `ImageAnalyzerService`:
  - Builds OpenAI chat-completion request using prompt stack.
  - Sends image + instruction prompt to vision model.
  - Parses JSON response into `AIRoomResponse`.
  - Triggers Home Assistant reporting.
- `PromptsService`:
  - Builds system prompts from config (`FixedPrompts`, `TrackedObjects`).
  - Exposes combined prompt list.
- `HomeAssistantService`:
  - Uses `HADotNet` `ServiceClient` to call Home Assistant services (currently `input_text.set_value`).

### Contracts and Models
- `IPromptsService`, `IImageAnalyzerService` define service boundaries.
- `AIRoomResponse` is the main AI output schema (furniture, objects, alerts, counts, tracked objects, feel).
- `ImageAnalyzeReport` wraps raw image bytes, error state, and parsed response.
- `CameraServiceConfiguration` defines per-camera runtime settings (URL, ffmpeg args, interval, analyzer toggle).

## Runtime Flow

### Continuous Camera Pipeline
1. App starts and registers dependencies in `Program.cs`.
2. `CameraServiceFactory` starts as hosted service.
3. For each configured camera, a `CameraService` starts.
4. `CameraService` launches `ffmpeg` and reads PNG byte stream.
5. On full frame detection:
   - If analyzer enabled: frame is sent to `ImageAnalyzerService` asynchronously.
   - If analyzer disabled: frame is stored without AI analysis.
6. Latest frame/report is exposed through `CameraServiceController`.

### One-Off API Analysis Pipeline
1. Client posts image to `AIImageAnalyzeController`.
2. Controller merges configured prompts + image analysis instruction.
3. OpenAI completion is requested.
4. JSON response is parsed into `AIRoomResponse` and returned.

### Home Assistant Reporting
- When enabled and configured, `ServiceClient` is initialized at startup.
- `ImageAnalyzerService` and `HomeAssistantController` can send text updates to Home Assistant entities.

## Dependency Injection and Startup
`Program.cs` wires:
- OpenAI client (`AddOpenAIService`) from `OpenAI:Organization` and `OpenAI:ApiKey`
- Singleton prompt/analyzer/home-assistant services
- Optional Home Assistant `ServiceClient` when `HomeAssistant:Enabled=true`
- Camera configurations via options binding (`Cameras`)
- `CameraServiceFactory` as both singleton and hosted service

## Configuration-Driven Behavior
Key config sections:
- `OpenAI`: org + API key (required)
- `HomeAssistant`: enabled/url/apikey (required only when enabled)
- `FixedPrompts`: base system prompts for model behavior/output format
- `TrackedObjects`: appended into prompt instructions
- `Cameras[]`: per-camera ingestion and analyzer settings

## External Integrations
- OpenAI (`Betalgo.OpenAI`) for image understanding
- Home Assistant (`HADotNet.Core`) for reporting automation state
- `ffmpeg` process for camera stream frame extraction (also installed in Docker image)

## Deployment Shape
- Containerized via multi-stage Dockerfile:
  - Build/publish in .NET SDK image
  - Runtime on ASP.NET image with `ffmpeg` installed
- Swagger is enabled for API exploration.