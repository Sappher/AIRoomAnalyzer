# General Description

This project is a .NET 8 ASP.NET Core Web API for automated room and camera-image analysis using OpenAI vision models, with optional Home Assistant integration.

It is designed to:
- Ingest snapshots from one or more configured cameras (via `ffmpeg`) in background services.
- Analyze room images to extract structured JSON data such as furniture, other objects, alerts, tracked objects, people count, pet count, and an overall room “general feel.”
- Expose HTTP endpoints (with Swagger) for one-off image analysis, camera service status, latest image/report retrieval, analyzer enable/disable controls, and configuration inspection.
- Optionally push analysis summaries to Home Assistant entities for home automation workflows.

Behavior is configuration-driven (`OpenAI`, `HomeAssistant`, `FixedPrompts`, `TrackedObjects`, `Cameras`), and the project includes Docker support for containerized deployment.