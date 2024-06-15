using KalkulatorMAUI_MVVM.ViewModels;

namespace KalkulatorMAUI_MVVM.Pages;

public partial class LandscapePage : ContentPage
{
    private PageViewModel viewModel;

    public LandscapePage()
    {
        InitializeComponent();
        BindingContext = new PageViewModel();
        viewModel = BindingContext as PageViewModel;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        var orientation = DeviceDisplay.Current.MainDisplayInfo.Orientation;
        bool isPortrait = orientation == DisplayOrientation.Portrait;
        viewModel.UpdateOrientation(isPortrait);
    }
}
