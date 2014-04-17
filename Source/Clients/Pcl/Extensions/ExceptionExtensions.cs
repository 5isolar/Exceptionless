﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Exceptionless {
    public static class ExceptionExtensions {
        internal static Exception GetInnermostException(this Exception exception) {
            if (exception == null)
                return null;

            Exception current = exception;
            while (current.InnerException != null)
                current = current.InnerException;

            return current;
        }

        internal static string GetMessage(this Exception exception) {
            if (exception == null)
                return String.Empty;

            if (exception is AggregateException)
                return String.Join(Environment.NewLine, ((AggregateException)exception).InnerExceptions.Where(ex => !String.IsNullOrEmpty(ex.Message)).Select(ex => ex.Message));

            return exception.Message;
        }

        /// <summary>
        /// Creates a builder object for constructing error reports in a fluent api.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="addDefaultInformation">Wether the default information should be included in the report.</param>
        /// <param name="pluginContextData">
        /// Any contextual data objects to be used by Exceptionless plugins to gather default
        /// information for inclusion in the report information.
        /// </param>
        /// <param name="client">
        /// The ExceptionlessClient instance used for configuration. If a client is not specified, it will use
        /// ExceptionlessClient.Current.
        /// </param>
        /// <returns></returns>
        public static EventBuilder ToExceptionless(this Exception exception, bool addDefaultInformation = true, IDictionary<string, object> pluginContextData = null, ExceptionlessClient client = null) {
            if (client == null)
                client = ExceptionlessClient.Current;

            var builder = new EventBuilder(ExceptionlessClient.ToEvent(client, exception));
            return addDefaultInformation ? builder.AddDefaultInformation(pluginContextData) : builder;
        }
    }
}