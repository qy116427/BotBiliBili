﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace BotBiliBili.Utils
{
    class Tools
    {
        public static Bitmap GetImgUrl(string url)
        {
            if (url.EndsWith(".webp"))
            {
                //var data = HttpUtils.GetByte(url);
                //var data1 = new SimpleAnimDecoder().DecodeFromBytes(data);
                //if (data1.FramesCount == 1)
                //    return data1.Frames[0].Image as Bitmap;
                //else
                //    return data1.Frames[new Random().Next(data1.FramesCount - 1)].Image as Bitmap;
                url += "@.jpg";
            }
                return Image.FromStream(HttpUtils.GetData(url)) as Bitmap;
        }
        public static Bitmap ZoomImage(Bitmap bitmap, int destHeight, int destWidth)
        {
            try
            {
                int width = 0, height = 0;
                //按比例缩放             
                int sourWidth = bitmap.Width;
                int sourHeight = bitmap.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if (sourWidth * destHeight > sourHeight * destWidth)
                    {
                        width = destWidth;
                        height = destWidth * sourHeight / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = sourWidth * destHeight / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                Bitmap destBitmap = new(width, height);
                Graphics g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);
                //设置画布的描绘质量           
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
                g.Dispose();
                return destBitmap;
            }
            catch
            {
                return bitmap;
            }
        }
        public static int SubstringCount(string str, string data)
        {
            int index = 0;
            int count = 0;
            while ((index = str.IndexOf(data, index)) != -1)
            {
                count++;
                index += data.Length;
            }
            return count;
        }
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, "^[0-9]*$");
        }
    }
}
