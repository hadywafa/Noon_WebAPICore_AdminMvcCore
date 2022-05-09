namespace BL.Helpers
{
   public static class StringHelper
    {
        public static string ToImageUrl(this string image)
        {
            return $"http://localhost:25248/Images/{image}";
        }
    }
}
