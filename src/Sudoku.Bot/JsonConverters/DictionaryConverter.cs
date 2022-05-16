﻿namespace Sudoku.Bot.JsonConverters;

/// <summary>
/// Defines a dictionary JSON converter that can convert a list of key value pairs into a JSON string.
/// </summary>
internal sealed class DictionaryConverter : JsonConverter<Dictionary<string, string>>
{
	/// <inheritdoc/>
	public override Dictionary<string, string> Read(
		ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			return new();
		}

		var result = new Dictionary<string, string>();
		var jsonElement = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
		foreach (var element in jsonElement.EnumerateObject())
		{
			result.Add(element.Name, element.Value.ToString());
		}

		return reader.Read() && reader.TokenType == JsonTokenType.EndObject ? result : throw new JsonException();
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		foreach (var (name, correspondingValue) in value)
		{
			writer.WriteString(name, correspondingValue);
		}
		writer.WriteEndObject();
	}
}