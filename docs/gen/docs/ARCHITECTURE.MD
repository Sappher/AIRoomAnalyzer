# Architecture Overview of AIProxy Project

## Overview
AIProxy is a .NET 8.0 web API project designed to analyze images from cameras using AI (OpenAI GPT-4 Vision) and optionally report results to Home Assistant. It acts as a proxy service that connects to multiple camera streams, captures frames, sends them for AI analysis, and exposes REST endpoints for managing cameras and retrieving analysis results.

---

## Key Components

### 1. **Web API Layer**
- Implemented using ASP.NET Core Web API.
- Exposes REST endpoints via controllers:
  - `AIImageAnalyzeController`: Accepts image uploads for AI analysis.
  - `CameraServiceController`: Manages camera services, enables/disables analyzers, retrieves last image and analysis.
  - `ConfigurationController`: Provides configuration data such as tracked objects.
  - `HomeAssistantController`: Interacts with Home Assistant to report analysis results or set dummy reports.
- Swagger UI is enabled for API documentation and testing.

### 2. **Camera Services**
- **CameraServiceFactory** (BackgroundService):
  - Reads camera configurations from appsettings.
  - Creates and manages multiple `CameraService` instances, one per camera.
  - Starts all camera services concurrently.
- **CameraService** (BackgroundService):
  - Connects to a camera stream using FFmpeg.
  - Captures frames at configured intervals.
  - Detects PNG images from the FFmpeg output stream.
  - If enabled, sends captured images to the AI image analyzer service asynchronously.
  - Maintains the last captured image and last analysis report.
  - Supports enabling/disabling the analyzer dynamically.

### 3. **AI Image Analysis**
- **ImageAnalyzerService**:
  - Implements `IImageAnalyzerService`.
  - Uses OpenAI GPT-4 Vision model to analyze images.
  - Sends images with prompts to OpenAI chat completion API.
  - Parses JSON responses into `AIRoomResponse` model.
  - Logs and handles errors gracefully.
  - Reports analysis results asynchronously to Home Assistant if enabled.

### 4. **Prompts Management**
- **PromptsService**:
  - Implements `IPromptsService`.
  - Loads fixed prompts and tracked objects from configuration.
  - Provides combined prompt list for AI requests.

### 5. **Home Assistant Integration**
- **HomeAssistantService**:
  - Wraps HADotNet client to interact with Home Assistant.
  - Reports analysis results by setting input_text entity values.
  - Configured optionally via appsettings.

---

## Data Models

- **CameraServiceConfiguration**: Configuration for each camera (URL, name, analyzer enabled, FFmpeg args, etc.).
- **AIRoomResponse**: AI analysis result including furniture, objects, alerts, general feel, people and pet counts.
- **ImageAnalyzeReport**: Contains analyzed image bytes, error flag, and AI response.
- **CameraServiceResponse**: Combines camera metadata and last analysis report for API responses.

---

## External Dependencies

- **OpenAI SDK** (`Betalgo.OpenAI`): For AI image analysis using GPT-4 Vision.
- **HADotNet.Core**: For Home Assistant API client.
- **FFmpeg**: Used to capture frames from camera streams.
- **Newtonsoft.Json**: For JSON serialization/deserialization.
- **Swashbuckle.AspNetCore**: For Swagger/OpenAPI support.

---

## Configuration

- Uses `appsettings.json` or environment-specific variants.
- Configures OpenAI API keys and organization.
- Configures Home Assistant URL and API key.
- Defines tracked objects for AI prompts.
- Defines cameras with their streaming URLs and capture settings.

---

## Execution Flow

1. On startup, `CameraServiceFactory` creates and starts `CameraService` instances for each configured camera.
2. Each `CameraService` connects to its camera stream via FFmpeg, capturing PNG frames at intervals.
3. If analyzer is enabled, frames are sent to `ImageAnalyzerService` for AI analysis.
4. AI responses are parsed and stored in the service instance.
5. API endpoints allow clients to query camera statuses, last images, and analysis results.
6. Optionally, analysis results are reported to Home Assistant entities.

---

## Deployment

- Can be run locally via `dotnet run`.
- Dockerfile provided for containerized deployment.
- FFmpeg installed in container for video processing.

---

This architecture enables scalable management of multiple camera streams with AI-powered image analysis and integration into smart home systems like Home Assistant.