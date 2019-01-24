using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Constants
{
    public enum FileType
    {
        [ContentType("image/png")]
        PNG = 0,

        [ContentType("image/jpg")]
        JPG = 1,

        [ContentType("image/jpeg")]
        JPEG = 2,

        [ContentType("image/bmp")]
        BMP = 3,

        [ContentType("image/gif")]
        GIF = 4,

        [ContentType("application/pdf")]
        PDF = 5,

        [ContentType("text/plain")]
        TXT = 6,

        [ContentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        DOCX = 7
    }
}
