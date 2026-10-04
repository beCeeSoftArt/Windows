namespace ndow.Base
{
    internal class Os
    {
        /// <summary>
        /// Gets the command line parameters as a list of lists of strings.
        /// </summary>
        private static List<List<string>>? _CommandLineParameters;

        /// <summary>
        /// Gets the command line parameters as a list of lists of strings.
        /// </summary>
        public static List<List<string>> CommandLineParameters
        {
            get
            {
                if (_CommandLineParameters == null)
                    _CommandLineParameters = GetCommandLineParameters();
                return _CommandLineParameters;
            }
        }

        /// <summary>
        /// Gets the command line parameters.
        /// </summary>
        /// <returns></returns>
        private static List<List<string>> GetCommandLineParameters()
        {
            var commandLineParameters = new List<List<string>>();

            var args = Environment.GetCommandLineArgs();

            var options = new List<string>();
            foreach (var argument in args)
            {
                if (argument.StartsWith("-"))
                {
                    commandLineParameters.Add(options);
                    options = new List<string> { argument };
                }
                else
                    options.Add(argument);
            }
            commandLineParameters.Add(options);

            return (commandLineParameters);
        }

        /// <summary>
        /// Gets the command line parameter value for the given parameter names.
        /// </summary>
        /// <param name="parameterNames"></param>
        /// <returns></returns>
        public static string GetCommandLineParameter(List<string> parameterNames)
        {
            var existingParameter = CommandLineParameters?
                .SelectMany(n => n)
                .FirstOrDefault(
                    n => parameterNames.Any(
                        name => n.StartsWith(
                            name,
                            StringComparison.InvariantCultureIgnoreCase)));


            // Remove the parameter name and colon from the existing parameter to get the value
            if (existingParameter != null)
            {
                var parameterValue = existingParameter.Split(':', 2);
                if (parameterValue.Length > 1)
                    existingParameter = parameterValue[1];
            }

            return existingParameter ?? string.Empty;
        }

        /// <summary>
        /// Gets the parameter value from the command line parameters.
        /// </summary>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        /// <param name="parameterNames">The names of the parameter to search for.</param>
        /// <param name="defaultValue">The default value to return if the parameter is not found.</param>
        /// <returns>The value of the parameter, or the default value if not found.</returns>
        public static T? GetParameterValue<T>(List<string> parameterNames, T? defaultValue = default)
        {
            var parameterValue = GetCommandLineParameter(parameterNames);
            if (string.IsNullOrEmpty(parameterValue))
                return defaultValue;
            try
            {
                return (T)Convert.ChangeType(parameterValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

    }
}
