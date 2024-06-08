using KalkulatorMAUI_MVVM.ViewModels;

namespace KalkulatorMAUI_MVVM.Views;

public partial class MainCalculatorPage : ContentPage
{
    private StandardView standardView;
    private ScientificView scientificView;

	public MainCalculatorPage()
	{
        var viewModel = new CalculatorViewModel();
        BindingContext = viewModel;
        standardView = new StandardView();
        scientificView = new ScientificView();

        scientificView.BindingContext = viewModel;
        standardView.BindingContext = viewModel;

        Content = standardView;
		SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object? sender, EventArgs e)
	{
        var orientation = DeviceDisplay.Current.MainDisplayInfo.Orientation;

        if (orientation == DisplayOrientation.Portrait)
        {
            Content = standardView;
        }
        else if (orientation == DisplayOrientation.Landscape)
        {
            Content = scientificView;
        }
    }
}