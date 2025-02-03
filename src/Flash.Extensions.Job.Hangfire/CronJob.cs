using System;
using System.Collections.Generic;
using System.Linq;

namespace Flash.Extensions.Job.Hangfire
{
    /// <summary>
    /// The helper class to build <see cref="RecurringJob"/> automatically.
    /// </summary>
    public class CronJob
	{
		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically within specified interface or class.
		/// </summary>
		/// <param name="types">Specified interface or class</param>
		public static void AddOrUpdate(params Type[] types)
		{
			AddOrUpdate(() => types);
		}

		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically within specified interface or class.
		/// </summary>
		/// <param name="typesProvider">The provider to get specified interfaces or class.</param>
		public static void AddOrUpdate(Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			IRecurringJobBuilder builder = new RecurringJobBuilder();

			builder.Build(typesProvider);
		}

		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically with the collection of <seealso cref="RecurringJobInfo"/>.
		/// </summary>
		/// <param name="recurringJobInfos">The collection of <see cref="RecurringJobInfo"/>.</param>
		public static void AddOrUpdate(IEnumerable<RecurringJobInfo> recurringJobInfos)
		{
			if (recurringJobInfos == null) throw new ArgumentNullException(nameof(recurringJobInfos));

			IRecurringJobBuilder builder = new RecurringJobBuilder();

			builder.Build(() => recurringJobInfos);
		}

		/// <summary>
		/// Builds <see cref="RecurringJob"/> automatically with the array of <seealso cref="RecurringJobInfo"/>.
		/// </summary>
		/// <param name="recurringJobInfos">The array of <see cref="RecurringJobInfo"/>.</param>
		public static void AddOrUpdate(params RecurringJobInfo[] recurringJobInfos)
		{
			if (recurringJobInfos == null) throw new ArgumentNullException(nameof(recurringJobInfos));

			AddOrUpdate(recurringJobInfos.AsEnumerable());
		}
	}
}
