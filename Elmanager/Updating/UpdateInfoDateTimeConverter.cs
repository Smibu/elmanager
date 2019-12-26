using Newtonsoft.Json.Converters;

namespace Elmanager.Updating
{
    internal class UpdateInfoDateTimeConverter : IsoDateTimeConverter
    {
        public UpdateInfoDateTimeConverter()
        {
            DateTimeFormat = "d.M.yyyy";
        }
    }
}