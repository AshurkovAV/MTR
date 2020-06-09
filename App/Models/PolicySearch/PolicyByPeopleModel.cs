using System;
using System.Windows.Media;
using Core;
using Core.Extensions;
using GalaSoft.MvvmLight;
using Medical.DatabaseCore.Services.Database;


namespace Medical.AppLayer.Models.PolicySearch
{
    public class PolicyByPeopleModel : ViewModelBase
    {
        private Brush _background;

        public int? Id { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateStop { get; set; }
        public DateTime? DateEnd { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicySerial { get; set; }
        public int? PolicyType { get; set; }

        public DateTime? EventBegin { get; set; }
        public DateTime? EventEnd { get; set; }

        public string FullPolicy
        {
            get
            {

                if (TerritoryService.TerritoryDefaultOmsVersion != 46)
                {
                    if ((DateBegin.Value <= EventBegin)
                        && ((DateStop == null) || (DateStop.Value >= EventBegin)))
                    {
                        Background = new LinearGradientBrush(Color.FromArgb(100, 0, 255, 0), Color.FromArgb(0, 255, 0, 0), 0);
                    }
                    else
                    {
                        Background = new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(0, 255, 0, 0), 0);
                    }
                }
                else
                {
                    if ((DateStop == null || (DateStop.Value >= EventBegin)) &&
                       (DateEnd == null || (DateEnd.Value >= EventBegin)))
                    {
                        Background = new LinearGradientBrush(Color.FromArgb(100, 0, 255, 0), Color.FromArgb(0, 255, 0, 0), 0);
                    }
                    else
                    {
                        Background = new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(0, 255, 0, 0), 0);
                    }
                }
               

                if ((PolicyType?) PolicyType == Core.PolicyType.INP)
                {
                    return "{0} - {1}/{2}".F(PolicyNumber,
                        DateStop.HasValue ? DateStop.Value.ToString("yyyy-MM-dd") : "нет",
                        DateEnd.HasValue ? DateEnd.Value.ToString("yyyy-MM-dd") : "нет");

                }
                else
                {
                    return "{0} {1} - {2}/{3}".F(PolicySerial, PolicyNumber,
                        DateStop.HasValue ? DateStop.Value.ToString("yyyy-MM-dd") : "нет",
                        DateEnd.HasValue ? DateEnd.Value.ToString("yyyy-MM-dd") : "нет");
                }
            }
        }

        public Brush Background
        {
            get { return _background; }
            set { _background = value; RaisePropertyChanged(()=>Background); }
        }
    }
}
