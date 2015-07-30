/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.Utils
{
    public class IconProvider
    {
        public static ImageSource GetIconForAchievement(BaseAchievement achievement)
        {
            ImageSource imageSource;
            if (!achievement.IsCompleted)
            {
                try
                {
                    imageSource =
                        GetImageSourceFromBitmap(
                            (Bitmap) icons.ResourceManager.GetObject("uncompleted_" + achievement.Id));
                }
                catch (Exception)
                {
                    imageSource = GetImageSourceFromBitmap(icons.DummyUncompleted);
                }
            }

            else
            {
                try
                {
                    imageSource =
                        GetImageSourceFromBitmap(
                            (Bitmap) icons.ResourceManager.GetObject("completed_" + achievement.Id));
                }
                catch (Exception)
                {
                    imageSource = GetImageSourceFromBitmap(icons.DummyCompleted);
                }
            }
            return imageSource;
        }

        public static ImageSource GetImageSourceFromBitmap(Bitmap bitmap)
        {
            var hbitmap = bitmap.GetHbitmap();
            var imageSourceFromBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hbitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbitmap);
            return imageSourceFromBitmap;
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}