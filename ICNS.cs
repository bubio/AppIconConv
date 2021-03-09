using System;
using System.IO;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Text;

class ICNS
{
    static string getICNSIconType(int width, bool isScale2x)
    {
        string iconType = null;

        switch (width)
        {
            case 16:
                iconType = isScale2x ? null : "icp4";
                break;

            case 32:
                iconType = isScale2x ? "ic11" : "icp5";
                break;

            case 64:
                iconType = isScale2x ? "ic12" : "icp6";
                break;

            case 128:
                iconType = isScale2x ? null : "ic07";
                break;

            case 256:
                iconType = isScale2x ? "ic13" : "ic08";
                break;

            case 512:
                iconType = isScale2x ? "ic14" : "ic09";
                break;

            default:
                iconType = "ic10";
                break;
        }

        return iconType;
    }

    private static byte[] GetBigEndianBytes(Int32 value)
    {
        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }

    public static void ConvertToICNS(string sourceDirectory, string outputFile)
    {
        var icnsData = new List<byte>();
        var sizeAll = 0;

        var files = Directory.EnumerateFiles(sourceDirectory, "*.png", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string sourceFilePath in files)
        {
            // 画像のメタ情報を取得しICNSのタイプを決定します
            string iconType = null;
            using (var image = Image.Load(sourceFilePath))
            {
                var height = image.Height;
                var width = image.Width;
                var isScale2x = false;
                if (Path.GetFileNameWithoutExtension(sourceFilePath).Contains("@2x"))
                {
                    // ファイル名に"@2x"が含まれていれば倍密度と判定
                    isScale2x = true;
                }
                iconType = getICNSIconType(width, isScale2x);
            }

            if (string.IsNullOrEmpty(iconType))
            {
                // 判別不能
                throw new InvalidDataException();
            }

            // タイプを追加
            var iconTypeArray = Encoding.ASCII.GetBytes(iconType);
            icnsData.AddRange(iconTypeArray);

            // サイズと実データを追加
            using (var rs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                // サイズを追加
                var sizeIcon = Convert.ToInt32(rs.Length);
                sizeIcon = 4 + 4 + sizeIcon;
                var sizeIconArray = GetBigEndianBytes(sizeIcon);
                icnsData.AddRange(sizeIconArray);

                // 実データを追加
                byte[] bs = new byte[rs.Length];
                rs.Read(bs);
                icnsData.AddRange(bs);

                // 全体のサイズを更新
                sizeAll = sizeAll + sizeIcon;
            }
        }

        // ICNSファイル作成
        using (var ws = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write))
        {
            // ヘッダの書き込み
            ws.Write(Encoding.ASCII.GetBytes("icns"));
            sizeAll = 4 + 4 + sizeAll;
            var sizeAllArray = GetBigEndianBytes(sizeAll);
            ws.Write(sizeAllArray);

            // データの書き込み
            ws.Write(icnsData.ToArray());
        }
    }
}