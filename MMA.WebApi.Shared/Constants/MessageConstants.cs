namespace MMA.WebApi.Shared.Constants
{
    public class MessageConstants
    {
        public struct DeleteMessages
        {
            public static string ItemSuccessfullyDeleted(string item) => $"{item} deleted successfully.";
        }

        public struct SaveMessages
        {
            public static string ItemSuccessfullySaved(string item) => $"{item} saved successfully.";
        }

        public struct UpdateMessages
        {
            public static string ItemSuccessfullyUpdated(string item) => $"{item} updated successfully.";
        }


    }
}
