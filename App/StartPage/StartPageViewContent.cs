using Medical.AppLayer.StartPage.ViewModels;
using Medical.AppLayer.StartPage.Views;

namespace Medical.AppLayer.StartPage
{
	public class StartPageViewContent 
	{
        private readonly StartPageView _content;
		
		public object Control {
			get {
				return _content;
			}
		}
		
		public StartPageViewContent()
		{
		    _content = new StartPageView {DataContext = new StartPageViewModel()};
		    _content.LoadLayout();
		    
		}
	}
}
