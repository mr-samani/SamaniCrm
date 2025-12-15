using MediatR;
using SamaniCrm.Application.FileManager.Commands;
using SamaniCrm.Core;
using System.Net;
using System.Text;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Stores;
namespace SamaniCrm.Api.TUS
{
    public static class TusRouteMap
    {

        public static WebApplication InitializeTUS(this WebApplication app, IConfiguration configuration)
        {
            var path = configuration.GetSection("Filemanger:tusfiles").Value ?? "./tusfiles";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            app.MapTus("/api/tus", async httpContext => new()
            {
                // This method is called on each request so different configurations can be returned per user, domain, path etc.
                // Return null to disable tusdotnet for the current request.

                // Where to store data?
                Store = new tusdotnet.Stores.TusDiskStore(path),
                Events = new()
                {
                    OnAuthorizeAsync = ctx =>
                    {
                        ctx.HttpContext.Request.EnableBuffering();
                        if (!ctx.HttpContext.User.Identity?.IsAuthenticated ?? true)
                            ctx.FailRequest(HttpStatusCode.Unauthorized.ToString());
                        return Task.CompletedTask;
                    },
                    OnBeforeCreateAsync = ctx =>
                    {
                        var meta = ctx.Metadata;
                        if (!meta.ContainsKey("parentId"))
                            ctx.FailRequest("parentId is required");
                        if (!meta.ContainsKey("filename") || !meta.ContainsKey("filetype"))
                            ctx.FailRequest("filename and filetype are required");
                        if (!AppConsts.AllowedTusUploadTypes.Contains(meta["filetype"].GetString(UTF8Encoding.UTF8)))
                        {
                            ctx.FailRequest("unsupported file type");
                        }

                        return Task.CompletedTask;
                    },
                    // What to do when file is completely uploaded?
                    OnFileCompleteAsync = async eventContext =>
                    {
                        tusdotnet.Interfaces.ITusFile file = await eventContext.GetFileAsync();
                        Dictionary<string, tusdotnet.Models.Metadata> metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
                        metadata.TryGetValue("parentId", out var parentId);

                        await using (Stream content = await file.GetContentAsync(eventContext.CancellationToken))
                        {
                            var id = Guid.Parse(file.Id);
                            var size = content.Length;
                            UploadFileCommand input = new UploadFileCommand()
                            {
                                FileStreem = content,
                                FolderId = parentId!.GetString(UTF8Encoding.UTF8),
                                FileName = metadata["filename"].GetString(UTF8Encoding.UTF8),
                                filetype = metadata["filetype"].GetString(UTF8Encoding.UTF8)

                            };
                            var mediator = eventContext.HttpContext.RequestServices.GetRequiredService<IMediator>();
                            Guid result = await mediator.Send(input, eventContext.CancellationToken);

                            eventContext.HttpContext.Response.Headers.Append("FileId", result.ToString());
                        }

                        // 🔥 حذف فایل chunk
                        if (eventContext.Store is TusDiskStore store)
                        {
                            await store.DeleteFileAsync(file.Id, eventContext.CancellationToken);
                        }

                    }
                },
            });
            return app;
        }




    }
}
