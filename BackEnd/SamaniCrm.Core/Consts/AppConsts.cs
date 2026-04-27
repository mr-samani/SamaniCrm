using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core;

public static class AppConsts
{
    public const string DefaultLanguage = "fa-IR";

    // used in github external login , ....
    public static string AppName = "SamaniCrm";


    public static string[] AllowedTusUploadTypes { get; set; } = [
        // images
        "jpg", "png", "jpeg", "gif", "hiec", "tiff",
        // documents
        "pdf","doc", "docx" ,
        // audio
        "mp3","ogg",
        // video
        "mp4","3gp","avi" ];
    public static Guid PluginsFolderId { get; set; } = Guid.Parse("CA49546C-4BAD-4924-80AF-2DF8E641387F");
}

