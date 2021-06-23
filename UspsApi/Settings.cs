﻿using System.ComponentModel.DataAnnotations;

namespace UspsApi
{
    static class Settings
    {
        /// <summary>
        /// Your USPS API UserId, you can obtain this at https://registration.shippingapis.com/
        /// </summary>
        [Required(ErrorMessage = "UspsApi: You must configure a USPS API UserId before using the API.")]
        public static string UserId { get; set; }

        /// <summary>
        /// The number of times a web request will retry before aborting. Default = 5
        /// (set to 0 for unlimited)
        /// </summary>
        public static int MaxRetries { get; set; }

        /// <summary>
        /// The amount of time in Milliseconds to wait between retries. Default = 2500
        /// </summary>
        public static int RetryDelay { get; set; }
    }
}