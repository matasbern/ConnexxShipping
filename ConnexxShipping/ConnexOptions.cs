namespace ConnexxShipping
{
    /// <summary>
    /// Defines optional settings for the ConnexxClient, including the base URL.
    /// </summary>
    public class ConnexxOptions
    {
        /// <summary>
        /// The base URL for all Connexxx API calls. Defaults to https://api.connexx.co.uk/api/v1/
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.connexx.co.uk/api/v1/";
    }
}
