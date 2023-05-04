using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PdfGenerator
{
    class PdfFontResolver : IFontResolver
    {
        private string _fontPath;
        public PdfFontResolver(string fontPath)
        {
            _fontPath = fontPath;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Ignore case of font names.
            var name = familyName.ToLowerInvariant().TrimEnd('#');

            // Deal with the fonts we know.
            switch (name)
            {
                case "arial":
                    if (isBold)
                    {
                        if (isItalic)
                            return new FontResolverInfo("Arial#bi");
                        return new FontResolverInfo("Arial#b");
                    }
                    if (isItalic)
                        return new FontResolverInfo("Arial#i");
                    return new FontResolverInfo("Arial#");
            }

            // We pass all other font requests to the default handler.
            // When running on a web server without sufficient permission, you can return a default font at this stage.
            return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
        }

        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case "Arial#":
                    return LoadFontData("arial.ttf"); ;

                case "Arial#b":
                    return LoadFontData("arialbd.ttf"); ;

                case "Arial#i":
                    return LoadFontData("ariali.ttf");

                case "Arial#bi":
                    return LoadFontData("arialbi.ttf");
            }

            return null;
        }

        private byte[] LoadFontData(string fontFilename)
        {
            var path = Path.Combine(_fontPath, "Fonts", fontFilename);
            return File.ReadAllBytes(path);
        }

        internal static PdfFontResolver OurGlobalFontResolver = null;

        internal static void Apply(string basePath)
        {
            if (OurGlobalFontResolver == null || GlobalFontSettings.FontResolver == null)
            {
                if (OurGlobalFontResolver == null)
                    OurGlobalFontResolver = new PdfFontResolver(basePath);

                GlobalFontSettings.FontResolver = OurGlobalFontResolver;
            }
        }
    }
}
