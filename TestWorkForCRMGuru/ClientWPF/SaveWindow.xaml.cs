using System;
using System.Windows;
using CountriesLibrary.Models;
using CountriesLibrary.Services;

namespace ClientWPF
{
    public partial class SaveWindow : Window
    {
        private readonly ICountriesService countriesService;
        private readonly CountryViewModel countryViewModel;
        public SaveWindow(ICountriesService countriesService, CountryViewModel countryViewModel)
        {
            InitializeComponent();
            this.countriesService = countriesService;
            this.countryViewModel = countryViewModel;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                countriesService.SaveCountryAsync(countryViewModel);
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}