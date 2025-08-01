using System;
using System.Globalization;
using Newtonsoft.Json;

public class FloatTrimConverter : JsonConverter<float>
{
    public override void WriteJson ( JsonWriter writer , float value , JsonSerializer serializer )
    {
        writer.WriteRawValue(value.ToString("0.0########" , CultureInfo.InvariantCulture));
    }

    public override float ReadJson ( JsonReader reader , Type objectType , float existingValue , bool hasExistingValue , JsonSerializer serializer )
    {
        return Convert.ToSingle(reader.Value);
    }

    public override bool CanRead => true;
}

