using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.PDF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View : ContentPage
    {
        
        public View(string idt)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                CarregarPdf(idt);
            }
            catch
            {
                Close(this, EventArgs.Empty);
            }
        }
        internal string _filepath;

        public async void Close(object sender, EventArgs e) => await Navigation.PopModalAsync();
        public void SHARE(object sender, EventArgs e)
        {
            if (_filepath == null)
                return;
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Share.RequestAsync(new ShareFileRequest()
                    {
                        Title = "Compartilhar seu PPP",
                        File = new ShareFile(_filepath)
                    });

                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private void CarregarPdf(string urlT = null)
        {

            var dependency = DependencyService.Get<ILocalFileProvider>();

            if (dependency == null)
            {
                DisplayAlert("Erro ao carregar dependencia", "Dependencia não encontrada", "OK");

                return;
            }

            var localPath = string.Empty;

            string url = urlT ?? "https://repositorio.unesp.br/bitstream/handle/11449/118389/000793203.pdf";

            var fileName = Guid.NewGuid().ToString();

            using (var httpClient = new HttpClient())
            {
                var pdfStream = Task.Run(() => httpClient.GetStreamAsync(url)).Result;
                localPath =
                    Task.Run(() => dependency.SaveFileToDisk(pdfStream, $"{fileName}.pdf")).Result;
            }

            if (string.IsNullOrWhiteSpace(localPath))
            {
                DisplayAlert("Error baixar PDF", "não foi possivel encontrar o arquivo", "OK");

                return;
            }
            _filepath = localPath;
            PdfView.Uri = localPath;
        }
    }
}