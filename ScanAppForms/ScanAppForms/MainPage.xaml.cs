using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScanAppForms
{
    public partial class MainPage : ContentPage
    {
      
        Button btnActivarEscaner, btnDesactivarEscaner;
        Label statusLabel;

        public MainPage()
        {
            // InitializeComponent();
            this.Padding = new Thickness(20, Device.OnPlatform(40, 20, 20), 20, 20);

            StackLayout panel = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Spacing = 15,
            };

            panel.Children.Add(btnActivarEscaner = new Button
            {
                Text = "Activar Escaner"
            });

            panel.Children.Add(btnDesactivarEscaner = new Button
            {
                Text = "Desactivar Escaner"
            });

            btnActivarEscaner.Clicked += ActivarEscaner;
            btnDesactivarEscaner.Clicked += DesactivarEscaner;
            panel.Children.Add(new Label
            {
                Text = "Status:"
            });

            panel.Children.Add(statusLabel = new Label
            {
                Text = ""
            });

            this.Content = panel;
        }

        private void DesactivarEscaner(object sender, EventArgs e)
        {
            var deviceConfig = DependencyService.Get<IDeviceConfig>();
            if (deviceConfig != null)
            {
                deviceConfig.ApagarEscaner();
            }
        }

        private void ActivarEscaner(object sender, EventArgs e)
        {
            var deviceConfig = DependencyService.Get<IDeviceConfig>();
            if (deviceConfig != null)
            {
                deviceConfig.EnciendeEscaner();
            }
        }
    }
}
