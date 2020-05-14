using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Compo_Request.XMLViewer
{
    /// <summary>
    /// Логика взаимодействия для Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {
        private XmlDocument _xmldocument;

        public Viewer()
        {
            InitializeComponent();
        }

        public XmlDocument xmlDocument
        {
            get { return _xmldocument; }
            set
            {
                _xmldocument = value;
                BindXMLDocument();
            }
        }

        private void BindXMLDocument()
        {
            if (_xmldocument == null)
            {
                xmlTree.ItemsSource = null;
                return;
            }

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = _xmldocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }

        public void Load(string XmlData)
        {
            XmlDocument XMLdoc = new XmlDocument();
            try
            {
                byte[] EncodedString = Encoding.UTF8.GetBytes(XmlData);
                using (var Stream = new MemoryStream(EncodedString))
                {
                    XMLdoc.Load(Stream);
                    xmlDocument = null;
                    xmlDocument = XMLdoc;
                }
            }
            catch (XmlException ex)
            {
                Debug.LogError("The XML file is invalid. XmlException: \n" + ex);
            }
        }
    }
}
