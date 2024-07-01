using OpenAI.Files;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Models;

public partial class OpenAIBatchInfo : IJsonModel<OpenAIBatchInfo>
{
    public OpenAIBatchInfo Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        using JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
        throw new NotImplementedException();
    }

    public OpenAIBatchInfo Create(BinaryData data, ModelReaderWriterOptions options)
    {
        return ModelReaderWriter.Read<OpenAIBatchInfo>(data) ?? new();
    }

    public string GetFormatFromOptions(ModelReaderWriterOptions options)
    {
        throw new NotImplementedException();
    }

    public void Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        throw new NotImplementedException();
    }

    public BinaryData Write(ModelReaderWriterOptions options)
    {
        throw new NotImplementedException();
    }
}


