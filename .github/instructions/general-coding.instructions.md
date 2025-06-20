---
applyTo: "**"
---
# RunJS Project Overview
- This application is building an MCP server in C# and .NET 9 (C# version 13)
- You can use any C# features up to this version

## Naming Conventions
- In .cs files, do not use `this` and prefer to use `_` prefix for private fields

## Coding Style
- Prefer modern .NET conventions like primary constructors, inline arrays, var, record types, etc.
- Always use braces { } for control structures (if, for, while, etc.); do not leave them out even for single line statements

## Commenting
- Add comments to explain logical decision
- Document all function inputs
- Be descriptive with function comments
- When mentioning types, class names, etc in comments, use markdown backticks like `this` or `MyClass`

## Structure
- The /app directory contains a sample client application built with TypeScript and the Vercel AI SDK
- The /server directory contains the C# MCP server implementation

## Key Files

## Logging
This codebase is using Serilog for logging.  To create a logger for a class, us the following pattern:

```csharp
// If it is a static class
private static readonly ILogger Log = Serilog.Log.ForContext(typeof(SomeStaticClass));

// If it is an instance class
private static readonly ILogger Log = Serilog.Log.ForContext<SomeInstanceClass>();
```

When logging, use the following pattern:

```csharp
Log.Here().Information("This is an informational message with a parameter: {Parameter}", parameterValue);
Log.Here().Warning("This is a warning message with a parameter: {Parameter}", parameterValue);
Log.Here().Error(ex, "An error occurred with parameter: {Parameter}", parameterValue);
```

The `Here()` is important; it is a special method that captures the current location in code.
