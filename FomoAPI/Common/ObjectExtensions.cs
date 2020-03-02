
namespace FomoAPI.Common
{
    public static class ObjectExtensions
    {
        public static int CalculateHashCodeForParams(this object obj, params object[] fields)
        {
            int hash = 17;

            foreach (var field in fields)
            {
                hash = hash * 23 + field.GetHashCode();
            }

            return hash;
        }
    }
}
