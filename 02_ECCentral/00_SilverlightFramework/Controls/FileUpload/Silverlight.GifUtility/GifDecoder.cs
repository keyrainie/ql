#region Copyright & License
/*----------------------------------------------------------------
// Copyright (C) 2008 jillzhang 版权所有。 
//  
// 文件名：GifDecoder.cs    使用请保留版权信息  
// 文件功能描述： 更多信息请访问 http://jillzhang.cnblogs.com
// 
// 创建标识：jillzhang 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
/*-------------------------New BSD License ------------------
 Copyright (c) 2008, jillzhang
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of jillzhang nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 */
#endregion

using System.Collections.Generic;
using System.IO;

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Newegg.Oversea.Silverlight.GifUtility
{
    public class GIFDecoder
    {
        private static void ReadImage(StreamHelper streamHelper, Stream fs, GifImage gifImage, List<GraphicEx> graphics, int frameCount)
        {
            ImageDescriptor imgDes = streamHelper.GetImageDescriptor(fs);
            GifFrame frame = new GifFrame();

            frame.ImageDescriptor = imgDes;
            frame.LocalColorTable = gifImage.GlobalColorTable;

            if (imgDes.LctFlag)
            {
                frame.LocalColorTable = streamHelper.ReadByte(imgDes.LctSize * 3);
            }

            LZWDecoder lzwDecoder = new LZWDecoder(fs);
            int dataSize = streamHelper.Read();

            frame.ColorDepth = dataSize;

            byte[] pixels = lzwDecoder.DecodeImageData(imgDes.Width, imgDes.Height, dataSize, imgDes.InterlaceFlag);

            frame.IndexedPixel = pixels;

            int blockSize = streamHelper.Read();
            DataStruct data = new DataStruct(blockSize, fs);
            GraphicEx graphicEx = null;

            if (graphics.Count > 0)
            {
                graphicEx = graphics[frameCount];
            }

            frame.GraphicExtension = graphicEx;

            // Handle delta GIF images
            WriteableBitmap prev;

            if ((gifImage.LogicalScreenDescriptor.Packed == 245) && (frameCount > 0))
            {
                prev = gifImage.Frames[frameCount - 1].Image;
            }
            else
            {
                prev = null;
            }

            WriteableBitmap img = GetImageFromPixel(pixels, frame.Palette, imgDes.InterlaceFlag, imgDes.Width, imgDes.Height, prev);
            frame.Image = img;
            gifImage.Frames.Add(frame);
        }

        private static Color GetPixel(WriteableBitmap wb, int col, int row)
        {
            int pixel = wb.Pixels[(row * wb.PixelWidth) + col];

            return Color.FromArgb((byte)((pixel >> 0x18) & 0xFF),
                                                        (byte)((pixel >> 0x10) & 0xFF),
                                                        (byte)((pixel >> 8) & 0xFF),
                                                        (byte)(pixel & 0xFF));
        }

        private static void SetPixel(WriteableBitmap wb, int col, int row, Color color)
        {
            wb.Pixels[(row * wb.PixelWidth) + col] = color.A << 24 | color.R << 16 | color.G << 8 | color.B;
        }

        private static WriteableBitmap GetImageFromPixel(byte[] pixels, Color32[] colorTable, bool interlactFlag, int width, int height, WriteableBitmap prevFrame)
        {
            WriteableBitmap img = new WriteableBitmap(width, height);
            int idx = 0;
            Color color;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    color = colorTable[pixels[idx++]].Color;

                    // If color == 0 then get from the previous frame (deltas)
                    if ((null != prevFrame) && ((color.A == 0) && (color.R == 0) && (color.G == 0) && (color.B == 0)))
                    {
                        color = GetPixel(prevFrame, col, row);
                    }

                    SetPixel(img, col, row, color);
                }
            }
            return img;
        }

        public static GifImage Decode(Stream fs)
        {
            StreamHelper streamHelper = null;
            GifImage gifImage = new GifImage();
            List<GraphicEx> graphics = new List<GraphicEx>();
            int frameCount = 0;

            streamHelper = new StreamHelper(fs);

            gifImage.Header = streamHelper.ReadString(6);
            gifImage.LogicalScreenDescriptor = streamHelper.GetLCD(fs);

            if (gifImage.LogicalScreenDescriptor.GlobalColorTableFlag)
            {
                gifImage.GlobalColorTable = streamHelper.ReadByte(gifImage.LogicalScreenDescriptor.GlobalColorTableSize * 3);
            }

            int nextFlag = streamHelper.Read();

            while (nextFlag != 0)
            {
                if (nextFlag == GifExtensions.ImageLabel)
                {
                    ReadImage(streamHelper, fs, gifImage, graphics, frameCount);
                    frameCount++;
                }
                else if (nextFlag == GifExtensions.ExtensionIntroducer)
                {
                    int gcl = streamHelper.Read();
                    switch (gcl)
                    {
                        case GifExtensions.GraphicControlLabel:
                            {
                                GraphicEx graphicEx = streamHelper.GetGraphicControlExtension(fs);
                                graphics.Add(graphicEx);
                                break;
                            }
                        case GifExtensions.CommentLabel:
                            {
                                CommentEx comment = streamHelper.GetCommentEx(fs);
                                gifImage.CommentExtensions.Add(comment);
                                break;
                            }
                        case GifExtensions.ApplicationExtensionLabel:
                            {
                                ApplicationEx applicationEx = streamHelper.GetApplicationEx(fs);
                                gifImage.ApplictionExtensions.Add(applicationEx);
                                break;
                            }
                        case GifExtensions.PlainTextLabel:
                            {
                                PlainTextEx textEx = streamHelper.GetPlainTextEx(fs);
                                gifImage.PlainTextEntensions.Add(textEx);
                                break;
                            }
                    }
                }
                else if (nextFlag == GifExtensions.EndIntroducer)
                {
                    break;
                }
                nextFlag = streamHelper.Read();
            }
            return gifImage;
        }
    }
}
