using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Gamestore.Database;
public class CustomDateTimeDeserializer : IBsonSerializer<DateTime>
{
    public Type ValueType => typeof(DateTime);

    public DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.CurrentBsonType;
        if (type == BsonType.String)
        {
            var date = context.Reader.ReadString();
            return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
        else
        {
            var date = context.Reader.ReadDateTime();
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(date);
        }
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
    {
        context.Writer.WriteString(value.ToString("yyyy-MM-dd HH:mm:ss.fff"));
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        Serialize(context, args, (DateTime)value);
    }
}