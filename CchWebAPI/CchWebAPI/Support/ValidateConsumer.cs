using System;

namespace CchWebAPI.Support
{
    public static class ValidateConsumer
    {
        public static bool IsValidConsumer(string hsId)
        {
            bool validActiveClient = false;
            using (var vmp = new ValidateMobileProvider(hsId))
                vmp.ForEachProvider(delegate(Boolean valid) { if (valid) validActiveClient = true; });

            return validActiveClient;
        }
    }
}