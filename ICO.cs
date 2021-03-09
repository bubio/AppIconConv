using System;
using System.IO;
using SixLabors.ImageSharp;
using System.Collections.Generic;

class ICO
{
    private static byte[] GetLitteEndianBytes(Int32 value)
    {
        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }
    private static byte[] GetLitteEndianBytes(Int16 value)
    {
        var bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }

    public static void ConvertToICO(string sourceDirectory, string outputFile)
    {
        var icoData = new List<byte>();

        var files = Directory.GetFiles(sourceDirectory, "*.png", SearchOption.AllDirectories);
        long offset = (files.Length * 16) + 6;
        foreach (string sourceFilePath in files)
        {
            // 画像のサイズを取得
            var height = 0;
            var width = 0;
            using (var image = Image.Load(sourceFilePath))
            {
                // 256以上は0になります
                height = image.Height > 255 ? 0 : image.Height;
                width = image.Width > 255 ? 0 : image.Width;
            }

            // サイズと実データを追加
            using (var rs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                // ICONDIRENTRY
                icoData.Add(Convert.ToByte(width)); // Width
                icoData.Add(Convert.ToByte(height));  // Height
                icoData.Add((byte)0); // Color Count, 0 = more than 256
                icoData.Add((byte)0); // must be 0
                icoData.AddRange(GetLitteEndianBytes(Convert.ToInt16(1))); // color planes, should be 0 or 1
                icoData.AddRange(GetLitteEndianBytes(Convert.ToInt16(32))); // color planes, should be 0 or 1
                var sizeIcon = Convert.ToInt32(rs.Length);
                var sizeIconArray = GetLitteEndianBytes(sizeIcon);
                icoData.AddRange(sizeIconArray);

                // ファイル先頭から実データまでのバイト数です
                var offsetArray = GetLitteEndianBytes(Convert.ToInt32(offset));
                icoData.AddRange(offsetArray);

                offset = offset + rs.Length;
            }
        }

        foreach (string sourceFilePath in files)
        {
            // サイズと実データを追加
            using (var rs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                // 実データを追加
                byte[] bs = new byte[rs.Length];
                rs.Read(bs);
                icoData.AddRange(bs);
            }
        }

        // ICOファイル作成
        using (var ws = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write))
        {
            // ICONDIR
            ws.Write(GetLitteEndianBytes(Convert.ToInt16(0)));              // must be 0
            ws.Write(GetLitteEndianBytes(Convert.ToInt16(1)));              // 1 = ico file
            ws.Write(GetLitteEndianBytes(Convert.ToInt16(files.Length)));   // number of sizes

            // データの書き込み
            ws.Write(icoData.ToArray());
        }
    }
}