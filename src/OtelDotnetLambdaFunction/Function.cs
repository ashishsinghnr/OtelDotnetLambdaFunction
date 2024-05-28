using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AWSLambda;
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OtelDotnetLambdaFunction;

public class Function
{
    public static TracerProvider tracerProvider;

    static Function()
        {
           tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddAWSLambdaConfigurations(options => options.DisableAwsXRayContextExtraction = true)
                .AddOtlpExporter()
                .Build();
        }
    /*
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(InputModel input, ILambdaContext context)
    {
        return input.Message.ToUpper();
    }
    */
    
    // use AwsSdkSample::AwsSdkSample.Function::TracingFunctionHandler as input Lambda handler instead
    public APIGatewayProxyResponse TracingFunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        return AWSLambdaWrapper.Trace(tracerProvider, FunctionHandler, request, context);
    }

    /// <summary>
    /// A simple function that takes a APIGatewayProxyRequest and returns a APIGatewayProxyResponse
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context) => new APIGatewayProxyResponse()
    {
        StatusCode = 200,
        Body = Environment.GetEnvironmentVariable(new Guid().ToString())
    };
}
public class InputModel
{
    public string Message { get; set; }
}
