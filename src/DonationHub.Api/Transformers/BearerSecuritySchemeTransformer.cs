using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DonationHub.Api.Transformers;

/// <summary>
/// Configures OpenAPI document with HTTP Bearer security scheme for JWT authentication in Scalar / Swagger (Task 9).
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    /// <summary>
    /// Initializes transformer with authentication scheme provider.
    /// </summary>
    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
    }

    /// <summary>
    /// Transforms the OpenAPI document by registering Bearer auth scheme and securing operations.
    /// </summary>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (schemes.Any(s => s.Name == "Bearer"))
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your valid JWT Access Token to access protected endpoints."
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = securityScheme;

            var schemeRef = new OpenApiSecuritySchemeReference("Bearer", document);
            var securityRequirement = new OpenApiSecurityRequirement
            {
                [schemeRef] = new List<string>()
            };

            if (document.Paths != null)
            {
                foreach (var path in document.Paths.Values)
                {
                    if (path.Operations != null)
                    {
                        foreach (var operation in path.Operations.Values)
                        {
                            operation.Security ??= new List<OpenApiSecurityRequirement>();
                            operation.Security.Add(securityRequirement);
                        }
                    }
                }
            }
        }
    }
}
