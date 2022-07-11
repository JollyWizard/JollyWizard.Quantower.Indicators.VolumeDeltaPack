// Copyright James Arlow / Jolly Wizard 2022

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace JollyWizard.Quantower.Indicators.VolumeDeltaPack
{

    /// <summary>
    /// Singular Volume Delta Indicator
    /// </summary>
	public class SVD : Indicator
    {
        public static Color DefaultColor = Color.Transparent;

        public LineSeries LinePlot_Delta;

        public LineSeries LinePlot_Buy;
        public LineSeries LinePlot_Sell;

        const int DEFAULT_Transparency = 200;

        [InputParameter("Bull Color")]
        public Color Bull_Color = Color.FromArgb(DEFAULT_Transparency, Color.DarkGreen);

        [InputParameter("Bear Color")]
        public Color Bear_Color = Color.FromArgb(DEFAULT_Transparency, Color.DarkRed);

        [InputParameter("Delta Width (Max)")]
        public int DeltaWidth = 5;

        [InputParameter("Render Buy Sell")]
        public bool RenderBuySell = true;

        [InputParameter("Buy/Sell Width (Max)")]
        public int BuySellWidth = 2;

        /// <summary>
        /// Indicator's constructor.
        /// </summary>
        public SVD()
            : base()
        {
            // By default indicator will be applied on separate window of the chart.
            this.SeparateWindow = true;

            LinePlot_Delta = AddLineSeries("Volume Delta", DefaultColor, DeltaWidth, LineStyle.Histogramm);

            LinePlot_Buy = AddLineSeries("Volume Buy", DefaultColor, BuySellWidth, LineStyle.Dot);
            LinePlot_Sell = AddLineSeries("Volume Sell", DefaultColor, BuySellWidth, LineStyle.Dot);

            LineLevel ZeroLine = new LineLevel(0, "Zero Line", Color.SteelBlue, 1, LineStyle.Dot);
            // TODO. Find and remove the price line marker setting for this before adding.
            AddLineLevel(ZeroLine);

            // Defines indicator's name and description.
            Name = "SVD";
            Description = "Singular Volume Delta + Cumulative Volume Delta";
        }

        /// <summary>
        /// This function will be called after creating an indicator as well as after its input params reset or chart (symbol or timeframe) updates.
        /// </summary>
        protected override void OnInit()
        {
            LinePlot_Buy.Visible = RenderBuySell;
            LinePlot_Sell.Visible = RenderBuySell;

            LinePlot_Delta.Width = DeltaWidth;
            LinePlot_Buy.Width = BuySellWidth;
            LinePlot_Sell.Width = BuySellWidth;

            LinePlot_Buy.Color = Bull_Color;
            LinePlot_Sell.Color = Bear_Color;
        }

        /// <summary>
        /// Calculations go here.
        /// </summary>
        protected override void OnUpdate(UpdateArgs args)
        {

            // <essential calculations>

            bool bullCandle = (Open() < Close());
            double price_spread = (High() - Low());

            // Establish lengths.

            double upper_wick_length = bullCandle ? (High() - Close()) : (High() - Open());
            double lower_wick_length = bullCandle ? (Open() - Low())   : (Close() - Low());
            double body_length = price_spread - (upper_wick_length + lower_wick_length);

            // Convert lengths to Percents
            double upper_wick_percent = (upper_wick_length / price_spread);
            double lower_wick_percent = (lower_wick_length / price_spread);
            double body_percent = (body_length / price_spread);

            // Convert percentage lengths into assignment groups.
            double wicks_effective_divisor = 2;
            double wicks_percent = (upper_wick_percent + lower_wick_percent);
            double wicks_effective_percent = (wicks_percent / wicks_effective_divisor);

            double volume_buy_percent  = wicks_effective_percent + ( bullCandle ? body_percent : 0);
            double volume_sell_percent = wicks_effective_percent + (!bullCandle ? body_percent : 0);

            // Assign to volumes.
            double volume_buy = Volume() * volume_buy_percent;
            double volume_sell = Volume() * volume_sell_percent;

            LinePlot_Buy.SetValue(volume_buy);
            LinePlot_Sell.SetValue(-volume_sell);

            double volume_delta = (volume_buy - volume_sell);

            bool volume_delta_isBullish = (volume_delta > 0);

            LinePlot_Delta.SetValue(volume_delta);
            
            // Color bar based on direction.
            Color volume_delta_color = volume_delta_isBullish ? Bull_Color : Bear_Color;
            LinePlot_Delta.SetMarker(offset: 0, volume_delta_color);
        }
    }
}
