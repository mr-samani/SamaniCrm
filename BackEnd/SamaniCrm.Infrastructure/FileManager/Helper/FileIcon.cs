using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.FileManager.Helper
{
    public static class FileIcon
    {
        public static string GetIcon(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return "";
            }
            switch (extension.ToLower())
            {
                case "apk":
                    return "images/file-icons/apk.svg";
                case "bat":
                    return "images/file-icons/bin.svg";
                case "html":
                case "css":
                case "less":
                case "scss":
                case "cs":
                case "php":
                case "cshtml":
                    return "images/file-icons/code.svg";
                case "config":
                    return "images/file-icons/config.svg";
                case "ttf":
                case "woff":
                case "woff2":
                case "eot":
                    return "images/file-icons/font.svg";
                case "ai":
                    return "images/file-icons/illustrator.svg";
                case "png":
                case "jpg":
                case "jpeg":
                case "bmp":
                case "webp":
                case "svg":
                case "tiff":
                case "gif":
                    return "images/file-icons/images.svg";
                case "exe":
                    return "images/file-icons/install.svg";
                case "java":
                    return "images/file-icons/java.svg";
                case "js":
                    return "images/file-icons/javascript.svg";
                case "json":
                    return "images/file-icons/json.svg";
                case "key":
                    return "images/file-icons/keys.svg";
                case "xls":
                case "xlsx":
                    return "images/file-icons/ms-excel.svg";
                case "ppt":
                case "pptm":
                case "pptx":
                    return "images/file-icons/ms-powerpoint.svg";
                case "doc":
                case "docx":
                    return "images/file-icons/ms-word.svg";
                case "eml":
                    return "images/file-icons/msoutlook.svg";
                case "mp3":
                case "wma":
                case "ogg":
                case "3ga":
                case "wav":
                    return "images/file-icons/music.svg";
                case "pdf":
                    return "images/file-icons/pdf.svg";
                case "psd":
                    return "images/file-icons/photoshop.svg";
                case "rss":
                    return "images/file-icons/rss.svg";
                case "sign":
                    return "images/file-icons/signature.svg";
                case "txt":
                case "rtf":
                    return "images/file-icons/txt.svg";
                case "mp4":
                case "mpeg":
                case "mpg":
                case "wmv":
                case "avi":
                case "mov":
                case "webm":
                case "flv":
                case "mkv":
                    return "images/file-icons/video.svg";
                case "xml":
                    return "images/file-icons/xml.svg";
                case "zip":
                case "rar":
                case "7zip":
                case "tar":
                    return "images/file-icons/zip.svg";
                default:
                    return "images/file-icons/unknown.svg";
            }
        }

    }
}
