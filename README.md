# RuleEngine Solution

A .NET-based rule engine for dynamic business rule evaluation with JSON configuration.

## Features

- Dynamic rule loading from JSON
- Context-based rule evaluation
- Performance tracking and optimization
- Extensible rule system

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or VS Code (recommended)

## Getting Started

### 1. Clone the Repository
```
git clone https://github.com/yourusername/RuleEngineSolution.git
cd RuleEngineSolution
```

2. Restore Dependencies
```dotnet restore```

4. Run the Demo
```dotnet run --project RuleEngine.ConsoleDemo```


Configuration
Rule Definition (JSON)
Create rules in RuleEngine.ConsoleDemo/Rules/sample-rules.json:

```[
  {
    "name": "AgeCheck",
    "description": "Verify adult status",
    "priority": 1,
    "isEnabled": true,
    "condition": "ctx.GetInt(\"Age\") >= 18",
    "action": "ctx.Set(\"IsAdult\", true)"
  }
]
```

Running Tests
```dotnet test```

Advanced Usage
API Endpoints
Run the API project:

```dotnet run --project RuleEngine.API```

Available endpoints:

POST /api/rules/execute - Execute rules with provided context

GET /api/rules - List loaded rules

Custom Rule Types
Implement IRule interface to create custom rule types.

Expected Output
14:22:10 Starting rule engine demo...
14:22:10 ✅ AgeCheck        Success <1ms
14:22:10 ✅ PremiumDiscount Success 0.8ms
14:22:10 Final Context:
14:22:10 - IsAdult: True
14:22:10 - Discount:    $25.00
Troubleshooting
Emoji Display Issues
Set console encoding:

```// Add to Program.cs
Console.OutputEncoding = System.Text.Encoding.UTF8;
```

Currency Formatting
Ensure proper culture settings:

csharp
CultureInfo.CurrentCulture = new CultureInfo("en-US");
License
MIT


### Key Sections Included:
1. **Quick Start** - Minimal setup instructions
2. **Project Structure** - Solution overview
3. **Configuration** - Rule JSON format
4. **API Usage** - Web interface options
5. **Troubleshooting** - Common fixes
6. **Sample Output** - Expected results

### Recommended GitHub Additions:

Create a .github/workflows/dotnet.yml for CI:

```
name: .NET

on: [push]

jobs:
  build:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```
