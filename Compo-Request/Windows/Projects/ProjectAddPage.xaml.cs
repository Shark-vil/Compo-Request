using Compo_Request.Network.Client;
using Compo_Request.Network.Interfaces;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System.Windows;
using System.Windows.Controls;

namespace Compo_Request.Windows.Projects
{
    /// <summary>
    /// Логика взаимодействия для ProjectAddPage.xaml
    /// </summary>
    public partial class ProjectAddPage : Page, ICustomPage
    {
        internal ProjectsMainPage _ProjectsMainPage;

        public ProjectAddPage(ProjectsMainPage _ProjectsMainPage)
        {
            InitializeComponent();
            EventsInitialize();
            NetworkEventsLoad();

            this._ProjectsMainPage = _ProjectsMainPage;
        }

        private void EventsInitialize()
        {
            Button_ProjectAdd.Click += Button_ProjectAdd_Click;
        }

        private void NetworkEventsLoad()
        {
            NetworkDelegates.Add(delegate (MResponse ServerResponse)
            {
                var MProject = Package.Unpacking<Project>(ServerResponse.DataBytes);

                if (MProject.Uid == TextBox_ProjectUid.Text)
                {
                    new AlertWindow("Оповещение", AlertWindow.AlertCode.AddConfirm, () =>
                    {
                        TextBox_ProjectUid.Text = string.Empty;
                        TextBox_ProjectName.Text = string.Empty;

                        TextBox_ProjectUid.IsEnabled = true;
                        TextBox_ProjectName.IsEnabled = true;
                    });
                }

            }, Dispatcher, -1, "Project.Add.Confirm", "ProjectAddPage");
        }

        private void Button_ProjectAdd_Click(object sender, RoutedEventArgs e)
        {
            var MProject = new Project();
            MProject.Uid = TextBox_ProjectUid.Text;
            MProject.Title = TextBox_ProjectName.Text;

            if (!Sender.SendToServer("Project.Add", MProject))
            {
                new AlertWindow("Ошибка", AlertWindow.AlertCode.SendToServer);
            }
            else
            {
                TextBox_ProjectUid.IsEnabled = false;
                TextBox_ProjectName.IsEnabled = false;
            }
        }

        public void OpenPage()
        {
            //
        }

        public void ClosePage()
        {
            NetworkDelegates.RemoveByUniqueName("ProjectAddPage");
        }
    }
}
