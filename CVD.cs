// Copyright James Arlow / Jolly Wizard 2022

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace JollyWizard.Quantower.Indicators.VolumeDeltaPack
{
    /// <summary>
    /// Cumulative Volume Delta Indicator
    /// </summary>
	public class CVD : SVD
    {

        [InputParameter("Cumulation Length")]
        public int CumulationLength = 14;

        /// <summary>
        /// Defines the types of cumulation available.
        /// </summary>
        public enum CumulationType { SMA, EMA };

        /// <summary>
        /// Declare cumulation type as a menu option and variable.
        /// </summary>
        [InputParameter("Cumulation Type", variants:new object[] { 
            "SMA", CumulationType.SMA
        ,   "EMA", CumulationType.EMA
        })]
        public CumulationType CumulationMode = CumulationType.EMA;

        /// <summary>
        /// Because averages produce a singular sized result, cumulations look very small in comparison to large single candles.
        /// This normalizes the scale by making multiplying the average by the window length to create a SUM sized result with the same profile.
        /// </summary>
        [InputParameter("Normalize MAs")]
        public bool NormalizeMAs = true;

        /// <summary>
        /// Can be used to tweak visual scale for coexistence with other indicators,
        /// such as singular delta.
        /// </summary>
        [InputParameter("Scale Factor")]
        public double ScaleFactor = 1.0;

        /// <summary>
        /// The smoothing variable for EMA calculations.
        /// </summary>
        [InputParameter("EMA Smoothing")]
        public double EMA_Smoothing = 2.0;

        /// <summary>
        /// This is the multiplier that is used to Normalize MAs.
        /// Will be changed after config if applicable.
        /// </summary>
        public double NormalizeMultiplier = 1;

        /// <summary>
        /// The delta cumulative oscillator bars.
        /// </summary>
        public LineSeries LinePlot_Delta_Cumulative;

        /// <summary>
        /// The Cumulative buy trend line.
        /// </summary>
        public LineSeries LinePlot_Buy_Cumulative;

        /// <summary>
        /// The cumulative sell trend line.
        /// </summary>
        public LineSeries LinePlot_Sell_Cumulative;

        /// <summary>
        /// Indicator's constructor.
        /// </summary>
        public CVD()
            : base()
        {

            DeltaWidth = 50;

            Name = "CVD";
            Description = "Cumulative Volume Delta";

            LinePlot_Delta_Cumulative = AddLineSeries("Volume Delta Cumulative", DefaultColor, DeltaWidth, LineStyle.Histogramm);
            LinePlot_Buy_Cumulative = AddLineSeries("Volume Buy Cumulative", DefaultColor, BuySellWidth);
            LinePlot_Sell_Cumulative = AddLineSeries("Volume Sell Cumulative", DefaultColor, BuySellWidth);
        }

        /// <summary>
        /// This function will be called after creating an indicator as well as after its input params reset or chart (symbol or timeframe) updates.
        /// </summary>
        protected override void OnInit() 
        {
            base.OnInit();

            // Will out of bounds if this is not set.
            LinePlot_Delta_Cumulative.DrawBegin = CumulationLength;

            // This can be const once the configuration is set.
            NormalizeMultiplier = NormalizeMAs ? CumulationLength : 1;

            // Use transparency to toggle Buy/Sell
            if (RenderBuySell)
            {
                LinePlot_Buy_Cumulative.Color = Bull_Color;
                LinePlot_Sell_Cumulative.Color = Bear_Color;
            }
            else
            {
                LinePlot_Buy_Cumulative.Color = DefaultColor;
                LinePlot_Sell_Cumulative.Color = DefaultColor;
            }

            // Widths need to be reconfigured here to accept variable setting.
            LinePlot_Delta_Cumulative.Width = DeltaWidth;
            LinePlot_Buy_Cumulative.Width = BuySellWidth;
            LinePlot_Sell_Cumulative.Width = BuySellWidth;

            // These are hidden, but you can put the ancestor in the same panel, so no need for config.
            base.LinePlot_Buy.Visible = false;
            base.LinePlot_Sell.Visible = false;
            base.LinePlot_Delta.Visible = false;
        }

        /// <summary>
        /// Calculations go here.
        /// </summary>
        protected override void OnUpdate(UpdateArgs args)
        {
            base.OnUpdate(args);

            double volume_buy_cumulative = Cumulate(LinePlot_Buy);
            double volume_sell_cumulative = Cumulate(LinePlot_Sell);

            double volume_delta_cumulative = volume_buy_cumulative + volume_sell_cumulative; 

            if (RenderBuySell)
            {
                LinePlot_Buy_Cumulative.SetValue(volume_buy_cumulative);
                LinePlot_Sell_Cumulative.SetValue(volume_sell_cumulative);
            }

            Color volume_delta_cumulative_color = volume_delta_cumulative > 0 ? Bull_Color : Bear_Color;
            LinePlot_Delta_Cumulative.SetValue(volume_delta_cumulative);
            LinePlot_Delta_Cumulative.SetMarker(offset: 0, color: volume_delta_cumulative_color);
        }

        public double Cumulate(LineSeries series)
        {
            return CumulationMode switch
            {
                CumulationType.SMA => Cumulate_SMA(series, CumulationLength)
            ,   CumulationType.EMA => Cumulate_EMA(series, CumulationLength)
            ,   _ => Double.NaN
            } * NormalizeMultiplier * ScaleFactor;
        }

        public double Cumulate_SUM(LineSeries series, int cumulation_length)
        {
            double r = 0;
            for (int i = 0; i < cumulation_length; i++)
                r += series.GetValue(offset:i);
            return r;
        }

        public double Cumulate_SMA(LineSeries series, int cumulation_length)
        {
            return Cumulate_SUM(series, cumulation_length) / cumulation_length;
        }

        public double Cumulate_EMA(LineSeries series, int cumulation_length)
        {
            // This is the range in the series we are going to iterate.
            int startIndex = cumulation_length - 1;
            int lastIndex = 0;

            // Default EMA_k for now. Smoothing / (N + 1)
            double EMA_k = EMA_Smoothing / (cumulation_length + 1);

            double VALUE_this = series.GetValue(startIndex);
            double EMA_this = VALUE_this * EMA_k;
            double EMA_last = 0.0;

            for (int i = startIndex - 1; i >= lastIndex; i--)
            {
                EMA_last = EMA_this;

                VALUE_this = series.GetValue(i);

                EMA_this = (VALUE_this * EMA_k) + (EMA_last * (1 - EMA_k));
            }

            return EMA_this;
        }
    }
}
