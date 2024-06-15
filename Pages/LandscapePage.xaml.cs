using KalkulatorMAUI_MVVM.ViewModels;

namespace KalkulatorMAUI_MVVM.Pages;

public partial class LandscapePage : ContentPage
{
    private LandscapeViewModel viewModel;

    public LandscapePage()
    {
        InitializeComponent();
        BindingContext = new LandscapeViewModel();
        viewModel = BindingContext as LandscapeViewModel;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        var orientation = DeviceDisplay.Current.MainDisplayInfo.Orientation;
        bool isPortrait = orientation == DisplayOrientation.Portrait;
        if (isPortrait)
        {
            viewModel.BackToPortraitCommand.Execute(null);
        }
    }
}
