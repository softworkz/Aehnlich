﻿namespace AehnlichViewLib.Controls
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using AehnlichViewLib.Enums;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Collections.Specialized;
    using AehnlichViewLib.Interfaces;

    /// <summary>
    /// Implements the Overview control that contains the marker items that can be used
    /// to overview differences in a couple of files that are to be merged.
    /// </summary>
    [TemplatePart(Name = PART_ViewPortContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ImageViewport, Type = typeof(Image))]
    public class Overview : Slider
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(Overview), new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));

        /// <summary>
        /// Implements the backing store of the <see cref="ThumbHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThumbHeightProperty =
            DependencyProperty.Register("ThumbHeight", typeof(double),
                typeof(Overview), new PropertyMetadata(0.0d));

        /// <summary>
        /// Implements the backing store of the <see cref="MinThumbHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinThumbHeightProperty =
            DependencyProperty.Register("MinThumbHeight", typeof(double),
                typeof(Overview), new PropertyMetadata(12.0d, OnBitmapParameterChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="NumberOfTextLinesInViewPort"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberOfTextLinesInViewPortProperty =
            DependencyProperty.Register("NumberOfTextLinesInViewPort", typeof(int),
                typeof(Overview), new PropertyMetadata(0, OnViewPortLinesChanged));

        #region Diff Color Definitions
        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundBlank"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundBlankProperty =
            DependencyProperty.Register("ColorBackgroundBlank", typeof(SolidColorBrush),
                typeof(Overview), new PropertyMetadata(default(SolidColorBrush), OnBitmapParameterChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundAddedProperty =
            DependencyProperty.Register("ColorBackgroundAdded", typeof(SolidColorBrush),
                typeof(Overview), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xba, 0xff)), OnBitmapParameterChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundDeletedProperty =
            DependencyProperty.Register("ColorBackgroundDeleted", typeof(SolidColorBrush),
                typeof(Overview), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x80, 0x80)), OnBitmapParameterChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundContextProperty =
            DependencyProperty.Register("ColorBackgroundContext", typeof(SolidColorBrush),
                typeof(Overview), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0xFF, 0x80)), OnBitmapParameterChanged));
        #endregion Diff Color Definitions

        /// <summary>
        /// Defines the name of the grid that contains the overview image that is rendered from the
        /// available diff information.
        /// </summary>
        public const string PART_ViewPortContainer = "PART_ViewPortContainer";

        /// <summary>
        /// Defines the name of <see cref="Image"/> control that contains the overview diff image.
        /// </summary>
        public const string PART_ImageViewport = "PART_ImageViewport";

        private Grid _PART_ViewPortContainer;
        private Image _PART_ImageViewport;
        private WriteableBitmap writeableBmp;
        private INotifyCollectionChanged _observeableDiffContext;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        static Overview()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Overview), new FrameworkPropertyMetadata(typeof(Overview)));
        }
        #endregion Constructors

        #region properties
        /// <summary>
        /// Gets/sets a source of items that can be used to populate marker elements
        /// on the overview bar. This should ideally be an ObservableCollection{T} or
        /// at least an <see cref="IList{T}"/> where T is a <see cref="DiffContext"/>
        /// for a particular line in the merged text views.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets/sets the number of (text) lines that are currently visible in
        /// the viewport of the (text) diff control.
        /// </summary>
        public int NumberOfTextLinesInViewPort
        {
            get { return (int)GetValue(NumberOfTextLinesInViewPortProperty); }
            set { SetValue(NumberOfTextLinesInViewPortProperty, value); }
        }

        /// <summary>
        /// Gets the height of the <see cref="Slider"/> thumb used to slide the
        /// currently visible window to a particular location in the <see cref="Overview"/> control.
        /// </summary>
        public double ThumbHeight
        {
            get { return (double)GetValue(ThumbHeightProperty); }
            protected set { SetValue(ThumbHeightProperty, value); }
        }

        /// <summary>
        /// Gets/sets the minimum height of the <see cref="Slider"/> thumb used to slide the
        /// currently visible window to a particular location in the <see cref="Overview"/> control.
        /// </summary>
        public double MinThumbHeight
        {
            get { return (double)GetValue(MinThumbHeightProperty); }
            set { SetValue(MinThumbHeightProperty, value); }
        }

        #region Diff Color definitions
        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorBackgroundContext
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundContextProperty); }
            set { SetValue(ColorBackgroundContextProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundDeletedProperty); }
            set { SetValue(ColorBackgroundDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundAdded
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundAddedProperty); }
            set { SetValue(ColorBackgroundAddedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is <see cref="Default(Color)"/> - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorBackgroundBlank
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundBlankProperty); }
            set { SetValue(ColorBackgroundBlankProperty, value); }
        }
        #endregion Diff Color Definitions
        #endregion properties

        #region methods
        /// <summary>
        /// Is called when the control is loaded to
        /// build the visual tree for the <see cref="Overview"/> control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _PART_ViewPortContainer = GetTemplateChild(PART_ViewPortContainer) as Grid;
            _PART_ImageViewport = GetTemplateChild(PART_ImageViewport) as Image;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event,
        /// using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            CreateBitmap(ItemsSource as IEnumerable<IDiffLineInfo>);
        }

        /// <summary>
        /// Creates the overview bitmap image whenever the layout or compared lines model
        /// has been changed.
        /// </summary>
        /// <param name="newList"></param>
        private void CreateBitmap(IEnumerable<IDiffLineInfo> newList)
        {
            if (_PART_ViewPortContainer == null || _PART_ImageViewport == null || newList == null)
            {
                this.Minimum = 0;
                this.Maximum = 0;
                OnViewPortLinesChanged(0);       // Initialize with 0 lines to hide the thumb control
                return;
            }

            if (_PART_ViewPortContainer.IsVisible == false)
                return;

            int numLines = newList.Count();
            UpdateMinMaxSliderBounds(numLines);

            int width = (int)this._PART_ViewPortContainer.ActualWidth;
            int height = (int)this._PART_ViewPortContainer.ActualHeight;

            if (width <= 0 || height <= 0)
                return;

            DrawBitMap(newList, numLines, width, height);
        }

        private void DrawBitMap(IEnumerable<IDiffLineInfo> newList,
                                int numLines, int width, int height)
        {
            // Init WriteableBitmap
            writeableBmp = BitmapFactory.New(width, height);

            _PART_ImageViewport.Source = writeableBmp;

            Color controlBackgroundColor;
            if (Background is SolidColorBrush)
                controlBackgroundColor = ((SolidColorBrush)Background).Color;
            else
                controlBackgroundColor = Color.FromRgb(0xff, 0xff, 0xff);

            // Clear Bitmap here
            using (writeableBmp.GetBitmapContext())
            {
                // Clear the WriteableBitmap with control background color
                writeableBmp.Clear(controlBackgroundColor);

                if (numLines == 0)
                    return;

                const float GutterWidth = 0.0F;

                // Make sure each line is at least 1 pixel high
                float lineHeight = (float)Math.Max(1.0, this.GetPixelLineHeight(1, numLines, height));

                int i = 0;
                foreach (var line in newList)
                {
                    if (line.Context != DiffContext.Blank)
                    {
                        double y = this.GetPixelLineHeight(i, numLines, height);
                        double fullFillWidth = width - (2 * GutterWidth);

                        var color = default(SolidColorBrush);
                        switch (line.Context)
                        {
                            case DiffContext.Context:
                                color = ColorBackgroundContext;
                                break;

                            case DiffContext.Deleted:
                                color = ColorBackgroundDeleted;
                                break;

                            case DiffContext.Added:
                                color = ColorBackgroundAdded;
                                break;

                            case DiffContext.Blank:
                                color = ColorBackgroundBlank;
                                break;

                            default:
                                break;
                        }

                        if (color != default(SolidColorBrush))
                        {
                            writeableBmp.FillRectangle((int)GutterWidth, (int)y,
                                                        (int)GutterWidth + (int)fullFillWidth,
                                                        (int)y + (int)lineHeight,
                                                        color.Color);
                        }
                    }

                    i++;
                }
            }
        }

        private double GetPixelLineHeight(int lines, int lineCount, double clientSize_Height)
		{
            double result = 0;

			if (lineCount > 0)
				result = clientSize_Height * (lines / (double)lineCount);

			return result;
		}

        #region Dependency Property Changed
        #region static handlers
        /// <summary>
        /// Is invoked when the <see cref="ItemsSource"/> dependency property has been changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Overview)d).ItemsSourceChanged(e.NewValue);
        }

        /// <summary>
        /// Is invoked when the <see cref="NumberOfTextLinesInViewPort"/> dependency property has been changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnViewPortLinesChanged(DependencyObject d,
                                                   DependencyPropertyChangedEventArgs e)
        {
            ((Overview)d).OnViewPortLinesChanged((int)e.NewValue);
        }

        /// <summary>
        /// Is invoked if one of the drawing parameters:
        /// 1> color definitions for deleted, added, changed, or blank line
        /// 2> minimum thumb height
        /// 
        /// has been changed.
        /// 
        /// This re-creates the complete drawing if one of the color definitions has been changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnBitmapParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Overview;
            if (sender != null)
                sender.OnBitmapParameterChanged(e.NewValue);
        }
        #endregion static handlers

        /// <summary>
        /// Is invoked if one of the drawing parameters:
        /// 1> color definitions for deleted, added, changed, or blank line
        /// 2> minimum thumb height
        /// 
        /// has been changed.
        /// 
        /// This re-creates the complete drawing if one of the color definitions has been changed.
        /// </summary>
        /// <param name="newValue"></param>
        private void OnBitmapParameterChanged(object newValue)
        {
            if (ItemsSource != null)
                CreateBitmap(ItemsSource as IEnumerable<IDiffLineInfo>);
        }

        /// <summary>
        /// Is invoked when the collection bound on the <see cref="ItemsSource"/> dependency
        /// property has changed.
        /// </summary>
        /// <param name="newValue"></param>
        private void ItemsSourceChanged(object newValue)
        {
            // Get observable events should they be available
            if (_observeableDiffContext != null)
            {
                _observeableDiffContext.CollectionChanged -= Observeable_CollectionChanged;
                _observeableDiffContext = null;
            }

            var observeable = newValue as INotifyCollectionChanged;
            if (observeable != null)
                observeable.CollectionChanged += Observeable_CollectionChanged;

            _observeableDiffContext = observeable;

            var newList = newValue as IEnumerable<IDiffLineInfo>;
            if (newList != null)
            {
                //System.Diagnostics.Debug.WriteLine("Overview items list changed {0}", newList.Count());
                CreateBitmap(newList);
                OnViewPortLinesChanged(NumberOfTextLinesInViewPort);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Overview items list changed to null");
                UpdateMinMaxSliderBounds(0);
            }
        }

        private void Observeable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newList = ItemsSource as IEnumerable<IDiffLineInfo>;
            if (newList != null)
            {
                //System.Diagnostics.Debug.WriteLine("Overview items list changed {0}", newList.Count());
                CreateBitmap(newList);
                OnViewPortLinesChanged(NumberOfTextLinesInViewPort);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Overview items list changed to null");
                UpdateMinMaxSliderBounds(0);
            }
        }
        #endregion  Dependency Property Changed

        /// <summary>
        /// Gets the number of items currently bound in the <see cref="ItemsSource"/>
        /// dependency property.
        /// </summary>
        /// <returns></returns>
        private int GetItemsCount()
        {
            var list = ItemsSource as IReadOnlyList<IDiffLineInfo>;
            if (list != null)
            {
                return list.Count;
            }

            var enumerable = ItemsSource as IEnumerable<IDiffLineInfo>;
            if (enumerable != null)
            {
                return enumerable.Count();
            }

            return 0;
        }

        /// <summary>
        /// Is invoked when the number of visible items in the (text) diff view
        /// control has changed.
        /// </summary>
        /// <param name="lines">The number of items currently
        /// visible in the (text) diff view.</param>
        private void OnViewPortLinesChanged(int lines)
        {
            double height = this._PART_ViewPortContainer.ActualHeight;
            int countLines = GetItemsCount();

            double thumbHeight = 0;
            bool isThumbVisible = false;

            // spare the computation if text viewport is larger than the available text
            if ((lines + 1) < countLines)
            {
                thumbHeight = GetPixelLineHeight(lines, countLines, height);

                // Larger thumb than image means there is nothing to scroll here
                // so we make thumb invisible by setting its height to 0
                if (thumbHeight > this._PART_ImageViewport.ActualHeight)
                {
                    thumbHeight = 0;
                    isThumbVisible = false;
                }
                else
                    isThumbVisible = true;
            }

            if (isThumbVisible == false)
                ThumbHeight = 0;
            else
            {
                if (thumbHeight < MinThumbHeight)
                    ThumbHeight = MinThumbHeight;
                else
                    ThumbHeight = thumbHeight;
            }

            UpdateMinMaxSliderBounds(countLines);
        }

        /// <summary>
        /// Updates the maximum and minimum value of the slider to reflect the
        /// bounds between which we can change the current value.
        /// </summary>
        /// <param name="countLines"></param>
        private void UpdateMinMaxSliderBounds(int countLines)
        {
            this.Minimum = 0;

            // Adding plus 1 to ensure that scrolling down always makes the last line fully visible
            if (countLines > 0)
                this.Maximum = countLines - NumberOfTextLinesInViewPort + 1;
            else
                this.Maximum = 0;
        }
        #endregion methods
    }
}
