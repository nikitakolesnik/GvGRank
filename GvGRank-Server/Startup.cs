﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GvGRank_Server.Models;
using GvGRank_Server.Hubs;
using System;
using System.Threading;
using System.Linq;

namespace GvGRank_Server
{
	public class Startup
	{
		private Timer _decrementingTimer;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			string connectionString = Configuration.GetConnectionString("DefaultConnection");

			services.AddDbContext<VoteDbContext>(o => o.UseSqlServer(connectionString));

			//services.AddCors(options =>
			//{
			//	options.AddPolicy("CorsPolicy",
			//		builder =>
			//		{
			//			builder
			//				.WithOrigins("https://gvgrank.azurewebsites.net/")
			//				//.AllowAnyOrigin()
			//				.AllowAnyHeader()
			//				.AllowAnyMethod()
			//				.AllowCredentials()
			//			;
			//		});
			//});

			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
		{
			appLifetime.ApplicationStarted.Register(() => StartDecrementingTimer(app.ApplicationServices));
			appLifetime.ApplicationStopping.Register(StopDecrementingTimer);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			//app.UseCors("CorsPolicy"); // https://stackoverflow.com/a/56984245/10874809

			app.UseDefaultFiles(); // https://stackoverflow.com/a/49126167/10874809
			app.UseStaticFiles();  // ^

			app.UseSignalR(options => options.MapHub<VoteHub>("/recentvote"));

			app.UseMvc();
		}

		private void StartDecrementingTimer(IServiceProvider provider)
		{
			//const int ALLOWED_VOTES_PER_MINUTE = 30;
			const int ANTI_TAMPERING_THRESHOLD = 100;

			_decrementingTimer = new Timer(
				state =>
				{
					using (var scope = provider.CreateScope())
					{
						var context = scope.ServiceProvider.GetService<VoteDbContext>();

						context.Users
							.Where(user => user.VoteLimit > 0)
							.ToList()
							.ForEach(user => user.VoteLimit = 0);
						
						context.Users
							.Where(user => user.AntiTamper > 1)
							.ToList()
							.ForEach(user => user.AntiTamper = Math.Max(1, user.AntiTamper - ANTI_TAMPERING_THRESHOLD / 2));

						context.SaveChanges();
					}
				},
				null,
				TimeSpan.Zero,
				TimeSpan.FromMinutes(1));
		}

		private void StopDecrementingTimer()
		{
			if (_decrementingTimer != null) //stop timer so it doesn't call callback again and dispose it
			{
				_decrementingTimer.Change(Timeout.Infinite, Timeout.Infinite);
				_decrementingTimer.Dispose();
				_decrementingTimer = null;
			}
		}
	}
}
