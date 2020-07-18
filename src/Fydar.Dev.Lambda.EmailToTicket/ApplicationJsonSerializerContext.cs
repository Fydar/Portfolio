using Amazon.Lambda.SimpleEmailEvents;
using Amazon.Lambda.SimpleEmailEvents.Actions;
using System.Text.Json.Serialization;

namespace Fydar.Dev.Lambda.EmailToTicket;

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// </summary>
[JsonSerializable(typeof(SimpleEmailEvent<LambdaReceiptAction>))]
public partial class ApplicationJsonSerializerContext : JsonSerializerContext
{
}
