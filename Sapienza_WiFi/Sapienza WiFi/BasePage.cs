using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Sapienza_WiFi
{
    public partial class BasePage : PhoneApplicationPage
    {
        public BasePage()
        {
            //Activate Page Transition effects on every page
            NavigationInTransition navigationInTransition = new NavigationInTransition()
            {
                Backward = new TurnstileTransition { Mode = TurnstileTransitionMode.BackwardIn },
                Forward = new TurnstileTransition { Mode = TurnstileTransitionMode.ForwardIn }
            };

            NavigationOutTransition navigationOutTransition = new NavigationOutTransition()
            {
                Backward = new TurnstileTransition { Mode = TurnstileTransitionMode.BackwardOut },
                Forward = new TurnstileTransition { Mode = TurnstileTransitionMode.ForwardOut }
            };

            TransitionService.SetNavigationInTransition(this, navigationInTransition);
            TransitionService.SetNavigationOutTransition(this, navigationOutTransition);

        }
    }
}
