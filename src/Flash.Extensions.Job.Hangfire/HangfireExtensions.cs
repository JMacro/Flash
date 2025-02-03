using Hangfire;
using System;
using System.Collections.Generic;

namespace Flash.Extensions.Job.Hangfire
{
    /// <summary>
    /// Hangfire <see cref="RecurringJob"/> extensions.
    /// </summary>
    public static class HangfireExtensions
	{
		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically within specified interface or class.
		/// To the Hangfire client, alternatively way is to use the class <seealso cref="CronJob"/> to add or update <see cref="RecurringJob"/>.
		/// </summary>
		/// <param name="configuration"><see cref="IGlobalConfiguration"/></param>
		/// <param name="types">Specified interface or class</param>
		/// <returns><see cref="IGlobalConfiguration"/></returns>
		public static IGlobalConfiguration UseRecurringJob(this IGlobalConfiguration configuration, params Type[] types)
		{
			return UseRecurringJob(configuration, () => types);
		}

		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically within specified interface or class.
		/// To the Hangfire client, alternatively way is to use the class <seealso cref="CronJob"/> to add or update <see cref="RecurringJob"/>.
		/// </summary>
		/// <param name="configuration"><see cref="IGlobalConfiguration"/></param>
		/// <param name="typesProvider">The provider to get specified interfaces or class.</param>
		/// <returns><see cref="IGlobalConfiguration"/></returns>
		public static IGlobalConfiguration UseRecurringJob(this IGlobalConfiguration configuration, Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			CronJob.AddOrUpdate(typesProvider);

			return configuration;
		}		
	}
}
