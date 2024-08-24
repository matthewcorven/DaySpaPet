using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;

namespace DaySpaPet.WebApi.Api.Endpoints.Authentication;

public class UserLogin : Endpoint<LoginRequest> {
    public override void Configure() {
        Post("/api/authentication/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct) {
        var jwtPublicSigningKey = Config.GetRequiredSection("Authentication:Schemes:Bearer:PublicSigningKey");
        var jwtPrivateSigningKey = Config.GetRequiredSection("Authentication:Schemes:Bearer:PrivateSigningKey");
        var jwtIssuer = Config.GetRequiredSection("Authentication:Schemes:Bearer:ValidIssuer");
        var jwtAudiences = Config.GetRequiredSection("Authentication:Schemes:Bearer:ValidAudiences");



        //if (await authService.CredentialsAreValid(req.Email, req.Password, ct, out string userId))
        if (req.Email == "administrator@dayspa.pet") {
            var userId = "c3d20521-0f33-491d-8a68-c6bc7a1159d3";
            var adminId = "57a791d6-10e1-4453-a295-f53147959152";
            var jwtToken = JwtBearer.CreateToken(
                    o => {
                        o.SigningKey = jwtPrivateSigningKey.Value!;
                        o.SigningStyle = TokenSigningStyle.Asymmetric;
                        o.SigningAlgorithm = SecurityAlgorithms.RsaSha256;
                        o.Issuer = jwtIssuer.Value!;
                        o.Audience = jwtAudiences.Value!; // Hmm. Need to set this based on request? Or instead derive audience from user?
                        o.ExpireAt = DateTime.UtcNow.AddDays(1);
                        o.User.Roles.Add("Manager", "Administrator");
                        o.User.Claims.Add(("AdministratorID", adminId));
                        o.User.Claims.Add(("UserUPN", req.Email));
                        o.User.Claims.Add(("UserID", userId)); // or indexer based claim setting: o.User["UserId"] = "...";

                    });

            await SendAsync(new {
                req.Email,
                Token = jwtToken
            }, cancellation: ct);
        } else
            ThrowError("The supplied credentials are invalid!");
    }
}