﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusExample.Contracts;

namespace ServiceBusExample
{
    internal sealed class DeserializerFactory<T> : IDeserializerFactory<T>
    {
        private readonly IEnumerable<IDeserializer<T>> _deserializers;
        private readonly ILogger<DeserializerFactory<T>> _logger;

        public DeserializerFactory(IEnumerable<IDeserializer<T>> deserializers, ILogger<DeserializerFactory<T>> logger)
        {
            _deserializers = deserializers;
            _logger = logger;
        }
        
        public T Deserialize(string contentType, byte[] body)
        {
            try
            {
                var types = contentType?.Split(";");
                var applicationType = types?.FirstOrDefault()?.Split("/")?.Last();
                var charset= types?.LastOrDefault()?.Split("=")?.Last();
                if (string.IsNullOrWhiteSpace(applicationType) || string.IsNullOrWhiteSpace(charset))
                {
                    throw new NotSupportedException(contentType);
                }

                var deserializer =
                    _deserializers.SingleOrDefault(d => nameof(d).Contains(applicationType, StringComparison.OrdinalIgnoreCase) && nameof(d).Contains(charset, StringComparison.OrdinalIgnoreCase));

                if (deserializer is null)
                {
                    throw new NotSupportedException(contentType);
                }

                return deserializer.Deserialize(body);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "deserialization_error");
                throw new NotSupportedException(contentType, e);
            }
        }
    }
}