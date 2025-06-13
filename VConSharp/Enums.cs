namespace VConSharp
{
    /// <summary>
    /// Defines encoding types used for data in vCon.
    /// </summary>
    public enum Encoding
    {
        /// <summary>
        /// Base64 encoding.
        /// </summary>
        Base64,

        /// <summary>
        /// Base64URL encoding (URL-safe).
        /// </summary>
        Base64Url,

        /// <summary>
        /// JSON encoding.
        /// </summary>
        Json,

        /// <summary>
        /// No specific encoding.
        /// </summary>
        None,
    }
}
