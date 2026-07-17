using System.Windows;
using System.Windows.Controls;
using WpfApp2.ViewModels;
using static WpfApp2.ViewModels.MainViewModel;
namespace WpfApp2.Views
{
    /// <summary>
    /// Logika interakcji dla klasy MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        public MainView()
        {
            InitializeComponent();
        }

        private void EntryUp_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "wejsciowe/do_gory";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }
        private void EntryDown_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "wejsciowe/na_dol";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }

        private void AllUp_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "wszystkie/do_gory";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }

        private void AllDown_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "wszystkie/na_dol";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }

        private void Garage_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "garaz";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }

        private void ET_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "brama";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }
        private void Refresh_Clicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Call.called = "refresh";
            /*
             * 
             * Call.called == "brama" || Call.called == "garaz" || Call.called == "wszystkie/do_gory" || Call.called
                       == "wszystkie/na_dol" || Call.called == "wejsciowe/do_gory" || Call.called == "wejsciowe/na_dol")
             */
        }
    }
}
