//-----------------------------------------------------------------------
// <copyright file="PerformanceChart.cs" company="YuGuan Corporation">
//     Copyright (c) YuGuan Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DBCHM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// Scale mode for value aspect ratio.
    /// </summary>
    public enum ScaleMode
    {
        /// <summary>
        /// Values from 0 to 100 are accepted and displayed.
        /// </summary>
        Absolute,

        /// <summary>
        /// All values are allowed and displayed in a proper relation.
        /// </summary>
        Relative
    }

    /// <summary>
    /// Chart refresh mode.
    /// </summary>
    public enum RefreshMode
    {
        /// <summary>
        /// Chart is refreshed when a value is added.
        /// </summary>
        Disabled,

        /// <summary>
        /// Chart is refreshed every refresh interval milliseconds, adding all values in the queue to the chart. If there are no values in the queue, 0 (zero) is added.
        /// </summary>
        Simple,

        /// <summary>
        /// Chart is refreshed every refresh interval milliseconds, adding an average of all values in the queue to the chart. If there are no values in the queue, 0 (zero) is added.
        /// </summary>
        SynchronizedAverage,

        /// <summary>
        /// Chart is refreshed every refresh interval milliseconds, adding the sum of all values in the queue to the chart. If there are no values in the queue, 0 (zero) is added.
        /// </summary>
        SynchronizedSum
    }

    /// <summary>
    /// PerformanceChart UserControl.
    /// </summary>
    public class PerformanceChart : UserControl
    {
        /// <summary>
        /// Field MaxValueCount.
        /// </summary>
        private const int MaxValueCount = 512;

        /// <summary>
        /// Field GridSpacing.
        /// </summary>
        private const int GridSpacing = 12;

        /// <summary>
        /// Field _valueSpacing.
        /// </summary>
        private const int ValueSpacing = 2;

        /// <summary>
        /// Field components.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Field _timer.
        /// </summary>
        private System.Windows.Forms.Timer _timer;

        /// <summary>
        /// Field _visibleValues.
        /// </summary>
        private int _visibleValues = 0;

        /// <summary>
        /// Field _gridScrollOffset.
        /// </summary>
        private int _gridScrollOffset = 0;

        /// <summary>
        /// Field _range.
        /// </summary>
        private decimal _range = 100;

        /// <summary>
        /// Field _maximum.
        /// </summary>
        private decimal _maximum = 100;

        /// <summary>
        /// Field _minimum.
        /// </summary>
        private decimal _minimum = 0;

        /// <summary>
        /// Field _refreshMode.
        /// </summary>
        private RefreshMode _refreshMode;

        /// <summary>
        /// Field _drawValues.
        /// </summary>
        private List<decimal> _drawValues = new List<decimal>(MaxValueCount);

        /// <summary>
        /// Field _waitingValues.
        /// </summary>
        private Queue<decimal> _waitingValues = new Queue<decimal>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceChart"/> class.
        /// </summary>
        public PerformanceChart()
        {
            this.components = new Container();
            this.ChartStyle = new PerformanceChartStyle();
            this._timer = new System.Windows.Forms.Timer(this.components);

            this.SuspendLayout();

            this.Name = "PerformanceChart";
            this._timer.Tick += this.TimerTick;
            this.ScaleMode = ScaleMode.Absolute;
            this.Font = SystemInformation.MenuFont;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(100)]
        public decimal Maximum
        {
            get
            {
                return this._maximum;
            }

            set
            {
                this._maximum = value;
                this._range = this.Maximum - this.Minimum;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        public decimal Minimum
        {
            get
            {
                return this._minimum;
            }

            set
            {
                this._minimum = value;
                this._range = this.Maximum - this.Minimum;
            }
        }

        /// <summary>
        /// Gets or sets the chart style.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        public PerformanceChartStyle ChartStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale mode.
        /// </summary>
        public ScaleMode ScaleMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the refresh mode.
        /// </summary>
        public RefreshMode RefreshMode
        {
            get
            {
                return this._refreshMode;
            }

            set
            {
                if (value == RefreshMode.Disabled)
                {
                    if (this._refreshMode != RefreshMode.Disabled)
                    {
                        this._refreshMode = value;

                        this._timer.Stop();
                        this.ChartAppendFromQueue();
                    }
                }
                else
                {
                    this._refreshMode = value;
                    this._timer.Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets the refresh interval in milliseconds.
        /// </summary>
        public int RefreshInterval
        {
            get
            {
                return this._timer.Interval;
            }

            set
            {
                this._timer.Interval = value;
            }
        }

        /// <summary>
        /// Clears the whole chart.
        /// </summary>
        public void Clear()
        {
            this._drawValues.Clear();
            this.Invalidate();
        }

        /// <summary>
        /// Adds a value to the Chart Line.
        /// </summary>
        /// <param name="value">The performance value.</param>
        public void AddValue(decimal value)
        {
            if (this.ScaleMode == ScaleMode.Absolute && value > this.Maximum)
            {
                value = this.Maximum;
            }

            switch (this._refreshMode)
            {
                case RefreshMode.Disabled:
                    this.ChartAppend(value);
                    this.Invalidate();
                    break;

                case RefreshMode.Simple:
                case RefreshMode.SynchronizedAverage:
                case RefreshMode.SynchronizedSum:
                    this.AddValueToQueue(value);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();

                if (this._timer != null)
                {
                    this._timer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.ChartStyle.AntiAliasing)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            this.DrawBackgroundAndGrid(e.Graphics);

            this.DrawChart(e.Graphics);
        }

        /// <summary>
        /// Raises the OnResize event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Invalidate();
        }

        /// <summary>
        /// Add value to the queue for a timed refresh.
        /// </summary>
        /// <param name="value">The value.</param>
        private void AddValueToQueue(decimal value)
        {
            this._waitingValues.Enqueue(value);
        }

        /// <summary>
        /// Appends value <paramref name="value" /> to the chart without redrawing.
        /// </summary>
        /// <param name="value">The performance value.</param>
        private void ChartAppend(decimal value)
        {
            this._drawValues.Insert(0, Math.Max(value, this.Minimum) - this.Minimum);

            if (this._drawValues.Count > MaxValueCount)
            {
                this._drawValues.RemoveAt(MaxValueCount);
            }

            this._gridScrollOffset += ValueSpacing;

            if (this._gridScrollOffset > GridSpacing)
            {
                this._gridScrollOffset = this._gridScrollOffset % GridSpacing;
            }
        }

        /// <summary>
        /// Appends values from queue.
        /// </summary>
        private void ChartAppendFromQueue()
        {
            if (this._waitingValues.Count > 0)
            {
                if (this._refreshMode == RefreshMode.Simple)
                {
                    while (this._waitingValues.Count > 0)
                    {
                        this.ChartAppend(this._waitingValues.Dequeue());
                    }
                }
                else if (this._refreshMode == RefreshMode.SynchronizedAverage || this._refreshMode == RefreshMode.SynchronizedSum)
                {
                    decimal appendValue = decimal.Zero;

                    int valueCount = this._waitingValues.Count;

                    while (this._waitingValues.Count > 0)
                    {
                        appendValue += this._waitingValues.Dequeue();
                    }

                    if (this._refreshMode == RefreshMode.SynchronizedAverage)
                    {
                        appendValue = appendValue / (decimal)valueCount;
                    }

                    this.ChartAppend(appendValue);
                }
            }
            else
            {
                this.ChartAppend(decimal.Zero);
            }

            this.Invalidate();
        }

        /// <summary>
        /// Calculates the vertical position of a value in relation the chart size, scale mode and, if scale mode is relative, to the current maximum value.
        /// </summary>
        /// <param name="value">The performance value.</param>
        /// <returns>The vertical point position in pixels.</returns>
        private int CalcVerticalPosition(decimal value)
        {
            decimal result = decimal.Zero;

            if (this.ScaleMode == ScaleMode.Absolute)
            {
                result = (value * this.Height) / this._range;
            }
            else if (this.ScaleMode == ScaleMode.Relative)
            {
                decimal currentMaxValue = this.GetCurrentMaxValue();

                result = (currentMaxValue > this.Minimum) ? ((value * this.Height) / (currentMaxValue - this.Minimum)) : 0;
            }

            result = this.Height - result;

            return Convert.ToInt32(Math.Round(result));
        }

        /// <summary>
        /// Returns the currently highest displayed value.
        /// </summary>
        /// <returns>The highest value.</returns>
        private decimal GetCurrentMaxValue()
        {
            decimal maxValue = this.Minimum;

            for (int i = 0; i < this._visibleValues; i++)
            {
                if (this._drawValues[i] > maxValue)
                {
                    maxValue = this._drawValues[i];
                }
            }

            return maxValue;
        }

        /// <summary>
        /// Returns the currently lowest displayed value.
        /// </summary>
        /// <returns>The lowest value.</returns>
        private decimal GetCurrentMinValue()
        {
            decimal minValue = this.Maximum;

            for (int i = 0; i < this._visibleValues; i++)
            {
                if (this._drawValues[i] < minValue)
                {
                    minValue = this._drawValues[i];
                }
            }

            return minValue;
        }

        /// <summary>
        /// Returns the currently average displayed value.
        /// </summary>
        /// <returns>The average value.</returns>
        private decimal GetCurrentAvgValue()
        {
            decimal avgValue = 0;

            for (int i = 0; i < this._visibleValues; i++)
            {
                avgValue += this._drawValues[i];
            }

            avgValue = avgValue / (this._visibleValues == 0 ? 1 : this._visibleValues);

            return Math.Round(avgValue);
        }

        /// <summary>
        /// Draws the chart to the graphics canvas.
        /// </summary>
        /// <param name="g">The graphics instance.</param>
        private void DrawChart(Graphics g)
        {
            this._visibleValues = Math.Min(this.Width / ValueSpacing, this._drawValues.Count);

            Point previousPoint = new Point(this.Width + ValueSpacing, this.Height);
            Point currentPoint = new Point();

            for (int i = 0; i < this._visibleValues; i++)
            {
                currentPoint.X = previousPoint.X - ValueSpacing;
                currentPoint.Y = this.CalcVerticalPosition(this._drawValues[i]);

                g.DrawLine(this.ChartStyle.ChartLinePen.Pen, previousPoint, currentPoint);

                previousPoint = currentPoint;
            }

            decimal currentMaxValue = this.GetCurrentMaxValue();

            decimal currentMinValue = this.GetCurrentMinValue();

            decimal currentAvgValue = this.GetCurrentAvgValue();

            if (this._visibleValues > 0)
            {
                if (this.ChartStyle.ShowMaxLine)
                {
                    int verticalPosition = this.CalcVerticalPosition(currentMaxValue);
                    g.DrawLine(this.ChartStyle.MaxLinePen.Pen, 0, verticalPosition, this.Width, verticalPosition);
                }

                if (this.ChartStyle.ShowMinLine)
                {
                    int verticalPosition = this.CalcVerticalPosition(currentMinValue);

                    if (verticalPosition >= this.Height)
                    {
                        verticalPosition = this.Height - 1;
                    }

                    g.DrawLine(this.ChartStyle.MinLinePen.Pen, 0, verticalPosition, this.Width, verticalPosition);
                }

                if (this.ChartStyle.ShowAvgLine)
                {
                    int verticalPosition = this.CalcVerticalPosition(currentAvgValue);
                    g.DrawLine(this.ChartStyle.AvgLinePen.Pen, 0, verticalPosition, this.Width, verticalPosition);
                }

                if (this.ChartStyle.ShowCurLine)
                {
                    int verticalPosition = this.CalcVerticalPosition(this._drawValues[0]);
                    g.DrawLine(this.ChartStyle.CurLinePen.Pen, 0, verticalPosition, this.Width, verticalPosition);
                }
            }

            if (this.ChartStyle.ShowMaxText)
            {
                TextFormatFlags flags = this.GetTextFormatFlags(this.ChartStyle.MaxTextAlign, false) | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(g, "MAX: " + (this._drawValues.Count > 0 ? currentMaxValue.ToString() : "n/a"), this.Font, this.ClientRectangle, this.ChartStyle.MaxLinePen.Color, flags);
            }

            if (this.ChartStyle.ShowMinText)
            {
                TextFormatFlags flags = this.GetTextFormatFlags(this.ChartStyle.MinTextAlign, false) | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(g, "MIN: " + (this._drawValues.Count > 0 ? currentMinValue.ToString() : "n/a"), this.Font, this.ClientRectangle, this.ChartStyle.MinLinePen.Color, flags);
            }

            if (this.ChartStyle.ShowAvgText)
            {
                TextFormatFlags flags = this.GetTextFormatFlags(this.ChartStyle.AvgTextAlign, false) | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(g, "AVG: " + (this._drawValues.Count > 0 ? currentAvgValue.ToString() : "n/a"), this.Font, this.ClientRectangle, this.ChartStyle.AvgLinePen.Color, flags);
            }

            if (this.ChartStyle.ShowCurText)
            {
                TextFormatFlags flags = this.GetTextFormatFlags(this.ChartStyle.CurTextAlign, false) | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;

                TextRenderer.DrawText(g, "CUR: " + (this._drawValues.Count > 0 ? this._drawValues[0].ToString() : "n/a"), this.Font, this.ClientRectangle, this.ChartStyle.CurLinePen.Color, flags);
            }
        }

        /// <summary>
        /// Draws the background gradient and the grid into Graphics <paramref name="g" />
        /// </summary>
        /// <param name="g">The graphics instance.</param>
        private void DrawBackgroundAndGrid(Graphics g)
        {
            Rectangle baseRectangle = new Rectangle(0, 0, this.Width, this.Height);

            using (Brush brush = new LinearGradientBrush(baseRectangle, this.ChartStyle.BackgroundColorTop, this.ChartStyle.BackgroundColorBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, baseRectangle);
            }

            if (this.ChartStyle.ShowVerticalGridLines)
            {
                for (int i = this.Width - this._gridScrollOffset; i >= 0; i -= GridSpacing)
                {
                    g.DrawLine(this.ChartStyle.VerticalGridPen.Pen, i, 0, i, this.Height);
                }
            }

            if (this.ChartStyle.ShowHorizontalGridLines)
            {
                for (int i = 0; i < this.Height; i += GridSpacing)
                {
                    g.DrawLine(this.ChartStyle.HorizontalGridPen.Pen, 0, i, this.Width, i);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            this.ChartAppendFromQueue();
        }

        /// <summary>
        /// Get TextFormatFlags.
        /// </summary>
        /// <param name="textAlign">Specifies alignment of text on the drawing surface.</param>
        /// <param name="wrap">Wrap text or not.</param>
        /// <returns>TextFormatFlags instance.</returns>
        private TextFormatFlags GetTextFormatFlags(ContentAlignment textAlign, bool wrap)
        {
            TextFormatFlags controlFlags = wrap ? TextFormatFlags.WordBreak : TextFormatFlags.EndEllipsis;

            switch (textAlign)
            {
                case ContentAlignment.TopLeft:
                    controlFlags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;

                case ContentAlignment.TopCenter:
                    controlFlags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.TopRight:
                    controlFlags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;

                case ContentAlignment.MiddleLeft:
                    controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;

                case ContentAlignment.MiddleCenter:
                    controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.MiddleRight:
                    controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;

                case ContentAlignment.BottomLeft:
                    controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;

                case ContentAlignment.BottomCenter:
                    controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.BottomRight:
                    controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
            }

            return controlFlags;
        }
    }

    /// <summary>
    /// PerformanceChart Style.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class PerformanceChartStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceChartStyle"/> class.
        /// </summary>
        public PerformanceChartStyle()
        {
            this.ShowMaxLine = true;
            this.ShowMaxText = true;
            this.MaxTextAlign = ContentAlignment.TopLeft;
            this.MaxLinePen = new PerformanceChartPen() { Color = Color.Red };

            this.ShowMinLine = true;
            this.ShowMinText = true;
            this.MinTextAlign = ContentAlignment.BottomLeft;
            this.MinLinePen = new PerformanceChartPen() { Color = Color.FromArgb(0, 128, 255) };

            this.ShowAvgLine = true;
            this.ShowAvgText = true;
            this.AvgTextAlign = ContentAlignment.MiddleLeft;
            this.AvgLinePen = new PerformanceChartPen() { Color = Color.Yellow };

            this.ShowCurLine = true;
            this.ShowCurText = true;
            this.CurTextAlign = ContentAlignment.TopRight;
            this.CurLinePen = new PerformanceChartPen() { Color = Color.Lime };

            this.BackgroundColorTop = Color.Black;
            this.BackgroundColorBottom = Color.Black;
            this.ShowVerticalGridLines = true;
            this.ShowHorizontalGridLines = true;
            this.AntiAliasing = true;
            this.VerticalGridPen = new PerformanceChartPen();
            this.HorizontalGridPen = new PerformanceChartPen();
            this.ChartLinePen = new PerformanceChartPen() { Color = Color.Lime };
        }

        /// <summary>
        /// Gets or sets a value indicating whether show maximum line.
        /// </summary>
        public bool ShowMaxLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show maximum value.
        /// </summary>
        public bool ShowMaxText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum value align.
        /// </summary>
        public ContentAlignment MaxTextAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of maximum line pen.
        /// </summary>
        public PerformanceChartPen MaxLinePen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show minimum line.
        /// </summary>
        public bool ShowMinLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show minimum value.
        /// </summary>
        public bool ShowMinText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum value align.
        /// </summary>
        public ContentAlignment MinTextAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of minimum line pen.
        /// </summary>
        public PerformanceChartPen MinLinePen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show average line.
        /// </summary>
        public bool ShowAvgLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show average value.
        /// </summary>
        public bool ShowAvgText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the average value align.
        /// </summary>
        public ContentAlignment AvgTextAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of average line pen.
        /// </summary>
        public PerformanceChartPen AvgLinePen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show current line.
        /// </summary>
        public bool ShowCurLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show current value.
        /// </summary>
        public bool ShowCurText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current value align.
        /// </summary>
        public ContentAlignment CurTextAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of current line pen.
        /// </summary>
        public PerformanceChartPen CurLinePen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show vertical grid lines.
        /// </summary>
        public bool ShowVerticalGridLines
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show horizontal grid lines.
        /// </summary>
        public bool ShowHorizontalGridLines
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical grid pen.
        /// </summary>
        public PerformanceChartPen VerticalGridPen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal grid pen.
        /// </summary>
        public PerformanceChartPen HorizontalGridPen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the chart line pen.
        /// </summary>
        public PerformanceChartPen ChartLinePen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether anti aliasing.
        /// </summary>
        public bool AntiAliasing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the background top color.
        /// </summary>
        public Color BackgroundColorTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the background bottom color.
        /// </summary>
        public Color BackgroundColorBottom
        {
            get;
            set;
        }
    }

    /// <summary>
    /// PerformanceChart Pen.
    /// </summary>
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class PerformanceChartPen
    {
        /// <summary>
        /// Field _pen.
        /// </summary>
        private Pen _pen;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceChartPen"/> class.
        /// </summary>
        public PerformanceChartPen()
        {
            this.Pen = new Pen(Color.FromArgb(0, 128, 64));
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.Pen.Color;
            }

            set
            {
                this.Pen.Color = value;
            }
        }

        /// <summary>
        /// Gets or sets the dash style.
        /// </summary>
        public DashStyle DashStyle
        {
            get
            {
                return this.Pen.DashStyle;
            }

            set
            {
                this.Pen.DashStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width
        {
            get
            {
                return this.Pen.Width;
            }

            set
            {
                this.Pen.Width = value;
            }
        }

        /// <summary>
        /// Gets the pen.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Pen Pen
        {
            get
            {
                return this._pen;
            }

            private set
            {
                this._pen = value;
            }
        }
    }
}
