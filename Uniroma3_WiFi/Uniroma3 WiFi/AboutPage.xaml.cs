using System.Windows;
using Microsoft.Phone.Tasks;

namespace Roma_Tre_WiFi
{
    public partial class AboutPage : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceSearchTask mdt = new MarketplaceSearchTask();
            mdt.SearchTerms = "vfede";
            mdt.ContentType = MarketplaceContentType.Applications;
            mdt.Show();
        }

        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "usafed@libero.it";
            emailComposeTask.Show();
        }
    }
}