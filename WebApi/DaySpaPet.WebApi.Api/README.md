## Configuration

For local development, all application configuration is maintained via
this appsettings.json and appsettings.Development.json files, except for
any secret values which should be stored in the User Secrets store.

In production, the appsettings.json file is used and then override by any
secret values stored in environment variables. 

### About .NET Configuration

.NET Core/5+, configuration is loaded in this order of precedence:

1. appsettings.json
1. appsettings.{Environment}.json
1. User secrets
1. Environment variables
1. Command line arguments

Once loaded, configuration values are accessible by the application via 
the IConfiguration interface dependency resolution, or using the Options pattern. 

> Note: When storing values in environment variables, it is necessary to form the full hierarchical key name without line endings or colons. i.e.: `Authentication__Schemes__Bearer__PrivateSigningKey`, `Authentication__Schemes__Bearer__PublicSigningKey`.

## Application Security

Public API exposed by this application is protected by JWT tokens, and 
additional methods of security may be integrated as well.

### RSA Keys

On application startup, the WebApp.Api loads a private RSA key and configures it to validate JWT bearer tokens, as well as a matching public PEM-encoded key for signing JWT bearer tokens when users login or refresh their token.

These keys are specific to each environment, be that a deployed container/server or even a single developer workstation.
- No keys are shared by any one developer.
- No keys are shared across any deployed location configuration.

## Application Environment Requirements

For this application to start and operate successfully, the following environment requirements must be met:

1. A valid RSA private key and its public PEM-encoded key must be generated and stored in the environment at one of the following .NET 8 configuration locations (in this order of precedence):

	|Location | Purpose | May be exclusively used by<super>👮</super> | Currently used by<super>🤝</super> |
	|-|-|-|-|
	|appsettings.{Environment}.json | Non-secret values | localhost, Development, Production | localhost, Development, Production |
	|User secrets | Secret values in localhost development environment only | localhost | localhost |
	|Environment variables | Non-secret and secret values<super>*</super> | Production | Production |

	* Key names must use double underscore character in place of hierarchical separators. Ex: `Authentication__Schemes__Bearer__PrivateSigningKey`.
	👮 Governance Team says so!
	🤝 On the honor system, and a team effort - we all work together to maintain this documentation as close to real-time as possible, including during any step of the local development and automated pipelines.
1. 

### Generating RSA Keys


```powershell
.\generate_rsa_keys.ps1 -EnvironmentName Development -Force

```

If the environment name provided is `Development`, keys will be saved in .NET Secrets Manager (aka User Secrets). For all other environments, the keys will be emitted to the console as flat strings to be copied and stored as environment variables in the target environment (such as a Docker container, Azure App Service instance, Windows OS with IIS, etc.)