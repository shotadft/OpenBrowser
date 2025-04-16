using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenBrowser.Windows.Controls
{
    [ToolboxBitmap(typeof(TabControl))]
    public class CustomTabControl : TabControl, IDisposable
    {
        static CustomTabControl() =>
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTabControl), new FrameworkPropertyMetadata(typeof(TabControl)));

        public CustomTabControl()
        {
            this.Loaded += OnLoaded;
            this.SizeChanged += new((sender, e) => UpdateTabWidths());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private async void OnLoaded(object? sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && this.Items.Count == 0) await AddTabAsync();

            UpdateTabWidths();
        }

        /// <summary>
        /// Add New Tab
        /// </summary>
        internal async Task AddTabAsync()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                if (this.Items.Count >= 28) return;

                TabItem tabItem = new()
                {
                    Name = $"Tab{this.Items.Count}",
                    Header = "New Tab",
                    Content = new ScrollViewer
                    {
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = new Grid
                        {
                            Background = System.Windows.Media.Brushes.White
                        }
                    }
                };

                this.Items.Add(tabItem);
                this.SelectedItem = tabItem;
                UpdateTabWidths();
            });
        }

        /// <summary>
        /// Delete Specific Tab
        /// </summary>
        internal async Task CloseTabAsync(TabItem tabItem)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                if (this.Items.Contains(tabItem))
                {
                    this.Items.Remove(tabItem);
                    if (this.Items.Count <= 0) Application.Current.Shutdown();
                    UpdateTabWidths();
                }
            });
        }

        // Auto tab size adjustment
        private const double MinTabWidth = 20.0;
        private const double MaxTabWidth = 170.0;
        private const double PlusButtonWidth = 20.0 + 10.0;

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            System.Windows.Size size = base.MeasureOverride(constraint);

            if (this.Items.Count > 0) UpdateTabWidths(constraint.Width);

            return size;
        }

        public void UpdateTabWidths(double availableWidth = double.NaN)
        {
            if (this.Items.Count == 0) return;

            if (double.IsNaN(availableWidth)) availableWidth = ActualWidth;

            double totalHeaderWidth = this.Items.Cast<TabItem>().Sum(tab => MeasureHeaderWidth(tab));
            double tabWidth = (availableWidth - PlusButtonWidth) / this.Items.Count;

            if (tabWidth < MinTabWidth) tabWidth = MinTabWidth;
            if (tabWidth > MaxTabWidth) tabWidth = MaxTabWidth;

            double remainingWidth = availableWidth - PlusButtonWidth;
            double newTabWidth = remainingWidth / this.Items.Count;

            foreach (TabItem tabItem in this.Items)
            {
                if (totalHeaderWidth + PlusButtonWidth > availableWidth)
                    tabItem.Width = Math.Max(newTabWidth, MinTabWidth);
                else
                    tabItem.Width = Math.Min(tabWidth, MaxTabWidth);
            }
        }

        private double MeasureHeaderWidth(TabItem tabItem)
        {
            if (tabItem.Header is string header)
            {
                var dpi = VisualTreeHelper.GetDpi(this);
                var formattedText = new FormattedText(
                    header,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(tabItem.FontFamily, tabItem.FontStyle, tabItem.FontWeight, tabItem.FontStretch),
                    tabItem.FontSize,
                    tabItem.Foreground,
                    dpi.PixelsPerDip);

                return formattedText.Width;
            }

            return MinTabWidth;
        }

        public void Dispose()
        {
            this.Loaded -= OnLoaded;
            this.SizeChanged -= new((sender, e) => UpdateTabWidths());
            GC.SuppressFinalize(this);
        }
    }
}