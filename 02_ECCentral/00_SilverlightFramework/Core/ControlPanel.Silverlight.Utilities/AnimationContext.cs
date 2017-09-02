//-----------------------------------------------------------------------
// <copyright company="Newegg">
//      (c) Copyright Newegg Corporation.
//       Author:Ryan.W.Li@Newegg.com
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Newegg.Oversea.Silverlight.Utilities
{
    public delegate void ProcessCompleted(object sender, CompletedEventArgs e);

    public class CompletedEventArgs : EventArgs
    {
        public object UserState { get; set; }
    }


    /// <summary>
    /// Provide a series of animation to change element.
    /// </summary>
    public class AnimationContext
    {
        #region Fields

        private Storyboard _storyboard = new Storyboard();

        private ProcessCompleted _playCompleted;

        private object m_userState;

        #endregion


        #region Properties

        /// <summary>
        /// 获取当前动画执行的一个状态，停止/进行中
        /// </summary>
        public AnimationStatus Status { get; private set; }

        #endregion


        #region Events

        /// <summary>
        /// 当动画执行完毕的时候发生.
        /// </summary>
        public event ProcessCompleted PlayCompleted
        {
            add
            {
                _playCompleted += value;
            }
            remove
            {
                _playCompleted -= value;
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Create animation to change the element
        /// </summary>
        /// <param name="element">element that wait for change</param>
        public AnimationContext()
        {
            this._storyboard.Completed += new EventHandler(_storyboard_Completed);
            Status = AnimationStatus.Stopped;
        }

        void _storyboard_Completed(object sender, EventArgs e)
        {
            _storyboard.Stop();
            Status = AnimationStatus.Stopped;
            if (_playCompleted != null)
            {
                _playCompleted(this, new CompletedEventArgs() { UserState = m_userState });
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 开始执行动画
        /// </summary>
        public AnimationContext Play()
        {
            _storyboard.Begin();
            Status = AnimationStatus.Playing;
            return this;
        }

        public AnimationContext Play(object userState)
        {
            m_userState = userState;
            return Play();
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public AnimationContext Stop()
        {
            _storyboard.Stop();
            Status = AnimationStatus.Stopped;
            return this;
        }

        /// <summary>
        /// 清除所有帧
        /// </summary>
        public AnimationContext Clear()
        {
            _storyboard.Children.Clear();
            return this;
        }

        /// <summary>
        /// 设置启用或禁止3D硬件加速
        /// </summary>
        /// <param name="isEnable"></param>
        public AnimationContext EnableThreeDAcceleration(FrameworkElement element, bool isEnable)
        {
            if (element.CacheMode == null && isEnable)
            {
                BitmapCache cache = new BitmapCache();
                cache.RenderAtScale = 1.0;
                element.CacheMode = cache;
            }
            else if (!isEnable && element.CacheMode != null)
            {
                element.CacheMode = null;
            }
            return this;
        }

        /// <summary>
        /// 初始化元素的TransformGroup
        /// </summary>
        public AnimationContext EnsureDefaultTransforms(FrameworkElement element)
        {
            TransformGroup renderTransform = element.RenderTransform as TransformGroup;
            renderTransform = new TransformGroup();
            renderTransform.Children.Add(new ScaleTransform());
            renderTransform.Children.Add(new SkewTransform());
            renderTransform.Children.Add(new RotateTransform());
            renderTransform.Children.Add(new TranslateTransform());
            element.RenderTransform = renderTransform;
            return this;
        }

        /// <summary>
        /// 初始化PlaneProjection为3D旋转做准备
        /// </summary>
        /// <param name="element"></param>
        public AnimationContext EnsureDefaultProjection(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException();
            }

            if (element.Projection == null)
            {
                element.Projection = new PlaneProjection();
            }
            return this;
        }

        /// <summary>
        /// 沿着Y轴做3D变换
        /// </summary>
        /// <param name="frames">动画帧数</param>
        public AnimationContext ThreeDRotateY(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            if (element.Projection != null)
            {
                element.Projection.GetValue(PlaneProjection.RotationYProperty);
            }
            else
            {
                element.Projection = new PlaneProjection();
            }
            SetFrames(ConvertSplineDoubleFrame(frameValues), "(UIElement.Projection).(PlaneProjection.RotationY)", element);
            return this;
        }

        /// <summary>
        /// 改变变换的中心点
        /// </summary>
        /// <param name="x1">x coordinate</param>
        /// <param name="x2">y coordinate</param>
        public AnimationContext Origin(double x1, double x2, FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException();
            }

            element.RenderTransformOrigin = new Point(x1, x2);
            return this;
        }

        /// <summary>
        /// 添透明的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext Opacity(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineDoubleFrame(frameValues), "UIElement.Opacity", element);
            return this;
        }

        /// <summary>
        /// 添加通过百分比来放大缩小高度的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext ScaleHeightByPrecent(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            if (element.RenderTransform != null)
            {
                element.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);
            }
            else
            {
                EnsureDefaultTransforms(element);
            }
            SetFrames(ConvertSplineDoubleFrame(frameValues), "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", element);
            return this;
        }

        /// <summary>
        /// 添加通过百分比来方法缩小宽度的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext ScaleWidthByPrecent(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            if (element.RenderTransform != null)
            {
                element.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
            }
            else
            {
                EnsureDefaultTransforms(element);
            }
            SetFrames(ConvertSplineDoubleFrame(frameValues), "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", element);
            return this;
        }

        /// <summary>
        /// 添加放大缩小高度的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext ScaleHeight(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            if (element.RenderTransform != null)
            {
                element.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);
            }
            else
            {
                EnsureDefaultTransforms(element);
            }
            SetFrames(ConvertSplineDoubleFrame(frameValues), "Height", element);
            return this;
        }

        /// <summary>
        /// 添加放大缩小宽度的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext ScaleWidth(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            if (element.RenderTransform != null)
            {
                element.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
            }
            else
            {
                EnsureDefaultTransforms(element);
            }
            SetFrames(ConvertSplineDoubleFrame(frameValues), "Width", element);
            return this;
        }

        /// <summary>
        /// 添加沿着X轴移动的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext MoveX(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineDoubleFrame(frameValues), "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)", element);
            return this;
        }

        /// <summary>
        /// 添加沿着Y轴移动的变换帧
        /// </summary>
        /// <param name="frames">instance of animation frame</param>
        public AnimationContext MoveY(FrameworkElement element, params double[][] frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineDoubleFrame(frameValues), "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)", element);
            return this;
        }

        /// <summary>
        /// 添加改变背景颜色的变换帧
        /// </summary>
        /// <param name="element"></param>
        /// <param name="frameValues"></param>
        /// <returns></returns>
        public AnimationContext ChangeBgColor(FrameworkElement element, Dictionary<double, Color> frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineColorFrame(frameValues), "(UIElement.Background).(SolidColorBrush.Color)", element);
            return this;
        }

        /// <summary>
        /// 添加改变边框颜色的变换帧
        /// </summary>
        public AnimationContext ChangeBorderColor(FrameworkElement element, Dictionary<double, Color> frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineColorFrame(frameValues), "(UIElement.BorderBrush).(SolidColorBrush.Color)", element);
            return this;
        }

        /// <summary>
        /// 添加改变前景颜色的变换帧
        /// </summary>
        public AnimationContext ChangeFgColor(FrameworkElement element, Dictionary<double, Color> frameValues)
        {
            if (element == null || frameValues == null)
            {
                throw new ArgumentNullException();
            }

            SetFrames(ConvertSplineColorFrame(frameValues), "(UIElement.Foreground).(SolidColorBrush.Color)", element);
            return this;
        }


        #endregion


        #region Private Methods

        private void SetFrames(List<SplineColorKeyFrame> frames, string path, FrameworkElement element)
        {
            ColorAnimationUsingKeyFrames timeline = new ColorAnimationUsingKeyFrames();
            Storyboard.SetTarget(timeline, element);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(path, new object[0]));
            foreach (SplineColorKeyFrame frame in frames)
            {
                timeline.KeyFrames.Add(frame);
            }
            _storyboard.Children.Add(timeline);
        }

        private void SetFrames(List<SplineDoubleKeyFrame> frames, string path, FrameworkElement element)
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(timeline, element);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(path, new object[0]));
            foreach (SplineDoubleKeyFrame frame in frames)
            {
                timeline.KeyFrames.Add(frame);
            }
            _storyboard.Children.Add(timeline);
        }

        private List<SplineDoubleKeyFrame> ConvertSplineDoubleFrame(params double[][] frameValues)
        {
            List<SplineDoubleKeyFrame> frames = new List<SplineDoubleKeyFrame>();

            for (int i = 0; i < frameValues.Length; i++)
            {
                if (frameValues[i].Length > 6)
                {

                }
                if (frameValues[i].Length == 6)
                {
                    frames.Add(new SplineDoubleKeyFrame 
                    {
                        KeySpline = new KeySpline() { ControlPoint1 = new Point(frameValues[i][2], frameValues[i][3]), ControlPoint2 = new Point(frameValues[i][4], frameValues[i][5]) },
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameValues[i][0])),
                        Value = frameValues[i][1]
                    });
                }
                else if (frameValues[i].Length == 2)
                {
                    frames.Add(new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameValues[i][0])),
                        Value = frameValues[i][1]
                    });
                }
            }
            return frames;
        }

        private List<SplineColorKeyFrame> ConvertSplineColorFrame(Dictionary<double, Color> frameValues)
        {
            List<SplineColorKeyFrame> result = new List<SplineColorKeyFrame>();
            foreach (var item in frameValues)
            {
                result.Add(new SplineColorKeyFrame() 
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(item.Key)),
                    Value = item.Value
                });
            }
            return result;
        }

        #endregion
    }

    public enum AnimationStatus
    {
        Playing,
        Stopped
    }
}
