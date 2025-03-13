using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HelloGreetingApplication
{
    public class CustomSwaggerOperations : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var descriptions = new Dictionary<string, string>
        {
            { "GetMethod", "Retrieves a greeting message." },
            { "Post", "Adds a new greeting." },
            { "Put", "Updates an existing greeting." },
            { "Delete", "Removes a greeting by key." },
            { "Patch", "Partially updates a greeting." },
            { "GetGreeting", "Fetches the greeting message from business logic." },
            { "PostGreeting", "Creates a personalized greeting using request body." },
            { "GetGreetingById", "Retrieves a greeting by its ID." },
            { "GetAllGreetings", "Retrieves all stored greetings." },
            { "UpdateGreeting", "Updates a greeting by ID." },
            { "DeleteGreeting", "Deletes a greeting by ID." }
        };

            if (descriptions.ContainsKey(context.MethodInfo.Name))
            {
                operation.Summary = descriptions[context.MethodInfo.Name];
                operation.Description = $"API Endpoint: `{context.ApiDescription.HttpMethod} {context.ApiDescription.RelativePath}`";
            }
        }
    }
}
