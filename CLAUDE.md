# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Confi is a .NET configuration management ecosystem that provides distributed configuration tools and best practices. The project consists of multiple components:

- **Core Library** (`dotnet/core`): Base configuration extensions and utilities
- **Configuration Manager** (`manager/`): Distributed configuration editing and synchronization API
- **JSON HTTP Provider** (`json/lib`): HTTP-based configuration loading with polling
- **MongoDB Provider** (`mongo/dotnet/lib`): MongoDB-based configuration storage and loading
- **Background Store Provider** (`providers/dotnet/background`): In-memory configuration store with background updates
- **Environment Variable Provider** (`envs/fluenv`): Fluent environment variable configuration

## Development Commands

### Building and Testing

**Main solution (confi.sln)**:
```bash
dotnet build
dotnet test
```

**Individual component testing**:
```bash
# Core tests
cd dotnet/tests && dotnet test

# Manager tests
cd manager/tests && dotnet test

# Mongo tests  
cd mongo/dotnet/tests && dotnet test

# Background store tests
cd providers/dotnet/background/tests && dotnet test

# Environment provider tests
cd envs/fluenv/tests && dotnet test
```

### Manager Development

**Run Manager API**:
```bash
cd manager && make run-api
# or
cd manager/host && dotnet run
```

**Manager testing with httpyac**:
```bash
cd manager && make yac TEST=about
cd manager/tests && httpyac send --all *.http --env=local
```

**Full Manager development cycle**:
```bash
cd manager && make prep      # Start dependencies (Docker compose)
cd manager && make play      # Run API + execute tests
cd manager && make reset     # Reset environment
```

### Playground Applications

**JSON HTTP Playground**:
```bash
cd json/play && make run
cd json/play && make play    # Run + test
```

**Manager Consumer Playground**:
```bash
cd manager/consumer/play && make run
cd manager/consumer/play && make play
```

**Mongo Playground**:
```bash
cd mongo/dotnet/playground && dotnet run
```

## Architecture

### Configuration Flow
1. **Configuration Sources**: Various providers (JSON HTTP, MongoDB, Environment) load configuration data
2. **Background Store**: Centralized in-memory store that notifies listeners of configuration changes  
3. **Manager**: Distributed configuration management system where nodes sync with a central manager
4. **Consumers**: Applications that consume configuration from various sources with periodic updates

### Key Components

**Configuration Extensions** (`dotnet/core/Core.cs`):
- `GetRequiredValue()`: Throws exception if configuration key is missing
- `GetRequiredConfigurationValue()`: Service provider extension for required values

**JSON HTTP Provider** (`json/lib/JsonHttp.cs`):
- `HttpStreamPoller`: Polls HTTP endpoints for JSON configuration
- Supports refresh intervals and error handling
- Integrates with .NET Configuration system

**Configuration Manager** (`manager/`):
- RESTful API for distributed configuration management
- MongoDB backend for persistence
- Swagger/OpenAPI documentation
- Node status tracking and synchronization

**Background Store** (`providers/dotnet/background/lib/ConfigurationBackgroundStore.cs`):
- Singleton pattern for shared configuration state
- Listener pattern for real-time updates
- Multiple named stores support

### Dependencies and Frameworks
- **NIST**: Custom framework for API development
- **Persic**: MongoDB integration library  
- **Fluenv**: Fluent environment variable configuration
- **Astor.Logging**: Enhanced logging capabilities
- **httpyac**: HTTP testing tool for API validation

## Testing Strategy

The project uses multiple testing approaches:
- .NET unit tests for core functionality
- HTTP tests using httpyac for API endpoints  
- Integration tests with Docker compose for full stack testing
- Playground applications for manual testing and development

## Version Management

Some playground projects support version switching:
```bash
# Save current version
make v-save V=2

# Switch to version
make v-use V=1

# Play with specific version  
make v-play V=1
```