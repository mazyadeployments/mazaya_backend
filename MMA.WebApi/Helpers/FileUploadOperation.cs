namespace MMA.WebApi
{
    //public class FileUploadOperation : IOperationFilter
    //{
    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        if (context.ApiDescription.RelativePath == "api/Document/{guid}" && context.ApiDescription.HttpMethod == "POST")
    //        {
    //            operation.Parameters.Remove(operation.Parameters[0]);//Clearing parameters
    //            operation.Parameters.Add(new OpenApiParameter
    //            {
    //                Name = "File",
    //                In = "formData",
    //                Description = "Upload file",
    //                Required = true,
    //                Type = "file"
    //            });
    //            operation.Consumes.Add("application/form-data");
    //        }

    //        if (context.ApiDescription.RelativePath == "api/Document/{guid}" && context.ApiDescription.HttpMethod == "GET")
    //        {
    //            operation.Parameters.Remove(operation.Parameters[0]);//Clearing parameters
    //        }
    //    }

    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
