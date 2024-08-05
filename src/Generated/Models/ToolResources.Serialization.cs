// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Assistants
{
    public partial class ToolResources : IJsonModel<ToolResources>
    {
        void IJsonModel<ToolResources>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<ToolResources>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ToolResources)} does not support writing '{format}' format.");
            }

            writer.WriteStartObject();
            if (SerializedAdditionalRawData?.ContainsKey("code_interpreter") != true && Optional.IsDefined(CodeInterpreter))
            {
                writer.WritePropertyName("code_interpreter"u8);
                writer.WriteObjectValue<CodeInterpreterToolResources>(CodeInterpreter, options);
            }
            if (SerializedAdditionalRawData?.ContainsKey("file_search") != true && Optional.IsDefined(FileSearch))
            {
                writer.WritePropertyName("file_search"u8);
                SerializeFileSearch(writer, options);
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

        ToolResources IJsonModel<ToolResources>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<ToolResources>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ToolResources)} does not support reading '{format}' format.");
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeToolResources(document.RootElement, options);
        }

        internal static ToolResources DeserializeToolResources(JsonElement element, ModelReaderWriterOptions options = null)
        {
            options ??= ModelSerializationExtensions.WireOptions;

            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            CodeInterpreterToolResources codeInterpreter = default;
            FileSearchToolResources fileSearch = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("code_interpreter"u8))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    codeInterpreter = CodeInterpreterToolResources.DeserializeCodeInterpreterToolResources(property.Value, options);
                    continue;
                }
                if (property.NameEquals("file_search"u8))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    fileSearch = FileSearchToolResources.DeserializeFileSearchToolResources(property.Value, options);
                    continue;
                }
                if (options.Format != "W")
                {
                    rawDataDictionary ??= new Dictionary<string, BinaryData>();
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
            serializedAdditionalRawData = rawDataDictionary;
            return new ToolResources(codeInterpreter, fileSearch, serializedAdditionalRawData);
        }

        BinaryData IPersistableModel<ToolResources>.Write(ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<ToolResources>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options);
                default:
                    throw new FormatException($"The model {nameof(ToolResources)} does not support writing '{options.Format}' format.");
            }
        }

        ToolResources IPersistableModel<ToolResources>.Create(BinaryData data, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<ToolResources>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    {
                        using JsonDocument document = JsonDocument.Parse(data);
                        return DeserializeToolResources(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ToolResources)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ToolResources>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        internal static ToolResources FromResponse(PipelineResponse response)
        {
            using var document = JsonDocument.Parse(response.Content);
            return DeserializeToolResources(document.RootElement);
        }

        internal virtual BinaryContent ToBinaryContent()
        {
            return BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
        }
    }
}
