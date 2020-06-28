using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using CountriesLibrary.Services;

namespace ClientWPF
{
    public partial class MainWindow : Window
    {
        private ICountriesService countriesService;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            countriesService = new CountriesService();
        }

        private async void ShowCountries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DgTable.ItemsSource = await countriesService.GetCountriesAsync();
                DgTable.IsEnabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private async void RequestCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var countryViewModel = await countriesService.RequestCountryAsync(TbInput.Text);
                DgTable.ItemsSource = new[] { countryViewModel };
                DgTable.IsEnabled = true;
                var saveWindow = new SaveWindow(countriesService, countryViewModel);
                saveWindow.Show();
            }
            catch (WebException exception)
            {

                if (exception.Status == WebExceptionStatus.ProtocolError
                    && ((HttpWebResponse)exception.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Country not found or the server is not responding.");
                }
                else
                {
                    MessageBox.Show(exception.Message);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            BRequest.IsEnabled = TbInput.Text != string.Empty;
        }
    }
}
