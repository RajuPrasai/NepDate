using Newtonsoft.Json;
using System.Text.Json;

namespace NepDate.Serialization
{
    /// <summary>
    /// Provides extension methods for configuring serialization of <see cref="NepaliDate"/> in different serialization frameworks.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Configures System.Text.Json to support serialization of <see cref="NepaliDate"/> using the string format.
        /// </summary>
        /// <param name="options">The JsonSerializerOptions to configure.</param>
        /// <param name="useObjectFormat">When true, serialize as a JSON object with Year, Month, and Day properties; 
        /// when false (default), serialize as a string in ISO format (YYYY-MM-DD).</param>
        /// <returns>The JsonSerializerOptions for chaining.</returns>
        public static JsonSerializerOptions ConfigureForNepaliDate(this JsonSerializerOptions options, bool useObjectFormat = false)
        {
            if (useObjectFormat)
            {
                options.Converters.Add(new SystemTextJsonConverters.NepaliDateObjectJsonConverter());
            }
            else
            {
                options.Converters.Add(new SystemTextJsonConverters.NepaliDateJsonConverter());
            }

            return options;
        }

        /// <summary>
        /// Configures Newtonsoft.Json to support serialization of <see cref="NepaliDate"/> using the string format.
        /// </summary>
        /// <param name="settings">The JsonSerializerSettings to configure.</param>
        /// <param name="useObjectFormat">When true, serialize as a JSON object with Year, Month, and Day properties; 
        /// when false (default), serialize as a string in ISO format (YYYY-MM-DD).</param>
        /// <returns>The JsonSerializerSettings for chaining.</returns>
        public static JsonSerializerSettings ConfigureForNepaliDate(this JsonSerializerSettings settings, bool useObjectFormat = false)
        {
            if (settings.Converters == null)
            {
                settings.Converters = new System.Collections.Generic.List<JsonConverter>();
            }

            if (useObjectFormat)
            {
                settings.Converters.Add(new NewtonsoftJsonConverters.NepaliDateObjectJsonConverter());
            }
            else
            {
                settings.Converters.Add(new NewtonsoftJsonConverters.NepaliDateJsonConverter());
            }

            return settings;
        }
    }
}