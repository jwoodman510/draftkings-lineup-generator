namespace DraftKings.LineupGenerator.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            rootComponent.Parameters = new Dictionary<string, object?>
            {
                { "IsConnected", Connectivity.Current.NetworkAccess == NetworkAccess.Internet }
            };

            Connectivity.Current.ConnectivityChanged += (object sender, ConnectivityChangedEventArgs args) =>
            {
                rootComponent.Parameters = new Dictionary<string, object?>
                {
                    { "IsConnected", Connectivity.Current.NetworkAccess == NetworkAccess.Internet }
                };
            };
        }
    }
}
