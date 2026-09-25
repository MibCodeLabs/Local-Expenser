using Microsoft.AspNetCore.Identity;

namespace LocalExpenser.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/logout",
            async (SignInManager<IdentityUser> signInManager) =>
            {
                await signInManager.SignOutAsync();

                return Results.Redirect("/login");
            }
        );

        app.MapPost(
            "/account/login",
            async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
            {
                var form = await context.Request.ReadFormAsync();

                var email = form["email"].ToString();

                var password = form["password"].ToString();

                var result = await signInManager.PasswordSignInAsync(email, password, false, false);

                if (result.Succeeded)
                {
                    return Results.Redirect("/");
                }

                return Results.Redirect("/login");
            }
        );
    }
}
