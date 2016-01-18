﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public class TenantNotFoundRedirectMiddleware<TTenant>
	{
		private readonly string redirectLocation;
		private readonly bool permanentRedirect;

		private readonly RequestDelegate next;

		public TenantNotFoundRedirectMiddleware(
			RequestDelegate next,
			string redirectLocation,
			bool permanentRedirect)
		{
			Ensure.Argument.NotNull(next, nameof(next));
			Ensure.Argument.NotNull(redirectLocation, nameof(redirectLocation));

			this.next = next;
			this.redirectLocation = redirectLocation;
			this.permanentRedirect = permanentRedirect;
		}

		public async Task Invoke(HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));

			var tenantContext = context.GetTenantContext<TTenant>();

			if (tenantContext == null)
			{

				Redirect(context, redirectLocation);
				return;
			}

			// otherwise continue processing
			await next(context);
		}
		private void Redirect(HttpContext context, string redirectLocation)
		{
			context.Response.Redirect(redirectLocation);
			context.Response.StatusCode = permanentRedirect ? StatusCodes.Status301MovedPermanently : StatusCodes.Status302Found;
		}
	}
}
