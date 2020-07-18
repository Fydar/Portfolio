using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Fydar.Dev.Lambda.EmailToTicket;

[assembly: LambdaGlobalProperties(GenerateMain = false)]

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
// [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<ApplicationJsonSerializerContext>))]
