using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

namespace FontPacker
{
    internal class Program
    {
        private static string _dirMcpcFontBin = @"assets/minecraft/font";
        private static string _dirMcpcFontImg = @"assets/minecraft/textures/font";
        private static string _dirMcpeFontImg = @"font";
        private static byte[] _glyphSizes;
        private static string _fontPackName = "Minecraft Font";
        private static Cfg Cfg { get; set; }


        // Параметры командной строки:
        // FontPacker <НазваниеНабор> [версия]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                _fontPackName = args[0];
            }

            Cfg = Serializer.Deserialize<Cfg>(_fontPackName + @"\config.json");
            
            
            
            Print(_fontPackName + " / " +Cfg.Name);
            Print(Cfg.Description);
            
            _glyphSizes = ReadGlyphBin(); // File.ReadAllBytes("glyph_sizes.bin");
            
            CreatePacks();
        }


        private static void CreatePacks()
        {
            var files = Directory.GetFiles(_fontPackName);
            if (files.Length == 0)
            {
                Console.WriteLine("Source directory is empty");
                return;
            }

            ZipOutputStream mcpeZip = new ZipOutputStream(File.Create(Cfg.Name.Replace(" ","") + ".mcpack"));
            ZipOutputStream mcpcZip = new ZipOutputStream(File.Create(Cfg.Name.Replace(" ","") + ".zip"));
            
            
            
            foreach (var fileName in files)
            {
                if (!fileName.EndsWith(".png")) continue;
                var numStr = Path.GetFileName(fileName).Replace(".png", "");
                if (numStr.Length !=2) continue;
                
                FileStream sourceImg = new FileStream(fileName, FileMode.Open);
                Bitmap img = new Bitmap(Image.FromStream(sourceImg));
                   
                int index = int.Parse(numStr, System.Globalization.NumberStyles.HexNumber);
                int charSize = img.Height / 16;
                
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Rectangle rectangle = new Rectangle(x*16,y*16,charSize, charSize);
                        Bitmap chImg = img.Clone(rectangle, img.PixelFormat);
                        byte data = GetGlyphData(chImg);
                        _glyphSizes[y * 16 + x + index * 256] = data;
                    }
                }
                
                mcpeZip.PutNextEntry(new ZipEntry(_dirMcpeFontImg+"/glyph_"+numStr+".png"));
                mcpcZip.PutNextEntry(new ZipEntry(_dirMcpcFontImg+"/unicode_page_"+numStr+".png"));

                sourceImg.Position = 0;
                sourceImg.CopyTo(mcpcZip);
                sourceImg.Position = 0;
                sourceImg.CopyTo(mcpeZip);
                sourceImg.Dispose();

            }

            
            McpePackManifest packManifest = new McpePackManifest(Cfg);
            mcpeZip.PutNextEntry(new ZipEntry("pack_manifest.json"));
            packManifest.Serialize(mcpeZip);
            
            McpeManifest manifest = new McpeManifest(Cfg);
            mcpeZip.PutNextEntry(new ZipEntry("manifest.json"));
            manifest.Serialize(mcpeZip);
                
            McpcPackMeta packMeta = new McpcPackMeta(Cfg);
            mcpcZip.PutNextEntry(new ZipEntry("pack.mcmeta"));
            packMeta.Serialize(mcpcZip);
            
            FileStream iconImg = new FileStream(_fontPackName + @"\icon.png", FileMode.Open);
            
            mcpcZip.PutNextEntry(new ZipEntry("pack.png"));
            mcpeZip.PutNextEntry(new ZipEntry("pack_icon.png"));
            iconImg.CopyTo(mcpcZip);
            iconImg.Position = 0;
            iconImg.CopyTo(mcpeZip);
            iconImg.Dispose();
            
            mcpeZip.Flush();
            mcpeZip.Dispose();
            mcpcZip.PutNextEntry(new ZipEntry(_dirMcpcFontBin+"/glyph_sizes.bin"));
            mcpcZip.Write(_glyphSizes,0, _glyphSizes.Length);
            mcpcZip.Flush();
            mcpcZip.Dispose();
        }

        private static byte GetGlyphData(Bitmap bitmap)
        {
            int charStart = 0;
            int charEnd = 15;
            for (int x = 0; x < 16; x++)
            {
                bool startFound = false;
                for (int y = 0; y < 16; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c.ToArgb() != 0)
                    {
                        startFound = true;
                    }
                }
                
                if (startFound)
                {
                    charStart = x;
                    break;
                }
            }
            
            for (int x = 15; x >= charStart; x--)
            {
                bool endFound = false;

                for (int y = 0; y < 16; y++)
                {
    

                    Color c = bitmap.GetPixel(x, y);
                    if (c.ToArgb() != 0)
                    {
                        endFound = true;
                    }
                }
                if (endFound)
                {
                    charEnd = x;
                    break;
                }
            }
            if (charEnd <= charStart)
            {
                charEnd = 15;
                charStart = 0;
            }
            return (byte) ((charStart << 4) + (charEnd & 15));
        }


        public static void Print(string s)
        {
            Console.WriteLine(s);
        }
        
 
        /*private static void loadLib()
        {
            foreach (var manifestResourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                Print(manifestResourceName);
            }
            
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FontPacker.lib.ICSharpCodeSharpZipLib.dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                
                Assembly ass =  Assembly.Load(assemblyData);
                   

                //AppDomain.CurrentDomain.Load(assemblyData);
            }
        } */
        
        public static byte[] ReadGlyphBin ()
        {
            Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("FontPacker.glyph_sizes.bin");
            
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}