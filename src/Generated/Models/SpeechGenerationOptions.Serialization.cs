// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Audio
{
    public partial class SpeechGenerationOptions : IJsonModel<SpeechGenerationOptions>
    {
        void IJsonModel<SpeechGenerationOptions>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<SpeechGenerationOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(SpeechGenerationOptions)} does not support writing '{format}' format.");
            }

            writer.WriteStartObject();
            if (SerializedAdditionalRawData?.ContainsKey("model") != true)
            {
                writer.WritePropertyName("model"u8);
                writer.WriteStringValue(Model.ToString());
            }
            if (SerializedAdditionalRawData?.ContainsKey("input") != true)
            {
                writer.WritePropertyName("input"u8);
                writer.WriteStringValue(Input);
            }
            if (SerializedAdditionalRawData?.ContainsKey("voice") != true)
            {
                writer.WritePropertyName("voice"u8);
                writer.WriteStringValue(Voice.ToSerialString());
            }
            if (SerializedAdditionalRawData?.ContainsKey("response_format") != true && Optional.IsDefined(ResponseFormat))
            {
                writer.WritePropertyName("response_format"u8);
                writer.WriteStringValue(ResponseFormat.Value.ToSerialString());
            }
            if (SerializedAdditionalRawData?.ContainsKey("speed") != true && Optional.IsDefined(Speed))
            {
                writer.WritePropertyName("speed"u8);
                writer.WriteNumberValue(Speed.Value);
            }
            if (SerializedAdditionalRawData != null)
            {
                foreach (var item in SerializedAdditionalRawData)
                {
                    if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                    {
                        continue;
                    }
                    writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
				writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
                }
            }
            writer.WriteEndObject();
        }

        SpeechGenerationOptions IJsonModel<SpeechGenerationOptions>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<SpeechGenerationOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(SpeechGenerationOptions)} does not support reading '{format}' format.");
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeSpeechGenerationOptions(document.RootElement, options);
        }

        internal static SpeechGenerationOptions DeserializeSpeechGenerationOptions(JsonElement element, ModelReaderWriterOptions options = null)
        {
            options ??= ModelSerializationExtensions.WireOptions;

            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalCreateSpeechRequestModel model = default;
            string input = default;
            GeneratedSpeechVoice voice = default;
            GeneratedSpeechFormat? responseFormat = default;
            float? speed = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("model"u8))
                {
                    model = new InternalCreateSpeechRequestModel(property.Value.GetString());
                    continue;
                }
                if (property.NameEquals("input"u8))
                {
                    input = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("voice"u8))
                {
                    voice = property.Value.GetString().ToGeneratedSpeechVoice();
                    continue;
                }
                if (property.NameEquals("response_format"u8))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    responseFormat = property.Value.GetString().ToGeneratedSpeechFormat();
                    continue;
                }
                if (property.NameEquals("speed"u8))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    speed = property.Value.GetSingle();
                    continue;
                }
                if (options.Format != "W")
                {
                    rawDataDictionary ??= new Dictionary<string, BinaryData>();
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
            serializedAdditionalRawData = rawDataDictionary;
            return new SpeechGenerationOptions(
                model,
                input,
                voice,
                responseFormat,
                speed,
                serializedAdditionalRawData);
        }

        BinaryData IPersistableModel<SpeechGenerationOptions>.Write(ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<SpeechGenerationOptions>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options);
                default:
                    throw new FormatException($"The model {nameof(SpeechGenerationOptions)} does not support writing '{options.Format}' format.");
            }
        }

        SpeechGenerationOptions IPersistableModel<SpeechGenerationOptions>.Create(BinaryData data, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<SpeechGenerationOptions>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    {
                        using JsonDocument document = JsonDocument.Parse(data);
                        return DeserializeSpeechGenerationOptions(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(SpeechGenerationOptions)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<SpeechGenerationOptions>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        internal static SpeechGenerationOptions FromResponse(PipelineResponse response)
        {
            using var document = JsonDocument.Parse(response.Content);
            return DeserializeSpeechGenerationOptions(document.RootElement);
        }

        internal virtual BinaryContent ToBinaryContent()
        {
            return BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
        }
    }
}
