using System;
using CommandLine;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace appicongen
{

    class Program
    {
        // macOS用の必要画像一覧です
        static (int Width, string Name, double Dpi)[] macIconSet = new (int Width, string Name, double Dpi)[]
        {
            (16, "icon_16x16", 72.0),
            (32, "icon_16x16@2x", 144.0),
            (32, "icon_32x32", 72.0),
            (64, "icon_32x32@2x", 144.0),
            (128, "icon_128x128", 72.0),
            (256, "icon_128x128@2x", 144.0),
            (256, "icon_256x256", 72.0),
            (512, "icon_256x256@2x", 144.0),
            (512, "icon_512x512", 72.0),
            (1024, "icon_512x512@2x", 144.0),
        };

        // Windows用の必要画像一覧です
        static (int Width, string Name, double Dpi)[] winIconSet = new (int Width, string Name, double Dpi)[]
        {
            (16, "icon_16x16", 96.0),
            (24, "icon_24x24", 96.0),
            (32, "icon_32x32", 96.0),
            (48, "icon_48x48", 96.0),
            (64, "icon_64x64", 96.0),
            (128, "icon_128x128", 96.0),
            (256, "icon_256x256", 96.0),
        };

        // 画像のサイズを変更します
        static void ResizeImage(string sourceFilePath, string outputDirectory, (int Width, string Name, double Dpi)[] outputInfo)
        {
            Directory.CreateDirectory(outputDirectory);

            // macOS用の画像ファイル群を作成します
            foreach (var imageInfo in outputInfo)
            {
                using (Image image = Image.Load(sourceFilePath))
                {
                    image.Mutate(x => x.Resize(imageInfo.Width, imageInfo.Width));
                    var filePath = Path.ChangeExtension(Path.Combine(outputDirectory, imageInfo.Name), "png");
                    image.Save(filePath);
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                       .WithParsed<Options>(o =>
                       {
                           var outputPath = Directory.GetCurrentDirectory();
                           if (!string.IsNullOrEmpty(o.dest))
                           {
                               // 指定がある
                               if (Directory.Exists(o.dest))
                               {
                                   // 出力先が存在する
                                   outputPath = o.dest;
                               }
                               else
                               {
                                   // 出力先が存在しない、もしくはファイル
                                   throw new ArgumentException("Ouput Directory Not Found.");
                               }
                           }

                           // ソースがフォルダ
                           //   画像ファイルが用意されているものとしてICO or ICNSを作成
                           // ソースがファイル
                           //   必要な画像をリサイズしてから、ICO or ICNSを作成
                           if (File.Exists(o.source))
                           {
                               if (o.type == IconType.BOTH || o.type == IconType.ICNS)
                               {
                                   // macOS用の画像群を作成します
                                   var macIconSetPath = Path.Combine(outputPath, "mac.iconset");
                                   ResizeImage(o.source, macIconSetPath, macIconSet);

                                   // macOS用のICNSファイルを作成します
                                   var icnsFilePath = Path.ChangeExtension(Path.Combine(outputPath, "macOS"), "icns");
                                   ICNS.ConvertToICNS(macIconSetPath, icnsFilePath);
                               }

                               if (o.type == IconType.BOTH || o.type == IconType.ICO)
                               {
                                   // Windows用の画像群を作成します
                                   var winIconSetPath = Path.Combine(outputPath, "win.iconset");
                                   ResizeImage(o.source, winIconSetPath, winIconSet);

                                   // Windows用のICOファイルを作成します
                                   var icnsFilePath = Path.ChangeExtension(Path.Combine(outputPath, "win"), "ico");
                                   ICO.ConvertToICO(winIconSetPath, icnsFilePath);
                               }
                           }
                           else if (Directory.Exists(o.source))
                           {
                               if (o.type == IconType.BOTH || o.type == IconType.ICNS)
                               {
                                   // macOS用のICNSファイルを作成します
                                   var icnsFilePath = Path.ChangeExtension(Path.Combine(outputPath, "macOS"), "icns");
                                   ICNS.ConvertToICNS(o.source, icnsFilePath);
                               }

                               if (o.type == IconType.BOTH || o.type == IconType.ICO)
                               {
                                   // Windows用のICOファイルを作成します
                                   var icnsFilePath = Path.ChangeExtension(Path.Combine(outputPath, "win"), "ico");
                                   ICO.ConvertToICO(o.source, icnsFilePath);
                               }
                           }
                           else
                           {
                               // 存在しない
                               Console.WriteLine("Input File or Directory Not Found.");
                               return;
                           }
                       });
            }
            catch (Exception e)
            {
                // Calling Environment.Exit() is required, to force all background threads to exit as well
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}
