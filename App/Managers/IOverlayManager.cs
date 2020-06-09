using System;
using System.Threading.Tasks;

namespace Medical.AppLayer.Managers
{
    public interface IOverlayManager
    {
        void ShowOverlay();
        Task ShowOverlay(string title, string message, int? min = null, int? max = null);
        void HideOverlay();
        void SetOverlayMessage(string message);
        void SetOverlayTitle(string title);
        void SetOverlayTitleAndMessage(string title = "", string message = "");
        void SetOverlayProgress(double percent);
        void SetOverlayProgress(double total, double progress);

    }
}
