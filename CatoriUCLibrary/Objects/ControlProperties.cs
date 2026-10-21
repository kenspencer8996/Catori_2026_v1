using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriUCLibrary.Objects
{
    public class ControlProperties
    {
        public Dictionary<string, string> Properties { get; set; } = new();
        /// <summary>
        /// Add or update a property value by key.
        /// </summary>
        public void AddProperty(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Property key cannot be null or whitespace.", nameof(key));

            Properties[key] = value ?? string.Empty;
        }

        /// <summary>
        /// Get a property value by key. Returns null when the key does not exist.
        /// </summary>
        public string? GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return Properties.TryGetValue(key, out var value) ? value : null;
        }

    }
}
