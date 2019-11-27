﻿// https://raw.githubusercontent.com/NAXAM/effects-xamarin-forms/master/Naxam.Effects.Platform.iOS/ViewEffect.cs
// d3c790b  on 9 Jun 17 @fleuverouge fleuverouge[iOS] Remove default border
// This file isn't generated, but this comment is necessary to exclude it from StyleCop analysis.
// <auto-generated/>

using System;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Presentation.Forms.iOS.Effects;
using Sharpnado.Tasks;

using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(iOSViewStyleEffect), nameof(ViewStyleEffect))]

namespace Sharpnado.Presentation.Forms.iOS.Effects
{
    [Preserve]
    public class iOSViewStyleEffect : PlatformEffect
    {
        private UIView _view;
        private UIView _layer;
        private double _alpha;

        bool _DefaultMaskToBounds { get; set; }

        CALayer _BorderLayer { get; set; } //Shadow & corner radius doesn't play nice with each other

        UIColor _DefaultBackgroundColor { get; set; }

        public static void Initialize()
        {
        }

        protected override void OnAttached()
        {
            try
            {
                if (ViewEffect.IsTapFeedbackColorSet(Element))
                {
                    UpdateTapFeedbackColor();
                }

                if (ViewEffect.IsStyleSet(Element))
                {
                    UpdateStyle();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Cannot set property on attached control." + ex);
            }
        }

        protected override void OnDetached()
        {
            var view = Control ?? Container;
            if (view != null)
            {
                view.BackgroundColor = _DefaultBackgroundColor;
                if (view.Layer != null)
                {
                    view.Layer.MasksToBounds = _DefaultMaskToBounds;
                }
            }

            _BorderLayer?.RemoveFromSuperLayer();
            _BorderLayer = null;

            TapGestureCollector.Delete(_view, TapAction);
            LongTapGestureCollector.Delete(_view, LongTapAction);
            _layer?.Dispose();
            _layer = null;
        }

        private void UpdateTapFeedbackColor()
        {
            _view = Control ?? Container;

            _view.UserInteractionEnabled = true;

            TapGestureCollector.Add(_view, TapAction);
            LongTapGestureCollector.Add(_view, LongTapAction);

            UpdateEffectColor();
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (_BorderLayer == null)
            {
                return;
            }

            if (args.PropertyName == "Height")
            {
                var frame = _BorderLayer.Frame;
                var newHeight = (double)Element.GetValue(VisualElement.HeightProperty);
                _BorderLayer.Frame = new CGRect(0, 0, frame.Width, new nfloat(newHeight));
            }
            else if (args.PropertyName == "Width")
            {
                var frame = _BorderLayer.Frame;
                var newWidth = (double)Element.GetValue(VisualElement.WidthProperty);
                _BorderLayer.Frame = new CGRect(0, 0, new nfloat(newWidth), frame.Height);
            }
            else if (args.PropertyName == ViewEffect.TouchFeedbackColorProperty.PropertyName)
            {
                UpdateEffectColor();
            }
        }

        private void UpdateStyle()
        {
            var view = Control ?? Container;
            _DefaultMaskToBounds = view.Layer.MasksToBounds;
            _DefaultBackgroundColor = view.BackgroundColor;

            if (_BorderLayer == null)
            {
                _BorderLayer = new CALayer();
                view.Layer.InsertSublayer(_BorderLayer, 0);
            }
            _BorderLayer.Frame = new CGRect(CGPoint.Empty, view.Layer.Frame.Size);
            _BorderLayer.BorderWidth = (nfloat)ViewEffect.GetBorderWidth(Element);
            _BorderLayer.BorderColor = ViewEffect.GetBorderColor(Element).ToUIColor().CGColor;
            _BorderLayer.CornerRadius = (nfloat)ViewEffect.GetCornerRadius(Element);
            _BorderLayer.MasksToBounds = true;
            _BorderLayer.BackgroundColor = view.BackgroundColor.CGColor;

            view.BackgroundColor = UIColor.Clear;
            view.Layer.ShadowColor = ViewEffect.GetShadowColor(Element).ToUIColor().CGColor;
            view.Layer.ShadowRadius = (nfloat)ViewEffect.GetShadowRadius(Element);
            view.Layer.ShadowOpacity = ViewEffect.GetShadowOpacity(Element);
            view.Layer.ShadowOffset = new CGSize(
                (nfloat)ViewEffect.GetShadowOffsetX(Element),
                (nfloat)ViewEffect.GetShadowOffsetY(Element));

            view.Layer.MasksToBounds = false;

            //Remove default border
            view.Layer.BorderWidth = 0;
            view.Layer.BorderColor = UIColor.Clear.CGColor;

            view.ClipsToBounds = false;
        }

        private void LongTapAction(UIGestureRecognizerState state)
        {
            switch (state)
            {
                case UIGestureRecognizerState.Began:
                    TaskMonitor.Create(TapAnimationAsync(0.5, 0, _alpha, false));
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    TaskMonitor.Create(TapAnimationAsync(0.5, _alpha));
                    break;
            }
        }

        private void TapAction()
        {
            TaskMonitor.Create(TapAnimationAsync(0.3, _alpha, 0));
        }

        private void UpdateEffectColor()
        {
            _layer?.Dispose();
            _layer = null;

            var color = ViewEffect.GetTouchFeedbackColor(Element);
            if (color == Color.Default)
            {
                return;
            }
            _alpha = color.A < 1.0 ? 1 : 0.3;

            _layer = new UIView {BackgroundColor = color.ToUIColor()};
        }

        private async Task TapAnimationAsync(double duration, double start = 1, double end = 0, bool remove = true)
        {
            if (_layer != null)
            {
                _layer.Frame = new CGRect(0, 0, Container.Bounds.Width, Container.Bounds.Height);
                Container.AddSubview(_layer);
                Container.BringSubviewToFront(_layer);
                _layer.Alpha = (float)start;
                await UIView.AnimateAsync(duration, () => {
                    _layer.Alpha = (float)end;
                });
                if (remove)
                {
                    _layer?.RemoveFromSuperview();
                }
            }
        }

    }
}