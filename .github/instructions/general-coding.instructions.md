---
applyTo: "**"
---
# RunJS Project Overview
- This application is building an MCP server in C# and .NET 9 (C# version 13)
- You can use any C# features up to this version
- The Docker Hub image name is `cdigs/runjs-mcp-server`; the username is `cdigs` and the repository is `runjs-mcp-server`

## Naming Conventions
- In .cs files, do not use `this` and prefer to use `_` prefix for private fields

## C# (*.cs) Coding Style
- Prefer modern .NET conventions like primary constructors, collection initializers, var, record types, etc.
- Always use braces { } for control structures (if, for, while, etc.); do not leave them out even for single line statements
- When writing tests, use test function names like `MethodName_StateUnderTest_ExpectedBehavior` to clearly describe the test's purpose

## Vue (*.vue) Coding Style
- Use defineModel, defineProps, and defineEmits macros where applicable

## Commenting
- Add comments to explain logical decisions
- Document all function inputs
- Be descriptive with function comments
- When mentioning types, class names, etc in comments, use markdown backticks like `this` or `MyClass`

## Structure
- The /app directory is a Nuxt 3 Vue web application which is using Quasar for the UI components
- The /cli directory contains a sample client application built with TypeScript and the Vercel AI SDK
- The /server directory contains the C# MCP server implementation
- The /tests directory contains integration tests using XUnit

## Key Files

## Logging
This codebase is using Serilog for logging.  To create a logger for a class, use the following pattern:

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
