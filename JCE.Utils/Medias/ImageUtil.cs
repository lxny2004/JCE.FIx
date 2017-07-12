﻿/************************************************************************************
 * Copyright (c) 2017 All Rights Reserved. 
 * CLR版本：4.0.30319.42000
 * 机器名称：JIAN
 * 命名空间：JCE.Utils.Medias
 * 文件名：ImageUtil
 * 版本号：v1.0.0.0
 * 唯一标识：2e3cd6d3-3acd-42d6-9904-1931f09db773
 * 当前的用户域：JIAN
 * 创建人：简玄冰
 * 电子邮箱：jianxuanhuo1@126.com
 * 创建时间：2017/3/5 15:19:51
 * 描述：
 *
 * =====================================================================
 * 修改标记：
 * 修改时间：2017/3/5 15:19:51
 * 修改人：简玄冰
 * 版本号：v1.0.0.0
 * 描述：
 *
/************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JCE.Utils.VerifyCodes;
using Encoder = System.Drawing.Imaging.Encoder;

namespace JCE.Utils.Medias
{
    /// <summary>
    /// 图片操作工具类
    /// </summary>
    public class ImageUtil
    {
        #region 变量
        /// <summary>
        /// 文本字体数组
        /// </summary>
        private static readonly string[] Fonts = { "Arial", "courier new", "微软雅黑", "宋体" };
        /// <summary>
        /// 文本样式数组
        /// </summary>
        private static readonly FontStyle[] Styles = { FontStyle.Regular, FontStyle.Bold, FontStyle.Bold, FontStyle.Bold };
        #endregion

        #region MakeThumbnail(生成缩略图)
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height,
            ThumbnailMode mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailMode.FixedBoth:
                    break;
                case ThumbnailMode.FixedW:
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.FixedH:
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //1、新建一个BMP图片
            Image bitmap = new Bitmap(towidth, toheight);
            //2、新建一个画板
            Graphics g = Graphics.FromImage(bitmap);
            //3、设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //4、设置高质量，低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //5、清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //6、在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        /// <summary>
        /// 转换生成缩略图
        /// </summary>
        /// <param name="imgByte">缓存字节流</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="mode">缩略图方式</param>
        /// <returns></returns>
        public static Image MakeThumbnail(byte[] imgByte, int width, int height, ThumbnailMode mode)
        {
            Image originalImage = ByteToImage(imgByte);
            return MakeThumbnail(originalImage, width, height, mode);
        }
        /// <summary>
        /// 转换生成缩略图
        /// </summary>
        /// <param name="originalImage">原图</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        /// <returns></returns>
        public static Image MakeThumbnail(Image originalImage, int width, int height, ThumbnailMode mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailMode.FixedBoth:
                    break;
                case ThumbnailMode.FixedW:
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.FixedH:
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //1、新建一个BMP图片
            Image bitmap = new Bitmap(towidth, toheight);
            //2、新建一个画板
            Graphics g = Graphics.FromImage(bitmap);
            try
            {
                //3、设置高质量插值法
                g.InterpolationMode = InterpolationMode.High;
                //4、设置高质量，低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.HighQuality;
                //5、清空画布并以透明背景色填充
                g.Clear(Color.Transparent);
                //6、在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);
                return bitmap;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

        #region ByteToImage(将字节数组转换成图片)
        /// <summary>
        /// 将字节数组转换成图片
        /// </summary>
        /// <param name="buffer">缓存字节流</param>
        /// <returns></returns>
        public static Image ByteToImage(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                Image image = Image.FromStream(ms);
                return image;
            }
        }
        #endregion

        #region ImageToByte(将图片转换成字节数组)
        /// <summary>
        /// 将图片转换成字节数组
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        #endregion

        #region ImageToStream(将图片转换成字节流)
        /// <summary>
        /// 将图片转换成字节流
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns></returns>
        public static Stream ImageToStream(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return ms;
        }
        #endregion

        #region ImageToBase64(将图片转换成Base64编码)
        /// <summary>
        /// 将图片转换成Base64编码
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns></returns>
        public static string ImageToBase64(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, GetImageFormate(image));
            byte[] bytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将图片转换成Base64编码，带有头部
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ImageToBase64WithHeader(Image image)
        {
            return string.Format(@"data:image/{0};base64,{1}", GetImageExtension(image), ImageToBase64(image));
        }
        #endregion

        #region Base64ToImage(将Base64编码转换成图片)
        /// <summary>
        /// 将Base64编码转换成图片
        /// </summary>
        /// <param name="base64">Base64编码</param>
        /// <returns></returns>
        public static Image Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);
            return image;
        }
        /// <summary>
        /// 将带有头部编码的Base64编码转换成图片
        /// </summary>
        /// <param name="base64">Base64编码</param>
        /// <returns></returns>
        public static Image Base64ToImageWithHeader(string base64)
        {
            base64 = base64.Substring(base64.IndexOf(',') + 1);
            return Base64ToImage(base64);
        }
        #endregion

        #region ImageWatermark(图片水印)
        /// <summary>
        /// 设置图片水印
        /// </summary>
        /// <param name="path">需要加载水印的图片路径（绝对路径）</param>
        /// <param name="waterpath">水印图片（绝对路径）</param>
        /// <param name="location">水印位置</param>
        /// <returns></returns>
        public static string ImageWatermark(string path, string waterpath, ImageLocationMode location)
        {
            string extName = Path.GetExtension(path);
            if (extName == ".jpg" || extName == ".bmp" || extName == ".jpeg")
            {
                DateTime time = DateTime.Now;
                string fileName = "" + time.Year.ToString() + time.Month.ToString() + time.Day.ToString() +
                                  time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString() +
                                  time.Millisecond.ToString();
                Image img = Bitmap.FromFile(path);
                Image waterImg = Image.FromFile(waterpath);
                Graphics g = Graphics.FromImage(img);
                ArrayList coors = GetLocation(location, img, waterImg);
                g.DrawImage(waterImg, new Rectangle(int.Parse(coors[0].ToString()), int.Parse(coors[1].ToString()),
                    waterImg.Width, waterImg.Height));
                waterImg.Dispose();
                g.Dispose();
                string newPath = Path.GetDirectoryName(path) + fileName + extName;
                img.Save(newPath);
                img.Dispose();
                File.Copy(newPath, path, true);
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
            }
            return path;
        }
        /// <summary>
        /// 获取水印位置
        /// </summary>
        /// <param name="location">水印位置</param>
        /// <param name="img">需要添加水印的图片</param>
        /// <param name="waterImg">水印图片</param>
        /// <returns></returns>
        private static ArrayList GetLocation(ImageLocationMode location, Image img, Image waterImg)
        {
            ArrayList coords = new ArrayList();
            int x = 0;
            int y = 0;
            switch (location)
            {
                case ImageLocationMode.LeftTop:
                    x = 10;
                    y = 10;
                    break;
                case ImageLocationMode.Top:
                    x = img.Width / 2 - waterImg.Width / 2;
                    y = img.Height - waterImg.Height;
                    break;
                case ImageLocationMode.RightTop:
                    x = img.Width - waterImg.Width;
                    y = 10;
                    break;
                case ImageLocationMode.LeftCenter:
                    x = 10;
                    y = img.Height / 2 - waterImg.Height / 2;
                    break;
                case ImageLocationMode.Center:
                    x = img.Width / 2 - waterImg.Width / 2;
                    y = img.Height / 2 - waterImg.Height / 2;
                    break;
                case ImageLocationMode.RightCenter:
                    x = img.Width - waterImg.Width;
                    y = img.Height / 2 - waterImg.Height / 2;
                    break;
                case ImageLocationMode.LeftBottom:
                    x = 10;
                    y = img.Height - waterImg.Height;
                    break;
                case ImageLocationMode.Bottom:
                    x = img.Width / 2 - waterImg.Width / 2;
                    y = img.Height - waterImg.Height;
                    break;
                case ImageLocationMode.RightBottom:
                    x = img.Width - waterImg.Width;
                    y = img.Height - waterImg.Height;
                    break;
                default:
                    break;
            }
            coords.Add(x);
            coords.Add(y);
            return coords;
        }
        #endregion

        #region LetterWatermark(文字水印)
        /// <summary>
        /// 设置文字水印
        /// </summary>
        /// <param name="path">图片路径（绝对路径）</param>
        /// <param name="size">字体大小</param>
        /// <param name="letter">水印文字</param>
        /// <param name="color">颜色</param>
        /// <param name="location">水印位置</param>
        /// <returns></returns>
        public static string LetterWatermark(string path, int size, string letter, Color color,
            ImageLocationMode location)
        {
            string extName = Path.GetExtension(path);
            if (extName == ".jpg" || extName == ".bmp" || extName == ".jpeg")
            {
                DateTime time = DateTime.Now;
                string fileName = "" + time.Year.ToString() + time.Month.ToString() + time.Day.ToString() +
                                  time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString() +
                                  time.Millisecond.ToString();
                Image img = Bitmap.FromFile(path);
                Graphics g = Graphics.FromImage(img);
                ArrayList coors = GetLocation(location, img, size, letter.Length);
                Font font = new Font("宋体", size);
                Brush br = new SolidBrush(color);
                g.DrawString(letter, font, br, float.Parse(coors[0].ToString()), float.Parse(coors[1].ToString()));
                g.Dispose();
                string newPath = Path.GetDirectoryName(path) + fileName + extName;
                img.Save(newPath);
                img.Dispose();
                File.Copy(newPath, path, true);
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
            }
            return path;
        }

        /// <summary>
        /// 获取水印位置
        /// </summary>
        /// <param name="location">水印位置</param>
        /// <param name="img">需要添加水印的图片</param>
        /// <param name="width">宽(当水印类型为文字时,传过来的就是字体的大小)</param>
        /// <param name="height">高(当水印类型为文字时,传过来的就是字符的长度)</param>
        /// <returns></returns>
        private static ArrayList GetLocation(ImageLocationMode location, Image img, int width, int height)
        {
            ArrayList coords = new ArrayList();
            float x = 10;
            float y = 10;
            switch (location)
            {
                case ImageLocationMode.LeftTop:
                    x = 10;
                    y = 10;
                    break;
                case ImageLocationMode.Top:
                    x = img.Width / 2 - (width * height) / 2;
                    break;
                case ImageLocationMode.RightTop:
                    x = img.Width - width * height;
                    break;
                case ImageLocationMode.LeftCenter:
                    y = img.Height / 2;
                    break;
                case ImageLocationMode.Center:
                    x = img.Width / 2 - (width * height) / 2;
                    y = img.Height / 2;
                    break;
                case ImageLocationMode.RightCenter:
                    x = img.Width - height;
                    y = img.Height / 2;
                    break;
                case ImageLocationMode.LeftBottom:
                    y = img.Height - width - 5;
                    break;
                case ImageLocationMode.Bottom:
                    x = img.Width / 2 - (width * height) / 2;
                    y = img.Height - width - 5;
                    break;
                case ImageLocationMode.RightBottom:
                    x = img.Width - width * height;
                    y = img.Height - width - 5;
                    break;
                default:
                    break;
            }
            coords.Add(x);
            coords.Add(y);
            return coords;
        }
        #endregion

        #region BrightnessHandle(亮度处理)
        /// <summary>
        /// 亮度处理
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <param name="val">增加或减少的光暗值</param>
        /// <returns></returns>
        public static Bitmap BrightnessHandle(Bitmap bitmap, int width, int height, int val)
        {
            Bitmap bm = new Bitmap(width, height);//初始化一个记录经过处理后的图片对象
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    //红绿蓝三值
                    var resultR = pixel.R + val;
                    var resultG = pixel.G + val;
                    var resultB = pixel.B + val;
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }
            return bm;
        }
        #endregion

        #region FilterColor(滤色处理)
        /// <summary>
        /// 滤色处理
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        /// <returns></returns>
        public static Bitmap FilterColor(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    bitmap.SetPixel(x, y, Color.FromArgb(0, pixel.G, pixel.B));
                }
            }
            return bitmap;
        }
        #endregion

        #region StretchImage(拉伸图片)
        /// <summary>
        /// 拉伸图片?
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        /// <param name="width">新的宽度</param>
        /// <param name="height">新的高度</param>
        /// <returns></returns>
        public static Bitmap StretchImage(Bitmap bitmap, int width, int height)
        {
            try
            {
                Bitmap bm = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bm);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bm, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bm.Width, bm.Height),
                    GraphicsUnit.Pixel);
                g.Dispose();
                return bm;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region LeftRightTurn(左右翻转)
        /// <summary>
        /// 左右翻转
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        /// <returns></returns>
        public static Bitmap LeftRightTurn(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = width - 1, z = 0; x >= 0; x--)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    bitmap.SetPixel(z++, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            return bitmap;
        }
        #endregion

        #region TopBottomTurn(上下翻转)
        /// <summary>
        /// 上下翻转
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        /// <returns></returns>
        public static Bitmap TopBottomTurn(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = height - 1, z = 0; y >= 0; y--)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    bitmap.SetPixel(x, z++, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            return bitmap;
        }
        #endregion

        #region Compress(压缩图片)
        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="oldFile">源文件路径</param>
        /// <param name="newFile">新文件路径</param>
        /// <returns></returns>
        public static bool Compress(string oldFile, string newFile)
        {
            try
            {
                Image image = Image.FromFile(oldFile);
                ImageFormat thisFormat = image.RawFormat;
                Size newSize = new Size(100, 125);
                Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height);
                Graphics g = Graphics.FromImage(outBmp);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel);
                g.Dispose();
                EncoderParameters encoderParams = new EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                ImageCodecInfo[] array = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = array.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                image.Dispose();
                if (ici != null)
                {
                    outBmp.Save(newFile, ImageFormat.Jpeg);
                }
                outBmp.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ToBlackWhiteImage(转换为黑白图片)
        /// <summary>
        /// 转换为黑白图片
        /// </summary>
        /// <param name="bitmap">要进行处理的图片</param>
        /// <returns></returns>
        public static Bitmap ToBlackWhiteImage(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    int result = (pixel.R + pixel.G + pixel.B) / 3;
                    bitmap.SetPixel(x, y, Color.FromArgb(result, result, result));
                }
            }
            return bitmap;
        }
        #endregion

        #region TwistImage(扭曲图片，滤镜效果)
        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="isTwist">是否扭曲，true:扭曲,false:不扭曲</param>
        /// <param name="shapeMultValue">波形的幅度倍数，越大扭曲的程度越高，默认为3</param>
        /// <param name="shapePhase">波形的起始相位，取值区间[0-2*PI]</param>
        /// <returns></returns>
        public static Bitmap TwistImage(Bitmap bitmap, bool isTwist, double shapeMultValue, double shapePhase)
        {
            Bitmap destBmp = new Bitmap(bitmap.Width, bitmap.Height);
            //将位图背景填充为白色
            Graphics g = Graphics.FromImage(destBmp);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            g.Dispose();
            double dBaseAxisLen = isTwist ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = isTwist ? (2 * Math.PI * (double)j) / dBaseAxisLen : (2 * Math.PI * (double)i) / dBaseAxisLen;
                    dx += shapePhase;
                    double dy = Math.Sin(dx);
                    //取当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = isTwist ? i + (int)(dy * shapeMultValue) : i;
                    nOldY = isTwist ? j : j + (int)(dy * shapeMultValue);
                    Color color = bitmap.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX <= destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }
        #endregion

        #region GetFrames(获取图片帧)
        /// <summary>
        /// 获取图片帧
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <param name="savePath">帧保存路径</param>
        public static void GetFrames(string imgPath, string savePath)
        {
            Image gif = Image.FromFile(imgPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd);//获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(savePath + "\\frame_" + i + ".jpg", ImageFormat.Jpeg);
            }
        }
        #endregion

        #region GetImageExtension(获取图片扩展名)
        /// <summary>
        /// 获取图片扩展名
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns></returns>
        public static string GetImageExtension(Image image)
        {
            Type type = typeof(ImageFormat);
            PropertyInfo[] imageFormatList = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i != imageFormatList.Length; i++)
            {
                ImageFormat formatClass = (ImageFormat)imageFormatList[i].GetValue(null, null);
                if (formatClass.Guid.Equals(image.RawFormat.Guid))
                {
                    return imageFormatList[i].Name;
                }
            }
            return "";
        }
        #endregion

        #region GetImageFormate(获取图片格式)
        /// <summary>
        /// 获取图片格式
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormate(Image image)
        {
            Type type = typeof(ImageFormat);
            PropertyInfo[] imageFormatList = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i != imageFormatList.Length; i++)
            {
                ImageFormat formatClass = (ImageFormat)imageFormatList[i].GetValue(null, null);
                if (formatClass.Guid.Equals(image.RawFormat.Guid))
                {
                    return formatClass;
                }
            }
            return ImageFormat.Jpeg;
        }
        #endregion

        #region GetCodecInfo(获取特定图像编解码信息)
        /// <summary>
        /// 获取特定图像编解码信息
        /// </summary>
        /// <param name="format">图片格式</param>
        /// <returns></returns>
        public static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
        #endregion

        #region CreateImageByChar(根据字符内容生成图片)
        /// <summary>
        /// 根据字符内容生成图片
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static Image CreateImageByChar(string content)
        {
            int fontSize = 20;//字体大小
            int fontWidth = fontSize;//字体宽度
            int imageWidth = (int)(content.Length * fontWidth) * 2;//图片宽度，字体宽度*字数*2
            int imageHeight = fontSize * 2;//图片高度，字体高度*2
            Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(bitmap);
            //获取品质（压缩率）编码
            //System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            //EncoderParameter paramter = new EncoderParameter(encoder, 30L);
            //EncoderParameters paramters = new EncoderParameters(1);
            //paramters.Param[0] = paramter;
            //ImageCodecInfo jpgInfo = GetCodecInfo(ImageFormat.Jpeg);
            //g.CompositingQuality=CompositingQuality.HighSpeed;
            //g.SmoothingMode=SmoothingMode.HighSpeed;
            //g.InterpolationMode=InterpolationMode.Low;
            g.Clear(Color.White);//背景图
            //设置字体
            Random random = new Random(VerifyCodeUtil.Instance.GetRandomSeed());
            string fontName = Fonts[random.Next(Fonts.Length - 1)];
            FontStyle fontStyle = Styles[random.Next(Styles.Length - 1)];
            Font font = new Font(fontName, fontSize, fontStyle);
            //验证码颜色
            Brush brush = new SolidBrush(Color.Blue);
            int minLeft = 0;
            int maxLeft = imageWidth - content.Length * fontSize;
            int left = random.Next(minLeft, maxLeft);
            for (int i = 0; i < content.Length; i++)
            {
                int rLeft = left + (i * fontSize);//需要写入的字符
                //上下移动
                //int minTop = fontSize/8;
                //int maxTop = fontSize/4;
                //int top = random.Next(minTop, maxTop);
                //旋转
                int minRotation = -5;
                int maxRotation = 5;
                int rotation = random.Next(minRotation, maxRotation);
                g.RotateTransform(rotation);
                g.DrawString(content[i].ToString(), font, brush, rLeft, 0);
                g.RotateTransform(-rotation);
            }
            //画边框
            //g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            g.Dispose();
            //产生波形
            bitmap = TwistImage(bitmap, true, 2, 4);
            return bitmap;
        }
        #endregion

        #region DownImage(下载图片到本地)
        /// <summary>
        /// 获取图片标签
        /// </summary>
        /// <param name="htmlStr">html字符串</param>
        /// <returns></returns>
        private static string[] GetImageTag(string htmlStr)
        {
            Regex regex = new Regex("<img.+?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string[] strArray = new string[regex.Matches(htmlStr).Count];
            int i = 0;
            foreach (Match match in regex.Matches(htmlStr))
            {
                strArray[i] = GetImageUrl(match.Value);
                i++;
            }
            return strArray;
        }
        /// <summary>
        /// 获取图片Url地址
        /// </summary>
        /// <param name="imgTagStr">图片标签字符串</param>
        /// <returns></returns>
        private static string GetImageUrl(string imgTagStr)
        {
            string str = "";
            Regex regex = new Regex("http://.+.(?:jpg|gif|bmp|png)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(imgTagStr))
            {
                str = match.Value;
            }
            return str;
        }
        /// <summary>
        /// 下载图片到本地
        /// </summary>
        /// <param name="html">Html字符串</param>
        /// <param name="path">本地路径</param>
        /// <returns></returns>
        public static string DownImage(string html, string path)
        {
            string year = DateTime.Now.ToString("yyyy-MM");
            string day = DateTime.Now.ToString("dd");
            path = path + year + "/" + day;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] imgUrlArray = GetImageTag(html);
            try
            {
                for (int i = 0; i < imgUrlArray.Length; i++)
                {
                    string preStr = DateTime.Now.ToString() + "_";
                    preStr = preStr.Replace("-", "");
                    preStr = preStr.Replace(":", "");
                    preStr = preStr.Replace(" ", "");
                    WebClient wc = new WebClient();
                    wc.DownloadFile(imgUrlArray[i],
                        path + "/" + preStr + imgUrlArray[i].Substring(imgUrlArray[i].LastIndexOf("/") + 1));
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return html;
        }
        #endregion

        #region From(从指定文件创建图片)
        /// <summary>
        /// 从指定文件创建图片
        /// </summary>
        /// <param name="filePath">图片文件的绝对路径</param>
        /// <returns></returns>
        public static Image FromFile(string filePath)
        {
            return Image.FromFile(filePath);
        }
        /// <summary>
        /// 从指定流创建图片
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static Image FromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }
        /// <summary>
        /// 从指定字节流创建图片
        /// </summary>
        /// <param name="buffer">字节流</param>
        /// <returns></returns>
        public static Image FromStream(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return FromStream(stream);
            }
        }
        #endregion

        #region UndamageCompress(无损压缩图片)
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="oldFile">原文件路径</param>
        /// <param name="newFile">新文件路径</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否第一次调用</param>
        /// <returns></returns>
        public static bool UndamageCompress(string oldFile, string newFile, int flag = 90, int size = 300,
            bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo=new FileInfo(oldFile);
            if (sfsc == true && firstFileInfo.Length < size*1024)
            {
                firstFileInfo.CopyTo(newFile);
                return true;
            }
            Image iSource = Image.FromFile(oldFile);
            ImageFormat tFormat = iSource.RawFormat;
            int dHeight = iSource.Height/2;
            int dWidth = iSource.Width/2;
            int sW = 0, sH = 9;
            //按比例压缩
            Size temSize=new Size(iSource.Width,iSource.Height);
            if (temSize.Width > dHeight || temSize.Width > dWidth)
            {
                if ((temSize.Width*dHeight) > (temSize.Width*dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth*temSize.Height)/temSize.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (temSize.Width*dHeight)/temSize.Height;
                }
            }
            else
            {
                sW = temSize.Width;
                sH = temSize.Height;
            }

            Bitmap ob=new Bitmap(dWidth,dHeight);
            Graphics g=Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality=CompositingQuality.HighQuality;
            g.SmoothingMode=SmoothingMode.HighQuality;
            g.InterpolationMode=InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((dWidth - sW)/2, (dHeight - sH)/2, sW, sH), 0, 0, iSource.Width,
                iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep=new EncoderParameters();
            long[] qy=new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam=new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(newFile, jpegICIinfo, ep);
                    FileInfo fi = new FileInfo(newFile);
                    if (fi.Length > 1024*size)
                    {
                        flag = flag - 10;
                        UndamageCompress(oldFile, newFile, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(newFile,tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
        #endregion
    }
}
