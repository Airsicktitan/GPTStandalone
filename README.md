# GPTStandalone

An integrated AI assistant application combining ChatGPT and Syniti's proprietary SynitiSense AI engine into a unified interface, demonstrating practical multi-AI orchestration and enterprise AI integration patterns.

## Overview

GPTStandalone is a C# desktop application that provides seamless access to multiple AI engines through a single interface. The application demonstrates how to integrate both public AI APIs (OpenAI's ChatGPT) and private enterprise AI systems (SynitiSense) into a cohesive user experience.

## Key Features

- **Dual AI Engine Integration**: Query both ChatGPT and SynitiSense simultaneously from one prompt
- **Unified Interface**: Single application window for interacting with multiple AI systems
- **Parallel Processing**: Concurrent API calls to both engines for faster response times
- **Response Comparison**: Side-by-side display of responses from each AI system
- **Enterprise Context**: SynitiSense provides domain-specific knowledge about Syniti products and workflows

## Technical Architecture

### AI Engine Integration

**ChatGPT Integration**
- OpenAI API (GPT-3.5/GPT-4)
- RESTful HTTP requests
- JSON response parsing
- Token management and rate limiting

**SynitiSense Integration**
- Syniti's proprietary AI system
- Internal API endpoints
- Domain-specific training on Syniti products
- Enterprise security and authentication

### Application Stack

- **Language**: C# (.NET 6.0+)
- **UI Framework**: Windows Forms / WPF
- **HTTP Client**: HttpClient for async API calls
- **JSON Handling**: System.Text.Json
- **Authentication**: API key management for both services

## Architecture Diagram
```
┌──────────────────────────────┐
│   GPTStandalone UI (C#)      │
│   Single Prompt Interface    │
└──────────┬───────────────────┘
           │
           ├─────────────┬─────────────┐
           │             │             │
           ▼             ▼             ▼
    ┌───────────┐ ┌───────────┐ ┌──────────┐
    │  OpenAI   │ │  Syniti   │ │ Response │
    │ ChatGPT   │ │  Sense    │ │Aggregator│
    │    API    │ │    API    │ │          │
    └───────────┘ └───────────┘ └──────────┘
```

## Use Cases

### Why Integrate Multiple AI Systems?

1. **Complementary Strengths**: ChatGPT provides general knowledge; SynitiSense provides domain expertise
2. **Validation**: Compare responses across systems to verify accuracy
3. **Fallback Strategy**: If one service is unavailable, the other provides continuity
4. **Context Diversity**: General AI + specialized AI = more comprehensive answers

### Practical Applications

**Example Query**: "How do I configure data transformation rules in DSP?"

**ChatGPT Response**: General information about data transformation patterns and ETL concepts

**SynitiSense Response**: Specific steps for Syniti's DSP platform, including menu locations and configuration parameters

**User Benefit**: Combined general understanding with platform-specific implementation details

## Key Implementation Details

### Concurrent API Calls
```csharp
// Pseudo-code demonstrating parallel API calls
var chatGptTask = CallChatGptApi(userPrompt);
var synitiSenseTask = CallSynitiSenseApi(userPrompt);

await Task.WhenAll(chatGptTask, synitiSenseTask);

DisplayResponses(chatGptTask.Result, synitiSenseTask.Result);
```

### Error Handling

- Graceful degradation if one API fails
- Timeout management for both services
- User feedback on API availability
- Retry logic with exponential backoff

### Security Considerations

- API keys stored securely (not hardcoded)
- HTTPS for all API communications
- Token refresh management
- Rate limit handling

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- OpenAI API key
- Access to SynitiSense API (Syniti internal)

### Configuration

1. Clone the repository
2. Add API credentials to configuration file:
```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key",
    "Model": "gpt-4"
  },
  "SynitiSense": {
    "ApiKey": "your-synitisense-key",
    "Endpoint": "https://internal.syniti.com/api/ai"
  }
}
```

3. Build and run:
```bash
dotnet build
dotnet run
```

## Application Interface

The application provides:
- **Input Panel**: Text area for entering prompts
- **ChatGPT Response Panel**: Displays OpenAI responses
- **SynitiSense Response Panel**: Displays Syniti AI responses
- **Submit Button**: Triggers parallel API calls
- **Status Indicator**: Shows API call progress and errors

## Technical Challenges Solved

1. **Async Coordination**: Managing concurrent API calls without blocking UI
2. **Response Formatting**: Normalizing different response structures
3. **Error Recovery**: Handling partial failures gracefully
4. **Performance**: Optimizing for low latency despite dual API calls

## Learning Outcomes

This project demonstrates:
- Multi-AI system orchestration
- Enterprise AI integration patterns
- Async programming in C#
- API consumption and error handling
- Desktop application development

## Future Enhancements

- Add conversation history persistence
- Implement response caching for common queries
- Support additional AI engines (Claude, Gemini, etc.)
- Add response quality scoring
- Implement prompt templates for common tasks
- Add export functionality for responses

## Limitations

- Requires active API keys for both services
- SynitiSense access limited to Syniti employees
- Response time dependent on both API latencies
- No offline mode (cloud-dependent)

## Use in Production

This application demonstrates an integration pattern suitable for:
- Enterprise AI assistants combining public and private AI
- Multi-vendor AI comparison tools
- AI-powered help desk systems
- Internal knowledge management with AI augmentation

## Author

Built by Adam Hooper to explore practical AI integration patterns and demonstrate multi-system orchestration in enterprise contexts.

## Contact

- GitHub: [@Airsicktitan](https://github.com/Airsicktitan)
- Project Link: [GPTStandalone](https://github.com/Airsicktitan/GPTStandalone)

## License

MIT License - see LICENSE file for details

**Note**: SynitiSense is proprietary to Syniti. This project demonstrates integration patterns but cannot be fully replicated without internal Syniti access.
