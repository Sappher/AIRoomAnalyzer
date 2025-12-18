# General Description of the Project

This project, named **AIProxy**, is a .NET 8.0 web API application designed to analyze images from camera streams using AI, specifically leveraging OpenAI's GPT-4 Vision capabilities. It acts as a proxy service that connects to multiple cameras, captures frames at configurable intervals, and sends these images to an AI model for detailed analysis of the room contents and environment.

## Key Features

- **Camera Integration and Management**:  
  The project supports multiple camera configurations, each with customizable settings such as frame capture interval, URL, and FFmpeg arguments. It uses FFmpeg to capture images from RTSP or other video streams continuously.

- **Image Analysis with AI**:  
  Captured images are sent to OpenAI's GPT-4 Vision model for analysis. The AI focuses on identifying furniture, objects, people, pets, and the general feel of the room, including any alerts or unusual conditions.

- **Home Assistant Integration**:  
  The system can report analysis results to Home Assistant, a popular home automation platform, by updating input text entities with the AI's general feel description.

- **REST API Endpoints**:  
  The application exposes several API endpoints for:  
  - Analyzing images on demand (`AIImageAnalyzeController`)  
  - Managing camera services and retrieving their latest analysis and images (`CameraServiceController`)  
  - Accessing configuration data such as tracked objects (`ConfigurationController`)  
  - Interacting with Home Assistant services (`HomeAssistantController`)

- **Background Services**:  
  Each camera runs as a background service that continuously captures frames and optionally analyzes them asynchronously.

- **Configuration Driven**:  
  Cameras, AI prompts, tracked objects, and Home Assistant settings are all configurable via `appsettings.json` or environment-specific configuration files.

- **Docker Support**:  
  The project includes a Dockerfile for containerized deployment, installing necessary dependencies like FFmpeg and running the .NET application.

## Technologies Used

- **.NET 8.0** with ASP.NET Core Web API  
- **OpenAI GPT-4 Vision** for image analysis  
- **FFmpeg** for video stream frame capture  
- **Home Assistant API** integration via HADotNet.Core client  
- **Newtonsoft.Json** for JSON serialization/deserialization  
- **Swagger/OpenAPI** for API documentation and testing

## Typical Use Case

1. The service connects to configured cameras and captures images at set intervals.  
2. If enabled, images are sent to the AI model for analysis.  
3. The AI returns structured JSON describing the room's furniture, objects, people, pets, alerts, and general feel.  
4. The service stores the latest image and analysis report per camera.  
5. Results can be queried via API or pushed to Home Assistant for automation or monitoring purposes.

---

This project is ideal for smart home or security applications where automated visual analysis of rooms is needed, providing actionable insights through AI-powered image understanding.