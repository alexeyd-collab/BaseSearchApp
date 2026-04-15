namespace SearchApp.Constants
{
    public static class AppConstants
    {
        public static class Configuration
        {
            public const string GitHubApiSettingsSection = "GitHubApiSettings";
            public const string StorageSettingsSection = "StorageSettings";
        }

        public static class Logging
        {
            public const string LogFilePath = "Logs/searchapp-log-.txt";
        }

        public static class Telemetry
        {
            public const string ServiceName = "SearchApp";
        }

        public static class Routing
        {
            public const string ErrorRoute = "/Error";
            public const string RootRoute = "/";
        }

        public static class Development
        {
            public const string FrontendDevUrl = "http://localhost:5173";
        }
        
        public static class Headers
        {
            public const string Accept = "Accept";
            public const string UserAgent = "User-Agent";
        }
    }
}
