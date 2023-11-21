using System.Globalization;

namespace BanPhimCanhCach.Helpers
{
    public static class DateTimeConverter
    {
        public static DateTime ToDateTime(DateTime dateTime) {
            CultureInfo culutreInfo = new CultureInfo("vi-VN");

            return dateTime.ToLocalTime();
        }
    }
}
