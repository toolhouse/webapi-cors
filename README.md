# Toolhouse.WebApi.Cors

This is a small library to help enable CORS support in ASP.NET WebApi applications. Specifically, it makes it relatively easy to enable unrestricted access to an API from one or more origins, optionally using wildcards.

## What is CORS?

See [MDN documentation][mdn].

## Enabling CORS

First add a project reference to `Toolhouse.WebApi.Cors`. Then update `Web.config`, ensuring there's something like this in the `<system.webServer>` section:

```xml
<system.webServer>
  <handlers>
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <remove name="OPTIONSVerbHandler" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
    </handlers>
  </system.webServer>
```

(CORS makes extensive use of `OPTIONS` HTTP method. This change instructs ASP.NET to handle `OPTIONS` requests, which it won't by default.)

Next, enable CORS support. Example:

```csharp
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Not shown: other configuration

        config.EnableCors(new Toolhouse.WebApi.Cors.ConfigCorsPolicyProvider("MyApp.AllowedOrigins"));

    }
}
```

Make sure to add the allowed origins to `Web.config`:

```xml
<appSettings>
    <add key="MyApp.AllowedOrigins" value="http://localhost:*, http://*.example.org">
</appSettings>
```

## Internals

This library includes two [`ICorsPolicyProvider`][icorspolicyprovider] implementations:

- `WildcardCorsPolicyProvider` accepts a wildcard expression and automatically enables CORS for any origin that matches.
- `ConfigCorsPolicyProvider` reads a wildcard expression from a key in `<appSettings>`.

## Wildcard Support

For specifying origins, we support wildcards (`*`) for subdomains and ports.

| Example | Matches | Does Not Match |
| -- | -- | -- |
| `http://localhost:*` | `http://localhost:3000`, `http://localhost` | `http://foo.localhost` |
| `https://*.example.org` | `https://foo.example.org` | `https://foo.bar.example.org` |

Multiple patterns can be specified by separating them with a comma (`,`).

[mdn]: https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
[icorspolicyprovider]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.cors.infrastructure.icorspolicyprovider?view=aspnetcore-3.1
