using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeDetective.Storage;
using Newtonsoft.Json.Linq;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence;

public static class SeedExternalProviders
{
	public static async Task TrySeedAsync(ApplicationDbContext dbContext)
	{
		Console.WriteLine("Seeding external providers...");

		var providers = GetProviders();
		

		var existingKeys = await dbContext.ExternalProviders
			.Where(x => x.Culture == "fa-IR")
			.Select(x => x.name
			)
			.ToListAsync();

		// فقط مواردی که وجود ندارند را اضافه می‌کنیم
		var toAdd = seedLocalizations
			.Where(x => !existingKeys.Contains(x.Key))
			.ToList();

		if (toAdd.Any())
		{
			await dbContext.Localizations.AddRangeAsync(toAdd);
			await dbContext.SaveChangesAsync();
		}


		Console.WriteLine("seed enums ended");
	}



	public static List<ExternalProvider> GetProviders()
	{
		return new List<ExternalProvider>
		{
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "Google",
				DisplayName = "Google",
				ClientId = "", // بعداً از secrets/DB پر میشه
                ClientSecret = "",
				AuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth",
				TokenUrl = "https://oauth2.googleapis.com/token",
				UserInfoUrl = "https://openidconnect.googleapis.com/v1/userinfo",
				Scopes = "openid profile email"
			},
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "Microsoft",
				DisplayName = "Microsoft",
				ClientId = "",
				ClientSecret = "",
				AuthorizationUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize",
				TokenUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/token",
				UserInfoUrl = "https://graph.microsoft.com/oidc/userinfo",
				Scopes = "openid profile email"
			},
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "Facebook",
				DisplayName = "Facebook",
				ClientId = "",
				ClientSecret = "",
				AuthorizationUrl = "https://www.facebook.com/v15.0/dialog/oauth",
				TokenUrl = "https://graph.facebook.com/v15.0/oauth/access_token",
				UserInfoUrl = "https://graph.facebook.com/me?fields=id,name,email,picture",
				Scopes = "email public_profile"
			},
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "GitHub",
				DisplayName = "GitHub",
				ClientId = "",
				ClientSecret = "",
				AuthorizationUrl = "https://github.com/login/oauth/authorize",
				TokenUrl = "https://github.com/login/oauth/access_token",
				UserInfoUrl = "https://api.github.com/user",
				Scopes = "read:user user:email"
			},
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "LinkedIn",
				DisplayName = "LinkedIn",
				ClientId = "",
				ClientSecret = "",
				AuthorizationUrl = "https://www.linkedin.com/oauth/v2/authorization",
				TokenUrl = "https://www.linkedin.com/oauth/v2/accessToken",
				UserInfoUrl = "https://api.linkedin.com/v2/me",
				Scopes = "r_liteprofile r_emailaddress"
			},
			new ExternalProvider
			{
				Id = Guid.NewGuid(),
				Name = "Twitter",
				DisplayName = "Twitter (X)",
				ClientId = "",
				ClientSecret = "",
				AuthorizationUrl = "https://twitter.com/i/oauth2/authorize",
				TokenUrl = "https://api.twitter.com/2/oauth2/token",
				UserInfoUrl = "https://api.twitter.com/2/users/me",
				Scopes = "tweet.read users.read offline.access"
			}
		};
	}


}
