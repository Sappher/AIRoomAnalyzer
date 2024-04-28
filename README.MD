## Run locally by building with dotnet

Restore project:

    dotnet restore

Run:

    dotnet run

Access Swagger UI: http://localhost:5016/swagger/index.html

## Or build and run with Docker

Build image:

    docker build -t aiproxy .

Run with docker:

    docker run -p 8080:8080 -v ./appsettings.Development.json:/app/appsettings.json --network=host aiproxy

Access Swagger UI: http://localhost:8080/swagger/index.html

## Provide appsettings.json (appsettings.Development.json if running with dotnet.run)

Provide appsettings.json like this:

    {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
        },
        "FixedPrompts": [
            "You must only answer in valid JSON format. No other format is accepted.",
            "The format of JSON must be: { furniture: string[], otherobjects: string[], generalfeel: string, alerts: string[], peoplecount: number, petcount: number }"
        ],
        "OpenAI": {
            "Organization": "XXX",
            "ApiKey": "XXX"
        },
        "HomeAssistant": {
            "Enabled": true,
            "Url": "http://localhost:8123",
            "ApiKey": "XXX"
        },
        "Cameras": [
            {
                "Name": "CAM 1",
                "Url": "",
                "ExtraFFMpegInputArguments": "-rtsp_transport tcp",
                "ShowFFMpegOutput": true,
                "FrameCaptureInterval": 30,
                "EnableAnalyzer": false
            }
        ]
    }
