using System;

namespace web.Utilities
{
    public interface IBase64QrCodeGenerator
    {
        string Generate(Uri target);
    }
}
