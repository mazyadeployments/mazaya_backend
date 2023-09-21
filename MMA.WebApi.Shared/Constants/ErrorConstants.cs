namespace MMA.WebApi.Shared.Constants
{
    public class ErrorConstants
    {

        public struct Login
        {
            public const string INVALID_CREDENTIALS = "Invalid credentials";
            public const string MOBILE_ACCESS_FORBIDDEN = "You dont have access for mobile application.";
        }

        public struct MarineActivity
        {
            public static string MarineActivityExists(string code) => $"Marine Activity with {code} already exists.";
        }

        public struct Customer
        {
            public static string CustomerExists(string code) => $"Customer with {code} already exists.";
        }

        public struct Destination
        {
            public static string DestinationExists(string portCode) => $"Destination with {portCode} already exists.";
        }

        public struct User
        {
            public const string USER_NOT_FOUND = "User has not been found.";
        }


        public struct BolText
        {
            public static string BOLTextExists(string code) => $"Bol Text with {code} already exists.";
        }

        public struct Department
        {

        }

        public struct Vessels
        {

        }

        public struct Products
        {

        }

        public struct ProductsSubName
        {
            public static string ProductsSubNameExists(string code) => $"Products SubName with code {code} already exists.";
        }

        public struct Inspector
        {
            public static string InspectorExists(string code) => $"Inspector with {code} already exists.";
        }

        public struct NotifyParties
        {
            public static string NotifyPartiesExists(string code) => $"Notify parties with code {code} already exists.";
        }

        public struct Companies
        {
            public static string CompanyWithNameExists(string name) => $"Company with {name} already exists.";
        }

    }
}
