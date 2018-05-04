﻿using System.Collections.Generic;
using System.IO;
using CSharpTest.Net.Serialization;
using Umbraco.Core;

namespace Umbraco.Web.PublishedCache.NuCache.DataSource
{
    internal class DictionaryOfCultureVariationSerializer : SerializerBase, ISerializer<IReadOnlyDictionary<string, CultureVariation>>
    {
        public IReadOnlyDictionary<string, CultureVariation> ReadFrom(Stream stream)
        {
            var dict = new Dictionary<string, CultureVariation>();

            // read variations count
            var pcount = PrimitiveSerializer.Int32.ReadFrom(stream);

            // read each variation
            for (var i = 0; i < pcount; i++)
            {
                var languageId = PrimitiveSerializer.String.ReadFrom(stream);
                var cultureVariation = new CultureVariation { Name = ReadStringObject(stream) };
                dict[languageId] = cultureVariation;
            }
            return dict;
        }

        private static readonly IReadOnlyDictionary<string, CultureVariation> Empty = new Dictionary<string, CultureVariation>();

        public void WriteTo(IReadOnlyDictionary<string, CultureVariation> value, Stream stream)
        {
            var variations = value ?? Empty;

            // write variations count
            PrimitiveSerializer.Int32.WriteTo(variations.Count, stream);

            // write each variation
            foreach (var (culture, variation) in variations)
            {
                // fixme - it's weird we're dealing with cultures here, and languageId in properties

                PrimitiveSerializer.String.WriteTo(culture, stream); // should never be null
                WriteObject(variation.Name, stream); // write an object in case it's null (though... should not happen)
            }
        }
    }
}