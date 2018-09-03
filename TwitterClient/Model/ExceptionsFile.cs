using System;
using System.IO;
using System.Text;

namespace TwitterClient.Model
{
    public static class ExceptionsFile
    {
        private static string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exceptions.txt");

        public static void AppendExceptionToFile(Exception ex)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine($"Exception DateTime: {DateTime.Now.ToLocalTime().ToString()}");
            strBuilder.AppendLine($"Exception.Message: {ex.Message}");
            strBuilder.AppendLine($"Exception.StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
                strBuilder.AppendLine($"Exception.InnerException: {ex.InnerException.Message}");

            File.AppendAllText(_filePath, strBuilder.ToString());
        }
    }
}
