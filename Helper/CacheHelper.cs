namespace CacheLibrary.Helper
{
    public static class CacheHelper
    {

        /// <summary>
        /// Validates the specified cache key.
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the key is null or empty.</exception>
        public static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
            }
        }
    }
}
